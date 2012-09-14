using System.IO;
using System.Threading;
using System;

namespace Snaphappi
{
	public class FileFinder : IFileFinder
	{
		#region data

		private readonly IFileSystem fileSystem;

		private OriginalFileInfo[] files;

		private Thread downloadThread;
		private Thread uploadThread;

		private bool stopRequested;

		private object mainLock = new object();

		#endregion

		#region interface

		public FileFinder(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;

			downloadThread = new Thread(DownloadProc);
			uploadThread   = new Thread(UploadProc);
		}

		public void Wait()
		{
			downloadThread.Join();
			uploadThread.Join();
		}

		#endregion

		#region IOriginalFileManager Members

		public OriginalFileInfo[] FileInfo
		{
			set { files = value; }
		}

		public void Start()
		{
			downloadThread.Start();
			uploadThread.Start();
		}

		public void Stop()
		{
			lock (mainLock)
			{
				stopRequested = true;
			}
		}

		public event Action Done;

		public event Action<OriginalFileInfo> FileFound;
		public event Action<OriginalFileInfo> FileNotFound;

		#endregion

		#region utility functions

		private void VerifyFolders()
		{
			throw new NotImplementedException();
		}

		private void DownloadProc()
		{
			OriginalFileInfo[] files;
			lock (mainLock)
				files = this.files;

			foreach (var file in files)
			{
				lock (mainLock)
				{
					if (stopRequested)
						return;
				}

				var path = Path.Combine(file.directory, file.relativePath);
				if (fileSystem.FileExists(path))
					FileFound(file);
				else
					FileNotFound(file);
			}

			Done();
		}

		private void UploadProc()
		{
		}

		#endregion
	}
}
