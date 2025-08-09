namespace OSLC4Net.Core.Model;

public static partial class OslcConstants
{
    public static partial class Domains
    {
        public static class PROV
        {
            public const string NS = "http://www.w3.org/ns/prov#";
            public const string Prefix = "prov";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string ActedOnBehalfOf = NS + "actedOnBehalfOf";
                public const string Activity = NS + "activity";
                public const string Agent = NS + "agent";
                public const string AlternateOf = NS + "alternateOf";
                public const string AsInBundle = NS + "asInBundle";
                public const string AtLocation = NS + "atLocation";
                public const string AtTime = NS + "atTime";
                public const string DerivedByInsertionFrom = NS + "derivedByInsertionFrom";
                public const string DerivedByRemovalFrom = NS + "derivedByRemovalFrom";
                public const string DescribesService = NS + "describesService";
                public const string Dictionary = NS + "dictionary";
                public const string EndedAtTime = NS + "endedAtTime";
                public const string Entity = NS + "entity";
                public const string Generated = NS + "generated";
                public const string GeneratedAtTime = NS + "generatedAtTime";
                public const string HadActivity = NS + "hadActivity";
                public const string HadDictionaryMember = NS + "hadDictionaryMember";
                public const string HadGeneration = NS + "hadGeneration";
                public const string HadMember = NS + "hadMember";
                public const string HadPlan = NS + "hadPlan";
                public const string HadPrimarySource = NS + "hadPrimarySource";
                public const string HadRole = NS + "hadRole";
                public const string HadUsage = NS + "hadUsage";
                public const string HasAnchor = NS + "has_anchor";
                public const string HasProvenance = NS + "has_provenance";
                public const string HasQueryService = NS + "has_query_service";
                public const string Influenced = NS + "influenced";
                public const string Influencer = NS + "influencer";
                public const string InsertedKeyEntityPair = NS + "insertedKeyEntityPair";
                public const string Invalidated = NS + "invalidated";
                public const string InvalidatedAtTime = NS + "invalidatedAtTime";
                public const string MentionOf = NS + "mentionOf";
                public const string PairEntity = NS + "pairEntity";
                public const string PairKey = NS + "pairKey";
                public const string Pingback = NS + "pingback";
                public const string ProvenanceUriTemplate = NS + "provenanceUriTemplate";
                public const string QualifiedAssociation = NS + "qualifiedAssociation";
                public const string QualifiedAttribution = NS + "qualifiedAttribution";
                public const string QualifiedCommunication = NS + "qualifiedCommunication";
                public const string QualifiedDelegation = NS + "qualifiedDelegation";
                public const string QualifiedDerivation = NS + "qualifiedDerivation";
                public const string QualifiedEnd = NS + "qualifiedEnd";
                public const string QualifiedGeneration = NS + "qualifiedGeneration";
                public const string QualifiedInfluence = NS + "qualifiedInfluence";
                public const string QualifiedInsertion = NS + "qualifiedInsertion";
                public const string QualifiedInvalidation = NS + "qualifiedInvalidation";
                public const string QualifiedPrimarySource = NS + "qualifiedPrimarySource";
                public const string QualifiedQuotation = NS + "qualifiedQuotation";
                public const string QualifiedRemoval = NS + "qualifiedRemoval";
                public const string QualifiedRevision = NS + "qualifiedRevision";
                public const string QualifiedStart = NS + "qualifiedStart";
                public const string QualifiedUsage = NS + "qualifiedUsage";
                public const string RemovedKey = NS + "removedKey";
                public const string SpecializationOf = NS + "specializationOf";
                public const string StartedAtTime = NS + "startedAtTime";
                public const string Used = NS + "used";
                public const string Value = NS + "value";
                public const string WasAssociatedWith = NS + "wasAssociatedWith";
                public const string WasAttributedTo = NS + "wasAttributedTo";
                public const string WasDerivedFrom = NS + "wasDerivedFrom";
                public const string WasEndedBy = NS + "wasEndedBy";
                public const string WasGeneratedBy = NS + "wasGeneratedBy";
                public const string WasInfluencedBy = NS + "wasInfluencedBy";
                public const string WasInformedBy = NS + "wasInformedBy";
                public const string WasInvalidatedBy = NS + "wasInvalidatedBy";
                public const string WasQuotedFrom = NS + "wasQuotedFrom";
                public const string WasRevisionOf = NS + "wasRevisionOf";
                public const string WasStartedBy = NS + "wasStartedBy";
            }

