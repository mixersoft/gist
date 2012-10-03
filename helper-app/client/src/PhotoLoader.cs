using System;
using System.IO;

namespace Snaphappi
{
	public class PhotoLoader : IPhotoLoader
	{
		#region IPhotoLoader Members

		public byte[] GetPreview(string path)
		{
			using (var temp = new TempFile())
			{
				ImageMagick.Convert(path, temp.Path, "-resize 480x640 -auto-orient -quality 80");
				return File.ReadAllBytes(temp.Path);
			}
		}

		#endregion
	}
}
