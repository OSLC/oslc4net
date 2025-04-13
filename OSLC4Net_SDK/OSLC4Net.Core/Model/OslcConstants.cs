/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

namespace OSLC4Net.Core.Model;

/// <summary>
///     General purpose OSLC constants
/// </summary>
public static class OslcConstants
{
    public const string OSLC_CORE_DOMAIN = "http://open-services.net/ns/core#";

    [Obsolete("Use ")] public const string DCTERMS_NAMESPACE = "http://purl.org/dc/terms/";
    public const string OSLC_CORE_NAMESPACE = "http://open-services.net/ns/core#";
    public const string OSLC_DATA_NAMESPACE = "http://open-services.net/ns/servicemanagement/1.0/";
    public const string RDF_NAMESPACE = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
    public const string RDFS_NAMESPACE = "http://www.w3.org/2000/01/rdf-schema#";
    public const string XML_NAMESPACE = "http://www.w3.org/2001/XMLSchema#";

    public const string DCTERMS_NAMESPACE_PREFIX = "dcterms";
    public const string OSLC_CORE_NAMESPACE_PREFIX = "oslc";
    public const string OSLC_DATA_NAMESPACE_PREFIX = "oslc_data";
    public const string RDF_NAMESPACE_PREFIX = "rdf";
    public const string RDFS_NAMESPACE_PREFIX = "rdfs";

    public const string OSLC_USAGE_DEFAULT = "http://open-services.net/ns/core#default";

    public const string TYPE_ALLOWED_VALUES = OSLC_CORE_NAMESPACE + "AllowedValues";
    public const string TYPE_COMPACT = OSLC_CORE_NAMESPACE + "Compact";
    public const string TYPE_CREATION_FACTORY = OSLC_CORE_NAMESPACE + "CreationFactory";
    public const string TYPE_DIALOG = OSLC_CORE_NAMESPACE + "Dialog";
    public const string TYPE_ERROR = OSLC_CORE_NAMESPACE + "Error";
    public const string TYPE_EXTENDED_ERROR = OSLC_CORE_NAMESPACE + "ExtendedError";
    public const string TYPE_O_AUTH_CONFIGURATION = OSLC_CORE_NAMESPACE + "OAuthConfiguration";
    public const string TYPE_PREFIX_DEFINITION = OSLC_CORE_NAMESPACE + "PrefixDefinition";
    public const string TYPE_PREVIEW = OSLC_CORE_NAMESPACE + "Preview";
    public const string TYPE_PROPERTY = OSLC_CORE_NAMESPACE + "Property";
    public const string TYPE_PUBLISHER = OSLC_CORE_NAMESPACE + "Publisher";
    public const string TYPE_QUERY_CAPABILITY = OSLC_CORE_NAMESPACE + "QueryCapability";
    public const string TYPE_RESOURCE_SHAPE = OSLC_CORE_NAMESPACE + "ResourceShape";
    public const string TYPE_RESPONSE_INFO = OSLC_CORE_NAMESPACE + "ResponseInfo";
    public const string TYPE_SERVICE = OSLC_CORE_NAMESPACE + "Service";
    public const string TYPE_SERVICE_PROVIDER = OSLC_CORE_NAMESPACE + "ServiceProvider";

    public const string TYPE_SERVICE_PROVIDER_CATALOG =
        OSLC_CORE_NAMESPACE + "ServiceProviderCatalog";

    public const string PATH_RESOURCE_SHAPES = "resourceShapes";

    public const string PATH_ALLOWED_VALUES = "allowedValues";
    public const string PATH_CREATION_FACTORY = "creationFactory";
    public const string PATH_COMPACT = "compact";
    public const string PATH_DIALOG = "dialog";
    public const string PATH_ERROR = "error";
    public const string PATH_EXTENDED_ERROR = "extendedError";
    public const string PATH_OAUTH_CONFIGURATION = "oauthConfiguration";
    public const string PATH_PREFIX_DEFINITION = "prefixDefinition";
    public const string PATH_PREVIEW = "preview";
    public const string PATH_PROPERTY = "property";
    public const string PATH_PUBLISHER = "publisher";
    public const string PATH_QUERY_CAPABILITY = "queryCapability";
    public const string PATH_RESOURCE_SHAPE = "resourceShape";
    public const string PATH_SERVICE = "service";
    public const string PATH_SERVICE_PROVIDER = "serviceProvider";
    public const string PATH_SERVICE_PROVIDER_CATALOG = "serviceProviderCatalog";

    public static class Domains
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
    }
}
