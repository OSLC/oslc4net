namespace OSLC4Net.Core.Model;

public static partial class OslcConstants
{
    public static partial class Domains
    {
        public static class QUDT
        {
            public const string NS = "http://qudt.org/schema/qudt/";
            public const string Prefix = "qudt";

            public static QName QNameFor(string localResource)
            {
                return new QName(NS, localResource, Prefix);
            }

            public static class P
            {
                public const string Abbreviation = NS + "abbreviation";
                public const string Acronym = NS + "acronym";
                public const string AllowedPattern = NS + "allowedPattern";
                public const string AltSymbol = NS + "altSymbol";
                public const string AnsiSqlname = NS + "ansiSQLName";
                public const string ApplicableCgsunit = NS + "applicableCGSUnit";
                public const string ApplicableIsounit = NS + "applicableISOUnit";
                public const string ApplicableImperialUnit = NS + "applicableImperialUnit";
                public const string ApplicablePhysicalConstant = NS + "applicablePhysicalConstant";
                public const string ApplicablePlanckUnit = NS + "applicablePlanckUnit";
                public const string ApplicableSiunit = NS + "applicableSIUnit";
                public const string ApplicableSystem = NS + "applicableSystem";
                public const string ApplicableUscustomaryUnit = NS + "applicableUSCustomaryUnit";
                public const string ApplicableUnit = NS + "applicableUnit";
                public const string BaseDimensionEnumeration = NS + "baseDimensionEnumeration";
                public const string BaseUnitOfSystem = NS + "baseUnitOfSystem";
                public const string Basis = NS + "basis";

                public const string BelongsToSystemOfQuantities =
                    NS + "belongsToSystemOfQuantities";

                public const string BitOrder = NS + "bitOrder";
                public const string Bits = NS + "bits";
                public const string Bounded = NS + "bounded";
                public const string ByteOrder = NS + "byteOrder";
                public const string Bytes = NS + "bytes";
                public const string CName = NS + "cName";
                public const string Cardinality = NS + "cardinality";
                public const string CategorizedAs = NS + "categorizedAs";
                public const string CoherentUnitSystem = NS + "coherentUnitSystem";
                public const string ConversionMultiplier = NS + "conversionMultiplier";
                public const string ConversionMultiplierSn = NS + "conversionMultiplierSN";
                public const string ConversionOffset = NS + "conversionOffset";
                public const string ConversionOffsetSn = NS + "conversionOffsetSN";
                public const string CurrencyCode = NS + "currencyCode";
                public const string CurrencyExponent = NS + "currencyExponent";
                public const string CurrencyNumber = NS + "currencyNumber";
                public const string DataEncoding = NS + "dataEncoding";
                public const string DataStructure = NS + "dataStructure";
                public const string Datatype = NS + "datatype";
                public const string DbpediaMatch = NS + "dbpediaMatch";
                public const string Default = NS + "default";
                public const string DefinedUnitOfSystem = NS + "definedUnitOfSystem";
                public const string DenominatorDimensionVector = NS + "denominatorDimensionVector";
                public const string Deprecated = NS + "deprecated";

                public const string DerivedCoherentUnitOfSystem =
                    NS + "derivedCoherentUnitOfSystem";

                public const string DerivedNonCoherentUnitOfSystem =
                    NS + "derivedNonCoherentUnitOfSystem";

                public const string DerivedUnitOfSystem = NS + "derivedUnitOfSystem";
                public const string DimensionExponent = NS + "dimensionExponent";

                public const string DimensionExponentForAmountOfSubstance =
                    NS + "dimensionExponentForAmountOfSubstance";

                public const string DimensionExponentForElectricCurrent =
                    NS + "dimensionExponentForElectricCurrent";

                public const string DimensionExponentForLength = NS + "dimensionExponentForLength";

                public const string DimensionExponentForLuminousIntensity =
                    NS + "dimensionExponentForLuminousIntensity";

                public const string DimensionExponentForMass = NS + "dimensionExponentForMass";

