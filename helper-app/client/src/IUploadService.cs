using System;

public interface IURUploadService
{
	void UploadFile(string path);

	event Action<string> UploadFailed;
}