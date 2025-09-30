using VDS.RDF;

namespace OSLC4Net.Server.Providers;

internal readonly struct SerializationContext
{
    public IGraph Graph { get; init; }
    public RdfFormat? Format { get; init; }
}
