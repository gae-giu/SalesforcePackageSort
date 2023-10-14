namespace SalesforcePackageSort
{
    /// <summary>
    /// Interaction logic for TestSuiteConfig.xaml
    /// </summary>
    public partial class TestSuiteConfig : Window
    {
        public string testKeyword;
        public bool endsWithIncludeMode;
        public event EventHandler<EventClass> dialogClosed;

        public TestSuiteConfig()
        {
            InitializeComponent();
            rbEnds.IsChecked = true; // default
        }        

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            testKeyword = tbtestKeyword.Text;
            endsWithIncludeMode = (bool)rbEnds.IsChecked;

            if (dialogClosed != null)
                dialogClosed(this, new EventClass(tbtestKeyword.Text, (bool)rbEnds.IsChecked));
            
            this.Close();
        }
    }
}
