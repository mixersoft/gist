using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public class PhotoLoader : IPhotoLoader
	{
		private const int hashBpp    = 2;
		private const int hashWidth  = 8;
		private const int hashHeight = 8;

		#region IPhotoLoader Members

		public byte[] GetPreview(string path)
		{
			using (var temp = new TempFile())
			{
				ImageMagick.Convert(path, temp.Path, "-filter Lanczos -resize 640x640> -quality 80");
				return File.ReadAllBytes(temp.Path);
			}
		}

		public int GetImageHash(string path)
		{
			using (var temp = new TempFile())
			{
				ImageMagick.Convert
					( path
					, temp.Path
					, "-filter Lanczos -resize 640x640> -quality 80"
					);
				ImageMagick.Convert
					( temp.Path
					, temp.Path
					, string.Format
						( "-filter Lanczos -resize {0}x{1}! -format PNG"
						, hashWidth
						, hashHeight
						)
					);
				return GetFirstInt(ComputeHash(GetBitmapBits(temp.Path)));
			}
		}

		public string GetImageDateTime(string path)
		{
			try
			{
				using (var bmp = new Bitmap(path))
				{
					const int dateTimeId = 0x0132;
					var item = bmp.PropertyItems.FirstOrDefault(p => p.Id == dateTimeId);
					if (item == null)
						return "";
					return Encoding.ASCII.GetString(item.Value).TrimEnd(new char[] { '\0' });
				}
			}
			catch (ArgumentException)
			{
				return "";
			}
		}

		#endregion // IPhotoLoader Members

		#region implementation

		/// <summary>
		/// Retrieve the first bpp most significant bits of every pixel
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		byte[] GetBitmapBits(string path)
		{
			using (var bmp = new Bitmap(path))
			{
				const int bpp = hashBpp;
				const int w   = hashWidth;
				const int h   = hashHeight;

				var size = bmp.Size;

				Trace.Assert(w == bmp.Width);
				Trace.Assert(h == bmp.Height);

				var bits = new BitArray(bpp * w * h);

				int i = 0;

				for (int y = 0; y != h; ++y)
				for (int x = 0; x != w; ++x)
				{
					var color = bmp.GetPixel(x, y);

					// get the grayscale value
					int value = (color.R + color.G + color.B) / 3;

					// get the most significant bits
					value >>= 8 - bpp;
					for (int bit = 0; bit != bpp; ++bit)
					{
						bits[i] = (value & (1 << bit)) != 0;
						++i;
					}
				}

				var bytes = new byte[(bits.Count + 7) / 8]; // divide rounding up
				bits.CopyTo(bytes, 0);
				return bytes;
			}
		}

		byte[] ComputeHash(byte[] data)
		{
			using (var md5 = System.Security.Cryptography.MD5.Create())
				return md5.ComputeHash(data);
		}

		int GetFirstInt(byte[] hash)
		{
			return hash[0] | (hash[1] << 8) | (hash[2] << 16) | (hash[3] << 24);
		}

		#endregion // implementation
	}
}
