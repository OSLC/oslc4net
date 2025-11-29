// SPDX-FileCopyrightText: 2024 OSLC and contributors
//
// SPDX-License-Identifier: Apache-2.0

namespace OSLC4Net.Client.Oslc;

/// <summary>
/// Represents OAuth 1.0a configuration from an OSLC Root Services document.
/// </summary>
public sealed class RootServicesOAuth10aDocument
{
    /// <summary>
    /// Gets the OAuth request token URL.
    /// </summary>
    public string? RequestTokenUrl { get; }

    /// <summary>
    /// Gets the OAuth user authorization URL.
    /// </summary>
    public string? AuthorizationUrl { get; }

    /// <summary>
    /// Gets the OAuth access token URL.
    /// </summary>
    public string? AccessTokenUrl { get; }

    /// <summary>
    /// Gets the OAuth request consumer key URL (optional).
    /// </summary>
    public string? RequestConsumerKeyUrl { get; }

    /// <summary>
    /// Gets the OAuth consumer approval URL (optional).
    /// </summary>
    public string? ApprovalUrl { get; }

    /// <summary>
    /// Gets the OAuth realm name (optional).
    /// </summary>
    public string? Realm { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RootServicesOAuth10aDocument"/> class.
    /// </summary>
    public RootServicesOAuth10aDocument(
        string? requestTokenUrl = null,
        string? authorizationUrl = null,
        string? accessTokenUrl = null,
        string? requestConsumerKeyUrl = null,
        string? approvalUrl = null,
        string? realm = null)
    {
        RequestTokenUrl = requestTokenUrl;
        AuthorizationUrl = authorizationUrl;
        AccessTokenUrl = accessTokenUrl;
        RequestConsumerKeyUrl = requestConsumerKeyUrl;
        ApprovalUrl = approvalUrl;
        Realm = realm;
    }
}
