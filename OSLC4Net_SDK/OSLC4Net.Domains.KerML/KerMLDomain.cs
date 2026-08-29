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

namespace OSLC4Net.Domains.KerML;

[OslcVocabulary("https://www.omg.org/spec/kerml/vocabulary#")]
public static partial class KerMLVocabulary;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#AnnotatingElementShape")]
public partial record AnnotatingElement;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#AnnotationShape")]
public partial record Annotation;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#AssociationShape")]
public partial record Association;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#AssociationStructureShape")]
public partial record AssociationStructure;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#BehaviorShape")]
public partial record Behavior;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#BindingConnectorShape")]
public partial record BindingConnector;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#BooleanExpressionShape")]
public partial record BooleanExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ClassShape")]
public partial record Class;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ClassifierShape")]
public partial record Classifier;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#CollectExpressionShape")]
public partial record CollectExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#CommentShape")]
public partial record Comment;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ConjugationShape")]
public partial record Conjugation;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ConnectorShape")]
public partial record Connector;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ConstructorExpressionShape")]
public partial record ConstructorExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#CrossSubsettingShape")]
public partial record CrossSubsetting;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#DataTypeShape")]
public partial record DataType;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#DependencyShape")]
public partial record Dependency;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#DifferencingShape")]
public partial record Differencing;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#DisjoiningShape")]
public partial record Disjoining;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#DocumentationShape")]
public partial record Documentation;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ElementShape")]
public partial record Element;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ElementFilterMembershipShape")]
public partial record ElementFilterMembership;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#EndFeatureMembershipShape")]
public partial record EndFeatureMembership;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ExpressionShape")]
public partial record Expression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FeatureShape")]
public partial record Feature;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FeatureChainExpressionShape")]
public partial record FeatureChainExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FeatureChainingShape")]
public partial record FeatureChaining;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FeatureInvertingShape")]
public partial record FeatureInverting;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FeatureMembershipShape")]
public partial record FeatureMembership;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FeatureReferenceExpressionShape")]
public partial record FeatureReferenceExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FeatureTypingShape")]
public partial record FeatureTyping;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FeatureValueShape")]
public partial record FeatureValue;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FlowShape")]
public partial record Flow;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FlowEndShape")]
public partial record FlowEnd;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#FunctionShape")]
public partial record Function;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ImportShape")]
public partial record Import;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#IndexExpressionShape")]
public partial record IndexExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#InstantiationExpressionShape")]
public partial record InstantiationExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#InteractionShape")]
public partial record Interaction;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#IntersectingShape")]
public partial record Intersecting;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#InvariantShape")]
public partial record Invariant;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#InvocationExpressionShape")]
public partial record InvocationExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#LibraryPackageShape")]
public partial record LibraryPackage;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#LiteralBooleanShape")]
public partial record LiteralBoolean;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#LiteralExpressionShape")]
public partial record LiteralExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#LiteralInfinityShape")]
public partial record LiteralInfinity;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#LiteralIntegerShape")]
public partial record LiteralInteger;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#LiteralRationalShape")]
public partial record LiteralRational;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#LiteralStringShape")]
public partial record LiteralString;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#MembershipShape")]
public partial record Membership;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#MembershipImportShape")]
public partial record MembershipImport;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#MetaclassShape")]
public partial record Metaclass;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#MetadataAccessExpressionShape")]
public partial record MetadataAccessExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#MetadataFeatureShape")]
public partial record MetadataFeature;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#MultiplicityShape")]
public partial record Multiplicity;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#MultiplicityRangeShape")]
public partial record MultiplicityRange;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#NamespaceShape")]
public partial record Namespace;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#NamespaceImportShape")]
public partial record NamespaceImport;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#NullExpressionShape")]
public partial record NullExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#OperatorExpressionShape")]
public partial record OperatorExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#OwningMembershipShape")]
public partial record OwningMembership;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#PackageShape")]
public partial record Package;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ParameterMembershipShape")]
public partial record ParameterMembership;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#PayloadFeatureShape")]
public partial record PayloadFeature;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#PredicateShape")]
public partial record Predicate;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#RedefinitionShape")]
public partial record Redefinition;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ReferenceSubsettingShape")]
public partial record ReferenceSubsetting;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#RelationshipShape")]
public partial record Relationship;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ResultExpressionMembershipShape")]
public partial record ResultExpressionMembership;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#ReturnParameterMembershipShape")]
public partial record ReturnParameterMembership;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#SelectExpressionShape")]
public partial record SelectExpression;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#SpecializationShape")]
public partial record Specialization;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#StepShape")]
public partial record Step;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#StructureShape")]
public partial record Structure;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#SubclassificationShape")]
public partial record Subclassification;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#SubsettingShape")]
public partial record Subsetting;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#SuccessionShape")]
public partial record Succession;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#SuccessionFlowShape")]
public partial record SuccessionFlow;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#TextualRepresentationShape")]
public partial record TextualRepresentation;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#TypeShape")]
public partial record Type;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#TypeFeaturingShape")]
public partial record TypeFeaturing;

[OslcShape("https://www.omg.org/spec/kerml/20250201/shapes#UnioningShape")]
public partial record Unioning;
