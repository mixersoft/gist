using System;
using System.Windows.Forms;

namespace Snaphappi
{
	class SnaphappiHelper
	{
		public static void Main(string[] args)
		{
			var form = new Form();
			form.Text = args.Length > 0 ? args[0] : "<no args>";
			Application.Run(form);
		}
	}
}
