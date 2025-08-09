/*******************************************************************************
 * Copyright (c) 2012, 2013 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

namespace OSLC4Net.Core.Model;

/// <summary>
///     Class representing namespace-qualified names
/// </summary>
public class QName
{
    /// <summary>
    ///     Constructor with local part only
    /// </summary>
    /// <param name="localPart"></param>
    public QName(string localPart)
    {
        LocalPart = localPart ?? throw new ArgumentNullException(nameof(localPart));
    }

    /// <summary>
    ///     Constructior with namespace and local part
    /// </summary>
    /// <param name="namespaceUri"></param>
    /// <param name="localPart"></param>
    public QName(
        string namespaceUri,
        string localPart
    ) : this(namespaceUri, localPart, null)
    {
    }

    /// <summary>
    ///     Constructor with namespace, local part and prefix/alias
    /// </summary>
    /// <param name="namespaceUri"></param>
    /// <param name="localPart"></param>
    /// <param name="prefix"></param>
    public QName(
        string namespaceUri,
        string localPart,
        string? prefix
    )
    {
        NamespaceUri = namespaceUri ?? throw new ArgumentNullException(nameof(namespaceUri));
        LocalPart = localPart ?? throw new ArgumentNullException(nameof(localPart));
        Prefix = prefix;
    }

    /// <summary>
    ///     URI of the namespace, e.g. <c>http://open-services.net/ns/rm#</c>
    /// </summary>
    public string? NamespaceUri { get; }

    /// <summary>
    ///     URI part the comes after the namespace e.g. <c>Requirement</c> for URI
    ///     <c>http://open-services.net/ns/rm#Requirement</c>
    /// </summary>
    public string LocalPart { get; }

    /// <summary>
    ///     Prefix for the namespace e.g. <c>oslc_rm</c> for namespace
    ///     <c>http://open-services.net/ns/rm#</c>
    /// </summary>
    public string? Prefix { get; }

    [Obsolete("Use .NamespaceUri instead. This method will be removed in a future release.")]
    public string? GetNamespaceURI()
    {
        return NamespaceUri;
    }

    [Obsolete("Use .LocalPart instead. This method will be removed in a future release.")]
    public string GetLocalPart()
    {
        return LocalPart;
    }

    [Obsolete("Use .Prefix instead. This method will be removed in a future release.")]
    public string? GetPrefix()
    {
        return Prefix;
    }

    public override string ToString()
    {
        if (NamespaceUri == null)
        {
            return LocalPart;
        }

        return '{' + NamespaceUri + '}' + LocalPart;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is QName qNameOther)
        {
            return $"{NamespaceUri}{LocalPart}".Equals(
                $"{qNameOther.NamespaceUri}{qNameOther.LocalPart}",
                StringComparison.InvariantCulture);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return (NamespaceUri is null ? 0 : StringComparer.Ordinal.GetHashCode(NamespaceUri)) * 31
               + StringComparer.Ordinal.GetHashCode(LocalPart);
    }
}
