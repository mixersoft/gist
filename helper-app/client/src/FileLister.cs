using System.Threading;
using System;
using System.IO;

namespace Snaphappi
{
	public class FileLister : IFileLister
	{
		#region data

		private readonly IFileSystem fileSystem;

		private readonly string[] photoExtensions;

		#endregion

		#region interface

		public FileLister(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;

			this.photoExtensions = new string[] {"jpg", "jpeg"};
		}

		#endregion

		#region IFileLister Members

		public void SearchFolder(string folderPath)
		{
			if (Directory.Exists(folderPath))
			{
				SearchFolder(folderPath, folderPath, 1);
				FolderSearchComplete(folderPath);
			}
			else
			{
				FolderNotFound(folderPath);
			}
		}

		public event Action<string, string> FileFound;

		public event Action<string> FolderNotFound;

		public event Action<string> FolderSearchComplete;

		#endregion

		private void SearchFolder(string rootFolder, string folderPath, int depth)
		{
			const int maxSearchDepth = 8;
			if (depth > maxSearchDepth)
				return;
			try
			{
				foreach (var filePath in fileSystem.ListFiles(folderPath))
				{
					if (IsImagePath(filePath))
						FileFound(rootFolder, filePath);
				}
				foreach (var subfolderPath in fileSystem.ListFolders(folderPath))
				{
					SearchFolder(rootFolder, subfolderPath, depth + 1);
				}
			}
			catch (UnauthorizedAccessException)
			{
				// we can safely ignore inaccessible folders
			}
		}

		private bool IsImagePath(string path)
		{
			var extension = Path.GetExtension(path).ToLowerInvariant();
			if (extension.Length <= 1)
				return false;
			extension = extension.Substring(1);
			return Array.Exists(photoExtensions, extension.Equals);
		}
	}
}
