using System.Runtime.InteropServices;
using VDS.RDF.JsonLd.Syntax;
using VDS.RDF.Writing;

namespace OSLC4Net.Server.Providers;

[StructLayout(LayoutKind.Auto)]
public readonly struct OslcOutputFormatConfig
{
    public OslcOutputFormatConfig()
    {
    }

    public bool PrettyPrint { get; init; } = true;

    /// <summary>
    ///     See <see cref="WriterCompressionLevel" />
    /// </summary>
    public int CompressionLevel { get; init; } = WriterCompressionLevel.More;

    public JsonLdProcessingMode JsonLdMode { get; init; } = JsonLdProcessingMode.JsonLd11;
    public bool UseDtd { get; init; } = false;
}
