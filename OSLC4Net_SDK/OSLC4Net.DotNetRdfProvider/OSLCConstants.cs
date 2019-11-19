/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
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

namespace OSLC4Net.Core
{
    /// <summary>
    /// General OSLC constants
    /// </summary>
    public static class OSLCConstants
    {
        public const string RFC_DATE_FORMAT = "yyyy-MM-dd'T'h:m:ss.S'Z'";

        public const string DC = "http://purl.org/dc/terms/";
        public const string DCTERMS = "dcterms:";
        public const string RDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        public const string RDFS = "http://www.w3.org/2000/01/rdf-schema#";
        public const string ATOM = "http://www.w3.org/2005/Atom";
        public const string OSLC_V2 = "http://open-services.net/ns/core#";
        public const string CORE_DEFAULT = "http://open-services.net/ns/core#default";
        public const string OSLC_CM_V2 = "http://open-services.net/ns/cm#";
        public const string OSLC_AM_V2 = "http://open-services.net/ns/am#";
        public const string OSLC_ASSET_V2 = "http://open-services.net/ns/asset#";
        public const string OSLC_QM_V2 = "http://open-services.net/ns/qm#";
        public const string OSLC_RM_V2 = "http://open-services.net/ns/rm#";
        public const string OSLC_AUTO = "http://open-services.net/ns/auto#";

        // Version 1.0 namespace definitions
        public const string OSLC_DISC = "http://open-services.net/xmlns/discovery/1.0/";

        public const string OSLC_CM = "http://open-services.net/xmlns/cm/1.0/";
        public const string OSLC_QM = "http://open-services.net/xmlns/qm/1.0/";
        public const string OSLC_RM = "http://open-services.net/xmlns/rm/1.0/";

        //--------------------------------------------------------------------------
        // Content-types for Accept header requests
        // Standard headers:
        public const string CT_XML = "application/xml";

        public const string CT_RDF = "application/rdf+xml";
        public const string CT_JSON = "application/json";
        public const string CT_ATOM = "application/atom+xml";

        // Version 1 headers:
        public const string CT_CR_XML = "application/x-oslc-cm-change-request+xml";

        public const string CT_CR_JSON = "application/x-oslc-cm-change-request+json";
        public const string CT_CR_QUERY = "application/x-oslc-cm-change-request+xml";
        public const string CT_DISC_CAT_XML = "application/x-oslc-disc-service-provider-catalog+xml";
        public const string CT_DISC_DESC_XML = "application/x-oslc-cm-service-description+xml";

        // Version 2 headers:
        public const string OSLC_CORE_VERSION = "OSLC-Core-Version";

        public const string ETAG = "Etag";

        public const string POST = "POST";
        public const string SSL = "SSL";

        public const string JENA_RDF_XML = "RDF/XML";

        //--------------------------------------------------------------------------
        // Property URIs

        // OSLC Core
        public const string SERVICE_PROVIDER_PROP = OSLC_V2 + "serviceProvider";

        public const string SERVICE_PROVIDER_TYPE = OSLC_V2 + "ServiceProvider";
        public const string SERVICE_PROVIDER_CATALOG_PROP = OSLC_V2 + "serviceProviderCatalog";
        public const string SERVICE_PROVIDER_CATALOG_TYPE = OSLC_V2 + "ServiceProviderCatalog";
        public const string CREATION_PROP = OSLC_V2 + "creation";
        public const string QUERY_CAPABILITY_PROP = OSLC_V2 + "QueryCapability";
        public const string QUERY_BASE_PROP = OSLC_V2 + "queryBase";
        public const string RESP_INFO_TYPE = OSLC_V2 + "ResponseInfo";
        public const string SERVICE_PROP = OSLC_V2 + "service";
        public const string DISCUSSION_PROP = OSLC_V2 + "discussion";
        public const string INST_SHAPE_PROP = OSLC_V2 + "instanceShape";
        public const string USAGE_PROP = OSLC_V2 + "usage";
        public const string USAGE_DEFAULT_URI = OSLC_V2 + "default";
        public const string TOTAL_COUNT_PROP = OSLC_V2 + "totalCount";
        public const string RESOURCE_TYPE_PROP = OSLC_V2 + "resourceType";
        public const string RESOURCE_SHAPE_PROP = OSLC_V2 + "resourceShape";
        public const string DESCRIPTION_PROP = OSLC_V2 + "Description";

