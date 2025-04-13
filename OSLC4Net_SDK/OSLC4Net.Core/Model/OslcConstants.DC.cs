namespace OSLC4Net.Core.Model;

public static partial class OslcConstants
{
    public static partial class Domains
    {
        /// <summary>
        ///     DC Terms (ISO 15836-2:2019)
        /// </summary>
        public static class DCTerms
        {
            public const string NS = "http://purl.org/dc/terms/";
            public const string Prefix = "dcterms";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string Abstract = NS + "abstract";
                public const string AccessRights = NS + "accessRights";
                public const string AccrualMethod = NS + "accrualMethod";
                public const string AccrualPeriodicity = NS + "accrualPeriodicity";
                public const string AccrualPolicy = NS + "accrualPolicy";
                public const string Alternative = NS + "alternative";
                public const string Audience = NS + "audience";
                public const string Available = NS + "available";
                public const string BibliographicCitation = NS + "bibliographicCitation";
                public const string ConformsTo = NS + "conformsTo";
                public const string Contributor = NS + "contributor";
                public const string Coverage = NS + "coverage";
                public const string Created = NS + "created";
                public const string Creator = NS + "creator";
                public const string Date = NS + "date";
                public const string DateAccepted = NS + "dateAccepted";
                public const string DateCopyrighted = NS + "dateCopyrighted";
                public const string DateSubmitted = NS + "dateSubmitted";
                public const string Description = NS + "description";
                public const string EducationLevel = NS + "educationLevel";
                public const string Extent = NS + "extent";
                public const string Format = NS + "format";
                public const string HasFormat = NS + "hasFormat";
                public const string HasPart = NS + "hasPart";
                public const string HasVersion = NS + "hasVersion";
                public const string Identifier = NS + "identifier";
                public const string InstructionalMethod = NS + "instructionalMethod";
                public const string IsFormatOf = NS + "isFormatOf";
                public const string IsPartOf = NS + "isPartOf";
                public const string IsReferencedBy = NS + "isReferencedBy";
                public const string IsReplacedBy = NS + "isReplacedBy";
                public const string IsRequiredBy = NS + "isRequiredBy";
                public const string IsVersionOf = NS + "isVersionOf";
                public const string Issued = NS + "issued";
                public const string Language = NS + "language";
                public const string License = NS + "license";
                public const string Mediator = NS + "mediator";
                public const string Medium = NS + "medium";
                public const string Modified = NS + "modified";
                public const string Provenance = NS + "provenance";
                public const string Publisher = NS + "publisher";
                public const string References = NS + "references";
                public const string Relation = NS + "relation";
                public const string Replaces = NS + "replaces";
                public const string Requires = NS + "requires";
                public const string Rights = NS + "rights";
                public const string RightsHolder = NS + "rightsHolder";
                public const string Source = NS + "source";
                public const string Spatial = NS + "spatial";
                public const string Subject = NS + "subject";
                public const string TableOfContents = NS + "tableOfContents";
                public const string Temporal = NS + "temporal";
                public const string Title = NS + "title";
                public const string Type = NS + "type";
                public const string Valid = NS + "valid";
            }

