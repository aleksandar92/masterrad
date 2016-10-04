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
			txtOntology.IsEnabled = false;
			txtRoofOntology.IsEnabled = false;
			
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
				txtFile.Text = openFileDialog.FileName;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
            ConverterRDFSToOWL2 converter = new ConverterRDFSToOWL2(txtFile.Text,(bool)cbOntology.IsChecked,txtOntology.Text.Trim(),(bool)cbRoofOntology.IsChecked,txtRoofOntology.Text.Trim() );
            converter.Convert();
			MessageBox.Show("Konvertovanje iz RDFS u OWL2 format uspesno izvrseno");
		}



		private void cbOntology_Checked(object sender, RoutedEventArgs e)
		{
			if (cbOntology.IsChecked == true)
			{
				txtOntology.IsEnabled = true;
			}
			else 
			{
				txtOntology.IsEnabled = false;
			}
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (cbRoofOntology.IsChecked == true)
			{
				txtRoofOntology.IsEnabled = true;
			}
			else
			{
				txtRoofOntology.IsEnabled = false;
			}
		}
	}
}
