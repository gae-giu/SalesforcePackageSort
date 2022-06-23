namespace SalesforcePackageSort
{
    [XmlRoot(Namespace = "http://soap.sforce.com/2006/04/metadata")]
    public class Package
    {
        [XmlElement(ElementName = "types")]
        public List<Type> Types { get; set; }
        [XmlElement(ElementName = "version")]
        public string Version { get; set; }
    }

    public class Type
    {
        [XmlElement(ElementName = "members")]
        public List<string> Members { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }
}
