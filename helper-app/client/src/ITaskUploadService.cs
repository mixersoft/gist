﻿using System;

namespace Snaphappi
{
	public interface ITaskUploadService
	{
		void ScheduleAction(Action action);

		void Stop();

		void UploadFile
			( string       folderPath
			, string       pathPath
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			);

		void UploadFile
			( string       folderPath
			, string       oldFilePath
			, string       newFilePath
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			);

		event Action                 AuthTokenRejected;
		event Action<string, string> DuplicateUpload;
		event Action<string, string> FileNotFound;
		event Action<string, string> UploadFailed;
	}
}