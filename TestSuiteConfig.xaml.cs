namespace SalesforcePackageSort
{
    /// <summary>
    /// Interaction logic for TestSuiteConfig.xaml
    /// </summary>
    public partial class TestSuiteConfig : Window
    {
        public event EventHandler<EventClass> dialogClosed;

        public TestSuiteConfig()
        {
            InitializeComponent();
            rbEnds.IsChecked = true; // default
        }        

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            if (dialogClosed != null)
                dialogClosed(this, new EventClass(tbtestKeyword.Text, (bool)rbEnds.IsChecked));
            
            this.Close();
        }
    }
}
