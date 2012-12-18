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
				ImageMagick.Convert(path, temp.Path, "-filter Lanczos -resize 640x640> -quality 80");
				return File.ReadAllBytes(temp.Path);
			}
		}

		public int GetHash(string path)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
