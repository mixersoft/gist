using System;

namespace Snaphappi
{
	public interface IPhotoLoader
	{
		byte[] GetPreview(string path);

		int GetHash(string path);
	}
}