                public const string DimensionExponentForThermodynamicTemperature =
                    NS + "dimensionExponentForThermodynamicTemperature";

                public const string DimensionExponentForTime = NS + "dimensionExponentForTime";
                public const string DimensionInverse = NS + "dimensionInverse";
                public const string DimensionVectorForSi = NS + "dimensionVectorForSI";
                public const string DimensionlessExponent = NS + "dimensionlessExponent";
                public const string Element = NS + "element";
                public const string ElementKind = NS + "elementKind";
                public const string Encoding = NS + "encoding";
                public const string EnumeratedValue = NS + "enumeratedValue";
                public const string Enumeration = NS + "enumeration";
                public const string ExactConstant = NS + "exactConstant";
                public const string ExactMatch = NS + "exactMatch";
                public const string Exponent = NS + "exponent";
                public const string FactorUnitScalar = NS + "factorUnitScalar";
                public const string FieldCode = NS + "fieldCode";
                public const string Figure = NS + "figure";
                public const string FigureCaption = NS + "figureCaption";
                public const string FigureLabel = NS + "figureLabel";
                public const string Guidance = NS + "guidance";
                public const string HasAllowedUnit = NS + "hasAllowedUnit";
                public const string HasBaseQuantityKind = NS + "hasBaseQuantityKind";
                public const string HasBaseUnit = NS + "hasBaseUnit";
                public const string HasCitation = NS + "hasCitation";
                public const string HasCoherentUnit = NS + "hasCoherentUnit";
                public const string HasDefinedUnit = NS + "hasDefinedUnit";
                public const string HasDenominatorPart = NS + "hasDenominatorPart";
                public const string HasDerivedCoherentUnit = NS + "hasDerivedCoherentUnit";
                public const string HasDerivedNonCoherentUnit = NS + "hasDerivedNonCoherentUnit";
                public const string HasDerivedUnit = NS + "hasDerivedUnit";
                public const string HasDimension = NS + "hasDimension";
                public const string HasDimensionExpression = NS + "hasDimensionExpression";
                public const string HasDimensionVector = NS + "hasDimensionVector";
                public const string HasFactorUnit = NS + "hasFactorUnit";
                public const string HasNumeratorPart = NS + "hasNumeratorPart";
                public const string HasPrefixUnit = NS + "hasPrefixUnit";
                public const string HasQuantity = NS + "hasQuantity";
                public const string HasQuantityKind = NS + "hasQuantityKind";
                public const string HasReferenceQuantityKind = NS + "hasReferenceQuantityKind";
                public const string HasRule = NS + "hasRule";
                public const string HasUnit = NS + "hasUnit";
                public const string HasUnitSystem = NS + "hasUnitSystem";
                public const string HasVocabulary = NS + "hasVocabulary";
                public const string Height = NS + "height";
                public const string Id = NS + "id";
                public const string Iec61360Code = NS + "iec61360Code";
                public const string Image = NS + "image";
                public const string ImageLocation = NS + "imageLocation";
                public const string IsDeltaQuantity = NS + "isDeltaQuantity";
                public const string IsDimensionInSystem = NS + "isDimensionInSystem";
                public const string IsMetricUnit = NS + "isMetricUnit";
                public const string IsUnitOfSystem = NS + "isUnitOfSystem";
                public const string IsoNormativeReference = NS + "isoNormativeReference";
                public const string JavaName = NS + "javaName";
                public const string JsName = NS + "jsName";
                public const string Landscape = NS + "landscape";
                public const string LatexDefinition = NS + "latexDefinition";
                public const string LatexSymbol = NS + "latexSymbol";
                public const string Length = NS + "length";
                public const string LowerBound = NS + "lowerBound";
                public const string MathDefinition = NS + "mathDefinition";
                public const string MathMldefinition = NS + "mathMLdefinition";
                public const string MatlabName = NS + "matlabName";
                public const string MaxExclusive = NS + "maxExclusive";
                public const string MaxInclusive = NS + "maxInclusive";
                public const string MicrosoftSqlserverName = NS + "microsoftSQLServerName";
                public const string MinExclusive = NS + "minExclusive";
                public const string MinInclusive = NS + "minInclusive";
                public const string MySqlname = NS + "mySQLName";
                public const string NegativeDeltaLimit = NS + "negativeDeltaLimit";
                public const string NormativeReference = NS + "normativeReference";
                public const string NumeratorDimensionVector = NS + "numeratorDimensionVector";
                public const string NumericValue = NS + "numericValue";
                public const string OdbcName = NS + "odbcName";
                public const string OleDbname = NS + "oleDBName";
                public const string OmUnit = NS + "omUnit";
                public const string OnlineReference = NS + "onlineReference";
                public const string OracleSqlname = NS + "oracleSQLName";
                public const string Order = NS + "order";
                public const string OrderedType = NS + "orderedType";
                public const string OutOfScope = NS + "outOfScope";
                public const string PermissibleMaths = NS + "permissibleMaths";
                public const string PermissibleTransformation = NS + "permissibleTransformation";
                public const string PlainTextDescription = NS + "plainTextDescription";
                public const string PositiveDeltaLimit = NS + "positiveDeltaLimit";
                public const string Prefix = NS + "prefix";
                public const string PrefixMultiplier = NS + "prefixMultiplier";
                public const string PrefixMultiplierSn = NS + "prefixMultiplierSN";
                public const string ProtocolBuffersName = NS + "protocolBuffersName";
                public const string PythonName = NS + "pythonName";
                public const string QkdvDenominator = NS + "qkdvDenominator";
                public const string QkdvNumerator = NS + "qkdvNumerator";
                public const string Quantity = NS + "quantity";
                public const string QuantityValue = NS + "quantityValue";
                public const string Rationale = NS + "rationale";
                public const string RdfsDatatype = NS + "rdfsDatatype";
                public const string Reference = NS + "reference";
                public const string ReferenceUnit = NS + "referenceUnit";

