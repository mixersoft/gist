using System;
using System.IO;
using System.Diagnostics;

namespace Snaphappi
{
	public static class ImageMagick
	{
		private static readonly string root;

		static ImageMagick()
		{
			var appDirectory = Path.GetDirectoryName(typeof(ImageMagick).Assembly.Location);
			root = Path.Combine(appDirectory, "ImageMagick");
		}

		public static void Convert(string srcPath, string dstPath, string options)
		{
			if (!File.Exists(srcPath))
				throw new FileNotFoundException("Source image does not exist.", srcPath);
			Process.Start
				( new ProcessStartInfo()
					{
						FileName    = Path.Combine(root, "convert.exe"),
						Arguments   = string.Format("\"{0}\" {2} \"{1}\"", srcPath, dstPath, options),
						WindowStyle = ProcessWindowStyle.Hidden
					}
				).WaitForExit();
		}
	}
}
