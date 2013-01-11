using System;
using System.IO;

namespace Snaphappi
{
	public class FileFinder
	{
		#region data

		private readonly IFileSystem  fileSystem;
		private readonly IPhotoLoader photoLoader;

		#endregion // data

		#region interface

		public FileFinder
			( IFileSystem  fileSystem
			, IPhotoLoader photoLoader
			)
		{
			this.fileSystem  = fileSystem;
			this.photoLoader = photoLoader;
		}

		public void Find(string filePath, int timestamp, int hash)
		{
			var matchPath = FindFirst(filePath, timestamp, hash);
			if (matchPath != null)
				FileFound(new FileMatch(filePath, matchPath));
			else
				FileNotFound(filePath);
		}

		public event Action<FileMatch> FileFound    = delegate {};
		public event Action<string>    FileNotFound = delegate{};

		#endregion // interface

		#region implementation

		private string FindFirst(string filePath, int timestamp, int hash)
		{
			if (fileSystem.FileExists(filePath))
				return filePath;
			foreach (var file in fileSystem.ListFiles(Path.GetDirectoryName(filePath)))
			{
				if (TimestampMatches(timestamp, file) && HashMatches(hash, file))
					return file;
			}
			return null;
		}

		private bool TimestampMatches(int timestamp, string path)
		{
			DateTime time;
			if (DateTime.TryParse(photoLoader.GetImageDateTime(path), out time))
				return timestamp == time.ToUnixTime();
			else
				return false;
		}

		private bool HashMatches(int hash, string path)
		{
			return hash == photoLoader.GetImageHash(path);
		}

		#endregion // implementation
	}
}