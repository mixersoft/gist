using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Snaphappi
{
	public class Server : IApp, ITaskControlService, ITaskInfoService, ITaskUploadService
	{
		#region data

		private readonly Thread inputThread;

		private HashSet<string> folders = new HashSet<string>();

		private HashSet<string> watchedFolders = new HashSet<string>();

		private Multimap<string, string> files = new Multimap<string, string>();

		private List<UploadTarget> filesToUpload = new List<UploadTarget>();

		private bool loaded = false;

		#endregion // data

		#region interface

		public Server()
		{
			this.inputThread = new Thread(InputProc);
			this.inputThread.Start();
		}

		#endregion // interface

		#region IApp Members

		public event Action Loaded = delegate {};

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
			return filesToUpload.ToArray();
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

		public void ReportFolderNotFound(string folderPath)
		{
			Console.WriteLine("folder '{0}' not found", folderPath);
		}

		public void ReportUploadFailed(string folderPath, string filePath)
		{
			Console.WriteLine("upload of '{1}' from '{0}' failed", folderPath, filePath);
		}

		public void ReportFolderUploadComplete(string folderPath)
		{
			Console.WriteLine("completed uploading from '{0}'", folderPath);
		}

		public void ReportFolderFileCount(string folderPath, int count)
		{
			Console.WriteLine("folder '{0}' contains {1} files", folderPath, count);
		}

		#endregion // IURTaskControlService Members

		#region IURTaskInfoService Members

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

		#endregion // IURTaskInfoService Members

		#region IURTaskUploadService Members

		public void ScheduleAction(Action action)
		{
			action();
		}

		public void UploadFile
			( string       folderPath
			, string       filePath
			, UploadType   uploadType
			, Func<byte[]> LoadFile
			)
		{
			try
			{
				var size = LoadFile().Length;
				files.Add(folderPath.ToUpperInvariant(), filePath);
				Console.WriteLine("uploaded '{1}' ({2} bytes) from '{0}' for {3}", folderPath, filePath, size, uploadType);
			}
			catch (FileNotFoundException)
			{
				FileNotFound(folderPath, filePath);
			}
		}

		public event Action<string, string> DuplicateUpload = delegate {};
		public event Action<string, string> FileNotFound    = delegate {};
		public event Action<string, string> UploadFailed    = delegate {};

		#endregion // IURTaskUploadService Members

		#region implementation

		private void InputProc()
		{
			var commands = new Dictionary<string, Action>();
			commands.Add("add file",           ProcessAddFile);
			commands.Add("add file to upload", ProcessAddFileToUpload);
			commands.Add("add folder",         ProcessAddFolder);
			commands.Add("add watched folder", ProcessAddWatchedFolder);
			commands.Add("cancel task",        ProcessCancelTask);
			commands.Add("fail upload",        ProcessFailUpload);
			commands.Add("view files",         ProcessViewFiles);
			commands.Add("view folders",       ProcessViewFolders);

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
						return;
					case "start":
						loaded = true;
						Loaded();
						break;
					default:
						if (commands.ContainsKey(command))
							commands[command]();
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
			var filePath   = ReadLine("file");
			var timestamp  = DateTime.Parse(ReadLine("time")).ToUnixTime();
			var hash       = int.Parse(ReadLine("hash"));
			var folderPath = ReadLine("folder");
			filesToUpload.Add(new UploadTarget(filePath, timestamp, hash, folderPath));
			if (loaded)
				FilesUpdated();
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

		#endregion // implementation
	}
}
