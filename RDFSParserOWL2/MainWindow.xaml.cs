using Microsoft.Win32;
using RDFSParserOWL2.Converter;
using RDFSParserOWL2.Generator;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Parser;
using RDFSParserOWL2.Parser.Handler;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace RDFSParserOWL2
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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
				txtFile.Text = openFileDialog.FileName;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
            ConverterRDFSToOWL2 converter = new ConverterRDFSToOWL2(txtFile.Text);
            converter.Convert();
			MessageBox.Show("Konvertovanje iz RDFS u OWL2 format uspesno izvrseno");
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			//OWL2Generator generator = new OWL2Generator();
			//generator.LoadPredefinedNamespaces();
			//generator.GenerateNameForFile("TopologyProfileRDFSAugmented-v2_4_15-7Aug2014");			
		}

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OWLRDFXMLHandler handler = new OWLRDFXMLHandler(); 
            using(FileStream fs =new FileStream(txtFile.Text,FileMode.Open)) 
            {
                bool su;
                TimeSpan ts;
                XMLParser.DoParse(handler,fs,txtFile.Text,out su,out ts);
				
            }

			OWL2Generator generator = new OWL2Generator(handler.Profile);
			generator.GenerateProfile();
            //ConverterRDFSToOWL2 converter = new ConverterRDFSToOWL2(txtFile.Text);
            //converter.Convert();
            MessageBox.Show("Konvertovanje iz RDFS u OWL2 format uspesno izvrseno");
        }
	}
}