            public static class Q
            {
                public static QName ActedOnBehalfOf => QNameFor("actedOnBehalfOf");
                public static QName Activity => QNameFor("activity");
                public static QName Agent => QNameFor("agent");
                public static QName AlternateOf => QNameFor("alternateOf");
                public static QName AsInBundle => QNameFor("asInBundle");
                public static QName AtLocation => QNameFor("atLocation");
                public static QName AtTime => QNameFor("atTime");
                public static QName DerivedByInsertionFrom => QNameFor("derivedByInsertionFrom");
                public static QName DerivedByRemovalFrom => QNameFor("derivedByRemovalFrom");
                public static QName DescribesService => QNameFor("describesService");
                public static QName Dictionary => QNameFor("dictionary");
                public static QName EndedAtTime => QNameFor("endedAtTime");
                public static QName Entity => QNameFor("entity");
                public static QName Generated => QNameFor("generated");
                public static QName GeneratedAtTime => QNameFor("generatedAtTime");
                public static QName HadActivity => QNameFor("hadActivity");
                public static QName HadDictionaryMember => QNameFor("hadDictionaryMember");
                public static QName HadGeneration => QNameFor("hadGeneration");
                public static QName HadMember => QNameFor("hadMember");
                public static QName HadPlan => QNameFor("hadPlan");
                public static QName HadPrimarySource => QNameFor("hadPrimarySource");
                public static QName HadRole => QNameFor("hadRole");
                public static QName HadUsage => QNameFor("hadUsage");
                public static QName HasAnchor => QNameFor("has_anchor");
                public static QName HasProvenance => QNameFor("has_provenance");
                public static QName HasQueryService => QNameFor("has_query_service");
                public static QName Influenced => QNameFor("influenced");
                public static QName Influencer => QNameFor("influencer");
                public static QName InsertedKeyEntityPair => QNameFor("insertedKeyEntityPair");
                public static QName Invalidated => QNameFor("invalidated");
                public static QName InvalidatedAtTime => QNameFor("invalidatedAtTime");
                public static QName MentionOf => QNameFor("mentionOf");
                public static QName PairEntity => QNameFor("pairEntity");
                public static QName PairKey => QNameFor("pairKey");
                public static QName Pingback => QNameFor("pingback");
                public static QName ProvenanceUriTemplate => QNameFor("provenanceUriTemplate");
                public static QName QualifiedAssociation => QNameFor("qualifiedAssociation");
                public static QName QualifiedAttribution => QNameFor("qualifiedAttribution");
                public static QName QualifiedCommunication => QNameFor("qualifiedCommunication");
                public static QName QualifiedDelegation => QNameFor("qualifiedDelegation");
                public static QName QualifiedDerivation => QNameFor("qualifiedDerivation");
                public static QName QualifiedEnd => QNameFor("qualifiedEnd");
                public static QName QualifiedGeneration => QNameFor("qualifiedGeneration");
                public static QName QualifiedInfluence => QNameFor("qualifiedInfluence");
                public static QName QualifiedInsertion => QNameFor("qualifiedInsertion");
                public static QName QualifiedInvalidation => QNameFor("qualifiedInvalidation");
                public static QName QualifiedPrimarySource => QNameFor("qualifiedPrimarySource");
                public static QName QualifiedQuotation => QNameFor("qualifiedQuotation");
                public static QName QualifiedRemoval => QNameFor("qualifiedRemoval");
                public static QName QualifiedRevision => QNameFor("qualifiedRevision");
                public static QName QualifiedStart => QNameFor("qualifiedStart");
                public static QName QualifiedUsage => QNameFor("qualifiedUsage");
                public static QName RemovedKey => QNameFor("removedKey");
                public static QName SpecializationOf => QNameFor("specializationOf");
                public static QName StartedAtTime => QNameFor("startedAtTime");
                public static QName Used => QNameFor("used");
                public static QName Value => QNameFor("value");
                public static QName WasAssociatedWith => QNameFor("wasAssociatedWith");
                public static QName WasAttributedTo => QNameFor("wasAttributedTo");
                public static QName WasDerivedFrom => QNameFor("wasDerivedFrom");
                public static QName WasEndedBy => QNameFor("wasEndedBy");
                public static QName WasGeneratedBy => QNameFor("wasGeneratedBy");
                public static QName WasInfluencedBy => QNameFor("wasInfluencedBy");
                public static QName WasInformedBy => QNameFor("wasInformedBy");
                public static QName WasInvalidatedBy => QNameFor("wasInvalidatedBy");
                public static QName WasQuotedFrom => QNameFor("wasQuotedFrom");
                public static QName WasRevisionOf => QNameFor("wasRevisionOf");
                public static QName WasStartedBy => QNameFor("wasStartedBy");
            }
        }
    }
}