                public const string RelativeStandardUncertainty =
                    NS + "relativeStandardUncertainty";

                public const string RelevantQuantityKind = NS + "relevantQuantityKind";
                public const string RelevantUnit = NS + "relevantUnit";
                public const string RuleType = NS + "ruleType";
                public const string ScaleType = NS + "scaleType";
                public const string ScalingOf = NS + "scalingOf";
                public const string SiExactMatch = NS + "siExactMatch";
                public const string SiUnitsExpression = NS + "siUnitsExpression";
                public const string StandardUncertainty = NS + "standardUncertainty";
                public const string StandardUncertaintySn = NS + "standardUncertaintySN";
                public const string Symbol = NS + "symbol";
                public const string SystemDefinition = NS + "systemDefinition";
                public const string SystemDerivedQuantityKind = NS + "systemDerivedQuantityKind";
                public const string SystemDimension = NS + "systemDimension";
                public const string UcumCode = NS + "ucumCode";
                public const string UdunitsCode = NS + "udunitsCode";
                public const string UneceCommonCode = NS + "uneceCommonCode";
                public const string UnitFor = NS + "unitFor";
                public const string UpperBound = NS + "upperBound";
                public const string Url = NS + "url";
                public const string Value = NS + "value";
                public const string ValueQuantity = NS + "valueQuantity";
                public const string ValueSn = NS + "valueSN";
                public const string VbName = NS + "vbName";
                public const string VectorMagnitude = NS + "vectorMagnitude";
                public const string Width = NS + "width";
            }

            public static class Q
            {
                public static QName Abbreviation => QNameFor("abbreviation");
                public static QName Acronym => QNameFor("acronym");
                public static QName AllowedPattern => QNameFor("allowedPattern");
                public static QName AltSymbol => QNameFor("altSymbol");
                public static QName AnsiSqlname => QNameFor("ansiSQLName");
                public static QName ApplicableCgsunit => QNameFor("applicableCGSUnit");
                public static QName ApplicableIsounit => QNameFor("applicableISOUnit");
                public static QName ApplicableImperialUnit => QNameFor("applicableImperialUnit");

