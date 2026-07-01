// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Domains.ArchitectureManagement;

[OslcVocabulary("http://open-services.net/ns/am#")]
public static partial class AM;

[OslcShape("http://open-services.net/ns/am/shapes/3.0#ResourceShape")]
public partial record ArchitectureResource;

[OslcShape("http://open-services.net/ns/am/shapes/3.0#LinkTypeShape")]
public partial record LinkType;
