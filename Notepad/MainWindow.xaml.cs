using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Notepad
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        TabItem currentTab;
        IEnumerable<TabItem> allTabs;
        TextBox editableView;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Open_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectFileAndLoad();
        }

        private void ReloadThis_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.Items.IsEmpty) { return; }
            currentTab = GetCurrentTab();
            ReloadExistedFileToListBox(currentTab.ToolTip.ToString());
        }

        private void ReloadAll_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.Items.IsEmpty) { return; }
            ReloadAllFiles();
        }

        private void ClearThis_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.Items.IsEmpty) { return; }
            currentTab = GetCurrentTab();
            TabControl.Items.Remove(currentTab);
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.Items.IsEmpty) { return; }
            TabControl.Items.Clear();
            Btn_Edit.IsEnabled = false;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.Items.IsEmpty) { return; }
            currentTab = GetCurrentTab();

            allTabs = GetAllTabs();
            foreach (TabItem t in allTabs)
            {
                t.IsEnabled = false;
            }

            LoadFileToEditableView(currentTab.ToolTip.ToString());
            TabControl.Items.Remove(currentTab);
            Btn_Edit.IsEnabled = false;
            Btn_Save.IsEnabled = true;
            Btn_Open.IsEnabled = false;
            Btn_Reload_All.IsEnabled = false;
            Btn_Clear_All.IsEnabled = false;
            Btn_Reload_This.IsEnabled = false;
            Btn_Remove_This.IsEnabled = false;

            Btn_Cancel.IsEnabled = true;

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFiles();
            Btn_Edit.IsEnabled = true;
            Btn_Save.IsEnabled = false;
            Btn_Open.IsEnabled = true;
            Btn_Reload_All.IsEnabled = true;
            Btn_Clear_All.IsEnabled = true;
            Btn_Reload_This.IsEnabled = true;
            Btn_Remove_This.IsEnabled = true;
            Btn_Cancel.IsEnabled = false;

            allTabs = GetAllTabs();
            foreach (TabItem t in allTabs)
            {
                t.IsEnabled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            currentTab = GetCurrentTab();
            ReloadExistedFileToListBox(currentTab.ToolTip.ToString());

            Btn_Edit.IsEnabled = true;
            Btn_Save.IsEnabled = false;
            Btn_Open.IsEnabled = true;
            Btn_Reload_All.IsEnabled = true;
            Btn_Clear_All.IsEnabled = true;
            Btn_Reload_This.IsEnabled = true;
            Btn_Remove_This.IsEnabled = true;
            Btn_Cancel.IsEnabled = false;

            allTabs = GetAllTabs();
            foreach (TabItem t in allTabs)
            {
                t.IsEnabled = true;
            }
        }

        private void SelectFileAndLoad()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "choose a txt";
            fileDialog.Filter = "all files(*xls*)|*.txt*";
            if (fileDialog.ShowDialog() != false)
            {
                StartLoadFiles(fileDialog.FileName);
                Btn_Edit.IsEnabled = true;
            }
        }

        private void StartLoadFiles(string filePath)
        {
            allTabs = GetAllTabs();
            if (allTabs.Any(t => t.ToolTip.ToString().Equals(filePath)))
            {
                ReloadExistedFileToListBox(filePath);
            }
            else
            {
                LoadFileToListBox(filePath);
            }
        }

        private void ReloadExistedFileToListBox(string filePath)
        {
            var existedTb = allTabs.Single(t => t.ToolTip.ToString().Equals(filePath));
            TabControl.Items.Remove(existedTb);
            LoadFileToListBox(filePath);
        }

        private void LoadFileToListBox(string filePath)
        {
            var tabItem = new TabItem() { Header = Path.GetFileName(filePath)};
            var listBox = new ListBox();
            tabItem.Content = listBox;
            TabControl.Items.Add(tabItem);
            Read(filePath, listBox);
            listBox.SelectionChanged += ListBox_SelectionChanged;
            tabItem.IsSelected = true;
            tabItem.ToolTip = filePath;
        }

        private void LoadFileToEditableView(string filePath)
        {
            var tabItem = new TabItem() { Header = Path.GetFileName(filePath) + "*" };
            editableView = new TextBox() { AcceptsTab=true,AcceptsReturn=true };
            tabItem.Content = editableView;
            TabControl.Items.Add(tabItem);
            Read(filePath, editableView);
            tabItem.IsSelected = true;
            tabItem.ToolTip = filePath;
        }

        private void ReloadAllFiles()
        {
            allTabs = GetAllTabs();
            List<string> tabList = allTabs.Select(t => t.ToolTip.ToString()).ToList();
            TabControl.Items.Clear();
            foreach (string tab in tabList)
            {
                LoadFileToListBox(tab);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Clipboard.SetDataObject((sender as ListBox).SelectedItem.ToString());
        }

        public void Read(string path, ListBox listBox)
        {
            listBox.Items.Clear();
            try
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        listBox.Items.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void Read(string path, TextBox textBox)
        {
            textBox.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 20);
            try
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    textBox.Text = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        private TabItem GetCurrentTab()
        {
            return TabControl.Items.Cast<TabItem>().Single(t => t.IsSelected);
        }

        private IEnumerable<TabItem> GetAllTabs()
        {
            return TabControl.Items.Cast<TabItem>();
        }

        private void SaveFiles()
        {
            string filePath = currentTab.ToolTip.ToString();
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(filePath);
                sw.Write(editableView.Text);
                //Close the file
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

            ReloadExistedFileToListBox(filePath);
        }

    }
}