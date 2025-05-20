using System;
using OSLC4Net.Core.Exceptions; 

namespace OSLC4Net.Client.Exceptions
{
    public class OslcRdfTypeMismatchException : OslcCoreApplicationException
    {
        public string ExpectedTypes { get; }
        public string ActualTypes { get; }
        public string ResourceUri { get; }

        public OslcRdfTypeMismatchException(string resourceUri, string expectedTypes, string actualTypes)
            : base($"RDF type mismatch for resource <{resourceUri}>. Expected one of: [{expectedTypes}], but found: [{actualTypes}].")
        {
            ResourceUri = resourceUri;
            ExpectedTypes = expectedTypes;
            ActualTypes = actualTypes;
        }

        public OslcRdfTypeMismatchException(string resourceUri, string expectedTypes, string actualTypes, Exception innerException)
            : base($"RDF type mismatch for resource <{resourceUri}>. Expected one of: [{expectedTypes}], but found: [{actualTypes}].", innerException)
        {
            ResourceUri = resourceUri;
            ExpectedTypes = expectedTypes;
            ActualTypes = actualTypes;
        }
    }
}
