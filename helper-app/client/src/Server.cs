using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Snaphappi
{
	public class Server : IApp, ITaskControlService, ITaskInfoService, ITaskUploadService
	{
		#region data

		private readonly Thread inputThread;

		private IFileSystem  fileSystem;
		private IPhotoLoader photoLoader;

		private Multimap<string, string> files          = new Multimap<string, string>();
		private HashSet<string>          folders        = new HashSet<string>();
		private List<UploadTarget>       uploadTargets  = new List<UploadTarget>();
		private HashSet<string>          watchedFolders = new HashSet<string>();

		private bool loaded = false;

		#endregion // data

		#region interface

		public Server()
		{
			fileSystem  = new FileSystem();
			photoLoader = new PhotoLoader();

			inputThread = new Thread(InputProc);
			inputThread.Name = "Server";
			inputThread.Start();
		}

		#endregion // interface

		#region IApp Members

		public event Action Loaded     = delegate {};
		public event Action Terminated = delegate {};

		public void Quit()
		{
			Console.WriteLine("client exit");
		}

		#endregion // IApp Members

		#region IURTaskControlService Members

		public string[] GetFiles(string folder)
		{
			return files.Get(folder.ToUpperInvariant()).ToArray();
		}

		public UploadTarget[] GetFilesToUpload()
		{
			return uploadTargets.ToArray();
		}

		public string[] GetFolders()
		{
			return folders.ToArray();
		}

		public string[] GetWatchedFolders()
		{
			AuthTokenRejected();
			return watchedFolders.ToArray();
		}

		public void ReportFileNotFound(string folderPath, string filePath)
		{
			Console.WriteLine("file '{1}' from '{0}' not found", folderPath, filePath);
		}

		public void ReportFileNotFoundByID(int imageID)
		{
			Console.WriteLine("file '{0}' not found", imageID);
		}

		public void ReportFolderNotFound(string folderPath)
		{
			Console.WriteLine("folder '{0}' not found", folderPath);
		}

		public void ReportUploadFailed(string folderPath, string filePath)
		{
			Console.WriteLine("upload of '{1}' from '{0}' failed", folderPath, filePath);
		}

		public void ReportUploadFailedByID(int imageID)
		{
			Console.WriteLine("upload of '{0}' failed", imageID);
		}

		public void ReportFolderUploadComplete(string folderPath)
		{
			Console.WriteLine("completed uploading from '{0}'", folderPath);
		}

		public void ReportFolderFileCount(string folderPath, int count)
		{
			Console.WriteLine("folder '{0}' contains {1} files", folderPath, count);
		}

		#endregion // ITaskControlService Members

		#region ITaskInfoService Members

		public void StartPolling(int period)
		{
			Console.WriteLine("started polling info service");
		}

		public void StopPolling()
		{
			Console.WriteLine("stopped polling info service");
		}

		public event Action AuthTokenRejected = delegate {};
		public event Action FilesUpdated      = delegate {};
		public event Action FoldersUpdated    = delegate {};
		public event Action TaskCancelled     = delegate {};

		#endregion // ITaskInfoService Members

		#region ITaskUploadService Members

		public void ScheduleAction(Action action)
		{
			action();
		}

		void ITaskUploadService.Stop()
		{
			Console.WriteLine("stopped upload service");
		}

		public void UploadFile
			( string       folderPath
			, string       path
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			)
		{
			try
			{
				var size = LoadFile().Length;
				files.Add(folderPath.ToUpperInvariant(), path);
				Console.WriteLine
					( "uploaded '{1}' ({2} bytes) from '{0}' for {3}"
					, folderPath, path, size, uploadType
					);
				HandleUpload(path, uploadType);
			}
			catch (FileNotFoundException e)
			{
				FileNotFound(folderPath, e.FileName);
			}
		}

		public void UploadFile
			( string       folderPath
			, int          imageID
			, string       path
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			)
		{
			try
			{
				var size = LoadFile().Length;
				files.Add(folderPath.ToUpperInvariant(), path);
				Console.WriteLine
					( "uploaded '{2}' (id: {1}) ({3} bytes) from '{0}' for {4}"
					, folderPath, imageID, path, size, uploadType
					);
				HandleUpload(path, uploadType);
			}
			catch (FileNotFoundException e)
			{
				FileNotFound(folderPath, e.FileName);
			}
		}

		public event Action<string, string> DuplicateUpload = delegate {};
		public event Action<string, string> FileNotFound    = delegate {};
		public event Action<string, string> UploadFailed    = delegate {};

		#endregion // ITaskUploadService Members

		#region implementation

		private void InputProc()
		{
			var commands = new Dictionary<string, Action>();
			commands.Add("add file",             ProcessAddFile);
			commands.Add("add file to upload",   ProcessAddFileToUpload);
			commands.Add("add folder",           ProcessAddFolder);
			commands.Add("add watched folder",   ProcessAddWatchedFolder);
			commands.Add("cancel task",          ProcessCancelTask);
			commands.Add("fail upload",          ProcessFailUpload);
			commands.Add("view files",           ProcessViewFiles);
			commands.Add("view files to upload", ProcessViewFilesToUpload);
			commands.Add("view folders",         ProcessViewFolders);

			Console.WriteLine
				( "commands: start, exit, {0}"
				, string.Join(", ", commands.Keys.ToArray())
				);
			for (;;)
			{
				var command = ReadLine("");
				switch (command)
				{
					case "exit":
						Terminated();
						return;
					case "start":
						loaded = true;
						Loaded();
						break;
					default:
						if (commands.ContainsKey(command))
							commands[command]();
						else if (command != "")
							Console.WriteLine("unknown command: '{0}'", command);
						break;
				}
			}
		}

		private void ProcessAddFile()
		{
			files.Add
				( ReadLine("folder").ToUpperInvariant()
				, ReadLine("file")
				);
			if (loaded)
				FilesUpdated();
		}

		private void ProcessAddFileToUpload()
		{
			try
			{
				var filePath     = ReadLine("file");
				var imageID      = int.Parse(ReadLine("id"));
				var exifDateTime = DateTimeEx.ParseExifTime(photoLoader.GetImageDateTime(filePath)).ToUnixTime();

				uploadTargets.Add(new UploadTarget(filePath, exifDateTime, imageID));
				if (loaded)
					FilesUpdated();
			}
			catch (FormatException)
			{
				Console.WriteLine("no valid DateTime Exif property");
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("file not found: '{0}'", e.FileName);
			}
		}

		private void ProcessAddFolder()
		{
			folders.Add(ReadLine("folder"));
			if (loaded)
				FoldersUpdated();
		}

		private void ProcessAddWatchedFolder()
		{
			watchedFolders.Add(ReadLine("folder"));
		}

		private void ProcessCancelTask()
		{
			TaskCancelled();
		}

		private void ProcessFailUpload()
		{
			UploadFailed(ReadLine("folder"), ReadLine("file"));
		}

		private void ProcessViewFiles()
		{
			foreach (var folder in files)
			{
				Console.WriteLine(folder.Key);
				foreach (var file in folder.Value)
					Console.WriteLine("\t" + file);
			}
		}

		private void ProcessViewFilesToUpload()
		{
			int n = 0;
			foreach (var target in uploadTargets)
			{
				Console.WriteLine("{0,3}. path: {1}", n, target.FilePath);
				Console.WriteLine("     time: {0} ({1:u})", target.ExifDateTime, DateTimeEx.FromUnixTime(target.ExifDateTime));
				Console.WriteLine("     id:   {0}", target.ImageID);
				++n;
			}
		}

		private void ProcessViewFolders()
		{
			foreach (var folder in folders)
				Console.WriteLine(folder);
		}

		private string ReadLine(string message)
		{
			Console.Write("{0}> ", message);
			return Console.ReadLine();
		}

		private void HandleUpload(string path, UploadType uploadType)
		{
			if (uploadType == UploadType.Original)
			{
				var ucPath = path.ToUpperInvariant();
				var removedCount = uploadTargets.RemoveAll(t => t.FilePath.ToUpperInvariant() == ucPath);
				Trace.Assert(removedCount == 1);
			}
		}

		#endregion // implementation
	}
}