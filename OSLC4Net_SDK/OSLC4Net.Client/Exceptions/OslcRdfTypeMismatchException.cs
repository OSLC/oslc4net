/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 * Copyright (c) 2025 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *******************************************************************************/

using OSLC4Net.Core.Exceptions;

namespace OSLC4Net.Client.Exceptions;

/// <summary>
///     Exception thrown when there is an RDF type mismatch.
/// </summary>
public class OslcRdfTypeMismatchException : OslcCoreApplicationException
{
    /// <summary>
    ///     Gets the expected RDF types.
    /// </summary>
    public string ExpectedTypes { get; }

    /// <summary>
    ///     Gets the actual RDF types found.
    /// </summary>
    public string ActualTypes { get; }

    /// <summary>
    ///     Gets the URI of the resource with the type mismatch.
    /// </summary>
    public string ResourceUri { get; }

    /// <summary>
    ///     Initializes a new instance of the OslcRdfTypeMismatchException class.
    /// </summary>
    /// <param name="resourceUri">The URI of the resource with the type mismatch.</param>
    /// <param name="expectedTypes">The expected RDF types.</param>
    /// <param name="actualTypes">The actual RDF types found.</param>
    public OslcRdfTypeMismatchException(string resourceUri, string expectedTypes,
        string actualTypes)
        : base(
            $"RDF type mismatch for resource <{resourceUri}>. Expected one of: [{expectedTypes}], but found: [{actualTypes}].")
    {
        ResourceUri = resourceUri;
        ExpectedTypes = expectedTypes;
        ActualTypes = actualTypes;
    }

    /// <summary>
    ///     Initializes a new instance of the OslcRdfTypeMismatchException class.
    /// </summary>
    /// <param name="resourceUri">The URI of the resource with the type mismatch.</param>
    /// <param name="expectedTypes">The expected RDF types.</param>
    /// <param name="actualTypes">The actual RDF types found.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public OslcRdfTypeMismatchException(string resourceUri, string expectedTypes,
        string actualTypes, Exception innerException)
        : base(
            $"RDF type mismatch for resource <{resourceUri}>. Expected one of: [{expectedTypes}], but found: [{actualTypes}].",
            innerException)
    {
        ResourceUri = resourceUri;
        ExpectedTypes = expectedTypes;
        ActualTypes = actualTypes;
    }
}
