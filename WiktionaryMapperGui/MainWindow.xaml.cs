using System;
using System.Collections.Generic;
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
using Microsoft.Win32;

namespace WiktionaryMapperGui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		TextWriter _writer = null;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void WriteOutput(string text)
		{
			TextBoxOutput.Dispatcher.Invoke(new Action(() => TextBoxOutput.Text += text));
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
				WriteOutput("Wiktionary Process Executed");
				ExecCommand(@"WiktionaryMapper.exe", dialog.FileName);
			}
		}


		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

			//// Instantiate the writer
			//_writer = new TextBoxStreamWriter(txtConsole);
			//// Redirect the out Console stream
			//Console.SetOut(_writer);

		}

		/// <summary>
		/// Source: <![CDATA[http://stackoverflow.com/a/29535211/361100]]>
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private static string ExecCommand(string filename, string arguments)
		{
			Process process = new Process();
			ProcessStartInfo psi = new ProcessStartInfo(filename)
			{
				Arguments = arguments,
				CreateNoWindow = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			};
			process.StartInfo = psi;

			StringBuilder output = new StringBuilder();
			process.OutputDataReceived += (sender, e) => { output.AppendLine(e.Data); };
			process.ErrorDataReceived += (sender, e) => { output.AppendLine(e.Data); };
			process.Exited += (sender, args) => { output.AppendLine("Exited with Code " + args.ToString()); };

			// run the process
			process.Start();

			// start reading output to events
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			// wait for process to exit
			process.StandardInput.AutoFlush = true;
			process.StandardInput.Write(" ");
			process.WaitForExit();

			if (process.ExitCode != 0)
				throw new Exception("Command " + psi.FileName + " returned exit code " + process.ExitCode);

			return output.ToString();
		}
	}
}
