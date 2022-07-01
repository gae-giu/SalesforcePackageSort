namespace SalesforcePackageSort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region event fucntions

        private void btOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();

            if ((bool)openFileDialog.ShowDialog())
            {
                txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
                lbFilename.Content = openFileDialog.FileName;
                btOrderFile.IsEnabled = true;
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
                    newTypes.Add(new Type
                    {
                        Members = t.Members.Distinct().OrderBy(x => x).ToList(),
                        Name = t.Name,
                    });

                newTypes = MergeByNameAndMembers(newTypes);

                using (StringWriter sw = new())
                {
                    using (XmlTextWriter xw = new(sw) { Formatting = Formatting.Indented })
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

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            txtEditor.Clear();
            lbFilename.Content = string.Empty;
            btOrderFile.IsEnabled = false;
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
            List<Type> retValue = new List<Type>();
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
                    retValue.Add(new Type
                    {
                        Members = t.Members.Distinct().OrderBy(x => x).ToList(),
                        Name = t.Name,
                    });

                previousName = t.Name;
                index++;
            }

            return retValue;
        }
        #endregion
    }
}
