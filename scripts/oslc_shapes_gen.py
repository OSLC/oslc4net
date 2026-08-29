#!/usr/bin/env -S uv run --script

# /// script
# dependencies = ["rdflib==7.*", "jinja2", "beautifulsoup4"]
# ///

import rdflib
import argparse
import sys
import os
import re
from rdflib import Graph, URIRef, Literal, Namespace
from rdflib.namespace import RDF, RDFS, DCTERMS, XSD  # Common namespaces
from jinja2 import Environment, FileSystemLoader, Template # Added Template for inline
import html # For unescaping HTML entities
from bs4 import BeautifulSoup # For stripping HTML tags

# --- Try importing the helper function ---
def to_pascal_case(input_str: str) -> str:
    """
    Converts a snake_case or camelCase string to PascalCase.

    Handles:
    - snake_case_input -> SnakeCaseInput
    - camelCaseInput -> CamelCaseInput
    - alreadyPascalCase -> AlreadyPascalCase
    - with_numbers_123 -> WithNumbers123
    - _leading_underscore -> LeadingUnderscore
    - trailing_underscore_ -> TrailingUnderscore

    Args:
        input_str: The string to convert.

    Returns:
        The PascalCase version of the string.
    """
    if not isinstance(input_str, str) or not input_str:
        return ""

    # 1. Handle camelCase by inserting space before uppercase letters
    #    (but not if it's preceded by another uppercase letter or start of string)
    s = re.sub(r'(?<=[a-z0-9])([A-Z])', r' \1', input_str)
    # Optional: Handle transition from multiple UPPER to UpperLower (e.g. XMLHTTPRequest -> XML HTTP Request)
    # s = re.sub(r'(?<=[A-Z])([A-Z][a-z])', r' \1', s)

    # 2. Replace underscores and hyphens with spaces
    s = s.replace('_', ' ').replace('-', ' ')

    # 3. Split into words, capitalize each word, and join
    components = s.split()
    if not components:
        return ""

    # Capitalize the first letter of each component, keep rest as is initially
    # then join and ensure the very first letter is uppercase.
    # Using capitalize() lowercases the rest of the word which might be desired.
    pascal_components = [word.capitalize() for word in components]

    return "".join(pascal_components)

def clean_description(raw_text):
    """
    Strips HTML tags, unescapes HTML entities, and replaces newlines with spaces.
    """
    if not raw_text:
        return "" # Return empty string if input is None or empty

    # 1. Use BeautifulSoup to parse HTML and get text content
    #    'html.parser' is a built-in parser, no extra dependencies needed beyond bs4
    #    separator=' ' helps put spaces between text blocks from different tags
    soup = BeautifulSoup(raw_text, 'html.parser')
    text_content = soup.get_text(separator=' ')

    # 2. Unescape HTML entities (like <, &)
    text_unescaped = html.unescape(text_content)

    # 3. Replace newline characters (and carriage returns) with spaces
    #    and collapse multiple whitespace characters into a single space
    cleaned_text = ' '.join(text_unescaped.split()) # split() handles various whitespace

    # 4. Remove potential leading/trailing whitespace
    return cleaned_text.strip()

# --- Define Namespaces ---
OSLC = Namespace("http://open-services.net/ns/core#")
# Add other namespaces used in your shapes file if needed (e.g., OSLC_RM)
# OSLC_RM = Namespace("http://open-services.net/ns/rm#")

