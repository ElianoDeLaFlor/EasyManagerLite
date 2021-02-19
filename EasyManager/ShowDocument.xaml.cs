using EasyManagerLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for ShowDocument.xaml
    /// </summary>
    public partial class ShowDocument : Window
    {
        public string DocPath { get; set; }
        public string Path { get; set; } = string.Empty;
        public ShowDocument()
        {
            InitializeComponent();
            SizeChanged += ShowDocument_SizeChanged;
        }

        private void ShowDocument_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowDoc.Height = ActualHeight-150;
        }

        public ShowDocument(string path)
        {
            InitializeComponent();
            DocPath = path;
            SizeChanged += ShowDocument_SizeChanged;
        }

        private async Task LoadFile()
        {
            await Task.Run(() => Navig());
        }
        private void Navig()
        {
            ShowDoc.Dispatcher.Invoke(new Action(()=>ShowDoc.Navigate(DocPath)));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Navig();

        }

        private void Opendir_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = Properties.Resources.OpenFile;
            openFileDialog.Filter = "PDF (*.pdf)|*.pdf";
            //openFileDialog.InitialDirectory = InfoChecker.SaveDir(Properties.Resources.);

            if (openFileDialog.ShowDialog() == true)
            {
                // Todo print page
                DocPath = openFileDialog.FileName;
                ShowDoc.Navigate(DocPath);
            }
        }


        private void Printdoc_Click(object sender, RoutedEventArgs e)
        {
            mshtml.IHTMLDocument2 doc = ShowDoc.Document as mshtml.IHTMLDocument2;
            doc.execCommand("Print", true, null);
        }

    }
}
