/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace OSLC4Net.CodeGen.OslcSourceGenerator;

[Generator]
public sealed class OslcDomainGenerator : IIncrementalGenerator
{
    private const string Core = "http://open-services.net/ns/core#";
    private const string DcTerms = "http://purl.org/dc/terms/";
    private const string Owl = "http://www.w3.org/2002/07/owl#";
    private const string Rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
    private const string Rdfs = "http://www.w3.org/2000/01/rdf-schema#";
    private const string Sh = "http://www.w3.org/ns/shacl#";
    private const string Vann = "http://purl.org/vocab/vann/";
    private const string Xsd = "http://www.w3.org/2001/XMLSchema#";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<GenerationTarget?> targets = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) =>
                    node is TypeDeclarationSyntax typeDeclaration
                    && (
                        typeDeclaration.AttributeLists.Count > 0
                        || typeDeclaration
                            .Members.OfType<PropertyDeclarationSyntax>()
                            .Any(static property => property.AttributeLists.Count > 0)
                    ),
                static (syntaxContext, _) => GetGenerationTarget(syntaxContext)
            )
            .Where(static target => target is not null);

        IncrementalValueProvider<ImmutableArray<AdditionalText>> rdfFiles = context
            .AdditionalTextsProvider.Where(static text =>
                text.Path.EndsWith(".nt", StringComparison.OrdinalIgnoreCase)
            )
            .Collect();

        IncrementalValueProvider<(
            ImmutableArray<GenerationTarget?> Targets,
            ImmutableArray<AdditionalText> RdfFiles
        )> combined = targets.Collect().Combine(rdfFiles);

        context.RegisterSourceOutput(
            combined,
            static (sourceProductionContext, input) =>
            {
                if (input.Targets.IsDefaultOrEmpty)
                {
                    return;
                }

                Graph graph = Graph.Load(input.RdfFiles, sourceProductionContext.CancellationToken);
                List<GenerationTarget> concreteTargets = MergePartialTargets(
                    input.Targets.OfType<GenerationTarget>()
                );
                Dictionary<GenerationTarget, Shape> shapesByTarget = BuildShapeMap(
                    concreteTargets,
                    graph
                );
                Dictionary<string, GenerationTarget> resourceTypesByUri = BuildResourceTypeMap(
                    shapesByTarget
                );

                foreach (GenerationTarget target in concreteTargets)
                {
                    if (target.VocabularyUri is not null)
                    {
                        sourceProductionContext.AddSource(
                            $"{target.Namespace}.{target.TypeName}.Vocabulary.g.cs",
                            SourceText.From(GenerateVocabulary(target, graph), Encoding.UTF8)
                        );
                    }

                    if (target.ShapeUri is not null)
                    {
                        string? source = GenerateShape(
                            target,
                            shapesByTarget[target],
                            graph,
                            resourceTypesByUri
                        );
                        if (source is not null)
                        {
                            sourceProductionContext.AddSource(
                                $"{target.Namespace}.{target.TypeName}.Shape.g.cs",
                                SourceText.From(source, Encoding.UTF8)
                            );
                        }
                    }
                }
            }
        );
    }

    private static List<GenerationTarget> MergePartialTargets(IEnumerable<GenerationTarget> targets)
    {
        return targets
            .GroupBy(
                static target => target.Namespace + "\0" + target.TypeName,
                StringComparer.Ordinal
            )
            .Select(static group =>
            {
                GenerationTarget first = group.First();
                string? vocabularyUri = group
                    .Select(static target => target.VocabularyUri)
                    .FirstOrDefault(static value => value is not null);
                string? vocabularyPrefix = group
                    .Select(static target => target.VocabularyPrefix)
                    .FirstOrDefault(static value => value is not null);
                string? shapeUri = group
                    .Select(static target => target.ShapeUri)
                    .FirstOrDefault(static value => value is not null);
                string? shapeTitle = group
                    .Select(static target => target.ShapeTitle)
                    .FirstOrDefault(static value => value is not null);
                ImmutableArray<string> selectedPropertyUris = group
                    .SelectMany(static target => target.SelectedPropertyUris)
                    .Distinct(StringComparer.Ordinal)
                    .ToImmutableArray();
                ImmutableArray<string> overriddenPropertyUris = group
                    .SelectMany(static target => target.OverriddenPropertyUris)
                    .Distinct(StringComparer.Ordinal)
                    .ToImmutableArray();

                return new GenerationTarget(
                    first.Namespace,
                    first.TypeName,
                    group.Any(static target => target.IsRecord),
                    vocabularyUri,
                    vocabularyPrefix,
                    shapeUri,
                    shapeTitle,
                    selectedPropertyUris,
                    overriddenPropertyUris
                );
            })
            .ToList();
    }

    private static Dictionary<GenerationTarget, Shape> BuildShapeMap(
        IEnumerable<GenerationTarget> targets,
        Graph graph
    )
    {
        var shapesByTarget = new Dictionary<GenerationTarget, Shape>();
        foreach (
            GenerationTarget target in targets.Where(static target => target.ShapeUri is not null)
        )
        {
            shapesByTarget.Add(target, Shape.FromGraph(target.ShapeUri!, graph));
        }

        return shapesByTarget;
    }

    private static Dictionary<string, GenerationTarget> BuildResourceTypeMap(
        Dictionary<GenerationTarget, Shape> shapesByTarget
    )
    {
        var resourceTypesByUri = new Dictionary<string, GenerationTarget>(StringComparer.Ordinal);
        foreach (KeyValuePair<GenerationTarget, Shape> entry in shapesByTarget)
        {
            foreach (string describedResource in entry.Value.Describes)
            {
                resourceTypesByUri[describedResource] = entry.Key;
            }
        }

        return resourceTypesByUri;
    }

    private static GenerationTarget? GetGenerationTarget(GeneratorSyntaxContext context)
    {
        var typeDeclaration = (TypeDeclarationSyntax)context.Node;
        string? vocabularyUri = null;
        string? vocabularyPrefix = null;
        string? shapeUri = null;
        string? shapeTitle = null;
        var selectedProperties = new List<string>();
        var overriddenProperties = new List<string>();

        foreach (AttributeListSyntax attributeList in typeDeclaration.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                string name = attribute.Name.ToString();
                string? value = GetSingleStringArgument(attribute, context.SemanticModel);

                if (value is null)
                {
                    continue;
                }

                if (
                    name.EndsWith("OslcVocabulary", StringComparison.Ordinal)
                    || name.EndsWith("OslcVocabularyAttribute", StringComparison.Ordinal)
                )
                {
                    vocabularyUri = value;
                    vocabularyPrefix = GetStringArgument(attribute, 1, context.SemanticModel);
                }
                else if (
                    name.EndsWith("OslcShape", StringComparison.Ordinal)
                    || name.EndsWith("OslcShapeAttribute", StringComparison.Ordinal)
                )
                {
                    shapeUri = value;
                    shapeTitle = GetNamedStringArgument(attribute, "Title", context.SemanticModel);
                }
            }
        }

        foreach (MemberDeclarationSyntax member in typeDeclaration.Members)
        {
            if (member is not PropertyDeclarationSyntax property)
            {
                continue;
            }

            foreach (AttributeListSyntax attributeList in property.AttributeLists)
            {
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    string name = attribute.Name.ToString();
                    string? value = GetSingleStringArgument(attribute, context.SemanticModel);
                    if (
                        value is not null
                        && (
                            name.EndsWith("OslcShapeProperty", StringComparison.Ordinal)
                            || name.EndsWith("OslcShapePropertyAttribute", StringComparison.Ordinal)
                        )
                    )
                    {
                        selectedProperties.Add(value);
                    }

                    if (
                        value is not null
                        && (
                            name.EndsWith("OslcPropertyDefinition", StringComparison.Ordinal)
                            || name.EndsWith(
                                "OslcPropertyDefinitionAttribute",
                                StringComparison.Ordinal
                            )
                        )
                    )
                    {
                        overriddenProperties.Add(value);
                    }
                }
            }
        }

        if (
            vocabularyUri is null
            && shapeUri is null
            && selectedProperties.Count == 0
            && overriddenProperties.Count == 0
        )
        {
            return null;
        }

        INamedTypeSymbol? typeSymbol =
            context.SemanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;
        if (typeSymbol is null)
        {
            return null;
        }

        return new GenerationTarget(
            typeSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : typeSymbol.ContainingNamespace.ToDisplayString(),
            typeSymbol.Name,
            typeDeclaration is RecordDeclarationSyntax,
            vocabularyUri,
            vocabularyPrefix,
            shapeUri,
            shapeTitle,
            selectedProperties.ToImmutableArray(),
            overriddenProperties.ToImmutableArray()
        );
    }

    private static string? GetSingleStringArgument(
        AttributeSyntax attribute,
        SemanticModel semanticModel
    )
    {
        return GetStringArgument(attribute, 0, semanticModel);
    }

    private static string? GetStringArgument(
        AttributeSyntax attribute,
        int index,
        SemanticModel semanticModel
    )
    {
        ExpressionSyntax? expression = attribute
            .ArgumentList?.Arguments.Skip(index).FirstOrDefault()
            ?.Expression;
        if (expression is null)
        {
            return null;
        }

        Optional<object?> constant = semanticModel.GetConstantValue(expression);
        if (constant.HasValue && constant.Value is string value)
        {
            return value;
        }

        return expression switch
        {
            LiteralExpressionSyntax literal => literal.Token.ValueText,
            _ => null,
        };
    }

    private static string? GetNamedStringArgument(
        AttributeSyntax attribute,
        string name,
        SemanticModel semanticModel
    )
    {
        foreach (AttributeArgumentSyntax argument in attribute.ArgumentList?.Arguments ?? default)
        {
            if (
                !string.Equals(
                    argument.NameEquals?.Name.Identifier.ValueText,
                    name,
                    StringComparison.Ordinal
                )
            )
            {
                continue;
            }

            Optional<object?> constant = semanticModel.GetConstantValue(argument.Expression);
            if (constant.HasValue && constant.Value is string value)
            {
                return value;
            }

            if (argument.Expression is LiteralExpressionSyntax literal)
            {
                return literal.Token.ValueText;
            }
        }

        return null;
    }

    private static string GenerateVocabulary(GenerationTarget target, Graph graph)
    {
        string namespaceUri = target.VocabularyUri!;
        string prefix =
            target.VocabularyPrefix
            ?? FirstValue(graph.Objects(namespaceUri, Vann + "preferredNamespacePrefix"))
            ?? DefaultPrefix(target.TypeName);
        List<string> classes = graph
            .Subjects(Rdf + "type", Rdfs + "Class")
            .Concat(graph.Subjects(Rdf + "type", Owl + "Class"))
            .Where(uri => IsVocabularyTerm(uri, namespaceUri, graph))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(uri => VocabularyTermName(uri, namespaceUri), StringComparer.Ordinal)
            .ToList();
        List<string> properties = graph
            .Subjects(Rdf + "type", Rdf + "Property")
            .Concat(graph.Subjects(Rdf + "type", Owl + "ObjectProperty"))
            .Concat(graph.Subjects(Rdf + "type", Owl + "DatatypeProperty"))
            .Concat(graph.Subjects(Rdf + "type", Owl + "AnnotationProperty"))
            .Where(uri => IsVocabularyTerm(uri, namespaceUri, graph))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(uri => VocabularyTermName(uri, namespaceUri), StringComparer.Ordinal)
            .ToList();

        var builder = new StringBuilder();
        AppendAutoGeneratedHeader(builder);
        builder.AppendLine("using OSLC4Net.Core.Model;");
        builder.AppendLine();
        AppendNamespaceStart(builder, target.Namespace);
        AppendXmlSummary(builder, "", VocabularyDescription(namespaceUri, graph));
        builder.Append("public static partial class ").Append(target.TypeName).AppendLine();
        builder.AppendLine("{");
        builder
            .Append("    public const string NS = ")
            .Append(ToLiteral(namespaceUri))
            .AppendLine(";");
        builder
            .Append("    public const string Prefix = ")
            .Append(ToLiteral(prefix))
            .AppendLine(";");
        builder.AppendLine();

        foreach (string uri in classes)
        {
            string termName = VocabularyTermName(uri, namespaceUri);
            string name = ToIdentifier(termName);
            AppendXmlSummary(builder, "    ", VocabularyDescription(uri, graph));
            builder
                .Append("    public const string ")
                .Append(name)
                .Append(" = NS + ")
                .Append(ToLiteral(termName))
                .AppendLine(";");
        }

        if (classes.Count > 0)
        {
            builder.AppendLine();
        }

        builder.AppendLine("    public static QName QNameFor(string localResource)");
        builder.AppendLine("    {");
        builder.AppendLine("        return new QName(NS, localResource, Prefix);");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static partial class P");
        builder.AppendLine("    {");
        foreach (string uri in properties)
        {
            string termName = VocabularyTermName(uri, namespaceUri);
            string name = ToIdentifier(termName);
            AppendXmlSummary(builder, "        ", VocabularyDescription(uri, graph));
            builder
                .Append("        public const string ")
                .Append(name)
                .Append(" = NS + ")
                .Append(ToLiteral(termName))
                .AppendLine(";");
        }

        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    public static partial class Q");
        builder.AppendLine("    {");
        foreach (string uri in properties)
        {
            string termName = VocabularyTermName(uri, namespaceUri);
            string name = ToIdentifier(termName);
            AppendXmlSummary(builder, "        ", VocabularyDescription(uri, graph));
            builder
                .Append("        public static QName ")
                .Append(name)
                .Append(" => QNameFor(")
                .Append(ToLiteral(termName))
                .AppendLine(");");
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");
        AppendNamespaceEnd(builder, target.Namespace);
        return builder.ToString();
    }

    private static string? VocabularyDescription(string uri, Graph graph)
    {
        return FirstPlainValue(graph.Objects(uri, DcTerms + "description"))
            ?? FirstPlainValue(graph.Objects(uri, Rdfs + "comment"));
    }

    private static bool IsVocabularyTerm(string uri, string namespaceUri, Graph graph)
    {
        return graph
                .Objects(uri, Rdfs + "isDefinedBy")
                .Any(node => string.Equals(node.Value, namespaceUri, StringComparison.Ordinal))
            || uri.StartsWith(namespaceUri, StringComparison.Ordinal);
    }

    private static string VocabularyTermName(string uri, string namespaceUri)
    {
        return uri.StartsWith(namespaceUri, StringComparison.Ordinal)
            ? uri.Substring(namespaceUri.Length)
            : LocalName(uri);
    }

    private static string DefaultPrefix(string typeName)
    {
        const string suffix = "Vocabulary";
        string prefix = typeName.EndsWith(suffix, StringComparison.Ordinal) && typeName.Length > suffix.Length
            ? typeName.Substring(0, typeName.Length - suffix.Length)
            : typeName;
        return prefix.ToLowerInvariant();
    }

    private static string? GenerateShape(
        GenerationTarget target,
        Shape shape,
        Graph graph,
        IReadOnlyDictionary<string, GenerationTarget> resourceTypesByUri
    )
    {
        if (shape.Describes.Count == 0 && shape.Properties.Count == 0)
        {
            return null;
        }

        List<GenerationTarget> superTypes = GetDirectSuperTypes(
            target,
            shape,
            graph,
            resourceTypesByUri
        );
        HashSet<string> overriddenPropertyDefinitions = CollectOverriddenPropertyDefinitions(
            target,
            shape,
            graph,
            resourceTypesByUri
        );
        List<ShapeProperty> properties = target.SelectedPropertyUris.IsDefaultOrEmpty
            ? shape.Properties
            : shape
                .Properties.Where(property =>
                    target.SelectedPropertyUris.Contains(
                        property.PropertyDefinition,
                        StringComparer.Ordinal
                    )
                )
                .ToList();
        properties = properties
            .Where(property => !overriddenPropertyDefinitions.Contains(property.PropertyDefinition))
            .ToList();

        var builder = new StringBuilder();
        AppendAutoGeneratedHeader(builder);
        builder.AppendLine("#nullable enable");
        builder.AppendLine("using OSLC4Net.Core.Attribute;");
        builder.AppendLine("using OSLC4Net.Core.Model;");
        builder.AppendLine("using ValueType = OSLC4Net.Core.Model.ValueType;");
        builder.AppendLine();
        AppendNamespaceStart(builder, target.Namespace);
        List<ShapePropertyBinding> propertyBindings = BindProperties(target, properties);
        GenerationTarget? baseTarget = superTypes.FirstOrDefault();
        string baseType = baseTarget is null
            ? target.IsRecord
                ? "AbstractResourceRecord"
                : "AbstractResource"
            : GetTypeReference(target, baseTarget);

        builder.Append("public partial interface ").Append(GetInterfaceName(target));
        List<string> interfaceBases = superTypes
            .Select(superType => GetInterfaceTypeReference(target, superType))
            .Distinct(StringComparer.Ordinal)
            .ToList();
        if (interfaceBases.Count > 0)
        {
            builder.Append(" : ").Append(string.Join(", ", interfaceBases));
        }
        else
        {
            builder.Append(" : IExtendedResource");
        }

        builder.AppendLine();
        builder.AppendLine("{");
        builder.AppendLine("}");
        builder.AppendLine();

        builder
            .Append("[OslcNamespace(")
            .Append(ToLiteral(NamespaceFromShape(shape)))
            .AppendLine(")]");
        builder
            .Append("[OslcResourceShape(title = ")
            .Append(ToLiteral(target.ShapeTitle ?? shape.Title ?? Humanize(target.TypeName)))
            .Append(", describes = new string[] { ");
        builder.Append(string.Join(", ", shape.Describes.Select(ToLiteral)));
        builder.AppendLine(" })]");
        string interfaceName = GetInterfaceName(target);
        if (target.IsRecord)
        {
            builder
                .Append("public partial record ")
                .Append(target.TypeName)
                .Append(" : ")
                .Append(baseType)
                .Append(", ")
                .Append(interfaceName)
                .AppendLine();
        }
        else
        {
            builder
                .Append("public partial class ")
                .Append(target.TypeName)
                .Append(" : ")
                .Append(baseType)
                .Append(", ")
                .Append(interfaceName)
                .AppendLine();
        }

        builder.AppendLine("{");
        builder.Append("    public ").Append(target.TypeName).AppendLine("(Uri about)");
        builder.AppendLine("        : base(about) { }");
        builder.AppendLine();
        builder.Append("    public ").Append(target.TypeName).AppendLine("() { }");

        foreach (ShapePropertyBinding property in propertyBindings)
        {
            builder.AppendLine();
            builder.AppendLine();
            AppendProperty(builder, property.Property, property.PropertyName);
        }

        builder.AppendLine();
        builder.AppendLine("}");
        AppendNamespaceEnd(builder, target.Namespace);
        return builder.ToString();
    }

    private static HashSet<string> CollectOverriddenPropertyDefinitions(
        GenerationTarget target,
        Shape shape,
        Graph graph,
        IReadOnlyDictionary<string, GenerationTarget> resourceTypesByUri
    )
    {
        var propertyDefinitions = new HashSet<string>(
            target.OverriddenPropertyUris,
            StringComparer.Ordinal
        );
        var visited = new HashSet<string>(StringComparer.Ordinal)
        {
            target.ShapeUri ?? target.TypeName,
        };
        CollectInheritedOverriddenPropertyDefinitions(
            shape,
            graph,
            resourceTypesByUri,
            propertyDefinitions,
            visited
        );
        return propertyDefinitions;
    }

    private static void CollectInheritedOverriddenPropertyDefinitions(
        Shape shape,
        Graph graph,
        IReadOnlyDictionary<string, GenerationTarget> resourceTypesByUri,
        HashSet<string> propertyDefinitions,
        HashSet<string> visited
    )
    {
        foreach (string describedResource in shape.Describes)
        {
            foreach (
                Node superClass in graph
                    .Objects(describedResource, Rdfs + "subClassOf")
                    .Where(static node => node.Kind == NodeKind.Uri)
            )
            {
                if (
                    !resourceTypesByUri.TryGetValue(
                        superClass.Value,
                        out GenerationTarget? baseTarget
                    )
                    || baseTarget.ShapeUri is null
                    || !visited.Add(baseTarget.ShapeUri)
                )
                {
                    continue;
                }

                Shape baseShape = Shape.FromGraph(baseTarget.ShapeUri, graph);
                propertyDefinitions.UnionWith(baseTarget.OverriddenPropertyUris);
                if (shape.IsShaclNodeShape || baseShape.IsShaclNodeShape)
                {
                    propertyDefinitions.UnionWith(
                        baseShape.Properties.Select(static property => property.PropertyDefinition)
                    );
                }
                CollectInheritedOverriddenPropertyDefinitions(
                    baseShape,
                    graph,
                    resourceTypesByUri,
                    propertyDefinitions,
                    visited
                );
            }
        }
    }

    private static List<ShapePropertyBinding> BindProperties(
        GenerationTarget target,
        IEnumerable<ShapeProperty> properties
    )
    {
        var usedPropertyNames = new HashSet<string>(StringComparer.Ordinal)
        {
            target.TypeName,
            GetInterfaceName(target),
        };
        var bindings = new List<ShapePropertyBinding>();
        foreach (
            ShapeProperty property in properties.Where(static property =>
                !string.Equals(property.Name, "type", StringComparison.Ordinal)
            )
        )
        {
            bindings.Add(
                new ShapePropertyBinding(property, GetPropertyName(property, usedPropertyNames))
            );
        }

        return bindings;
    }

    private static List<GenerationTarget> GetDirectSuperTypes(
        GenerationTarget target,
        Shape shape,
        Graph graph,
        IReadOnlyDictionary<string, GenerationTarget> resourceTypesByUri
    )
    {
        var superTypes = new List<GenerationTarget>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (string describedResource in shape.Describes)
        {
            foreach (
                Node superClass in graph
                    .Objects(describedResource, Rdfs + "subClassOf")
                    .Where(static node => node.Kind == NodeKind.Uri)
            )
            {
                if (
                    resourceTypesByUri.TryGetValue(
                        superClass.Value,
                        out GenerationTarget? baseTarget
                    )
                    && !string.Equals(
                        baseTarget.TypeName,
                        target.TypeName,
                        StringComparison.Ordinal
                    )
                    && seen.Add(GetTypeReference(target, baseTarget))
                )
                {
                    superTypes.Add(baseTarget);
                }
            }
        }

        return superTypes;
    }

    private static string GetTypeReference(
        GenerationTarget target,
        GenerationTarget referencedTarget
    )
    {
        return
            string.Equals(target.Namespace, referencedTarget.Namespace, StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(referencedTarget.Namespace)
            ? referencedTarget.TypeName
            : "global::" + referencedTarget.Namespace + "." + referencedTarget.TypeName;
    }

    private static string GetInterfaceTypeReference(
        GenerationTarget target,
        GenerationTarget referencedTarget
    )
    {
        return
            string.Equals(target.Namespace, referencedTarget.Namespace, StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(referencedTarget.Namespace)
            ? GetInterfaceName(referencedTarget)
            : "global::" + referencedTarget.Namespace + "." + GetInterfaceName(referencedTarget);
    }

    private static string GetInterfaceName(GenerationTarget target)
    {
        return "I" + target.TypeName;
    }

    private static void AppendProperty(
        StringBuilder builder,
        ShapeProperty property,
        string propertyName
    )
    {
        AppendPropertyAttributes(builder, property, "    ");
        string typeName = GetClrType(property);
        builder
            .Append("    public ")
            .Append(typeName)
            .Append(' ')
            .Append(propertyName)
            .Append(" { get; set; }");
        if (typeName.StartsWith("HashSet<", StringComparison.Ordinal))
        {
            builder.Append(
                string.Equals(typeName, "HashSet<Uri>", StringComparison.Ordinal)
                    ? " = new(OslcUriEqualityComparer.Instance);"
                    : " = new();"
            );
        }
        else if (string.Equals(typeName, "string", StringComparison.Ordinal))
        {
            builder.Append(" = string.Empty;");
        }
        else if (IsReferenceType(typeName))
        {
            builder.Append(" = null!;");
        }

        builder.AppendLine();
    }

    private static void AppendPropertyAttributes(
        StringBuilder builder,
        ShapeProperty property,
        string indentation
    )
    {
        if (!string.IsNullOrWhiteSpace(property.Description))
        {
            builder
                .Append(indentation)
                .Append("[OslcDescription(")
                .Append(ToLiteral(property.Description!))
                .AppendLine(")]");
        }

        if (MapOccurs(property.Occurs) is { } occurs)
        {
            builder
                .Append(indentation)
                .Append("[OslcOccurs(Occurs.")
                .Append(occurs)
                .AppendLine(")]");
        }

        builder
            .Append(indentation)
            .Append("[OslcPropertyDefinition(")
            .Append(ToLiteral(property.PropertyDefinition))
            .AppendLine(")]");
        builder
            .Append(indentation)
            .Append("[OslcName(")
            .Append(ToLiteral(property.Name))
            .AppendLine(")]");

        if (MapValueType(property.ValueType) is { } valueType)
        {
            builder
                .Append(indentation)
                .Append("[OslcValueType(ValueType.")
                .Append(valueType)
                .AppendLine(")]");
        }

        if (MapRepresentation(property.Representation) is { } representation)
        {
            builder
                .Append(indentation)
                .Append("[OslcRepresentation(Representation.")
                .Append(representation)
                .AppendLine(")]");
        }

        if (property.Ranges.Count > 0)
        {
            builder
                .Append(indentation)
                .Append("[OslcRange(")
                .Append(string.Join(", ", property.Ranges.Select(ToLiteral)))
                .AppendLine(")]");
        }

        if (property.ReadOnly is { } readOnly)
        {
            builder
                .Append(indentation)
                .Append("[OslcReadOnly(")
                .Append(readOnly ? "true" : "false")
                .AppendLine(")]");
        }

        builder
            .Append(indentation)
            .Append("[OslcTitle(")
            .Append(ToLiteral(property.Title ?? property.Name))
            .AppendLine(")]");
    }

    private static string GetPropertyName(ShapeProperty property, HashSet<string> usedPropertyNames)
    {
        string baseName = ToIdentifier(property.Name);
        string propertyName = baseName;
        int suffix = 2;

        while (!usedPropertyNames.Add(propertyName))
        {
            propertyName = baseName + suffix.ToString(CultureInfo.InvariantCulture);
            suffix++;
        }

        return propertyName;
    }

    private static string NamespaceFromShape(Shape shape)
    {
        string? firstDescribe = shape.Describes.FirstOrDefault();
        if (firstDescribe is null)
        {
            return string.Empty;
        }

        int hash = firstDescribe.LastIndexOf('#');
        int slash = firstDescribe.LastIndexOf('/');
        int split = Math.Max(hash, slash);
        return split >= 0 ? firstDescribe.Substring(0, split + 1) : firstDescribe;
    }

    private static string GetClrType(ShapeProperty property)
    {
        string elementType = property.ValueType switch
        {
            Xsd + "anyURI" => "Uri",
            Xsd + "base64Binary" => "byte[]",
            Xsd + "boolean" => "bool",
            Xsd + "byte" => "sbyte",
            Xsd + "date" => "DateOnly",
            Xsd + "dateTime" => "DateTimeOffset",
            Xsd + "dateTimeStamp" => "DateTimeOffset",
            Xsd + "decimal" => "decimal",
            Xsd + "double" => "double",
            Xsd + "float" => "float",
            Xsd + "hexBinary" => "byte[]",
            Xsd + "int" => "int",
            // Known deviation: RDF/XSD integer is unbounded, but generated domains use long for now.
            Xsd + "integer" => "long",
            Xsd + "long" => "long",
            Xsd + "negativeInteger" => "long",
            Xsd + "nonNegativeInteger" => "long",
            Xsd + "nonPositiveInteger" => "long",
            Xsd + "positiveInteger" => "long",
            Xsd + "short" => "short",
            Xsd + "time" => "TimeOnly",
            Xsd + "unsignedByte" => "byte",
            Xsd + "unsignedInt" => "uint",
            Xsd + "unsignedLong" => "ulong",
            Xsd + "unsignedShort" => "ushort",
            Core + "Resource" => "Uri",
            Core + "AnyResource" => "Uri",
            Core + "LocalResource" => "Uri",
            _ => "string",
        };

        if (IsMany(property.Occurs))
        {
            return $"HashSet<{elementType}>";
        }

        if (IsValueType(elementType))
        {
            return IsRequired(property.Occurs) ? elementType : elementType + "?";
        }

        if (IsReferenceType(elementType))
        {
            return IsRequired(property.Occurs) ? elementType : elementType + "?";
        }

        return elementType;
    }

    private static bool IsMany(string? occurs)
    {
        return occurs is Core + "Zero-or-many" or Core + "One-or-many";
    }

    private static bool IsRequired(string? occurs)
    {
        return string.Equals(occurs, Core + "Exactly-one", StringComparison.Ordinal);
    }

    private static bool IsValueType(string typeName)
    {
        return typeName
            is "DateOnly"
                or "DateTimeOffset"
                or "TimeOnly"
                or "bool"
                or "byte"
                or "decimal"
                or "double"
                or "float"
                or "int"
                or "long"
                or "sbyte"
                or "short"
                or "uint"
                or "ulong"
                or "ushort";
    }

    private static bool IsReferenceType(string typeName)
    {
        return typeName is "Uri" or "string" or "byte[]";
    }

    private static string? MapOccurs(string? uri)
    {
        return uri switch
        {
            Core + "Exactly-one" => "ExactlyOne",
            Core + "Zero-or-one" => "ZeroOrOne",
            Core + "Zero-or-many" => "ZeroOrMany",
            Core + "One-or-many" => "OneOrMany",
            _ => null,
        };
    }

    private static string? MapValueType(string? uri)
    {
        return uri switch
        {
            Xsd + "anyURI" => "AnyUri",
            Xsd + "base64Binary" => "Base64Binary",
            Xsd + "boolean" => "Boolean",
            Xsd + "byte" => "Byte",
            Xsd + "date" => "Date",
            Xsd + "dateTime" => "DateTime",
            Xsd + "dateTimeStamp" => "DateTimeStamp",
            Xsd + "dayTimeDuration" => "DayTimeDuration",
            Xsd + "decimal" => "Decimal",
            Xsd + "double" => "Double",
            Xsd + "duration" => "Duration",
            Xsd + "float" => "Float",
            Xsd + "gDay" => "GDay",
            Xsd + "gMonth" => "GMonth",
            Xsd + "gMonthDay" => "GMonthDay",
            Xsd + "gYear" => "GYear",
            Xsd + "gYearMonth" => "GYearMonth",
            Xsd + "hexBinary" => "HexBinary",
            Xsd + "int" => "Int",
            Xsd + "integer" => "Integer",
            Xsd + "language" => "Language",
            Xsd + "long" => "Long",
            Xsd + "Name" => "Name",
            Xsd + "NCName" => "NCName",
            Xsd + "negativeInteger" => "NegativeInteger",
            Xsd + "NMTOKEN" => "Nmtoken",
            Xsd + "nonNegativeInteger" => "NonNegativeInteger",
            Xsd + "nonPositiveInteger" => "NonPositiveInteger",
            Xsd + "normalizedString" => "NormalizedString",
            Xsd + "positiveInteger" => "PositiveInteger",
            Xsd + "short" => "Short",
            Xsd + "string" => "String",
            Xsd + "time" => "Time",
            Xsd + "token" => "Token",
            Xsd + "unsignedByte" => "UnsignedByte",
            Xsd + "unsignedInt" => "UnsignedInt",
            Xsd + "unsignedLong" => "UnsignedLong",
            Xsd + "unsignedShort" => "UnsignedShort",
            Xsd + "yearMonthDuration" => "YearMonthDuration",
            Rdf + "dirLangString" => "DirLangString",
            Rdf + "HTML" => "Html",
            Rdf + "JSON" => "Json",
            Rdf + "langString" => "LangString",
            Rdf + "XMLLiteral" => "XMLLiteral",
            Core + "AnyResource" => "AnyResource",
            Core + "LocalResource" => "LocalResource",
            Core + "Resource" => "Resource",
            _ => null,
        };
    }

    private static string? MapRepresentation(string? uri)
    {
        return uri switch
        {
            Core + "Either" => "Either",
            Core + "Inline" => "Inline",
            Core + "Reference" => "Reference",
            _ => null,
        };
    }

    private static string LocalName(string uri)
    {
        int hash = uri.LastIndexOf('#');
        int slash = uri.LastIndexOf('/');
        int index = Math.Max(hash, slash);
        return index >= 0 && index + 1 < uri.Length ? uri.Substring(index + 1) : uri;
    }

    private static string ToIdentifier(string value)
    {
        var builder = new StringBuilder(value.Length);
        bool nextUpper = true;
        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c))
            {
                builder.Append(nextUpper ? char.ToUpperInvariant(c) : c);
                nextUpper = false;
            }
            else
            {
                nextUpper = true;
            }
        }

        if (builder.Length == 0 || char.IsDigit(builder[0]))
        {
            builder.Insert(0, '_');
        }

        return builder.ToString();
    }

    private static string Humanize(string value)
    {
        var builder = new StringBuilder(value.Length + 8);
        for (int i = 0; i < value.Length; i++)
        {
            if (i > 0 && char.IsUpper(value[i]) && !char.IsUpper(value[i - 1]))
            {
                builder.Append(' ');
            }

            builder.Append(value[i]);
        }

        return builder.ToString();
    }

    private static string ToLiteral(string value)
    {
        return "\""
            + value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t")
            + "\"";
    }

    private static string? FirstValue(IEnumerable<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            return node.Value;
        }

        return null;
    }

    private static string? FirstPlainValue(IEnumerable<Node> nodes)
    {
        List<Node> values = nodes.ToList();
        return FirstValue(values.Where(static node => node.Language is null)) ?? FirstValue(values);
    }

    private static void AppendXmlSummary(
        StringBuilder builder,
        string indentation,
        string? description
    )
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return;
        }

        builder.Append(indentation).AppendLine("/// <summary>");
        foreach (string line in NormalizeXmlDocLines(description))
        {
            builder.Append(indentation).Append("///");
            if (line.Length > 0)
            {
                builder.Append(' ').Append(EscapeXmlDoc(line));
            }

            builder.AppendLine();
        }

        builder.Append(indentation).AppendLine("/// </summary>");
    }

    private static IEnumerable<string> NormalizeXmlDocLines(string description)
    {
        using var reader = new StringReader(description.Replace("\r\n", "\n").Replace('\r', '\n'));
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            yield return NeutralizeMarkdownLinks(line.Trim());
        }
    }

    private static string NeutralizeMarkdownLinks(string value)
    {
        var builder = new StringBuilder(value.Length);
        int position = 0;
        while (position < value.Length)
        {
            int labelStart = value.IndexOf('[', position);
            if (labelStart < 0)
            {
                builder.Append(value, position, value.Length - position);
                break;
            }

            int labelEnd = value.IndexOf(']', labelStart + 1);
            if (
                labelEnd < 0
                || labelEnd + 1 >= value.Length
                || value[labelEnd + 1] != '('
            )
            {
                builder.Append(value, position, labelStart - position + 1);
                position = labelStart + 1;
                continue;
            }

            int linkEnd = value.IndexOf(')', labelEnd + 2);
            if (linkEnd < 0)
            {
                builder.Append(value, position, labelStart - position + 1);
                position = labelStart + 1;
                continue;
            }

            builder.Append(value, position, labelStart - position);
            builder.Append(value, labelStart + 1, labelEnd - labelStart - 1);
            builder.Append(" (");
            builder.Append(value, labelEnd + 2, linkEnd - labelEnd - 2);
            builder.Append(')');
            position = linkEnd + 1;
        }

        return builder.ToString();
    }

    private static string EscapeXmlDoc(string value)
    {
        return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }

    private static void AppendAutoGeneratedHeader(StringBuilder builder)
    {
        builder.AppendLine("// <auto-generated />");
    }

    private static void AppendNamespaceStart(StringBuilder builder, string @namespace)
    {
        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            builder.Append("namespace ").Append(@namespace).AppendLine(";");
            builder.AppendLine();
        }
    }

