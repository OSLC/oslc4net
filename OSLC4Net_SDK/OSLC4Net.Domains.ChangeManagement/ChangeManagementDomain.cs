// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Domains.ChangeManagement;

[OslcVocabulary("http://open-services.net/ns/cm#")]
public static partial class CM;

[OslcShape("http://open-services.net/ns/cm/shapes/3.0#ChangeRequestShape")]
public partial record ChangeRequest;

[OslcShape("http://open-services.net/ns/cm/shapes/3.0#ChangeNoticeShape")]
public partial record ChangeNotice;

[OslcShape("http://open-services.net/ns/cm/shapes/3.0#DefectShape")]
public partial record Defect;

[OslcShape("http://open-services.net/ns/cm/shapes/3.0#EnhancementShape")]
public partial record Enhancement;

[OslcShape("http://open-services.net/ns/cm/shapes/3.0#ReviewTaskShape")]
public partial record ReviewTask;

[OslcShape("http://open-services.net/ns/cm/shapes/3.0#TaskShape")]
public partial record Task;