                public static QName ApplicablePhysicalConstant =>
                    QNameFor("applicablePhysicalConstant");

                public static QName ApplicablePlanckUnit => QNameFor("applicablePlanckUnit");
                public static QName ApplicableSiunit => QNameFor("applicableSIUnit");
                public static QName ApplicableSystem => QNameFor("applicableSystem");

                public static QName ApplicableUscustomaryUnit =>
                    QNameFor("applicableUSCustomaryUnit");

                public static QName ApplicableUnit => QNameFor("applicableUnit");

                public static QName BaseDimensionEnumeration =>
                    QNameFor("baseDimensionEnumeration");

                public static QName BaseUnitOfSystem => QNameFor("baseUnitOfSystem");
                public static QName Basis => QNameFor("basis");

                public static QName BelongsToSystemOfQuantities =>
                    QNameFor("belongsToSystemOfQuantities");

                public static QName BitOrder => QNameFor("bitOrder");
                public static QName Bits => QNameFor("bits");
                public static QName Bounded => QNameFor("bounded");
                public static QName ByteOrder => QNameFor("byteOrder");
                public static QName Bytes => QNameFor("bytes");
                public static QName CName => QNameFor("cName");
                public static QName Cardinality => QNameFor("cardinality");
                public static QName CategorizedAs => QNameFor("categorizedAs");
                public static QName CoherentUnitSystem => QNameFor("coherentUnitSystem");
                public static QName ConversionMultiplier => QNameFor("conversionMultiplier");
                public static QName ConversionMultiplierSn => QNameFor("conversionMultiplierSN");
                public static QName ConversionOffset => QNameFor("conversionOffset");
                public static QName ConversionOffsetSn => QNameFor("conversionOffsetSN");
                public static QName CurrencyCode => QNameFor("currencyCode");
                public static QName CurrencyExponent => QNameFor("currencyExponent");
                public static QName CurrencyNumber => QNameFor("currencyNumber");
                public static QName DataEncoding => QNameFor("dataEncoding");
                public static QName DataStructure => QNameFor("dataStructure");
                public static QName Datatype => QNameFor("datatype");
                public static QName DbpediaMatch => QNameFor("dbpediaMatch");
                public static QName Default => QNameFor("default");
                public static QName DefinedUnitOfSystem => QNameFor("definedUnitOfSystem");

                public static QName DenominatorDimensionVector =>
                    QNameFor("denominatorDimensionVector");

                public static QName Deprecated => QNameFor("deprecated");

                public static QName DerivedCoherentUnitOfSystem =>
                    QNameFor("derivedCoherentUnitOfSystem");

                public static QName DerivedNonCoherentUnitOfSystem =>
                    QNameFor("derivedNonCoherentUnitOfSystem");

                public static QName DerivedUnitOfSystem => QNameFor("derivedUnitOfSystem");
                public static QName DimensionExponent => QNameFor("dimensionExponent");

                public static QName DimensionExponentForAmountOfSubstance =>
                    QNameFor("dimensionExponentForAmountOfSubstance");

                public static QName DimensionExponentForElectricCurrent =>
                    QNameFor("dimensionExponentForElectricCurrent");

                public static QName DimensionExponentForLength =>
                    QNameFor("dimensionExponentForLength");

                public static QName DimensionExponentForLuminousIntensity =>
                    QNameFor("dimensionExponentForLuminousIntensity");

                public static QName DimensionExponentForMass =>
                    QNameFor("dimensionExponentForMass");

                public static QName DimensionExponentForThermodynamicTemperature =>
                    QNameFor("dimensionExponentForThermodynamicTemperature");

                public static QName DimensionExponentForTime =>
                    QNameFor("dimensionExponentForTime");

