namespace OSLC4Net.Core.Model;

public static partial class OslcConstants
{
    public static partial class Domains
    {
        public static class RDF
        {
            public const string NS = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
            public const string Prefix = "rdf";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string Direction = NS + "direction";
                public const string First = NS + "first";
                public const string Language = NS + "language";
                public const string Object = NS + "object";
                public const string Predicate = NS + "predicate";
                public const string Rest = NS + "rest";
                public const string Subject = NS + "subject";
                public const string Type = NS + "type";
                public const string Value = NS + "value";
            }

            public static class Q
            {
                public static QName Direction => QNameFor("direction");
                public static QName First => QNameFor("first");
                public static QName Language => QNameFor("language");
                public static QName Object => QNameFor("object");
                public static QName Predicate => QNameFor("predicate");
                public static QName Rest => QNameFor("rest");
                public static QName Subject => QNameFor("subject");
                public static QName Type => QNameFor("type");
                public static QName Value => QNameFor("value");
            }
        }

        public static class RDFS
        {
            public const string NS = "http://www.w3.org/2000/01/rdf-schema#";
            public const string Prefix = "rdfs";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string Comment = NS + "comment";
                public const string Domain = NS + "domain";
                public const string IsDefinedBy = NS + "isDefinedBy";
                public const string Label = NS + "label";
                public const string Member = NS + "member";
                public const string Range = NS + "range";
                public const string SeeAlso = NS + "seeAlso";
                public const string SubClassOf = NS + "subClassOf";
                public const string SubPropertyOf = NS + "subPropertyOf";
            }

            public static class Q
            {
                public static QName Comment => QNameFor("comment");
                public static QName Domain => QNameFor("domain");
                public static QName IsDefinedBy => QNameFor("isDefinedBy");
                public static QName Label => QNameFor("label");
                public static QName Member => QNameFor("member");
                public static QName Range => QNameFor("range");
                public static QName SeeAlso => QNameFor("seeAlso");
                public static QName SubClassOf => QNameFor("subClassOf");
                public static QName SubPropertyOf => QNameFor("subPropertyOf");
            }
        }
    }
}
