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

		public void FindByName(UploadTarget target)
		{
			if (MatchExists(target.FilePath, target.ExifDateTime))
				FileFound(new FileMatch(target, target.FilePath));
			else
				FileNotFound(target, SearchType.Name);
		}

		public void FindByHash(UploadTarget target, int hash)
		{
			var newPath = FindFirst(target.FilePath, target.ExifDateTime, hash);
			if (newPath != null)
				FileFound(new FileMatch(target, newPath));
			else
				FileNotFound(target, SearchType.Hash);
		}

		public event Action<FileMatch>                FileFound    = delegate {};
		public event Action<UploadTarget, SearchType> FileNotFound = delegate{};

		#endregion // interface

		#region implementation

		private bool MatchExists(string filePath, int exifDateTime)
		{
			if  (!fileSystem.FileExists(filePath))
				return false;
			DateTime time;
			if (!DateTimeEx.TryParseExifTime(photoLoader.GetImageDateTime(filePath), out time))
				return false;
			return exifDateTime == time.ToUnixTime();
		}

		private string FindFirst(string filePath, int exifDateTime, int hash)
		{
			foreach (var file in fileSystem.ListFiles(Path.GetDirectoryName(filePath)))
			{
				if (TimestampMatches(exifDateTime, file) && HashMatches(hash, file))
					return file;
			}
			return null;
		}

		private bool TimestampMatches(int exifDateTime, string path)
		{
			DateTime time;
			if (DateTimeEx.TryParseExifTime(photoLoader.GetImageDateTime(path), out time))
				return exifDateTime == time.ToUnixTime();
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