using System;

namespace Snaphappi
{
	public class FileMatch
	{
		public readonly UploadTarget Target;
		public readonly string       NewPath;

		public FileMatch(UploadTarget target, string newPath)
		{
			Target  = target;
			NewPath = newPath;
		}
	}

	public interface IAsyncFileFinder
	{
		void FindByName(UploadTarget target);

		void FindByHash(UploadTarget target, int hash);

		void Stop();

		event Action<FileMatch>                FileFound;
		event Action<UploadTarget, SearchType> FileNotFound;
	}
}