# --- Jinja2 Template for C# Class ---
# Using an inline template string for simplicity here.
# You could move this to a separate file (e.g., 'csharp_class.jinja')
CSHARP_TEMPLATE_STR = """\
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;
using ValueType = OSLC4Net.Core.Model.ValueType;

namespace {{ csharp_namespace }} // Target C# namespace
{
    // Generated from OSLC Shape: {{ shape.uri }}
    [OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)] // Example: Define constants elsewhere
    [OslcResourceShape(title = "{{ shape.title }}", describes = "{{ shape.describes }}")] // Add attributes from shape if needed
    public partial record {{ shape.class_name }} : AbstractResource // Or another base class
    {
        public {{ shape.class_name }}(Uri about) : base(about) {}
        public {{ shape.class_name }}() : base() {}

        {% for prop in shape.properties %}
        {% if prop.description %}
        [OslcDescription("{{ prop.description | replace('"', '\\"') }}")] // Escape quotes in description
        {% endif %}
        {% if prop.occurs %}
        [OslcOccurs(Occurs.{{ prop.occurs }})]
        {% endif %}
        {% if prop.property_definition %}
        [OslcPropertyDefinition("{{ prop.property_definition }}")]
        {% endif %}
        {% if prop.name %}
        [OslcName("{{ prop.name }}")]
        {% endif %}
        {% if prop.value_type_enum %}
        [OslcValueType(ValueType.{{ prop.value_type_enum }})]
        {% elif prop.range %}
        // Range specified: {{ prop.range }} - Consider adding OslcRange attribute if needed
        {% endif %}
        {% if prop.representation %}
        [OslcRepresentation(Representation.{{ prop.representation }})]
        {% endif %}
        [OslcReadOnly({{ prop.read_only | lower }})] // Assuming read_only property exists
        [OslcTitle("{{ prop.title | default(prop.name, true) }}")] // Use prop name as fallback title
        public {{ prop.csharp_type }} {{ prop.csharp_name }} { get; set; }
        {% if not loop.last %}

        {% endif %}
        {% endfor %}

        // Method to add properties dynamically if needed (Example for multi-valued)
        // public void AddSomeMultiValueProperty(URI value) { ... }
    }

    // Define related constants if needed, e.g.:
    // public static class OslcConstants {
    //     public const string OSLC_CORE_NAMESPACE = "{{ oslc_ns }}";
    //     public const string DCTERMS_NAMESPACE = "{{ dcterms_ns }}";
    // }
}
"""

# --- Helper Functions ---

def get_literal_value(graph, subject, predicate):
    """
    Safely gets a literal value as a string.
    Specifically handles rdf:XMLLiteral by returning its string content.
    """
    value = graph.value(subject, predicate)
    if isinstance(value, Literal):
        # *** CHANGE HERE: Check for XMLLiteral FIRST ***
        if value.datatype == RDF.XMLLiteral:
            # Return the string value of the XMLLiteral directly
            return str(value)
        else:
            # For other literal types, attempt conversion via toPython()
            try:
                # toPython might return non-string types (bool, int, etc.)
                py_val = value.toPython()
                # Ensure we return a string representation for the template
                return str(py_val)
            except Exception as e:
                # Fallback if toPython fails for some reason
                print(f"Warning: Could not convert literal with datatype {value.datatype} using toPython() for {subject} {predicate}. Using raw string value. Error: {e}", file=sys.stderr)
                return str(value) # Use raw string value as fallback
    elif isinstance(value, URIRef):
        # If the object is a URI, return its string representation
        return str(value)
    # Return None or "" if no suitable value found
    return None # Or "" if you prefer empty strings over None


def get_uri_value(graph, subject, predicate):
    """Safely gets a URI value as a string."""
    value = graph.value(subject, predicate)
    if isinstance(value, URIRef):
        return str(value)
    return None

def get_local_name(uri_string):
    """Extracts the local name (fragment or last path segment) from a URI."""
    if not uri_string:
        return None
    if '#' in uri_string:
        return uri_string.split('#')[-1]
    elif '/' in uri_string:
        return uri_string.split('/')[-1]
    return uri_string # Fallback

def map_oslc_occurs_to_csharp(occurs_uri):
    """Maps OSLC occurs URI to C# enum string."""
    local_name = get_local_name(occurs_uri)
    mapping = {
        "Exactly-one": "ExactlyOne",
        "Zero-or-one": "ZeroOrOne",
        "Zero-or-many": "ZeroOrMany",
        "One-or-many": "OneOrMany",
    }
    return mapping.get(local_name) # Returns None if not found

