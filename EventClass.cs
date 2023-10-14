namespace SalesforcePackageSort
{
    public class EventClass : EventArgs
    {
        public string testKeyword { get; private set; }
        public bool endsWithIncludeMode { get; private set; }

        public EventClass(string tk, bool im)
        {
            testKeyword = tk;
            endsWithIncludeMode = im;
        }
    }
}
