#!/usr/bin/env -S uv run --script

# /// script
# dependencies = ["rdflib==7.*"]
# ///

"""Generate the seed C# file consumed by OSLC4Net.CodeGen.

Example:
    OSLC4Net_SDK/scripts/oslc_domain_seed_gen.py \
        --namespace OSLC4Net.Domains.SysMLV2 \
        --vocabulary-class SysMLVocabulary \
        --vocabulary-uri https://www.omg.org/spec/sysml/vocabulary# \
        --shapes OSLC4Net_SDK/OSLC4Net.Domains.SysMLV2/Resources/shapes.nt \
        --output OSLC4Net_SDK/OSLC4Net.Domains.SysMLV2/SysMLDomain.cs
"""

from __future__ import annotations

import argparse
import re
from pathlib import Path

from rdflib import Graph, URIRef


OSLC = "http://open-services.net/ns/core#"
RDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#"
BCL_TYPE_NAMES = {
    "Action",
    "Attribute",
    "Boolean",
    "Byte",
    "Char",
    "DateTime",
    "DateTimeOffset",
    "Decimal",
    "Delegate",
    "Double",
    "Enum",
    "Exception",
    "Guid",
    "Int16",
    "Int32",
    "Int64",
    "Object",
    "Single",
    "String",
    "Task",
    "TimeSpan",
    "Type",
    "Uri",
    "ValueType",
}


def main() -> None:
    parser = argparse.ArgumentParser(
        description="Generate minimal [OslcVocabulary]/[OslcShape] domain seed declarations."
    )
    parser.add_argument("--namespace", required=True, help="Target C# namespace.")
    parser.add_argument("--vocabulary-class", required=True, help="Name of the generated vocabulary class.")
    parser.add_argument("--vocabulary-uri", required=True, help="Vocabulary namespace URI.")
    parser.add_argument(
        "--shapes",
        required=True,
        nargs="+",
        type=Path,
        help="RDF shape files. Format is inferred from the extension.",
    )
    parser.add_argument("--output", type=Path, help="Output C# file. Defaults to stdout.")
    parser.add_argument(
        "--resource-kind",
        choices=("record", "class"),
        default="record",
        help="Generate partial records or partial classes.",
    )
    args = parser.parse_args()

    graph = Graph()
    for shape_file in args.shapes:
        graph.parse(shape_file, format=guess_format(shape_file))

    declarations = build_declarations(graph, args.resource_kind, domain_prefix(args.namespace))
    source = render_source(
        namespace=args.namespace,
        vocabulary_class=args.vocabulary_class,
        vocabulary_uri=args.vocabulary_uri,
        declarations=declarations,
    )

    if args.output is None:
        print(source, end="")
    else:
        args.output.write_text(source, encoding="utf-8")


def build_declarations(graph: Graph, resource_kind: str, domain_prefix: str) -> list[tuple[str, str]]:
    shape_type = URIRef(OSLC + "ResourceShape")
    describes = URIRef(OSLC + "describes")

    declarations: list[tuple[str, str]] = []
    used_names: set[str] = set()
    for shape in sorted(graph.subjects(URIRef(RDF + "type"), shape_type), key=str):
        if not isinstance(shape, URIRef):
            continue

        described_resources = sorted(
            resource for resource in graph.objects(shape, describes) if isinstance(resource, URIRef)
        )
        if not described_resources:
            continue

        for described_resource in described_resources:
            name = escape_bcl_type_name(
                to_identifier(local_name(str(described_resource))),
                domain_prefix,
            )
            name = unique_identifier(name, used_names)
            declarations.append((str(shape), f"public partial {resource_kind} {name}{class_suffix(resource_kind)}"))

    return declarations


def render_source(
    *,
    namespace: str,
    vocabulary_class: str,
    vocabulary_uri: str,
    declarations: list[tuple[str, str]],
) -> str:
    lines = [
        "/*",
        " * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.",
        " *",
        " * All rights reserved. This program and the accompanying materials",
        " * are made available under the terms of the Eclipse Public License v1.0",
        " * which accompanies this distribution.",
        " *",
        " * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html",
        " */",
        "",
        "using OSLC4Net.Core.Attribute;",
        "",
        f"namespace {namespace};",
        "",
        f'[OslcVocabulary("{escape_csharp_string(vocabulary_uri)}")]',
        f"public static partial class {vocabulary_class};",
    ]

    for shape_uri, declaration in declarations:
        lines.extend(
            [
                "",
                f'[OslcShape("{escape_csharp_string(shape_uri)}")]',
                declaration,
            ]
        )

    lines.append("")
    return "\n".join(lines)


def class_suffix(resource_kind: str) -> str:
    return ";" if resource_kind == "record" else "\n{\n}"


def guess_format(path: Path) -> str:
    suffix = path.suffix.lower()
    if suffix == ".nt":
        return "nt"
    if suffix in {".ttl", ".turtle"}:
        return "turtle"
    if suffix == ".rdf":
        return "xml"
    return "nt"


def local_name(uri: str) -> str:
    index = max(uri.rfind("#"), uri.rfind("/"))
    return uri[index + 1 :] if index >= 0 else uri


def domain_prefix(namespace: str) -> str:
    domain_name = namespace.rsplit(".", maxsplit=1)[-1]
    return domain_name[:1].upper()


def escape_bcl_type_name(identifier: str, prefix: str) -> str:
    return prefix + identifier if identifier in BCL_TYPE_NAMES else identifier


def to_identifier(value: str) -> str:
    result: list[str] = []
    next_upper = True
    for char in value:
        if char.isalnum():
            result.append(char.upper() if next_upper else char)
            next_upper = False
        else:
            next_upper = True

    identifier = "".join(result)
    if not identifier or identifier[0].isdigit():
        identifier = "_" + identifier

    return identifier


def unique_identifier(value: str, used_names: set[str]) -> str:
    name = value
    index = 2
    while name in used_names:
        name = f"{value}{index}"
        index += 1

    used_names.add(name)
    return name


def escape_csharp_string(value: str) -> str:
    return re.sub(r'(["\\])', r"\\\1", value)


if __name__ == "__main__":
    main()
