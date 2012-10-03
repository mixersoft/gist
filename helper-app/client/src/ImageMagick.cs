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
			Process.Start
				( new ProcessStartInfo()
					{
						FileName = Path.Combine(root, "convert.exe"),
						Arguments = string.Format("\"{0}\" {2} \"{1}\"", srcPath, dstPath, options),
						WindowStyle = ProcessWindowStyle.Hidden
					}
				).WaitForExit();
		}
	}
}