#pragma warning disable IDE0060 // Remove params
    // no-op for consistency with AppendNamespaceStart
    private static void AppendNamespaceEnd(StringBuilder builder, string @namespace) { }
#pragma warning restore IDE0060 // Remove params

    private sealed class GenerationTarget
    {
        public GenerationTarget(
            string @namespace,
            string typeName,
            bool isRecord,
            string? vocabularyUri,
            string? vocabularyPrefix,
            string? shapeUri,
            string? shapeTitle,
            ImmutableArray<string> selectedPropertyUris,
            ImmutableArray<string> overriddenPropertyUris
        )
        {
            Namespace = @namespace;
            TypeName = typeName;
            IsRecord = isRecord;
            VocabularyUri = vocabularyUri;
            VocabularyPrefix = vocabularyPrefix;
            ShapeUri = shapeUri;
            ShapeTitle = shapeTitle;
            SelectedPropertyUris = selectedPropertyUris;
            OverriddenPropertyUris = overriddenPropertyUris;
        }

        public string Namespace { get; }
        public string TypeName { get; }
        public bool IsRecord { get; }
        public string? VocabularyUri { get; }
        public string? VocabularyPrefix { get; }
        public string? ShapeUri { get; }
        public string? ShapeTitle { get; }
        public ImmutableArray<string> SelectedPropertyUris { get; }
        public ImmutableArray<string> OverriddenPropertyUris { get; }
    }

    private readonly struct ShapePropertyBinding
    {
        public ShapePropertyBinding(ShapeProperty property, string propertyName)
        {
            Property = property;
            PropertyName = propertyName;
        }

        public ShapeProperty Property { get; }
        public string PropertyName { get; }
    }

    private sealed class Shape
    {
        public string? Title { get; private set; }
        public bool IsShaclNodeShape { get; private set; }
        public List<string> Describes { get; } = new();
        public List<ShapeProperty> Properties { get; } = new();

        public static Shape FromGraph(string shapeUri, Graph graph)
        {
            bool isShaclNodeShape = graph
                .Objects(shapeUri, Rdf + "type")
                .Any(static node =>
                    node.Kind == NodeKind.Uri
                    && string.Equals(node.Value, Sh + "NodeShape", StringComparison.Ordinal)
            );
            var shape = new Shape
            {
                IsShaclNodeShape = isShaclNodeShape,
                Title =
                    FirstValue(
                        graph
                            .Objects(shapeUri, DcTerms + "title")
                            .Where(static node => node.Language is null)
                    )
                    ?? FirstValue(graph.Objects(shapeUri, DcTerms + "title"))
                    ?? FirstPlainValue(graph.Objects(shapeUri, Rdfs + "label")),
            };

            shape.Describes.AddRange(
                graph
                    .Objects(shapeUri, Core + "describes")
                    .Where(static node => node.Kind == NodeKind.Uri)
                    .Select(static node => node.Value)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(static value => value, StringComparer.Ordinal)
            );

            if (shape.Describes.Count == 0 && isShaclNodeShape)
            {
                shape.Describes.AddRange(
                    graph
                        .Objects(shapeUri, Sh + "targetClass")
                        .Where(static node => node.Kind == NodeKind.Uri)
                        .Select(static node => node.Value)
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(static value => value, StringComparer.Ordinal)
                );
                if (shape.Describes.Count == 0)
                {
                    shape.Describes.Add(shapeUri);
                }
            }

            foreach (
                Node propertyNode in graph
                    .Objects(shapeUri, Core + "property")
                    .Distinct()
                    .OrderBy(static node => node.Value, StringComparer.Ordinal)
            )
            {
                ShapeProperty? property = ShapeProperty.FromGraph(propertyNode, graph);
                if (property is not null)
                {
                    shape.Properties.Add(property);
                }
            }

            if (shape.Properties.Count == 0 && isShaclNodeShape)
            {
                foreach (
                    Node propertyNode in graph
                        .Objects(shapeUri, Sh + "property")
                        .Distinct()
                        .OrderBy(static node => node.Value, StringComparer.Ordinal)
                )
                {
                    ShapeProperty? property = ShapeProperty.FromShaclGraph(propertyNode, graph);
                    if (property is not null)
                    {
                        shape.Properties.Add(property);
                    }
                }
            }

            shape.DeduplicateProperties();
            shape.Properties.Sort(
                static (left, right) => string.CompareOrdinal(left.Name, right.Name)
            );
            return shape;
        }

        private void DeduplicateProperties()
        {
            List<ShapeProperty> properties = Properties
                .GroupBy(static property => property.PropertyDefinition, StringComparer.Ordinal)
                .Select(static group =>
                    group
                        .OrderByDescending(static property => property.ConstraintScore)
                        .ThenBy(static property => property.Name, StringComparer.Ordinal)
                        .First()
                )
                .ToList();

            Properties.Clear();
            Properties.AddRange(properties);
        }
    }

    private sealed class ShapeProperty
    {
        public string Name { get; private set; } = string.Empty;
        public string PropertyDefinition { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? Occurs { get; private set; }
        public string? ReadOnlyText { get; private set; }
        public bool? ReadOnly => bool.TryParse(ReadOnlyText, out bool value) ? value : null;
        public string? Representation { get; private set; }
        public string? Title { get; private set; }
        public string? ValueType { get; private set; }
        public List<string> Ranges { get; } = new();
        public int ConstraintScore =>
            (Occurs is null ? 0 : 1)
            + (ReadOnlyText is null ? 0 : 1)
            + (Representation is null ? 0 : 1)
            + (ValueType is null ? 0 : 1)
            + Ranges.Count;

        public static ShapeProperty? FromGraph(Node node, Graph graph)
        {
            string? name = FirstValue(graph.Objects(node, Core + "name"));
            string? propertyDefinition = FirstValue(
                graph.Objects(node, Core + "propertyDefinition")
            );
            if (name is null || propertyDefinition is null)
            {
                return null;
            }

            var property = new ShapeProperty
            {
                Name = name,
                PropertyDefinition = propertyDefinition,
                Description = FirstValue(graph.Objects(node, DcTerms + "description")),
                Occurs = FirstValue(graph.Objects(node, Core + "occurs")),
                ReadOnlyText = FirstValue(graph.Objects(node, Core + "readOnly")),
                Representation = FirstValue(graph.Objects(node, Core + "representation")),
                Title =
                    FirstValue(
                        graph
                            .Objects(node, DcTerms + "title")
                            .Where(static title => title.Language is null)
                    ) ?? FirstValue(graph.Objects(node, DcTerms + "title")),
                ValueType = FirstValue(graph.Objects(node, Core + "valueType")),
            };

            property.Ranges.AddRange(
                graph
                    .Objects(node, Core + "range")
                    .Where(static range => range.Kind == NodeKind.Uri)
                    .Select(static range => range.Value)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(static range => range, StringComparer.Ordinal)
            );

            return property;
        }

        public static ShapeProperty? FromShaclGraph(Node node, Graph graph)
        {
            string? pathUri = FirstValue(
                graph
                .Objects(node, Sh + "path")
                    .Where(static node => node.Kind == NodeKind.Uri)
            );
            if (pathUri is null)
            {
                return null;
            }

            string name = FirstPlainValue(graph.Objects(node, Sh + "name")) ?? LocalName(pathUri);
            var property = new ShapeProperty
            {
                Name = name,
                PropertyDefinition = pathUri,
                Description =
                    FirstPlainValue(graph.Objects(node, Sh + "description"))
                    ?? VocabularyDescription(pathUri, graph),
                Occurs = ShaclOccurs(
                    FirstValue(graph.Objects(node, Sh + "minCount")),
                    FirstValue(graph.Objects(node, Sh + "maxCount"))
                ),
                Representation = ShaclRepresentation(
                    FirstValue(graph.Objects(node, Sh + "nodeKind"))
                ),
                Title =
                    FirstPlainValue(graph.Objects(node, Sh + "name"))
                    ?? FirstPlainValue(graph.Objects(pathUri, Rdfs + "label"))
                    ?? name,
                ValueType = ShaclValueType(
                    FirstValue(graph.Objects(node, Sh + "datatype")),
                    FirstValue(graph.Objects(node, Sh + "nodeKind")),
                    graph.Objects(node, Sh + "class").Any(static range => range.Kind == NodeKind.Uri)
                ),
            };

            property.Ranges.AddRange(
                graph
                    .Objects(node, Sh + "class")
                    .Where(static range => range.Kind == NodeKind.Uri)
                    .Select(static range => range.Value)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(static range => range, StringComparer.Ordinal)
            );

            return property;
        }

        private static string? ShaclOccurs(string? minCount, string? maxCount)
        {
            int? min = ParseShaclCount(minCount);
            int? max = ParseShaclCount(maxCount);

            if (min >= 1 && max == 1)
            {
                return Core + "Exactly-one";
            }

            if (min >= 1)
            {
                return Core + "One-or-many";
            }

            if (max == 1)
            {
                return Core + "Zero-or-one";
            }

            return null;
        }

        private static int? ParseShaclCount(string? value)
        {
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int count)
                ? count
                : null;
        }

        private static string? ShaclValueType(
            string? datatype,
            string? nodeKind,
            bool hasClassRange
        )
        {
            if (!string.IsNullOrWhiteSpace(datatype))
            {
                return datatype;
            }

            return nodeKind switch
            {
                Sh + "BlankNode" => Core + "LocalResource",
                Sh + "BlankNodeOrIRI" => Core + "AnyResource",
                Sh + "IRI" => Core + "Resource",
                Sh + "Literal" => Xsd + "string",
                _ when hasClassRange => Core + "Resource",
                _ => null,
            };
        }

        private static string? ShaclRepresentation(string? nodeKind)
        {
            return nodeKind switch
            {
                Sh + "BlankNode" => Core + "Inline",
                Sh + "BlankNodeOrIRI" => Core + "Either",
                Sh + "IRI" => Core + "Reference",
                _ => null,
            };
        }
    }

    private sealed class Graph
    {
        private readonly Dictionary<(Node Subject, string Predicate), List<Node>> _objects = new();
        private readonly List<Triple> _triples = new();

        public static Graph Load(
            ImmutableArray<AdditionalText> files,
            System.Threading.CancellationToken cancellationToken
        )
        {
            var graph = new Graph();
            foreach (AdditionalText file in files)
            {
                SourceText? text = file.GetText(cancellationToken);
                if (text is null)
                {
                    continue;
                }

                foreach (Triple triple in NTriplesParser.Parse(text.ToString()))
                {
                    graph.Add(triple);
                }
            }

            return graph;
        }

        public IEnumerable<Node> Objects(string subjectUri, string predicateUri)
        {
            return Objects(Node.Uri(subjectUri), predicateUri);
        }

        public IEnumerable<Node> Objects(Node subject, string predicateUri)
        {
            return _objects.TryGetValue((subject, predicateUri), out List<Node> objects)
                ? objects
                : Enumerable.Empty<Node>();
        }

        public IEnumerable<string> Subjects(string predicateUri, string objectUri)
        {
            return _triples
                .Where(triple =>
                    string.Equals(triple.Predicate.Value, predicateUri, StringComparison.Ordinal)
                    && triple.Object.Kind == NodeKind.Uri
                    && string.Equals(triple.Object.Value, objectUri, StringComparison.Ordinal)
                    && triple.Subject.Kind == NodeKind.Uri
                )
                .Select(triple => triple.Subject.Value)
                .Distinct(StringComparer.Ordinal);
        }

        private void Add(Triple triple)
        {
            _triples.Add(triple);
            var key = (triple.Subject, triple.Predicate.Value);
            if (!_objects.TryGetValue(key, out List<Node> objects))
            {
                objects = new List<Node>();
                _objects.Add(key, objects);
            }

            objects.Add(triple.Object);
        }
    }

    private static class NTriplesParser
    {
        public static IEnumerable<Triple> Parse(string content)
        {
            using var reader = new StringReader(content);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                string trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed[0] == '#')
                {
                    continue;
                }

                int index = 0;
                Node subject = ParseNode(trimmed, ref index);
                SkipWhiteSpace(trimmed, ref index);
                Node predicate = ParseNode(trimmed, ref index);
                SkipWhiteSpace(trimmed, ref index);
                Node @object = ParseNode(trimmed, ref index);
                yield return new Triple(subject, predicate, @object);
            }
        }

        private static Node ParseNode(string line, ref int index)
        {
            return line[index] switch
            {
                '<' => ParseUri(line, ref index),
                '_' => ParseBlank(line, ref index),
                '"' => ParseLiteral(line, ref index),
                _ => throw new InvalidDataException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unsupported N-Triples node at index {0}.",
                        index
                    )
                ),
            };
        }

        private static Node ParseUri(string line, ref int index)
        {
            int end = line.IndexOf('>', index + 1);
            string uri = Unescape(line.Substring(index + 1, end - index - 1));
            index = end + 1;
            return Node.Uri(uri);
        }

        private static Node ParseBlank(string line, ref int index)
        {
            int start = index;
            while (index < line.Length && !char.IsWhiteSpace(line[index]))
            {
                index++;
            }

            return Node.Blank(line.Substring(start, index - start));
        }

        private static Node ParseLiteral(string line, ref int index)
        {
            index++;
            var builder = new StringBuilder();
            while (index < line.Length)
            {
                char current = line[index++];
                if (current == '\\' && index < line.Length)
                {
                    builder.Append(UnescapeCharacter(line[index++]));
                    continue;
                }

                if (current == '"')
                {
                    break;
                }

                builder.Append(current);
            }

            string? language = null;
            if (index < line.Length && line[index] == '@')
            {
                index++;
                int languageStart = index;
                while (index < line.Length && !char.IsWhiteSpace(line[index]))
                {
                    index++;
                }

                language = line.Substring(languageStart, index - languageStart);
            }
            else if (index + 1 < line.Length && line[index] == '^' && line[index + 1] == '^')
            {
                index += 2;
                _ = ParseUri(line, ref index);
            }

            return Node.Literal(builder.ToString(), language);
        }

        private static void SkipWhiteSpace(string line, ref int index)
        {
            while (index < line.Length && char.IsWhiteSpace(line[index]))
            {
                index++;
            }
        }

        private static string Unescape(string value)
        {
            return value.Replace("\\>", ">");
        }

        private static char UnescapeCharacter(char value)
        {
            return value switch
            {
                't' => '\t',
                'r' => '\r',
                'n' => '\n',
                '"' => '"',
                '\\' => '\\',
                _ => value,
            };
        }
    }

    private readonly struct Triple
    {
        public Triple(Node subject, Node predicate, Node @object)
        {
            Subject = subject;
            Predicate = predicate;
            Object = @object;
        }

        public Node Subject { get; }
        public Node Predicate { get; }
        public Node Object { get; }
    }

    private readonly struct Node : IEquatable<Node>
    {
        private Node(NodeKind kind, string value, string? language = null)
        {
            Kind = kind;
            Value = value;
            Language = language;
        }

        public NodeKind Kind { get; }
        public string Value { get; }
        public string? Language { get; }

        public static Node Uri(string value)
        {
            return new Node(NodeKind.Uri, value);
        }

        public static Node Blank(string value)
        {
            return new Node(NodeKind.Blank, value);
        }

        public static Node Literal(string value, string? language)
        {
            return new Node(NodeKind.Literal, value, language);
        }

        public bool Equals(Node other)
        {
            return Kind == other.Kind
                && string.Equals(Value, other.Value, StringComparison.Ordinal)
                && string.Equals(Language, other.Language, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is Node other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)Kind;
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(Value);
                hash =
                    (hash * 397)
                    ^ (Language is null ? 0 : StringComparer.Ordinal.GetHashCode(Language));
                return hash;
            }
        }
    }

    private enum NodeKind
    {
        Uri,
        Blank,
        Literal,
    }
}