        // OSLC CM 2.0
        public const string CM_CHANGE_REQUEST_TYPE = OSLC_CM_V2 + "ChangeRequest";

        public const string CM_CLOSE_DATE_PROP = OSLC_CM_V2 + "closeDate";
        public const string CM_STATUS_PROP = OSLC_CM_V2 + "status";
        public const string CM_CLOSED_PROP = OSLC_CM_V2 + "closed";
        public const string CM_INPROGRESS_PROP = OSLC_CM_V2 + "inprogress";
        public const string CM_FIXED_PROP = OSLC_CM_V2 + "fixed";
        public const string CM_APPROVED_PROP = OSLC_CM_V2 + "approved";
        public const string CM_REVIEWED_PROP = OSLC_CM_V2 + "reviewed";
        public const string CM_VERIFIED_PROP = OSLC_CM_V2 + "verified";

        // OSLC QM 2.0
        public const string QM_TEST_PLAN = OSLC_QM_V2 + "testPlan";

        public const string QM_TEST_CASE = OSLC_QM_V2 + "testCase";
        public const string QM_TEST_SCRIPT = OSLC_QM_V2 + "testScript";
        public const string QM_TEST_RESULT = OSLC_QM_V2 + "testResult";
        public const string QM_TEST_EXECUTION_RECORD = OSLC_QM_V2 + "testExecutionRecord";

        public const string QM_TEST_PLAN_QUERY = OSLC_QM_V2 + "TestPlanQuery";
        public const string QM_TEST_CASE_QUERY = OSLC_QM_V2 + "TestCaseQuery";
        public const string QM_TEST_SCRIPT_QUERY = OSLC_QM_V2 + "TestScriptQuery";
        public const string QM_TEST_RESULT_QUERY = OSLC_QM_V2 + "TestResultQuery";
        public const string QM_TEST_EXECUTION_RECORD_QUERY = OSLC_QM_V2 + "TestExecutionRecordQuery";

        //OSLC RM 2.0

        public const string RM_REQUIREMENT_TYPE = OSLC_RM_V2 + "Requirement";
        public const string RM_REQUIREMENT_COLLECTION_TYPE = OSLC_RM_V2 + "RequirementCollection";

        //OSLC AM 2.0
        public const string AM_RESOURCE_TYPE = OSLC_AM_V2 + "Resource";

        public const string AM_LINK_TYPE_TYPE = OSLC_AM_V2 + "LinkType";

        // RDF
        public const string RDF_TYPE_PROP = RDF + "type";

        public const string RDFS_MEMBER = RDFS + "member";

        // DCTERMS URIs
        public const string DC_TITLE_PROP = DC + "title";

        public const string DC_DESC_PROP = DC + "description";
        public const string DC_TYPE_PROP = DC + "type";
        public const string DC_PUBLISHER_PROP = DC + "publisher";
        public const string DC_ID_PROP = DC + "identifier";
        public const string DC_NAME_PROP = DC + "name";
        public const string DC_CREATED_PROP = DC + "created";
        public const string DC_MODIFIED_PROP = DC + "modified";

        // DCTERMSs
        public const string DCTERMS_TITLE = DCTERMS + "title";

        public const string DCTERMS_DESC = DCTERMS + "description";
        public const string DCTERMS_TYPE = DCTERMS + "type";
        public const string DCTERMS_PUBLISHER = DCTERMS + "publisher";
        public const string DCTERMS_ID = DCTERMS + "identifier";
        public const string DCTERMS_NAME = DCTERMS + "name";
        public const string DCTERMS_CREATED = DCTERMS + "created";
        public const string DCTERMS_MODIFIED = DCTERMS + "modified";
    }
}
