namespace OSLC4Net.Core.Model;

public static partial class OslcConstants
{
    public static partial class Domains
    {
        public static class SKOS
        {
            public const string NS = "http://www.w3.org/2004/02/skos/core#";
            public const string Prefix = "skos";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string AltLabel = NS + "altLabel";
                public const string BroadMatch = NS + "broadMatch";
                public const string Broader = NS + "broader";
                public const string BroaderTransitive = NS + "broaderTransitive";
                public const string ChangeNote = NS + "changeNote";
                public const string CloseMatch = NS + "closeMatch";
                public const string Definition = NS + "definition";
                public const string EditorialNote = NS + "editorialNote";
                public const string ExactMatch = NS + "exactMatch";
                public const string Example = NS + "example";
                public const string HasTopConcept = NS + "hasTopConcept";
                public const string HiddenLabel = NS + "hiddenLabel";
                public const string HistoryNote = NS + "historyNote";
                public const string InScheme = NS + "inScheme";
                public const string MappingRelation = NS + "mappingRelation";
                public const string Member = NS + "member";
                public const string MemberList = NS + "memberList";
                public const string NarrowMatch = NS + "narrowMatch";
                public const string Narrower = NS + "narrower";
                public const string NarrowerTransitive = NS + "narrowerTransitive";
                public const string Notation = NS + "notation";
                public const string Note = NS + "note";
                public const string PrefLabel = NS + "prefLabel";
                public const string Related = NS + "related";
                public const string RelatedMatch = NS + "relatedMatch";
                public const string ScopeNote = NS + "scopeNote";
                public const string SemanticRelation = NS + "semanticRelation";
                public const string TopConceptOf = NS + "topConceptOf";
            }


            public static class Q
            {
                public static QName AltLabel => QNameFor("altLabel");
                public static QName BroadMatch => QNameFor("broadMatch");
                public static QName Broader => QNameFor("broader");
                public static QName BroaderTransitive => QNameFor("broaderTransitive");
                public static QName ChangeNote => QNameFor("changeNote");
                public static QName CloseMatch => QNameFor("closeMatch");
                public static QName Definition => QNameFor("definition");
                public static QName EditorialNote => QNameFor("editorialNote");
                public static QName ExactMatch => QNameFor("exactMatch");
                public static QName Example => QNameFor("example");
                public static QName HasTopConcept => QNameFor("hasTopConcept");
                public static QName HiddenLabel => QNameFor("hiddenLabel");
                public static QName HistoryNote => QNameFor("historyNote");
                public static QName InScheme => QNameFor("inScheme");
                public static QName MappingRelation => QNameFor("mappingRelation");
                public static QName Member => QNameFor("member");
                public static QName MemberList => QNameFor("memberList");
                public static QName NarrowMatch => QNameFor("narrowMatch");
                public static QName Narrower => QNameFor("narrower");
                public static QName NarrowerTransitive => QNameFor("narrowerTransitive");
                public static QName Notation => QNameFor("notation");
                public static QName Note => QNameFor("note");
                public static QName PrefLabel => QNameFor("prefLabel");
                public static QName Related => QNameFor("related");
                public static QName RelatedMatch => QNameFor("relatedMatch");
                public static QName ScopeNote => QNameFor("scopeNote");
                public static QName SemanticRelation => QNameFor("semanticRelation");
                public static QName TopConceptOf => QNameFor("topConceptOf");
            }
        }
    }
}
