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

		public string[] GetFolders()
		{
			return folders.ToArray();
		}

		public string[] GetWatchedFolders()
		{
			AuthTokenRejected();
			return watchedFolders.ToArray();
		}

		public void ReportFolderNotFound(string folder)
		{
			Console.WriteLine("folder '{0}' not found", folder);
		}

		public void ReportUploadFailed(string folder, string path)
		{
			Console.WriteLine("upload of '{1}' from '{0}' failed", folder, path);
		}

		public void ReportFolderUploadComplete(string folder)
		{
			Console.WriteLine("completed uploading from '{0}'", folder);
		}

		public void ReportFolderFileCount(string folder, int count)
		{
			Console.WriteLine("folder '{0}' contains {1} files", folder, count);
		}

		#endregion // IURTaskControlService Members

		#region IURTaskInfoService Members

		public void StartPolling(int period)
		{
		}

		public void StopPolling()
		{
		}

		public event Action TaskCancelled = delegate {};

		public event Action FoldersUpdated = delegate {};

		#endregion // IURTaskInfoService Members

		#region IURTaskUploadService Members

		public void ScheduleAction(Action action)
		{
			action();
		}

		public void UploadFile(string folder, string path, Func<byte[]> LoadFile)
		{
			files.Add(folder.ToUpperInvariant(), path);

			var size = LoadFile().Length;
			Console.WriteLine("uploaded '{1}' ({2} bytes) from '{0}'", folder, path, size);
		}

		public event Action                 AuthTokenRejected = delegate {};
		public event Action<string, string> DuplicateUpload   = delegate {};
		public event Action<string, string> UploadFailed      = delegate {};

		#endregion // IURTaskUploadService Members

		#region implementation

		private void InputProc()
		{
			var commands = new Dictionary<string, Action>();
			commands.Add("add file",           ProcessAddFile);
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
					case "exit":  return;
					case "start": Loaded(); break;
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
		}

		private void ProcessAddFolder()
		{
			folders.Add(ReadLine("folder"));
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
