// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Domains.RequirementsManagement;

[OslcVocabulary("http://open-services.net/ns/rm#")]
public static partial class RM;

[OslcShape("http://open-services.net/ns/rm/shapes/2.1#RequirementShape", Title = "Requirement Resource Shape")]
public partial record Requirement;

[OslcShape("http://open-services.net/ns/rm/shapes/2.1#RequirementCollectionShape", Title = "Requirement Collection resource shape")]
public partial record RequirementCollection;
