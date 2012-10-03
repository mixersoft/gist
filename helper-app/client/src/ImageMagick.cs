using System;
using System.IO;
using System.Diagnostics;

namespace Snaphappi
{
	public static class ImageMagick
	{
		private const string root = "ImageMagick";

		public static void Convert(string srcPath, string dstPath, string options)
		{
			var fileName  = Path.Combine(root, "convert.exe");
			var arguments = string.Format("\"{0}\" {2} \"{1}\"", srcPath, dstPath, options);
			Process.Start(fileName, arguments)
				.WaitForExit();
		}
	}
}
