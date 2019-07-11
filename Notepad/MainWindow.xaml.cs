using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Notepad
{
    public partial class MainWindow
    {
        private string filePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "choose a txt";
            fileDialog.Filter = "all files(*xls*)|*.txt*";
            if (fileDialog.ShowDialog() != false)
            {
                filePath = fileDialog.FileName;
                var tabItem = new TabItem() { Header = fileDialog.SafeFileName };
                var listBox = new ListBox();
                tabItem.Content = listBox;
                TabControl.Items.Add(tabItem);
                Read(filePath, listBox);
                listBox.SelectionChanged += ListBox_SelectionChanged;
                tabItem.IsSelected = true;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Clipboard.SetDataObject((sender as ListBox).SelectedItem.ToString());
        }

        public void Read(string path, ListBox listBox)
        {
            listBox.Items.Clear();
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    listBox.Items.Add(line);
                }
            }
        }
    }
}