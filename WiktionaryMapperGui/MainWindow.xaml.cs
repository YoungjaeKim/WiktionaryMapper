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

namespace WiktionaryMapperGui
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

		private void WriteOutput(string text)
		{
			// TODO: cross-thread handling
			TextBoxOutput.Text += text;
		}

		private void ButtonBrowse_OnClick(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog()
			{
				Filter = "XML|*.xml",
				Multiselect = false
			};
			if (dialog.ShowDialog().GetValueOrDefault(false))
			{
				// TODO: execute WiktionaryMapper.exe
			}

		}
	}
}
