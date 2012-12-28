using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOModel
	{
		event Action TaskCancelled;

		event Action<string, string> FileFound;

		event Action<UploadTarget> TargetAdded;

		event Action<string, string> FileNotFound;
		event Action<string, string> UploadFailed;

		void FetchFiles();

		void FindFile(string path, int hash);

		void StartPolling();

		void Stop();

		void UploadFile(string folderPath, string filePath);
	}
}
