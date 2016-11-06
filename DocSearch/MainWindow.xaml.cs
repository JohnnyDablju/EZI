using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace DocSearch
{
    public partial class MainWindow : Window
    {
        private Controller controller;

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller();
        }

        private void TermsLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                TermsTextBox.Text = ofd.SafeFileName;
                controller.LoadTerms(ofd.FileName);
                TermsPreviewButton.IsEnabled = true;
            }
        }

        private void DocumentsLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                DocumentsTextBox.Text = ofd.SafeFileName;
                controller.LoadDocuments(ofd.FileName);
                DocumentsPreviewButton.IsEnabled = true;
            }
        }

        private void AnalyseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(TermsTextBox.Text) && !String.IsNullOrWhiteSpace(DocumentsTextBox.Text) && !String.IsNullOrWhiteSpace(QueryTextBox.Text))
            {
                controller.LoadQuery(QueryTextBox.Text);
                MainTextBox.Text = controller.GetDocumentsSimilarity();
            }
            else
            {
                MessageBox.Show("Please provide query, load documents and terms!");
            }
        }

        private void TermsPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.Text = controller.GetTermsPreview();
        }

        private void DocumentsPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.Text = controller.GetDocumentsPreview();
        }

        private void ExtensionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(QueryTextBox.Text))
            {
                MainTextBox.Text = controller.GetQueryExtensions(QueryTextBox.Text);
            }
            else
            {
                MessageBox.Show("Please provide query!");
            }
        }

        private void KmeansButton_Click(object sender, RoutedEventArgs e)
        {
            int seed, iterations;
            if (int.TryParse(SeedTextBox.Text, out seed) && int.TryParse(IterationsTextBox.Text, out iterations))
            {
                if (!String.IsNullOrWhiteSpace(TermsTextBox.Text) && !String.IsNullOrWhiteSpace(DocumentsTextBox.Text))
                {
                    MainTextBox.Text = controller.GetGroups(seed, iterations);
                }
                else
                {
                    MessageBox.Show("Please load documents and terms!");
                }
            }
            else
            {
                MessageBox.Show("Please provide valid seed and iterations values!");
            }
        }
    }
}
