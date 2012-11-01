using System;

namespace Snaphappi
{
	public interface IURTaskUploadService
	{
		void UploadFile(string folder, string path, Func<byte[]> LoadFile);

		void ScheduleAction(Action action);

		event Action<string, string> DuplicateUpload;

		event Action<string, string> UploadFailed;
	}
}