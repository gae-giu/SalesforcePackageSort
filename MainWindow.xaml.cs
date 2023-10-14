namespace SalesforcePackageSort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string testKeyword;
        private bool endsWithIncludeMode;

        public MainWindow()
        {
            InitializeComponent();
            endsWithIncludeMode = true;
        }

        #region event functions

        private void btOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();

            if ((bool)openFileDialog.ShowDialog())
            {
                txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
                lbFilename.Content = openFileDialog.FileName;
                btOrderFile.IsEnabled = true;
                btTestSuite.IsEnabled = true;
                btTestSuite_Config.IsEnabled = true;
                btClear.IsEnabled = true;
            }
        }

        private void btOrderFile_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new(lbFilename.Content.ToString(), FileMode.Open);

            try
            {
                XmlSerializer xSerializer = new(typeof(Package));
                Package pack = (Package)xSerializer.Deserialize(fs);

                List<Type> newTypes = new();
                foreach (Type t in pack.Types.OrderBy(n => n.Name))
                    newTypes.Add(new Type {
                        Members = t.Members.Distinct().OrderBy(x => x).ToList(),
                        Name = t.Name,
                    });

                newTypes = MergeByNameAndMembers(newTypes);

                using (StringWriter sw = new())
                {
                    using (XmlTextWriter xw = new(sw) { Formatting = Formatting.Indented, Indentation = 4 })
                    {
                        xw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"");
                        XmlSerializerNamespaces xmlns = new();
                        xmlns.Add(string.Empty, string.Empty);
                        xSerializer.Serialize(xw, new Package { Types = newTypes, Version = pack.Version }, xmlns);
                        txtEditor.Text = sw.ToString();
                        MessageBox.Show("package.xml file ordered");
                        btClipboard.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btTestSuite_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new(lbFilename.Content.ToString(), FileMode.Open);
            if (string.IsNullOrWhiteSpace(testKeyword))
                testKeyword = "Test";

            try
            {
                XmlSerializer xSerializer = new(typeof(Package));
                Package pack = (Package)xSerializer.Deserialize(fs);

                List<string> tests = new();
                foreach (Type t in pack.Types)
                {
                    if (t.Name == "ApexClass")
                    {
                        foreach (string @class in t.Members)
                        {
                            if (endsWithIncludeMode)
                            {
                                if (@class.EndsWith(testKeyword))
                                    tests.Add(@class);
                            }
                            else
                            {
                                if (@class.Contains(testKeyword))
                                    tests.Add(@class);
                            }
                        }
                        break;
                    }
                }

                using (StringWriter sw = new())
                {
                    using (XmlTextWriter xw = new(sw) { Formatting = Formatting.Indented, Indentation = 4 })
                    {
                        XmlSerializer xSerializerTest = new(typeof(ApexTestSuite));

                        xw.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                        XmlSerializerNamespaces xmlns = new();
                        xmlns.Add(string.Empty, string.Empty);
                        xSerializerTest.Serialize(xw, new ApexTestSuite { TestClassName = tests }, xmlns);
                        txtEditor.Text = sw.ToString();
                        MessageBox.Show("Test suite XML generated");
                        btClipboard.IsEnabled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btTestSuite_Config_Click(object sender, RoutedEventArgs e)
        {
            TestSuiteConfig testSuite = new();
            testSuite.dialogClosed += new EventHandler<EventClass>(TestSuite_DialogClosed);
            testSuite.ShowDialog(); // modal opening            
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            txtEditor.Clear();
            lbFilename.Content = string.Empty;
            btOrderFile.IsEnabled = false;
            btTestSuite.IsEnabled = false;
            btTestSuite_Config.IsEnabled = false;
            btClear.IsEnabled = false;
            btClipboard.IsEnabled = false;
        }

        private void btClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEditor.Text))
            {
                Clipboard.SetText(txtEditor.Text);
                MessageBox.Show("XML text copied to clipboard");
            }
            else
                MessageBox.Show("Warning: no text to copy");
        }

        #endregion

        #region support functions

        private List<Type> MergeByNameAndMembers(List<Type> input)
        {
            List<Type> retValue = new();
            string previousName = string.Empty;
            int index = 0;

            foreach (Type t in input)
            {
                if (t.Name == previousName)
                {
                    retValue[index-1].Members.AddRange(t.Members);
                    retValue[index-1].Members = retValue[index-1].Members.Distinct().OrderBy(x => x).ToList();
                    index--;
                }
                else
                    retValue.Add(new Type {
                        Members = t.Members.Distinct().OrderBy(x => x).ToList(),
                        Name = t.Name,
                    });

                previousName = t.Name;
                index++;
            }

            return retValue;
        }

        private void TestSuite_DialogClosed(object sender, EventClass e)
        {
            testKeyword = e.testKeyword;
            endsWithIncludeMode = e.endsWithIncludeMode;
        }

        #endregion


    }
}
