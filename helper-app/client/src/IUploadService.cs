using System;

public interface IURUploadService
{
	void UploadFile(string path, Func<byte[]> LoadFile);

	event Action<string> UploadFailed;
}