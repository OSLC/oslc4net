namespace OSLC4Net.Core.Model;

public static partial class OslcConstants
{
    public static partial class Domains
    {
        public static class LDP
        {
            public const string NS = "http://www.w3.org/ns/ldp#";
            public const string Prefix = "ldp";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string ConstrainedBy = NS + "constrainedBy";
                public const string Contains = NS + "contains";
                public const string HasMemberRelation = NS + "hasMemberRelation";
                public const string Inbox = NS + "inbox";
                public const string InsertedContentRelation = NS + "insertedContentRelation";
                public const string IsMemberOfRelation = NS + "isMemberOfRelation";
                public const string Member = NS + "member";
                public const string MembershipResource = NS + "membershipResource";
                public const string PageSequence = NS + "pageSequence";
                public const string PageSortCollation = NS + "pageSortCollation";
                public const string PageSortCriteria = NS + "pageSortCriteria";
                public const string PageSortOrder = NS + "pageSortOrder";
                public const string PageSortPredicate = NS + "pageSortPredicate";
            }


            public static class Q
            {
                public static QName ConstrainedBy => QNameFor("constrainedBy");
                public static QName Contains => QNameFor("contains");
                public static QName HasMemberRelation => QNameFor("hasMemberRelation");
                public static QName Inbox => QNameFor("inbox");
                public static QName InsertedContentRelation => QNameFor("insertedContentRelation");
                public static QName IsMemberOfRelation => QNameFor("isMemberOfRelation");
                public static QName Member => QNameFor("member");
                public static QName MembershipResource => QNameFor("membershipResource");
                public static QName PageSequence => QNameFor("pageSequence");
                public static QName PageSortCollation => QNameFor("pageSortCollation");
                public static QName PageSortCriteria => QNameFor("pageSortCriteria");
                public static QName PageSortOrder => QNameFor("pageSortOrder");
                public static QName PageSortPredicate => QNameFor("pageSortPredicate");
            }
        }
    }
}
