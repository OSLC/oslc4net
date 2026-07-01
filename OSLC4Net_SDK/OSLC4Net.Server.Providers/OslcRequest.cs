/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Model;
using VDS.RDF;

namespace OSLC4Net.Server.Providers;

public sealed class OslcRequest<T> where T : IResource
{
    public OslcRequest(IReadOnlyList<T> resources, IGraph graph)
    {
        Resources = resources;
        Graph = graph;
    }

    public IGraph Graph { get; }

    public IReadOnlyList<T> Resources { get; }

    public T? Resource => Resources.Count > 0 ? Resources[0] : default;
}
