using System;

using OSLC4Net.Core.Attribute;

namespace OSLC4Net.Core.Model;

[OslcNamespace(OslcConstants.OSLC_CORE_NAMESPACE)]
[OslcResourceShape(title = "OSLC Extended Error Resource Shape", describes = new string[]{OslcConstants.TYPE_EXTENDED_ERROR})]
class ExtendedError
{
    private string        hintHeight;
    private string        hintWidth;
    private Uri           moreInfo;
    private string        rel;

	    public ExtendedError():base()
    {
	    }

	    [OslcDescription("Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1) Em and ex units are interpreted relative to the default system font (at 100% size).")]
	    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "hintHeight")]
    [OslcReadOnly]
	    [OslcTitle("Hint Height")]
    public string GetHintHeight() {
        return hintHeight;
    }

    [OslcDescription("Values MUST be expressed in relative length units as defined in the W3C Cascading Style Sheets Specification (CSS 2.1) Em and ex units are interpreted relative to the default system font (at 100% size).")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "hintWidth")]
    [OslcReadOnly]
    [OslcTitle("Hint Width")]
    public string GetHintWidth()
    {
        return hintWidth;
    }

    [OslcDescription("A resource giving more information on the error SHOULD be of an HTML content-type.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "moreInfo")]
    [OslcReadOnly]
    [OslcTitle("More Info")]
    public Uri GetMoreInfo()
    {
        return moreInfo;
    }

    [OslcDescription("If present and set to 'alternate' then indicates that work-around is provided, behavior for other values is undefined.")]
    [OslcPropertyDefinition(OslcConstants.OSLC_CORE_NAMESPACE + "rel")]
    [OslcReadOnly]
    [OslcTitle("Rel")]
    public string GetRel()
    {
        return rel;
    }

    public void SetHintHeight(string hintHeight)
    {
        this.hintHeight = hintHeight;
    }

    public void SetHintWidth(string hintWidth)
    {
        this.hintWidth = hintWidth;
    }

    public void SetMoreInfo(Uri moreInfo)
    {
        this.moreInfo = moreInfo;
    }

    public void SetRel(string rel) {
        this.rel = rel;
    }
}
