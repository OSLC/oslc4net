/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Domains.SPDX.AI
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/AI/", "spdx_ai")]
    public static partial class SpdxAI;

    [OslcShape("https://spdx.org/rdf/3/terms/AI/AIPackage")]
    public partial record AIPackage;

    [OslcShape("https://spdx.org/rdf/3/terms/AI/EnergyConsumption")]
    public partial record EnergyConsumption;

    [OslcShape("https://spdx.org/rdf/3/terms/AI/EnergyConsumptionDescription")]
    public partial record EnergyConsumptionDescription;
}

namespace OSLC4Net.Domains.SPDX.Build
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Build/", "spdx_build")]
    public static partial class SpdxBuildVocabulary;

    [OslcShape("https://spdx.org/rdf/3/terms/Build/Build")]
    public partial record SpdxBuild;
}

namespace OSLC4Net.Domains.SPDX.Core
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Core/", "spdx")]
    public static partial class SpdxCore;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Action")]
    public partial record Action;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Annotation")]
    public partial record Annotation;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Artifact")]
    public partial record Artifact;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Bundle")]
    public partial record Bundle;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/ContactPointRelationship")]
    public partial record ContactPointRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/CreationInfo")]
    public partial record CreationInfo;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/DefinedProcess")]
    public partial record DefinedProcess;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/DefinedType")]
    public partial record DefinedType;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/DictionaryEntry")]
    public partial record DictionaryEntry;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Element")]
    public partial record Element;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/ElementCollection")]
    public partial record ElementCollection;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/ElementMap")]
    public partial record ElementMap;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/ExternalIdentifier")]
    public partial record ExternalIdentifier;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/ExternalMap")]
    public partial record ExternalMap;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/ExternalRef")]
    public partial record ExternalRef;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Hash")]
    public partial record Hash;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/IntegrityMethod")]
    public partial record IntegrityMethod;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/LifecycleScopedRelationship")]
    public partial record LifecycleScopedRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Location")]
    public partial record Location;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/NamespaceMap")]
    public partial record NamespaceMap;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Organization")]
    public partial record Organization;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/PackageVerificationCode")]
    public partial record PackageVerificationCode;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/PhysicalLocation")]
    public partial record PhysicalLocation;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/PositiveIntegerRange")]
    public partial record PositiveIntegerRange;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Relationship")]
    public partial record Relationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Requirement")]
    public partial record Requirement;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Role")]
    public partial record Role;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/RoleRelationship")]
    public partial record RoleRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/SpdxDocument")]
    public partial record SpdxDocument;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/Specification")]
    public partial record Specification;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/SupportRelationship")]
    public partial record SupportRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Core/UnitOfMeasure")]
    public partial record UnitOfMeasure;
}

namespace OSLC4Net.Domains.SPDX.Dataset
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Dataset/", "spdx_dataset")]
    public static partial class SpdxDataset;

    [OslcShape("https://spdx.org/rdf/3/terms/Dataset/DatasetPackage")]
    public partial record DatasetPackage;
}

namespace OSLC4Net.Domains.SPDX.ExpandedLicensing
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/ExpandedLicensing/", "spdx_expanded_licensing")]
    public static partial class SpdxExpandedLicensing;

    [OslcShape("https://spdx.org/rdf/3/terms/ExpandedLicensing/ConjunctiveLicenseSet")]
    public partial record ConjunctiveLicenseSet;

    [OslcShape("https://spdx.org/rdf/3/terms/ExpandedLicensing/DisjunctiveLicenseSet")]
    public partial record DisjunctiveLicenseSet;

    [OslcShape("https://spdx.org/rdf/3/terms/ExpandedLicensing/License")]
    public partial record License;

    [OslcShape("https://spdx.org/rdf/3/terms/ExpandedLicensing/LicenseAddition")]
    public partial record LicenseAddition;

    [OslcShape("https://spdx.org/rdf/3/terms/ExpandedLicensing/ListedLicense")]
    public partial record ListedLicense;

    [OslcShape("https://spdx.org/rdf/3/terms/ExpandedLicensing/ListedLicenseException")]
    public partial record ListedLicenseException;

    [OslcShape("https://spdx.org/rdf/3/terms/ExpandedLicensing/OrLaterOperator")]
    public partial record OrLaterOperator;

    [OslcShape("https://spdx.org/rdf/3/terms/ExpandedLicensing/WithAdditionOperator")]
    public partial record WithAdditionOperator;
}

namespace OSLC4Net.Domains.SPDX.Extension
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Extension/", "spdx_extension")]
    public static partial class SpdxExtension;

    [OslcShape("https://spdx.org/rdf/3/terms/Extension/CdxPropertiesExtension")]
    public partial record CdxPropertiesExtension;

    [OslcShape("https://spdx.org/rdf/3/terms/Extension/CdxPropertyEntry")]
    public partial record CdxPropertyEntry;
}

namespace OSLC4Net.Domains.SPDX.FunctionalSafety
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/FunctionalSafety/", "spdx_functional_safety")]
    public static partial class SpdxFunctionalSafety;

    [OslcShape("https://spdx.org/rdf/3/terms/FunctionalSafety/Assumption")]
    public partial record Assumption;

    [OslcShape("https://spdx.org/rdf/3/terms/FunctionalSafety/EvaluationResult")]
    public partial record EvaluationResult;

    [OslcShape("https://spdx.org/rdf/3/terms/FunctionalSafety/EvidenceRelationship")]
    public partial record EvidenceRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/FunctionalSafety/RequirementVerification")]
    public partial record RequirementVerification;
}

