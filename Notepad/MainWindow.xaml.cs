using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Notepad
{
    public partial class MainWindow
    {
        private string filePath;
        List<OpenFileDialog> fileList;

        public MainWindow()
        {
            InitializeComponent();
            fileList = new List<OpenFileDialog>();
        }

        private void Open_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectFileAndLoad();
        }

        private void SelectFileAndLoad()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "choose a txt";
            fileDialog.Filter = "all files(*xls*)|*.txt*";
            if (fileDialog.ShowDialog() != false)
            {
                StartLoadFiles(fileDialog);
            }
        }

        private void StartLoadFiles(OpenFileDialog fileDialog)
        {
            filePath = fileDialog.FileName;
            LoadFileToListBox(fileDialog);
            //tabsDict.Add(fileDialog.SafeFileName, filePath);

            if (fileList.Any(t => (t as OpenFileDialog).FileName.Equals(filePath)))
            {
                ReloadAllFiles();
            }
            else
            {
                fileList.Add(fileDialog);
            }
        }

        private void LoadFileToListBox(OpenFileDialog fileDialog)
        {
            var tabItem = new TabItem() { Header = fileDialog.SafeFileName };
            var listBox = new ListBox();
            tabItem.Content = listBox;
            TabControl.Items.Add(tabItem);
            Read(filePath, listBox);
            listBox.SelectionChanged += ListBox_SelectionChanged;
            tabItem.IsSelected = true;
            tabItem.ToolTip = filePath;
        }

        private void LoadFileToEdiatbleView(OpenFileDialog fileDialog)
        {
            var tabItem = new TabItem() { Header = fileDialog.SafeFileName+"*" };
            var textBox = new TextBox();
            tabItem.Content = textBox;
            TabControl.Items.Add(tabItem);
            Read(filePath, textBox);
            tabItem.IsSelected = true;
            tabItem.ToolTip = filePath;
        }

        private void ReloadAllFiles()
        {
            TabControl.Items.Clear();
            foreach (OpenFileDialog file in fileList)
            {
                LoadFileToListBox(file);
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
        public void Read(string path, TextBox textBox)
        {
            textBox.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 20);
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    textBox.Text += line;
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            TabControl.Items.Clear();
            if (fileList != null)
            {
                foreach (OpenFileDialog file in fileList)
                {
                    LoadFileToEdiatbleView(file);
                }
                Btn_Edit.Visibility = Visibility.Collapsed;
                Btn_Save.Visibility = Visibility.Visible;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            TabControl.Items.Clear();
            fileList = new List<OpenFileDialog>();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadAllFiles();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFiles();
            Btn_Edit.Visibility = Visibility.Visible;
            Btn_Save.Visibility = Visibility.Collapsed;
            ReloadAllFiles();
        }

        private void SaveFiles()
        {
        }
    }
}