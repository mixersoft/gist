using System;

public interface IURUploadService
{
	void UploadFile(string path, string SessionID, string TaskID);

	event Action<string> UploadFailed;
}