            public static class Q
            {
                public static QName Abstract => QNameFor("abstract");
                public static QName AccessRights => QNameFor("accessRights");
                public static QName AccrualMethod => QNameFor("accrualMethod");
                public static QName AccrualPeriodicity => QNameFor("accrualPeriodicity");
                public static QName AccrualPolicy => QNameFor("accrualPolicy");
                public static QName Alternative => QNameFor("alternative");
                public static QName Audience => QNameFor("audience");
                public static QName Available => QNameFor("available");
                public static QName BibliographicCitation => QNameFor("bibliographicCitation");
                public static QName ConformsTo => QNameFor("conformsTo");
                public static QName Contributor => QNameFor("contributor");
                public static QName Coverage => QNameFor("coverage");
                public static QName Created => QNameFor("created");
                public static QName Creator => QNameFor("creator");
                public static QName Date => QNameFor("date");
                public static QName DateAccepted => QNameFor("dateAccepted");
                public static QName DateCopyrighted => QNameFor("dateCopyrighted");
                public static QName DateSubmitted => QNameFor("dateSubmitted");
                public static QName Description => QNameFor("description");
                public static QName EducationLevel => QNameFor("educationLevel");
                public static QName Extent => QNameFor("extent");
                public static QName Format => QNameFor("format");
                public static QName HasFormat => QNameFor("hasFormat");
                public static QName HasPart => QNameFor("hasPart");
                public static QName HasVersion => QNameFor("hasVersion");
                public static QName Identifier => QNameFor("identifier");
                public static QName InstructionalMethod => QNameFor("instructionalMethod");
                public static QName IsFormatOf => QNameFor("isFormatOf");
                public static QName IsPartOf => QNameFor("isPartOf");
                public static QName IsReferencedBy => QNameFor("isReferencedBy");
                public static QName IsReplacedBy => QNameFor("isReplacedBy");
                public static QName IsRequiredBy => QNameFor("isRequiredBy");
                public static QName IsVersionOf => QNameFor("isVersionOf");
                public static QName Issued => QNameFor("issued");
                public static QName Language => QNameFor("language");
                public static QName License => QNameFor("license");
                public static QName Mediator => QNameFor("mediator");
                public static QName Medium => QNameFor("medium");
                public static QName Modified => QNameFor("modified");
                public static QName Provenance => QNameFor("provenance");
                public static QName Publisher => QNameFor("publisher");
                public static QName References => QNameFor("references");
                public static QName Relation => QNameFor("relation");
                public static QName Replaces => QNameFor("replaces");
                public static QName Requires => QNameFor("requires");
                public static QName Rights => QNameFor("rights");
                public static QName RightsHolder => QNameFor("rightsHolder");
                public static QName Source => QNameFor("source");
                public static QName Spatial => QNameFor("spatial");
                public static QName Subject => QNameFor("subject");
                public static QName TableOfContents => QNameFor("tableOfContents");
                public static QName Temporal => QNameFor("temporal");
                public static QName Title => QNameFor("title");
                public static QName Type => QNameFor("type");
                public static QName Valid => QNameFor("valid");
            }
        }

        /// <summary>
        ///     DC Elements (ISO 15836-1:2017). Consider using <see cref="DCTerms" /> where possible.
        /// </summary>
        public static class DCElements
        {
            public const string NS = "http://purl.org/dc/elements/1.1/";
            public const string Prefix = "dc";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string Contributor = NS + "contributor";
                public const string Coverage = NS + "coverage";
                public const string Creator = NS + "creator";
                public const string Date = NS + "date";
                public const string Description = NS + "description";
                public const string Format = NS + "format";
                public const string Identifier = NS + "identifier";
                public const string Language = NS + "language";
                public const string Publisher = NS + "publisher";
                public const string Relation = NS + "relation";
                public const string Rights = NS + "rights";
                public const string Source = NS + "source";
                public const string Subject = NS + "subject";
                public const string Title = NS + "title";
                public const string Type = NS + "type";
            }


            public static class Q
            {
                public static QName Contributor => QNameFor("contributor");
                public static QName Coverage => QNameFor("coverage");
                public static QName Creator => QNameFor("creator");
                public static QName Date => QNameFor("date");
                public static QName Description => QNameFor("description");
                public static QName Format => QNameFor("format");
                public static QName Identifier => QNameFor("identifier");
                public static QName Language => QNameFor("language");
                public static QName Publisher => QNameFor("publisher");
                public static QName Relation => QNameFor("relation");
                public static QName Rights => QNameFor("rights");
                public static QName Source => QNameFor("source");
                public static QName Subject => QNameFor("subject");
                public static QName Title => QNameFor("title");
                public static QName Type => QNameFor("type");
            }
        }
    }
}
