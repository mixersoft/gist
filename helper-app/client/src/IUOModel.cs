using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOModel
	{

		event Action<UploadTarget> FileFound;
		event Action<UploadTarget> FileNotFound;
		event Action<UploadTarget> TargetAdded;
		event Action               TaskCancelled;
		event Action<UploadTarget> UploadFailed;

		void FetchFiles();

		void FindFile(UploadTarget target);

		void StartPolling();

		void Stop();

		void UploadFile(string folderPath, string filePath);
	}
}
