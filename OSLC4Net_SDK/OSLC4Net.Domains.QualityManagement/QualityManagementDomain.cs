// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.CodeGen;
using OSLC4Net.Core.Model;

namespace OSLC4Net.Domains.QualityManagement;

[OslcVocabulary("http://open-services.net/ns/qm#")]
public static partial class QM;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestPlanShape")]
public partial record TestPlan : AbstractResourceRecord;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestCaseShape")]
public partial record TestCase : AbstractResourceRecord;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestScriptShape")]
public partial record TestScript : AbstractResourceRecord;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestExecutionRecordShape")]
public partial record TestExecutionRecord : AbstractResourceRecord;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestResultShape")]
public partial record TestResult : AbstractResourceRecord;
