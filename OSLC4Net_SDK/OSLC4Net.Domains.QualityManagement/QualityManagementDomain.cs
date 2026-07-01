// Copyright (c) 2026 Andrii Berezovskyi and OSLC4Net contributors.

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Domains.QualityManagement;

[OslcVocabulary("http://open-services.net/ns/qm#")]
public static partial class QM;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestPlanShape")]
public partial record TestPlan;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestCaseShape")]
public partial record TestCase;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestScriptShape")]
public partial record TestScript;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestExecutionRecordShape")]
public partial record TestExecutionRecord;

[OslcShape("https://open-services.net/ns/qm/shapes/2.1/#TestResultShape")]
public partial record TestResult;