def map_oslc_value_type_to_csharp_enum(value_type_uri):
    """Maps OSLC value type URI to C# ValueType enum string."""
    # This depends heavily on the specific C# library (like OSLC4Net)
    # Using common examples
    if not value_type_uri:
        return None

    if value_type_uri == str(RDF.XMLLiteral):
        return "XMLLiteral"
    elif value_type_uri == str(OSLC.Resource) or value_type_uri == str(OSLC.AnyResource):
         # OSLC4Net might represent these implicitly or via Resource/AnyResource enum?
         # Or sometimes it's inferred from range. Let's return None here
         # and rely on csharp_type mapping based on range.
         return None # Or "Resource" / "AnyResource" if your enum has them
    elif value_type_uri == str(XSD.string):
        return "String"
    elif value_type_uri == str(XSD.boolean):
        return "Boolean"
    elif value_type_uri == str(XSD.dateTime):
        return "DateTime"
    elif value_type_uri == str(XSD.integer) or value_type_uri == str(XSD.decimal):
        return "Decimal" # Or Integer? Check library specifics
    # Add more XSD types as needed
    else:
        # Attempt to use local name if it's an unknown URI
        local = get_local_name(value_type_uri)
        # Maybe map common ones like 'Resource', 'LocalResource' if they exist in ValueType
        # if local in ["Resource", "LocalResource", "AnyResource"]: return local
        return None # Fallback

def map_oslc_representation_to_csharp(rep_uri):
    """Maps OSLC representation URI to C# enum string."""
    local_name = get_local_name(rep_uri)
    mapping = {
        "Inline": "Inline",
        "Reference": "Reference",
        "Either": "Either",
    }
    return mapping.get(local_name)

def map_rdf_type_to_csharp_type(g, prop_uri):
    """Determines the C# property type based on oslc:valueType or oslc:range."""
    value_type = g.value(prop_uri, OSLC.valueType)
    range_uri = g.value(prop_uri, OSLC.range)
    occurs = g.value(prop_uri, OSLC.occurs)

    is_multi_valued = occurs in [OSLC['Zero-or-many'], OSLC['One-or-many']]
    csharp_base_type = "object" # Default fallback

    target_type_uri = value_type or range_uri

    if target_type_uri == RDF.XMLLiteral or target_type_uri == XSD.string:
        csharp_base_type = "string"
    elif target_type_uri == XSD.boolean:
        csharp_base_type = "bool"
    elif target_type_uri == XSD.integer:
        csharp_base_type = "int"
    elif target_type_uri == XSD.decimal or target_type_uri == XSD.double:
        csharp_base_type = "decimal" # Or double?
    elif target_type_uri == XSD.dateTime:
        # Use DateTimeOffset for better timezone handling, nullable for ZeroOrOne/ZeroOrMany
        csharp_base_type = "DateTimeOffset?" # Nullable by default, adjust if needed
        # If occurs is ExactlyOne or OneOrMany, maybe non-nullable? Check C# lib conventions
        if occurs == OSLC['Exactly-one'] or occurs == OSLC['One-or-many']:
             csharp_base_type = "DateTimeOffset"
    elif target_type_uri == OSLC.Resource or target_type_uri == OSLC.AnyResource or isinstance(target_type_uri, URIRef):
        # Could be a URI link or potentially a nested resource type
        # Often represented as URI in C# OSLC libs
        csharp_base_type = "Uri" # System.Uri
        # Could also check if range_uri points to another ResourceShape and use its class name
        # For simplicity, using Uri for now.
        # local_range_name = get_local_name(str(range_uri))
        # if local_range_name and "Shape" in local_range_name: # Heuristic
        #     csharp_base_type = to_pascal_case(local_range_name.replace("Shape", ""))

    # Handle multi-valued properties
    if is_multi_valued:
        # Common C# collections: List<T>, HashSet<T>, T[]
        # Using HashSet for uniqueness often makes sense for links
        return f"HashSet<{csharp_base_type}>" # Requires initialization in constructor or property
        # Or List<T>: return f"List<{csharp_base_type}>"
    else:
        # Handle potential nullability for ZeroOrOne
        if occurs == OSLC['Zero-or-one']:
            # Make reference types nullable (string, Uri, custom classes)
            # Value types (bool, int, decimal, DateTimeOffset) might need '?'
            if csharp_base_type in ["string", "Uri", "object"] or csharp_base_type.endswith("?"):
                 pass # Already reference type or explicitly nullable
            elif csharp_base_type in ["bool", "int", "decimal", "DateTimeOffset"]:
                 csharp_base_type += "?"

        return csharp_base_type


