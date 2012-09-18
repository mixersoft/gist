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

		private Thread workerThread;

		private bool stopRequested;

		private object mainLock = new object();

		#endregion

		#region interface

		public FileFinder(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;

			workerThread = new Thread(WorkerProc);
		}

		public void Wait()
		{
			workerThread.Join();
		}

		#endregion

		#region IFileFinder Members

		public void SetFiles(OriginalFileInfo[] files)
		{
			this.files = files;
		}

		public void Start()
		{
			workerThread.Start();
		}

		public void Stop()
		{
			lock (mainLock)
			{
				stopRequested = true;
			}
		}

		public event Action Finished;

		public event Action<OriginalFileInfo> FileFound;
		public event Action<OriginalFileInfo> FileNotFound;

		#endregion

		#region utility functions

		private void VerifyFolders()
		{
			throw new NotImplementedException();
		}

		private void WorkerProc()
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

			Finished();
		}

		#endregion
	}
}
