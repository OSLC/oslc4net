/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using System.Collections;
using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.DotNetRdfProvider;
using OSLC4Net.Core.Model;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace OSLC4Net.Server.Providers;

internal static class OslcResourceMaterializer
{
    public static IReadOnlyList<T> Materialize<T>(IGraph graph, DotNetRdfHelper rdfHelper)
        where T : IResource
    {
        return (IReadOnlyList<T>)Materialize(graph, typeof(T), rdfHelper);
    }

    public static object Materialize(IGraph graph, Type requestedType, DotNetRdfHelper rdfHelper)
    {
        if (RequiresPolymorphicResolution(requestedType))
        {
            return GraphToPolymorphicResources(graph, requestedType, rdfHelper);
        }

        return rdfHelper.FromDotNetRdfGraph(graph, requestedType);
    }

    private static object GraphToPolymorphicResources(
        IGraph graph,
        Type requestedType,
        DotNetRdfHelper rdfHelper)
    {
        var listType = typeof(List<>).MakeGenericType(requestedType);
        var resources = (IList)Activator.CreateInstance(listType)!;
        var add = listType.GetMethod("Add", [requestedType])!;
        foreach ((IUriNode subject, Type concreteType) in ResolveResourceTypes(graph, requestedType))
        {
            object resource = rdfHelper.FromDotNetRdfNode(subject, graph, concreteType);
            add.Invoke(resources, [resource]);
        }

        return resources;
    }

    public static bool RequiresPolymorphicResolution(Type type)
    {
        return type.IsInterface || type.IsAbstract;
    }

    private static IEnumerable<(IUriNode Subject, Type ConcreteType)> ResolveResourceTypes(
        IGraph graph,
        Type requestedType)
    {
        IUriNode rdfType = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        Dictionary<string, Type> candidateTypes = GetCandidateResourceTypes(requestedType);
        var candidatesBySubject = new Dictionary<IUriNode, List<Type>>();
        foreach (Triple triple in graph.GetTriplesWithPredicate(rdfType))
        {
            if (triple.Subject is not IUriNode subject || triple.Object is not IUriNode objectNode)
            {
                continue;
            }

            if (!candidateTypes.TryGetValue(objectNode.Uri.AbsoluteUri, out Type? candidateType))
            {
                continue;
            }

            if (!candidatesBySubject.TryGetValue(subject, out List<Type>? subjectCandidates))
            {
                subjectCandidates = new List<Type>();
                candidatesBySubject.Add(subject, subjectCandidates);
            }

            if (!subjectCandidates.Contains(candidateType))
            {
                subjectCandidates.Add(candidateType);
            }
        }

        return candidatesBySubject
            .OrderBy(entry => entry.Key.Uri.AbsoluteUri, StringComparer.Ordinal)
            .Select(entry => (entry.Key, GetMostConcreteType(entry.Key, entry.Value)));
    }

    private static Dictionary<string, Type> GetCandidateResourceTypes(Type requestedType)
    {
        var candidates = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies()
            .Where(static assembly => !assembly.IsDynamic)
            .SelectMany(GetLoadableTypes)
            .Where(type => !type.IsAbstract &&
                !type.IsInterface &&
                requestedType.IsAssignableFrom(type)))
        {
            foreach (OslcResourceShape resourceShape in type.GetCustomAttributes(typeof(OslcResourceShape), false))
            {
                foreach (string describedType in resourceShape.describes)
                {
                    candidates[describedType] = type;
                }
            }
        }

        return candidates;
    }

    private static IEnumerable<Type> GetLoadableTypes(System.Reflection.Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (System.Reflection.ReflectionTypeLoadException ex)
        {
            return ex.Types.OfType<Type>();
        }
    }

    private static Type GetMostConcreteType(IUriNode subject, List<Type> candidates)
    {
        for (int i = candidates.Count - 1; i >= 0; i--)
        {
            Type current = candidates[i];
            if (candidates.Any(candidate => current != candidate && current.IsAssignableFrom(candidate)))
            {
                candidates.RemoveAt(i);
            }
        }

        if (candidates.Count == 1)
        {
            return candidates[0];
        }

        throw new InvalidOperationException(
            $"Multiple unrelated CLR resource types match RDF resource '{subject.Uri}': {string.Join(", ", candidates.Select(static type => type.FullName))}.");
    }
}
