import rdflib
import argparse  # Import the argparse library
import sys       # To exit gracefully on error
from rdflib import Graph, URIRef, Namespace
from rdflib.namespace import RDF, OWL, RDFS
import re

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

# --- 1. Set up Argument Parser ---
parser = argparse.ArgumentParser(
    description="Read a local TTL file and print properties within a specified namespace."
)
# Add a positional argument for the filepath (required)
parser.add_argument(
    "filepath",
    type=str,
    help="Path to the local TTL file (e.g., dcterms.ttl)"
)
# Add an optional argument for the namespace
parser.add_argument(
    "-ns", "--namespace",
    type=str,
    # Set the default namespace value
    default='http://purl.org/dc/terms/',
    help="The target namespace URI prefix (default: http://purl.org/dc/terms/)"
)
# Parse the command-line arguments provided by the user
args = parser.parse_args()

# --- 2. Use the arguments ---
source_location = args.filepath
target_namespace_str = args.namespace # Use the namespace from args
# Ensure the namespace ends with a common separator if it doesn't
# (This helps catch cases where user might forget '#' or '/')
# Although for the specific task of startswith and slicing, it might not be strictly necessary,
# it's good practice for consistency if you were doing more complex URI manipulation.
# Let's keep it simple based on the original request for now.

# We assume the file is in Turtle format, common for .ttl extensions
# source_format = 'turtle'

# Create an RDF graph
g = Graph()

print(f"Attempting to load RDF data from local file: {source_location}")
print(f"Filtering for properties in namespace: {target_namespace_str}")

try:
    # --- 3. Parse the RDF data from the specified local file ---
   #  g.parse(source=source_location, format=source_format)
    g.parse(source=source_location)
    print(f"Successfully parsed data. Found {len(g)} triples.")

    # Define the RDF/OWL types that signify a property
    property_types = [RDF.Property, OWL.ObjectProperty, OWL.DatatypeProperty]

    # Use a set to store unique property local names to avoid duplicates
    found_properties = set()

    # --- 4. Find properties using RDF.type ---
    # Iterate through each property type and find subjects of that type
    for prop_type in property_types:
        for subject_uri in g.subjects(predicate=RDF.type, object=prop_type):
            # Ensure we are dealing with a URIRef
            if isinstance(subject_uri, URIRef):
                subject_str = str(subject_uri)
                # Check if the URI starts with the TARGET namespace from args
                if subject_str.startswith(target_namespace_str):
                    # Extract the local name (part after the namespace)
                    local_name = subject_str[len(target_namespace_str):]
                    # Add the local name to our set
                    found_properties.add(local_name)

    # --- 5. Print the results ---
    print(f"\nProperties found in namespace '{target_namespace_str}':")
    if found_properties:
        # Sort the list for predictable output
        sorted_properties = sorted(list(found_properties))
        print("""
        public static class P
        {
        """)
        for prop_name in sorted_properties:
            print(f"    public const string {to_pascal_case(prop_name)} = NS + \"{prop_name}\";")

        print("""
        }
        """)

        print("""
        public static class Q
        {
        """)
        for prop_name in sorted_properties:
            print(f"    public static QName {to_pascal_case(prop_name)} => QNameFor(\"{prop_name}\");")

        print("""

        }
        """)
    else:
        print("No properties found matching the criteria in the loaded data.")

# --- 6. Handle potential errors ---
except FileNotFoundError:
    print(f"Error: Local file '{source_location}' not found.", file=sys.stderr)
    print("Please ensure the file path is correct.", file=sys.stderr)
    sys.exit(1) # Exit with a non-zero status code indicates an error
except rdflib.exceptions.ParserError as pe:
    print(f"Error parsing RDF data from '{source_location}': {pe}", file=sys.stderr)
    print(f"Ensure the file is a valid '{source_format}' file.", file=sys.stderr)
    sys.exit(1)
except Exception as e:
    # Catch other potential errors
    print(f"An unexpected error occurred while processing '{source_location}': {e}", file=sys.stderr)
    sys.exit(1)
