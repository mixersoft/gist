using System.Threading;
using System;
using System.IO;

namespace Snaphappi
{
	public class FileLister : IFileLister
	{
		#region data

		private readonly IFileSystem fileSystem;

		private Thread workerThread;

		private bool stopRequested;

		private bool multithread;

		private readonly object mainLock = new object();

		private readonly BlockingQueue<string> folders = new BlockingQueue<string>();

		#endregion

		#region interface

		public FileLister(IFileSystem fileSystem, bool multithread = true)
		{
			this.fileSystem  = fileSystem;
			this.multithread = multithread;

			if (multithread)
				workerThread = new Thread(WorkerProc);
		}

		#endregion

		#region IFileLister Members

		public void AddFolder(string folderPath)
		{
			if (multithread)
				folders.Enqueue(folderPath);
			else
				SearchFolder(folderPath);
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

		public event Action<string, string> FileFound;

		public event Action<string> FolderNotFound;

		#endregion

		#region utility functions

		private void WorkerProc()
		{
			foreach (var folderPath in folders)
				SearchFolder(folderPath);
		}

		private void SearchFolder(string folderPath)
		{
			try
			{
				foreach (var filePath in fileSystem.ListFiles(folderPath))
				{
					lock (mainLock)
					{
						if (stopRequested)
							return;
					}
					FileFound(folderPath, filePath);
				}
			}
			catch (DirectoryNotFoundException)
			{
				FolderNotFound(folderPath);
			}
		}

		#endregion
	}
}
