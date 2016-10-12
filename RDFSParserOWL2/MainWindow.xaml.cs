using Microsoft.Win32;
using RDFSParserOWL2.Converter;
using RDFSParserOWL2.Generator;
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



		public ObservableCollection<string> FilesForParsing
		{
			get { return filesForParsing; }
			set { filesForParsing = value; }
		}

		public MainWindow()
		{
			filesForParsing = new ObservableCollection<string>();
			InitializeComponent();
			Init();			
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() == true) 
			{
				foreach(string file in openFileDialog.FileNames) 
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
			ConvertFiles(filesForParsing);
			sw.Stop();
			string message = String.Format("Konvertovanje iz RDFS u OWL2 format uspesno izvrseno.Vreme izvrsavanja je:{0}",sw.Elapsed);
			MessageBox.Show(message);
		}


		private void ConvertFiles(ObservableCollection<string> fileNames) 
		{
			if (fileNames != null && fileNames.Count>0 )
			{
				
				//foreach(string fileName in fileNames) 
				//{
					ConverterRDFSToOWL2 converter = new ConverterRDFSToOWL2(fileNames.ToList(), new GeneratorSettings((bool)cbRoofOntology.IsChecked, (bool)cbOntology.IsChecked, txtOntology.Text.Trim(), txtRoofOntology.Text.Trim(), txtExtOnt.Text.Trim(), txtRoofOntNS.Text.Trim(),(bool)rbEnumOpened.IsChecked,txtNS.Text.Trim()));
					converter.Convert();
				//}
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

			foreach(string s in listSelected) 
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
			ConvertFiles(SelectedItemsToList());
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
			txtRoofOntology.IsEnabled = false;
			txtExtOnt.IsEnabled = false;
			txtRoofOntNS.IsEnabled = false;
		}
	}
}
