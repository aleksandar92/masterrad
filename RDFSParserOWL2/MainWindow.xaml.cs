using Microsoft.Win32;
using RDFSParserOWL2.Common;
using RDFSParserOWL2.Converter;
using RDFSParserOWL2.Generator;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Model;
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Parser;
using RDFSParserOWL2.Parser.Handler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

		private ObservableCollection<string> filesForParsing;
		private List<Namespace> predefinedNamespace;



		public ObservableCollection<string> FilesForParsing
		{
			get { return filesForParsing; }
			set { filesForParsing = value; }
		}

		public MainWindow()
		{
			filesForParsing = new ObservableCollection<string>();
			predefinedNamespace = InputOutput.LoadPredefinedNamespaces();
			InitializeComponent();
			Init();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() == true)
			{
				foreach (string file in openFileDialog.FileNames)
				{
					if (!filesForParsing.Contains(file.Trim()))
					{
						filesForParsing.Add(file.Trim());
					}
				}
				//txtFile.Text = openFileDialog.FileName;
			}
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{

			Stopwatch sw = new Stopwatch();
			sw.Start();
			string report;
			
			GeneratorSettings ge = new GeneratorSettings((bool)cbRoofOntology.IsChecked, (bool)cbOntology.IsChecked, txtOntology.Text.Trim(), txtRoofOntology.Text.Trim(), txtExtOnt.Text.Trim(), txtRoofOntNS.Text.Trim(), (bool)rbEnumOpened.IsChecked, txtNS.Text.Trim(),txtMetaURI.Text.Trim());
			if (ge.CheckValidity(out report))
			{
				ConvertFiles(filesForParsing, ge);
			}
			else 
			{
				sw.Stop();
				MessageBox.Show(report);
				return;
			}
			
			sw.Stop();
			string message = String.Format("Konvertovanje iz RDFS u OWL2 format uspesno izvrseno.Vreme izvrsavanja je:{0}", sw.Elapsed);
			MessageBox.Show(message);
		}


		private void ConvertFiles(ObservableCollection<string> fileNames, GeneratorSettings ge)
		{

			//bool result = true;
			if (fileNames != null && fileNames.Count > 0)
			{
					ConverterRDFSToOWL2 converter = new ConverterRDFSToOWL2(fileNames.ToList(), ge);
					converter.Convert();
			}


		}



		private void cbOntology_Checked(object sender, RoutedEventArgs e)
		{
			if (cbOntology.IsChecked == true)
			{
				txtOntology.IsEnabled = true;
				txtExtOnt.IsEnabled = true;
			}
			else
			{
				txtOntology.IsEnabled = false;
				txtExtOnt.IsEnabled = false;
			}
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (cbRoofOntology.IsChecked == true)
			{
				txtRoofOntology.IsEnabled = true;
				txtRoofOntNS.IsEnabled = true;
				//	txtExtOnt.IsEnabled = true;

			}
			else
			{
				txtRoofOntology.IsEnabled = false;
				txtRoofOntNS.IsEnabled = false;
				//txtExtOnt.IsEnabled = false;
			}
		}


		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			//ObservableCollection<string> itemsToRemove = new ObservableCollection<string>();

			ObservableCollection<string> listSelected = SelectedItemsToList();

			foreach (string s in listSelected)
			{
				filesForParsing.Remove(s.Trim());
			}

			//filesForParsing = new ObservableCollection<string>((filesForParsing.ToList().Except(listSelected)));
			//filesForParsing.ToList();


			//filesForParsing =(ObservableCollection<string>) filesForParsing.Except(itemsToRemove);
			//filesForParsing.Except(); 
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			filesForParsing.Clear();
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			string report;
			GeneratorSettings ge = new GeneratorSettings((bool)cbRoofOntology.IsChecked, (bool)cbOntology.IsChecked, txtOntology.Text.Trim(), txtRoofOntology.Text.Trim(), txtExtOnt.Text.Trim(), txtRoofOntNS.Text.Trim(), (bool)rbEnumOpened.IsChecked, txtNS.Text.Trim(),txtMetaURI.Text.Trim());
			if (ge.CheckValidity(out report))
			{
				ConvertFiles(SelectedItemsToList(), ge);
			}
			else 
			{
				sw.Stop();
				MessageBox.Show(report);
				return;
			}
			
			sw.Stop();
			string message = String.Format("Konvertovanje iz RDFS u OWL2 format uspesno izvrseno.Vreme izvrsavanja je:{0} ", sw.Elapsed);
			MessageBox.Show(message);
		}

		private ObservableCollection<string> SelectedItemsToList()
		{
			ObservableCollection<string> result = new ObservableCollection<string>();
			foreach (string s in lbFilesParsing.SelectedItems)
			{
				result.Add(s);
			}

			return result;
		}


		private void Init()
		{
			lbFilesParsing.ItemsSource = filesForParsing;
			lbFilesParsing.SelectionMode = SelectionMode.Multiple;

			txtOntology.IsEnabled = false;
			txtNS.Text = BaseURI("base");
			//txtOntology.Text = BaseURI("base");   
			txtRoofOntology.IsEnabled = false;
			txtExtOnt.Text = BaseURI("base");
			txtExtOnt.IsEnabled = false;
			txtRoofOntNS.Text = BaseURI("base");
			txtRoofOntNS.IsEnabled = false;
			txtMetaURI.Text = BaseURI("meta");
		}

		private string BaseURI(string pfx)
		{
			Namespace n = predefinedNamespace.Where(x => !string.IsNullOrEmpty(x.Prefix) && x.Prefix.Equals(pfx)).SingleOrDefault();
			return n != null ? n.Value : "";
		}
	}
}
