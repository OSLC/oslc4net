/*
 * Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution.
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 */

# Migration guide

## Replace public and protected fields with properties

Rebuild applications that consume these types because fields have been removed
from the binary API. Update source references where the replacement property has
a different name.

- `OslcAllowedValue.value` → `OslcAllowedValue.Value`
- `OslcAllowedValues.value` → `OslcAllowedValues.Value`
- `OslcCreationFactory.label` → `OslcCreationFactory.Label`
- `OslcCreationFactory.resourceShapes` → `OslcCreationFactory.ResourceShapes`
- `OslcCreationFactory.resourceTypes` → `OslcCreationFactory.ResourceTypes`
- `OslcCreationFactory.title` → `OslcCreationFactory.Title`
- `OslcCreationFactory.usages` → `OslcCreationFactory.Usages`
- `OslcDefaultValue.value` → `OslcDefaultValue.Value`
- `OslcDescription.value` → `OslcDescription.Value`
- `OslcDialog.hintHeight` → `OslcDialog.HintHeight`
- `OslcDialog.hintWidth` → `OslcDialog.HintWidth`
- `OslcDialog.label` → `OslcDialog.Label`
- `OslcDialog.resourceTypes` → `OslcDialog.ResourceTypes`
- `OslcDialog.title` → `OslcDialog.Title`
- `OslcDialog.uri` → `OslcDialog.Uri`
- `OslcDialog.usages` → `OslcDialog.Usages`
- `OslcDialogs.Value` field → `OslcDialogs.Value` property
- `OslcHidden.value` → `OslcHidden.Value`
- `OslcMaxSize.value` → `OslcMaxSize.Value`
- `OslcMemberProperty.value` → `OslcMemberProperty.Value`
- `OslcName.value` → `OslcName.Value`
- `OslcNamespace.value` → `OslcNamespace.Value`
- `OslcNamespaceDefinition.namespaceURI` → `OslcNamespaceDefinition.NamespaceUri`
- `OslcNamespaceDefinition.prefix` → `OslcNamespaceDefinition.Prefix`
- `OslcOccurs.value` → `OslcOccurs.Value`
- `OslcPropertyDefinition.value` → `OslcPropertyDefinition.Value`
- `OslcQueryCapability.Label` field → `OslcQueryCapability.Label` property
- `OslcQueryCapability.ResourceShape` field → `OslcQueryCapability.ResourceShape` property
- `OslcQueryCapability.ResourceTypes` field → `OslcQueryCapability.ResourceTypes` property
- `OslcQueryCapability.Title` field → `OslcQueryCapability.Title` property
- `OslcQueryCapability.Usages` field → `OslcQueryCapability.Usages` property
- `OslcRange.value` → `OslcRange.Value`
- `OslcRdfCollectionType.collectionType` → `OslcRdfCollectionType.CollectionType`
- `OslcRdfCollectionType.namespaceURI` → `OslcRdfCollectionType.NamespaceUri`
- `OslcReadOnly.value` → `OslcReadOnly.Value`
- `OslcRepresentation.value` → `OslcRepresentation.Value`
- `OslcResourceShape.describes` → `OslcResourceShape.Describes`
- `OslcResourceShape.title` → `OslcResourceShape.Title`
- `OslcSchema.namespaceType` → `OslcSchema.NamespaceType`
- `OslcService.value` → `OslcService.Value`
- `OslcTitle.value` → `OslcTitle.Value`
- `OslcValueShape.value` → `OslcValueShape.Value`
- `OslcValueType.value` → `OslcValueType.Value`
- `OslcMediaType.APPLICATION_RDF_XML_TYPE` → `OslcMediaType.ApplicationRdfXmlType`
- `OslcMediaType.APPLICATION_JSON_LD_TYPE` → `OslcMediaType.ApplicationJsonLdType`
- `OslcMediaType.TEXT_TURTLE_TYPE` → `OslcMediaType.TextTurtleType`
- `OslcMediaType.APPLICATION_X_OSLC_COMPACT_XML_TYPE` → `OslcMediaType.ApplicationXOslcCompactXmlType`
- `OslcMediaType.APPLICATION_X_OSLC_COMPACT_JSON_TYPE` → `OslcMediaType.ApplicationXOslcCompactJsonType`
- `OslcMediaType.APPLICATION_JSON_TYPE` → `OslcMediaType.ApplicationJsonType`
- `OslcMediaType.APPLICATION_XML_TYPE` → `OslcMediaType.ApplicationXmlType`
- `OslcMediaType.TEXT_XML_TYPE` → `OslcMediaType.TextXmlType`
- `URI.uri` → `URI.Uri`
- `OSLC4NetConstants.OSLC4NET_PROPERTY_SINGLETON` → `OSLC4NetConstants.Oslc4NetPropertySingleton`
- `PName.ns` → `PName.Namespace`
- `PName.prefix` → `PName.Prefix`
- `PName.local` → `PName.LocalName`
- `OslcClient._formatters` → protected property `OslcClient.Formatters`
- `OslcClient._client` → protected property `OslcClient.Client`
- `RequirementBase.RdfTypes` field → protected property `RequirementBase.RdfTypes`
- `SampleBase.Logger` field → protected property `SampleBase.Logger`
- `TestBase.Config` → protected property `TestBase.Configuration`
- `TestBase.ServiceProviderCatalogUri` field → protected property `TestBase.ServiceProviderCatalogUri`
