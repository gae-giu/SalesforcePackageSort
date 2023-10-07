namespace SalesforcePackageSort;

[XmlRoot(Namespace = "http://soap.sforce.com/2006/04/metadata")]
public class ApexTestSuite
{
    [XmlElement(ElementName = "testClassName")]
    public List<string> TestClassName { get; set; }
}
