using System;
using System.Threading;

namespace Snaphappi
{
	public class AsyncFileFinder : IAsyncFileFinder
	{
		struct QueueItem
		{
			public readonly string Path;
			public readonly int    Hash;

			public QueueItem(string path, int hash)
			{
				Path = path;
				Hash = hash;
			}
		}

		#region data

		private readonly FileFinder fileFinder;

		private readonly BlockingQueue<QueueItem> queue = new BlockingQueue<QueueItem>();

		private bool stopRequested = false;

		#endregion // data

		#region interface

		public AsyncFileFinder
			( IFileSystem  fileSystem
			, IPhotoLoader photoLoader
			)
		{
			fileFinder = new FileFinder(fileSystem, photoLoader);
			new Thread(FindFiles).Start();
		}

		#endregion // interface

		#region IAsyncFileFinder Members

		public void Find(string filePath, int hash)
		{
			queue.Enqueue(new QueueItem(filePath, hash));
		}

		public void Stop()
		{
			lock (fileFinder)
				stopRequested = true;
		}

		public event Action<FileMatch> FileFound
		{
			add    { lock (fileFinder) fileFinder.FileFound += value; }
			remove { lock (fileFinder) fileFinder.FileFound -= value; }
		}

		#endregion

		#region implementation

		private void FindFiles()
		{
			foreach (var item in queue)
			{
				lock (fileFinder)
				{
					if (stopRequested)
						break;
					fileFinder.Find(item.Path, item.Hash);
				}
			}
		}

		#endregion
	}
}