namespace OSLC4Net.Domains.SPDX.Hardware
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Hardware/", "spdx_hardware")]
    public static partial class SpdxHardwareVocabulary;

    [OslcShape("https://spdx.org/rdf/3/terms/Hardware/BulkHardware")]
    public partial record BulkHardware;

    [OslcShape("https://spdx.org/rdf/3/terms/Hardware/Dimensions")]
    public partial record Dimensions;

    [OslcShape("https://spdx.org/rdf/3/terms/Hardware/Hardware")]
    public partial record SpdxHardware;

    [OslcShape("https://spdx.org/rdf/3/terms/Hardware/PhysicalHardware")]
    public partial record PhysicalHardware;

    [OslcShape("https://spdx.org/rdf/3/terms/Hardware/ProductSpecification")]
    public partial record ProductSpecification;

    [OslcShape("https://spdx.org/rdf/3/terms/Hardware/VirtualHardware")]
    public partial record VirtualHardware;
}

namespace OSLC4Net.Domains.SPDX.Operations
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Operations/", "spdx_operations")]
    public static partial class SpdxOperations;

    [OslcShape("https://spdx.org/rdf/3/terms/Operations/ExportControlClassification")]
    public partial record ExportControlClassification;

    [OslcShape("https://spdx.org/rdf/3/terms/Operations/ExportControlClassificationAssessment")]
    public partial record ExportControlClassificationAssessment;

    [OslcShape("https://spdx.org/rdf/3/terms/Operations/Project")]
    public partial record Project;
}

namespace OSLC4Net.Domains.SPDX.Security
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Security/", "spdx_security")]
    public static partial class SpdxSecurity;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/CvssV2VulnAssessmentRelationship")]
    public partial record CvssV2VulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/CvssV3VulnAssessmentRelationship")]
    public partial record CvssV3VulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/CvssV4VulnAssessmentRelationship")]
    public partial record CvssV4VulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/EpssVulnAssessmentRelationship")]
    public partial record EpssVulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/ExploitCatalogVulnAssessmentRelationship")]
    public partial record ExploitCatalogVulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/SsvcVulnAssessmentRelationship")]
    public partial record SsvcVulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/VexAffectedVulnAssessmentRelationship")]
    public partial record VexAffectedVulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/VexNotAffectedVulnAssessmentRelationship")]
    public partial record VexNotAffectedVulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/VexVulnAssessmentRelationship")]
    public partial record VexVulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/VulnAssessmentRelationship")]
    public partial record VulnAssessmentRelationship;

    [OslcShape("https://spdx.org/rdf/3/terms/Security/Vulnerability")]
    public partial record Vulnerability;
}

namespace OSLC4Net.Domains.SPDX.Service
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Service/", "spdx_service")]
    public static partial class SpdxService;

    [OslcShape("https://spdx.org/rdf/3/terms/Service/SoftwareService")]
    public partial record SoftwareService;
}

namespace OSLC4Net.Domains.SPDX.SimpleLicensing
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/SimpleLicensing/", "spdx_simple_licensing")]
    public static partial class SpdxSimpleLicensing;

    [OslcShape("https://spdx.org/rdf/3/terms/SimpleLicensing/LicenseExpression")]
    public partial record LicenseExpression;

    [OslcShape("https://spdx.org/rdf/3/terms/SimpleLicensing/SimpleLicensingText")]
    public partial record SimpleLicensingText;
}

namespace OSLC4Net.Domains.SPDX.Software
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/Software/", "spdx_software")]
    public static partial class SpdxSoftware;

    [OslcShape("https://spdx.org/rdf/3/terms/Software/ContentIdentifier")]
    public partial record ContentIdentifier;

    [OslcShape("https://spdx.org/rdf/3/terms/Software/File")]
    public partial record File;

    [OslcShape("https://spdx.org/rdf/3/terms/Software/Package")]
    public partial record Package;

    [OslcShape("https://spdx.org/rdf/3/terms/Software/Sbom")]
    public partial record Sbom;

    [OslcShape("https://spdx.org/rdf/3/terms/Software/Snippet")]
    public partial record Snippet;

    [OslcShape("https://spdx.org/rdf/3/terms/Software/SoftwareArtifact")]
    public partial record SoftwareArtifact;
}

namespace OSLC4Net.Domains.SPDX.SupplyChain
{
    [OslcVocabulary("https://spdx.org/rdf/3/terms/SupplyChain/", "spdx_supply_chain")]
    public static partial class SpdxSupplyChain;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/BoundaryDefinitionAction")]
    public partial record BoundaryDefinitionAction;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/DefinedStateProcess")]
    public partial record DefinedStateProcess;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/DestroyAction")]
    public partial record DestroyAction;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/InspectionProcess")]
    public partial record InspectionProcess;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/ResponsibilityChangeAction")]
    public partial record ResponsibilityChangeAction;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/ResponsibilityChangeProcess")]
    public partial record ResponsibilityChangeProcess;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/StateAction")]
    public partial record StateAction;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/StorageProcess")]
    public partial record StorageProcess;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/TransportAction")]
    public partial record TransportAction;

    [OslcShape("https://spdx.org/rdf/3/terms/SupplyChain/TransportProcess")]
    public partial record TransportProcess;
}