                public static QName DimensionInverse => QNameFor("dimensionInverse");
                public static QName DimensionVectorForSi => QNameFor("dimensionVectorForSI");
                public static QName DimensionlessExponent => QNameFor("dimensionlessExponent");
                public static QName Element => QNameFor("element");
                public static QName ElementKind => QNameFor("elementKind");
                public static QName Encoding => QNameFor("encoding");
                public static QName EnumeratedValue => QNameFor("enumeratedValue");
                public static QName Enumeration => QNameFor("enumeration");
                public static QName ExactConstant => QNameFor("exactConstant");
                public static QName ExactMatch => QNameFor("exactMatch");
                public static QName Exponent => QNameFor("exponent");
                public static QName FactorUnitScalar => QNameFor("factorUnitScalar");
                public static QName FieldCode => QNameFor("fieldCode");
                public static QName Figure => QNameFor("figure");
                public static QName FigureCaption => QNameFor("figureCaption");
                public static QName FigureLabel => QNameFor("figureLabel");
                public static QName Guidance => QNameFor("guidance");
                public static QName HasAllowedUnit => QNameFor("hasAllowedUnit");
                public static QName HasBaseQuantityKind => QNameFor("hasBaseQuantityKind");
                public static QName HasBaseUnit => QNameFor("hasBaseUnit");
                public static QName HasCitation => QNameFor("hasCitation");
                public static QName HasCoherentUnit => QNameFor("hasCoherentUnit");
                public static QName HasDefinedUnit => QNameFor("hasDefinedUnit");
                public static QName HasDenominatorPart => QNameFor("hasDenominatorPart");
                public static QName HasDerivedCoherentUnit => QNameFor("hasDerivedCoherentUnit");

                public static QName HasDerivedNonCoherentUnit =>
                    QNameFor("hasDerivedNonCoherentUnit");

                public static QName HasDerivedUnit => QNameFor("hasDerivedUnit");
                public static QName HasDimension => QNameFor("hasDimension");
                public static QName HasDimensionExpression => QNameFor("hasDimensionExpression");
                public static QName HasDimensionVector => QNameFor("hasDimensionVector");
                public static QName HasFactorUnit => QNameFor("hasFactorUnit");
                public static QName HasNumeratorPart => QNameFor("hasNumeratorPart");
                public static QName HasPrefixUnit => QNameFor("hasPrefixUnit");
                public static QName HasQuantity => QNameFor("hasQuantity");
                public static QName HasQuantityKind => QNameFor("hasQuantityKind");

                public static QName HasReferenceQuantityKind =>
                    QNameFor("hasReferenceQuantityKind");

                public static QName HasRule => QNameFor("hasRule");
                public static QName HasUnit => QNameFor("hasUnit");
                public static QName HasUnitSystem => QNameFor("hasUnitSystem");
                public static QName HasVocabulary => QNameFor("hasVocabulary");
                public static QName Height => QNameFor("height");
                public static QName Id => QNameFor("id");
                public static QName Iec61360Code => QNameFor("iec61360Code");
                public static QName Image => QNameFor("image");
                public static QName ImageLocation => QNameFor("imageLocation");
                public static QName IsDeltaQuantity => QNameFor("isDeltaQuantity");
                public static QName IsDimensionInSystem => QNameFor("isDimensionInSystem");
                public static QName IsMetricUnit => QNameFor("isMetricUnit");
                public static QName IsUnitOfSystem => QNameFor("isUnitOfSystem");
                public static QName IsoNormativeReference => QNameFor("isoNormativeReference");
                public static QName JavaName => QNameFor("javaName");
                public static QName JsName => QNameFor("jsName");
                public static QName Landscape => QNameFor("landscape");
                public static QName LatexDefinition => QNameFor("latexDefinition");
                public static QName LatexSymbol => QNameFor("latexSymbol");
                public static QName Length => QNameFor("length");
                public static QName LowerBound => QNameFor("lowerBound");
                public static QName MathDefinition => QNameFor("mathDefinition");
                public static QName MathMldefinition => QNameFor("mathMLdefinition");
                public static QName MatlabName => QNameFor("matlabName");
                public static QName MaxExclusive => QNameFor("maxExclusive");
                public static QName MaxInclusive => QNameFor("maxInclusive");
                public static QName MicrosoftSqlserverName => QNameFor("microsoftSQLServerName");
                public static QName MinExclusive => QNameFor("minExclusive");
                public static QName MinInclusive => QNameFor("minInclusive");
                public static QName MySqlname => QNameFor("mySQLName");
                public static QName NegativeDeltaLimit => QNameFor("negativeDeltaLimit");
                public static QName NormativeReference => QNameFor("normativeReference");

