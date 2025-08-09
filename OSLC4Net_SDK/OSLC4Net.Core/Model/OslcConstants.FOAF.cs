namespace OSLC4Net.Core.Model;

public static partial class OslcConstants
{
    public static partial class Domains
    {
        public static class FOAF
        {
            public const string NS = "http://xmlns.com/foaf/0.1/";
            public const string Prefix = "foaf";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string Account = NS + "account";
                public const string AccountName = NS + "accountName";
                public const string AccountServiceHomepage = NS + "accountServiceHomepage";
                public const string Age = NS + "age";
                public const string AimChatId = NS + "aimChatID";
                public const string BasedNear = NS + "based_near";
                public const string Birthday = NS + "birthday";
                public const string CurrentProject = NS + "currentProject";
                public const string Depiction = NS + "depiction";
                public const string Depicts = NS + "depicts";
                public const string DnaChecksum = NS + "dnaChecksum";
                public const string FamilyName = NS + "familyName";
                public const string Family_Name = NS + "family_name";
                public const string FirstName = NS + "firstName";
                public const string Focus = NS + "focus";
                public const string FundedBy = NS + "fundedBy";
                public const string Geekcode = NS + "geekcode";
                public const string Gender = NS + "gender";
                public const string GivenName = NS + "givenName";
                public const string Givenname = NS + "givenname";
                public const string HoldsAccount = NS + "holdsAccount";
                public const string Homepage = NS + "homepage";
                public const string IcqChatId = NS + "icqChatID";
                public const string Img = NS + "img";
                public const string Interest = NS + "interest";
                public const string IsPrimaryTopicOf = NS + "isPrimaryTopicOf";
                public const string JabberId = NS + "jabberID";
                public const string Knows = NS + "knows";
                public const string LastName = NS + "lastName";
                public const string Logo = NS + "logo";
                public const string Made = NS + "made";
                public const string Maker = NS + "maker";
                public const string Mbox = NS + "mbox";
                public const string MboxSha1sum = NS + "mbox_sha1sum";
                public const string Member = NS + "member";
                public const string MembershipClass = NS + "membershipClass";
                public const string MsnChatId = NS + "msnChatID";
                public const string MyersBriggs = NS + "myersBriggs";
                public const string Name = NS + "name";
                public const string Nick = NS + "nick";
                public const string Openid = NS + "openid";
                public const string Page = NS + "page";
                public const string PastProject = NS + "pastProject";
                public const string Phone = NS + "phone";
                public const string Plan = NS + "plan";
                public const string PrimaryTopic = NS + "primaryTopic";
                public const string Publications = NS + "publications";
                public const string SchoolHomepage = NS + "schoolHomepage";
                public const string Sha1 = NS + "sha1";
                public const string SkypeId = NS + "skypeID";
                public const string Status = NS + "status";
                public const string Surname = NS + "surname";
                public const string Theme = NS + "theme";
                public const string Thumbnail = NS + "thumbnail";
                public const string Tipjar = NS + "tipjar";
                public const string Title = NS + "title";
                public const string Topic = NS + "topic";
                public const string TopicInterest = NS + "topic_interest";
                public const string Weblog = NS + "weblog";
                public const string WorkInfoHomepage = NS + "workInfoHomepage";
                public const string WorkplaceHomepage = NS + "workplaceHomepage";
                public const string YahooChatId = NS + "yahooChatID";
            }

            public static class Q
            {
                public static QName Account => QNameFor("account");
                public static QName AccountName => QNameFor("accountName");
                public static QName AccountServiceHomepage => QNameFor("accountServiceHomepage");
                public static QName Age => QNameFor("age");
                public static QName AimChatId => QNameFor("aimChatID");
                public static QName BasedNear => QNameFor("based_near");
                public static QName Birthday => QNameFor("birthday");
                public static QName CurrentProject => QNameFor("currentProject");
                public static QName Depiction => QNameFor("depiction");
                public static QName Depicts => QNameFor("depicts");
                public static QName DnaChecksum => QNameFor("dnaChecksum");
                public static QName FamilyName => QNameFor("familyName");
                public static QName Family_Name => QNameFor("family_name");
                public static QName FirstName => QNameFor("firstName");
                public static QName Focus => QNameFor("focus");
                public static QName FundedBy => QNameFor("fundedBy");
                public static QName Geekcode => QNameFor("geekcode");
                public static QName Gender => QNameFor("gender");
                public static QName GivenName => QNameFor("givenName");
                public static QName Givenname => QNameFor("givenname");
                public static QName HoldsAccount => QNameFor("holdsAccount");
                public static QName Homepage => QNameFor("homepage");
                public static QName IcqChatId => QNameFor("icqChatID");
                public static QName Img => QNameFor("img");
                public static QName Interest => QNameFor("interest");
                public static QName IsPrimaryTopicOf => QNameFor("isPrimaryTopicOf");
                public static QName JabberId => QNameFor("jabberID");
                public static QName Knows => QNameFor("knows");
                public static QName LastName => QNameFor("lastName");
                public static QName Logo => QNameFor("logo");
                public static QName Made => QNameFor("made");
                public static QName Maker => QNameFor("maker");
                public static QName Mbox => QNameFor("mbox");
                public static QName MboxSha1sum => QNameFor("mbox_sha1sum");
                public static QName Member => QNameFor("member");
                public static QName MembershipClass => QNameFor("membershipClass");
                public static QName MsnChatId => QNameFor("msnChatID");
                public static QName MyersBriggs => QNameFor("myersBriggs");
                public static QName Name => QNameFor("name");
                public static QName Nick => QNameFor("nick");
                public static QName Openid => QNameFor("openid");
                public static QName Page => QNameFor("page");
                public static QName PastProject => QNameFor("pastProject");
                public static QName Phone => QNameFor("phone");
                public static QName Plan => QNameFor("plan");
                public static QName PrimaryTopic => QNameFor("primaryTopic");
                public static QName Publications => QNameFor("publications");
                public static QName SchoolHomepage => QNameFor("schoolHomepage");
                public static QName Sha1 => QNameFor("sha1");
                public static QName SkypeId => QNameFor("skypeID");
                public static QName Status => QNameFor("status");
                public static QName Surname => QNameFor("surname");
                public static QName Theme => QNameFor("theme");
                public static QName Thumbnail => QNameFor("thumbnail");
                public static QName Tipjar => QNameFor("tipjar");
                public static QName Title => QNameFor("title");
                public static QName Topic => QNameFor("topic");
                public static QName TopicInterest => QNameFor("topic_interest");
                public static QName Weblog => QNameFor("weblog");
                public static QName WorkInfoHomepage => QNameFor("workInfoHomepage");
                public static QName WorkplaceHomepage => QNameFor("workplaceHomepage");
                public static QName YahooChatId => QNameFor("yahooChatID");
            }
        }
    }
}
