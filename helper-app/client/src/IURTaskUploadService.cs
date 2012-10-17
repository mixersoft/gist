using System;

namespace Snaphappi
{
	public interface IURTaskUploadService
	{
		void UploadFile(string folder, string path, Func<byte[]> LoadFile);

		event Action<string, string> UploadFailed;
	}
}