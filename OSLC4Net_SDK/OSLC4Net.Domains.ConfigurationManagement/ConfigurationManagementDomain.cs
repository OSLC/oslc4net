// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Domains.ConfigurationManagement;

[OslcVocabulary("http://open-services.net/ns/config#")]
public static partial class Config;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#ActivityShape")]
public partial record Activity;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#BaselineShape")]
public partial record Baseline;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#ChangeSetShape")]
public partial record ChangeSet;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#ComponentShape")]
public partial record Component;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#ContributionShape")]
public partial record Contribution;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#CSelectionsShape")]
public partial record ChangeSetSelections;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#SelectionsShape")]
public partial record Selections;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#StreamShape")]
public partial record Stream;

[OslcShape("http://open-services.net/ns/config/shapes/1.0/#VersionResourceShape")]
public partial record VersionResource;
