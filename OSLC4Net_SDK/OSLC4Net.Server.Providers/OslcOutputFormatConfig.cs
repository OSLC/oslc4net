using System.Runtime.InteropServices;
using VDS.RDF.JsonLd.Syntax;

namespace OSLC4Net.Server.Providers;

[StructLayout(LayoutKind.Auto)]
public struct OslcOutputFormatConfig
{
    public bool? PrettyPrint { get; init; }
    public bool? UseDtd { get; init; }
    public int? CompressionLevel { get; init; }
    public JsonLdProcessingMode? JsonLdMode { get; init; }
}