# --- Main Execution Logic ---
def main():
    parser = argparse.ArgumentParser(
        description="Generate C# classes from an OSLC Shapes TTL file using Jinja2."
    )
    parser.add_argument(
        "filepath",
        type=str,
        help="Path to the local OSLC Shapes TTL file."
    )
    parser.add_argument(
        "-o", "--output-dir",
        type=str,
        default="generated_csharp",
        help="Directory to save the generated C# files (default: generated_csharp)."
    )
    parser.add_argument(
        "-ns", "--csharp-namespace",
        type=str,
        default="Generated.Oslc.Shapes",
        help="The C# namespace for the generated classes (default: Generated.Oslc.Shapes)."
    )
    args = parser.parse_args()

    source_location = args.filepath
    output_dir = args.output_dir
    csharp_namespace = args.csharp_namespace
    source_format = 'turtle'

    # --- Load RDF Graph ---
    g = Graph()
    print(f"Attempting to load RDF data from: {source_location}")
    try:
        g.parse(source=source_location, format=source_format)
        print(f"Successfully parsed {len(g)} triples.")
    except FileNotFoundError:
        print(f"Error: File not found at '{source_location}'", file=sys.stderr)
        sys.exit(1)
    except rdflib.exceptions.ParserError as pe:
        print(f"Error parsing RDF file: {pe}", file=sys.stderr)
        sys.exit(1)
    except Exception as e:
        print(f"An unexpected error occurred during parsing: {e}", file=sys.stderr)
        sys.exit(1)

    # --- Prepare Jinja Environment ---
    # Using inline template string
    template = Template(CSHARP_TEMPLATE_STR)
    # If using a file:
    # jinja_env = Environment(loader=FileSystemLoader('.'), trim_blocks=True, lstrip_blocks=True)
    # template = jinja_env.get_template('csharp_class.jinja') # Assuming template file exists

    # --- Ensure output directory exists ---
    try:
        os.makedirs(output_dir, exist_ok=True)
        print(f"Output directory: {output_dir}")
    except OSError as e:
        print(f"Error creating output directory '{output_dir}': {e}", file=sys.stderr)
        sys.exit(1)

    # --- Process Shapes ---
    shapes_processed = 0
    # Find all subjects that are of type oslc:ResourceShape
    for shape_uri in g.subjects(predicate=RDF.type, object=OSLC.ResourceShape):
        if not isinstance(shape_uri, URIRef):
            print(f"Skipping non-URI shape identifier: {shape_uri}", file=sys.stderr)
            continue

        shape_local_name = get_local_name(str(shape_uri))
        if not shape_local_name:
            print(f"Skipping shape with unparseable URI: {shape_uri}", file=sys.stderr)
            continue

        # Derive C# class name from shape's local name
        class_name = to_pascal_case(shape_local_name.replace("Shape", "")) # Remove "Shape" suffix common convention
        if not class_name:
             print(f"Skipping shape {shape_uri} due to empty derived class name.", file=sys.stderr)
             continue

        print(f"\nProcessing Shape: {shape_uri} -> Class: {class_name}")
        raw_shape_title = get_literal_value(g, shape_uri, DCTERMS.title)
        cleaned_shape_title = clean_description(raw_shape_title) or class_name # Use class name as fallback

        raw_shape_desc = get_literal_value(g, shape_uri, DCTERMS.description)
        cleaned_shape_desc = clean_description(raw_shape_desc)

        shape_data = {
            "uri": str(shape_uri),
            "class_name": class_name,
            "csharp_namespace": csharp_namespace,
            # Use cleaned values for attributes needing plain text
            "title": cleaned_shape_title,
            "description": cleaned_shape_desc, # This might not be used directly in shape attributes often
            "describes": get_uri_value(g, shape_uri, OSLC.describes),
            "properties": [],
            "oslc_ns": str(OSLC),
            "dcterms_ns": str(DCTERMS),
        }


        # Find all property URIs linked by oslc:property
        for prop_uri in g.objects(subject=shape_uri, predicate=OSLC.property):
            if not isinstance(prop_uri, URIRef):
                print(f"  Skipping non-URI property link: {prop_uri}", file=sys.stderr)
                continue

            # Verify the linked resource is actually an oslc:Property
            if not (prop_uri, RDF.type, OSLC.Property) in g:
                print(f"  Warning: Resource {prop_uri} linked by oslc:property is not explicitly typed as oslc:Property.", file=sys.stderr)
                # Continue processing it anyway, assuming it has the needed attributes

            prop_name = get_literal_value(g, prop_uri, OSLC.name)
            if not prop_name:
                prop_local_name = get_local_name(str(prop_uri))
                print(f"  Warning: Property {prop_uri} missing oslc:name. Using local name '{prop_local_name}' as fallback.", file=sys.stderr)
                prop_name = prop_local_name # Use fragment/local name as fallback
                if not prop_name:
                    print(f"  Skipping property {prop_uri} with no usable name.", file=sys.stderr)
                    continue # Skip property if no name available

            prop_csharp_name = to_pascal_case(prop_name)
            if not prop_csharp_name:
                print(f"  Skipping property {prop_uri} due to empty derived C# name from '{prop_name}'.", file=sys.stderr)
                continue

 # *** Apply cleaning to description ***
            raw_prop_desc = get_literal_value(g, prop_uri, DCTERMS.description)
            cleaned_prop_desc = clean_description(raw_prop_desc)

            raw_prop_title = get_literal_value(g, prop_uri, DCTERMS.title)
            cleaned_prop_title = clean_description(raw_prop_title)


            prop_data = {
                "uri": str(prop_uri),
                "name": prop_name,
                "csharp_name": prop_csharp_name,
                # Store both raw (if needed for comments) and cleaned versions
                "original_description": raw_prop_desc,
                "description": cleaned_prop_desc, # Use cleaned version for attribute
                "title": cleaned_prop_title, # Clean title too
                "occurs": map_oslc_occurs_to_csharp(get_uri_value(g, prop_uri, OSLC.occurs)),
                "property_definition": get_uri_value(g, prop_uri, OSLC.propertyDefinition),
                "value_type": get_uri_value(g, prop_uri, OSLC.valueType),
                "value_type_enum": map_oslc_value_type_to_csharp_enum(get_uri_value(g, prop_uri, OSLC.valueType)),
                "range": get_uri_value(g, prop_uri, OSLC.range),
                "representation": map_oslc_representation_to_csharp(get_uri_value(g, prop_uri, OSLC.representation)),
                "read_only": get_literal_value(g, prop_uri, OSLC.readOnly) or False,
                "csharp_type": map_rdf_type_to_csharp_type(g, prop_uri)
            }

            print(f"  Found Property: {prop_name} -> C#: {prop_csharp_name} (Type: {prop_data['csharp_type']})")
            shape_data["properties"].append(prop_data)


        # --- Generate C# file for this shape ---
        if not shape_data["properties"]:
            print(f"  Shape {shape_uri} has no valid properties defined. Skipping file generation.")
            continue

        output_filename = os.path.join(output_dir, f"{class_name}.cs")
        try:
            rendered_code = template.render(shape=shape_data)
            with open(output_filename, "w", encoding="utf-8") as f:
                f.write(rendered_code)
            print(f"  Successfully generated: {output_filename}")
            shapes_processed += 1
        except Exception as e:
            print(f"  Error generating or writing file for shape {shape_uri}: {e}", file=sys.stderr)


    print(f"\nFinished processing. Generated {shapes_processed} C# files.")

if __name__ == "__main__":
    main()
