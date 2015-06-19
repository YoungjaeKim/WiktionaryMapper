using System.IO;
using System.Text;
using System.Windows.Controls;

namespace WiktionaryMapperGui
{
	/// <summary>
	/// Source: <![CDATA[https://saezndaree.wordpress.com/2009/03/29/how-to-redirect-the-consoles-output-to-a-textbox-in-c/]]>
	/// </summary>
	public class TextBoxStreamWriter : TextWriter
	{
		readonly TextBox _output = null;

		public TextBoxStreamWriter(TextBox output)
		{
			_output = output;
		}

		public override void Write(char value)
		{
			base.Write(value);
			_output.AppendText(value.ToString()); // When character data is written, append it to the text box.
		}

		public override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}
	}
}
