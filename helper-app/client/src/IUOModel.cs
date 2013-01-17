using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaphappi
{
	public interface IUOModel
	{

		event Action<FileMatch>    FileFound;
		event Action<UploadTarget> FileNotFoundByHash;
		event Action<UploadTarget> FileNotFoundByName;
		event Action<UploadTarget> TargetAdded;
		event Action               TaskCancelled;
		event Action<UploadTarget> UploadFailed;

		void FetchFiles();

		void FindFileByHash(UploadTarget target, int hash);

		void FindFileByName(UploadTarget target);

		int GetImageHash(int imageID);

		void StartPolling();

		void Stop();

		void UploadFile(FileMatch match);
	}
}
