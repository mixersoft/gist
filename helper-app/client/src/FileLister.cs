using System.Threading;
using System;
using System.IO;

namespace Snaphappi
{
	public class FileLister : IFileLister
	{
		#region data

		private string[] folders;

		private readonly IFileSystem fileSystem;

		private Thread workerThread;

		private bool stopRequested;

		private object mainLock = new object();

		#endregion

		#region interface

		public FileLister(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;

			workerThread = new Thread(WorkerProc);
		}

		public void Wait()
		{
			workerThread.Join();
		}

		#endregion

		#region IFileLister Members

		public void UpdateFolders(string[] paths)
		{
			this.folders = paths;
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

		public event Action<string> FileFound;

		public event Action<string> FolderNotFound;

		public event Action Finished;

		#endregion

		#region utility functions

		private void WorkerProc()
		{
			foreach (var folder in folders)
			{
				try
				{
					foreach (var file in fileSystem.ListFiles(folder))
					{
						lock (mainLock)
						{
							if (stopRequested)
								return;
						}
						FileFound(file);
					}
				}
				catch (DirectoryNotFoundException)
				{
					FolderNotFound(folder);
				}
			}
			Finished();
		}

		#endregion
	}
}
