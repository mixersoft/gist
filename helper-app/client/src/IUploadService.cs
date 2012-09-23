using System;

public interface IURTaskUploadService
{
	void UploadFile(string path, Func<byte[]> LoadFile);

	event Action<string> UploadFailed;
}