                public static QName NumeratorDimensionVector =>
                    QNameFor("numeratorDimensionVector");

                public static QName NumericValue => QNameFor("numericValue");
                public static QName OdbcName => QNameFor("odbcName");
                public static QName OleDbname => QNameFor("oleDBName");
                public static QName OmUnit => QNameFor("omUnit");
                public static QName OnlineReference => QNameFor("onlineReference");
                public static QName OracleSqlname => QNameFor("oracleSQLName");
                public static QName Order => QNameFor("order");
                public static QName OrderedType => QNameFor("orderedType");
                public static QName OutOfScope => QNameFor("outOfScope");
                public static QName PermissibleMaths => QNameFor("permissibleMaths");

                public static QName PermissibleTransformation =>
                    QNameFor("permissibleTransformation");

                public static QName PlainTextDescription => QNameFor("plainTextDescription");
                public static QName PositiveDeltaLimit => QNameFor("positiveDeltaLimit");
                public static QName Prefix => QNameFor("prefix");
                public static QName PrefixMultiplier => QNameFor("prefixMultiplier");
                public static QName PrefixMultiplierSn => QNameFor("prefixMultiplierSN");
                public static QName ProtocolBuffersName => QNameFor("protocolBuffersName");
                public static QName PythonName => QNameFor("pythonName");
                public static QName QkdvDenominator => QNameFor("qkdvDenominator");
                public static QName QkdvNumerator => QNameFor("qkdvNumerator");
                public static QName Quantity => QNameFor("quantity");
                public static QName QuantityValue => QNameFor("quantityValue");
                public static QName Rationale => QNameFor("rationale");
                public static QName RdfsDatatype => QNameFor("rdfsDatatype");
                public static QName Reference => QNameFor("reference");
                public static QName ReferenceUnit => QNameFor("referenceUnit");

                public static QName RelativeStandardUncertainty =>
                    QNameFor("relativeStandardUncertainty");

                public static QName RelevantQuantityKind => QNameFor("relevantQuantityKind");
                public static QName RelevantUnit => QNameFor("relevantUnit");
                public static QName RuleType => QNameFor("ruleType");
                public static QName ScaleType => QNameFor("scaleType");
                public static QName ScalingOf => QNameFor("scalingOf");
                public static QName SiExactMatch => QNameFor("siExactMatch");
                public static QName SiUnitsExpression => QNameFor("siUnitsExpression");
                public static QName StandardUncertainty => QNameFor("standardUncertainty");
                public static QName StandardUncertaintySn => QNameFor("standardUncertaintySN");
                public static QName Symbol => QNameFor("symbol");
                public static QName SystemDefinition => QNameFor("systemDefinition");

                public static QName SystemDerivedQuantityKind =>
                    QNameFor("systemDerivedQuantityKind");

                public static QName SystemDimension => QNameFor("systemDimension");
                public static QName UcumCode => QNameFor("ucumCode");
                public static QName UdunitsCode => QNameFor("udunitsCode");
                public static QName UneceCommonCode => QNameFor("uneceCommonCode");
                public static QName UnitFor => QNameFor("unitFor");
                public static QName UpperBound => QNameFor("upperBound");
                public static QName Url => QNameFor("url");
                public static QName Value => QNameFor("value");
                public static QName ValueQuantity => QNameFor("valueQuantity");
                public static QName ValueSn => QNameFor("valueSN");
                public static QName VbName => QNameFor("vbName");
                public static QName VectorMagnitude => QNameFor("vectorMagnitude");
                public static QName Width => QNameFor("width");
            }
        }
    }
}
