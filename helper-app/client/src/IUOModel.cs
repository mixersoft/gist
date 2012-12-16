using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOModel
	{
		event Action TaskCancelled;

		event Action<string, string> FileAdded;
		event Action<string, string> FileNotFound;
		event Action<string, string> UploadFailed;

		void UploadFile(string folderPath, string filePath);

		void StartPolling();

		void FetchFiles();
	}
}
