using System;
using System.IO;

namespace Snaphappi
{
	public class FileFinder
	{
		private readonly IFileSystem  fileSystem;
		private readonly IPhotoLoader photoLoader;

		public FileFinder
			( IFileSystem  fileSystem
			, IPhotoLoader photoLoader
			)
		{
			this.fileSystem  = fileSystem;
			this.photoLoader = photoLoader;
		}

		public void Find(string filePath, int hash)
		{
			foreach (var file in fileSystem.ListFiles(Path.GetDirectoryName(filePath)))
			{
				if (hash == photoLoader.GetImageHash(file))
				{
					FileFound(new FileMatch(filePath, file));
					return;
				}
			}
		}

		public event Action<FileMatch> FileFound = delegate {};
	}
}