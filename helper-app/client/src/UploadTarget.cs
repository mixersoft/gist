using System;

namespace Snaphappi
{
	public class UploadTarget
	{
		public readonly string FilePath;
		public readonly string FolderPath;
		public readonly int    Hash;

		public UploadTarget(string filePath, int hash, string folderPath)
		{
			FilePath   = filePath;
			FolderPath = folderPath;
			Hash       = hash;
		}
	}
}
