using System;

namespace Snaphappi
{
	public interface IPhotoLoader
	{
		byte[] GetPreview(string path);

		int GetImageHash(string path);
	}
}
