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
using OSLC4Net.Core.Model;
using OslcValueKind = OSLC4Net.Core.Model.ValueType;

namespace OSLC4Net.Domains.ChangeManagement;

public partial record ChangeRequest
{
    // REVISIT: undo overrides once OSLC OP publishes errata (2026-07).
    [OslcDescription(
        "Associated resource that is affected by this Change Request. It is likely that the target resource will be an oslc_qm:TestResult but that is not necessarily the case."
    )]
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/cm#affectsTestResult")]
    [OslcName("affectsTestResult")]
    [OslcValueType(OslcValueKind.Resource)]
    [OslcRepresentation(Representation.Reference)]
    [OslcRange("http://open-services.net/ns/qm#TestResult")]
    [OslcTitle("Affects Test Result")]
    public HashSet<Uri> AffectsTestResult { get; set; } = new(OslcUriEqualityComparer.Instance);

    [OslcDescription(
        "Associated QM resource that is blocked by this Change Request. It is likely that the target resource will be an oslc_qm:TestExecutionRecord but that is not necessarily the case."
    )]
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/cm#blocksTestExecutionRecord")]
    [OslcName("blocksTestExecutionRecord")]
    [OslcValueType(OslcValueKind.Resource)]
    [OslcRepresentation(Representation.Reference)]
    [OslcRange("http://open-services.net/ns/qm#TestExecutionRecord")]
    [OslcTitle("Blocks Test Execution Record")]
    public HashSet<Uri> BlocksTestExecutionRecord { get; set; } =
        new(OslcUriEqualityComparer.Instance);

    [OslcDescription(
        "Related QM test case resource. It is likely that the target resource will be an oslc_qm:TestCase but that is not necessarily the case."
    )]
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/cm#relatedTestCase")]
    [OslcName("relatedTestCase")]
    [OslcValueType(OslcValueKind.Resource)]
    [OslcRepresentation(Representation.Reference)]
    [OslcRange("http://open-services.net/ns/qm#TestCase")]
    [OslcTitle("Related Test Case")]
    public HashSet<Uri> RelatedTestCase { get; set; } = new(OslcUriEqualityComparer.Instance);

    [OslcDescription(
        "Related to a test execution resource. It is likely that the target resource will be an oslc_qm:TestExecutionRecord but that is not necessarily the case."
    )]
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/cm#relatedTestExecutionRecord")]
    [OslcName("relatedTestExecutionRecord")]
    [OslcValueType(OslcValueKind.Resource)]
    [OslcRepresentation(Representation.Reference)]
    [OslcRange("http://open-services.net/ns/qm#TestExecutionRecord")]
    [OslcTitle("Related Test Execution Record")]
    public HashSet<Uri> RelatedTestExecutionRecord { get; set; } =
        new(OslcUriEqualityComparer.Instance);

    [OslcDescription(
        "Related test plan resource. It is likely that the target resource will be an oslc_qm:TestPlan but that is not necessarily the case."
    )]
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/cm#relatedTestPlan")]
    [OslcName("relatedTestPlan")]
    [OslcValueType(OslcValueKind.Resource)]
    [OslcRepresentation(Representation.Reference)]
    [OslcRange("http://open-services.net/ns/qm#TestPlan")]
    [OslcTitle("Related Test Plan")]
    public HashSet<Uri> RelatedTestPlan { get; set; } = new(OslcUriEqualityComparer.Instance);

    [OslcDescription(
        "Related QM test script resource. It is likely that the target resource will be an oslc_qm:TestScript but that is not necessarily the case."
    )]
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/cm#relatedTestScript")]
    [OslcName("relatedTestScript")]
    [OslcValueType(OslcValueKind.Resource)]
    [OslcRepresentation(Representation.Reference)]
    [OslcRange("http://open-services.net/ns/qm#TestScript")]
    [OslcTitle("Related Test Script")]
    public HashSet<Uri> RelatedTestScript { get; set; } = new(OslcUriEqualityComparer.Instance);

    [OslcDescription(
        "Test case by which this change request is tested. It is likely that the target resource will be an oslc_qm:TestCase but that is not necessarily the case."
    )]
    [OslcOccurs(Occurs.ZeroOrMany)]
    [OslcPropertyDefinition("http://open-services.net/ns/cm#testedByTestCase")]
    [OslcName("testedByTestCase")]
    [OslcValueType(OslcValueKind.Resource)]
    [OslcRepresentation(Representation.Reference)]
    [OslcRange("http://open-services.net/ns/qm#TestCase")]
    [OslcTitle("Tested By Test Case")]
    public HashSet<Uri> TestedByTestCase { get; set; } = new(OslcUriEqualityComparer.Instance);
}
