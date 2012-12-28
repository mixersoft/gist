using System;

namespace Snaphappi
{
	public class UploadTarget
	{
		public readonly string FilePath;
		public readonly string FolderPath;
		public readonly int    Hash;
		public readonly int    Timestamp;

		public UploadTarget(string filePath, int timestamp, int hash, string folderPath)
		{
			FilePath   = filePath;
			FolderPath = folderPath;
			Hash       = hash;
			Timestamp  = timestamp;
		}
	}
}
