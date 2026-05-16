namespace OSLC4Net.Client.Oslc;

/// <summary>
/// Represents an OSLC Root Services document with service provider catalog and optional OAuth configuration.
/// </summary>
public sealed class RootServicesDocument
{
    /// <summary>
    /// Gets the URI of the service provider catalog.
    /// </summary>
    public string ServiceProviderCatalog { get; }

    /// <summary>
    /// Gets the OAuth 1.0a configuration if present in the root services document.
    /// </summary>
    public RootServicesOAuth10aDocument? OAuth { get; }

    /// <summary>
    /// Gets the title of the root services document (optional).
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RootServicesDocument"/> class.
    /// </summary>
    public RootServicesDocument(
        string serviceProviderCatalog,
        RootServicesOAuth10aDocument? oAuth = null,
        string? title = null)
    {
        ServiceProviderCatalog = serviceProviderCatalog ?? throw new ArgumentNullException(nameof(serviceProviderCatalog));
        OAuth = oAuth;
        Title = title;
    }
}
