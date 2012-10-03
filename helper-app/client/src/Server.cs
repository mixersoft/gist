using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Snaphappi
{
	public class Server : IURTaskControlService, IURTaskInfoService, IURTaskUploadService
	{
		#region types

		public enum TaskType
		{
			UploadResampled,
			UploadOriginals,
		}

		#endregion

		#region data

		private readonly TaskType taskType;

		private readonly Thread inputThread;

		private List<string> folders = new List<string>();
		private List<string> files   = new List<string>();

		#endregion

		#region interface

		public Server(TaskType taskType)
		{
			this.taskType = taskType;

			this.inputThread = new Thread(InputProc);
			this.inputThread.Start();
		}

		#endregion

		#region IURTaskControlService Members

		public string[] GetFolders()
		{
			return folders.ToArray();
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

		#endregion

		#region IURTaskInfoService Members

		public void StartPolling(int period)
		{
		}

		public void StopPolling()
		{
		}

		public event Action TaskCancelled;

		public event Action FoldersUpdated;

		#endregion

		#region IURTaskUploadService Members

		public void UploadFile(string folder, string path, Func<byte[]> LoadFile)
		{
			var size = LoadFile().Length;
			Console.WriteLine("uploaded '{1}' ({2} bytes) from '{0}'", folder, path, size);
		}

		public event Action<string, string> UploadFailed;

		#endregion

		#region implementation

		private void InputProc()
		{
			Console.WriteLine("commands: exit, add folder, cancel task");
			for (;;)
			{
				switch (ReadLine(""))
				{
					case "exit":        return;
					case "add folder":  ProcessAddFolder();  break;
					case "cancel task": ProcessCancelTask(); break;
					case "fail upload": ProcessFailUpload(); break;
				}
			}
		}

		private void ProcessAddFolder()
		{
			folders.Add(ReadLine("folder"));
			FoldersUpdated();
		}

		private void ProcessCancelTask()
		{
			TaskCancelled();
		}

		private void ProcessFailUpload()
		{
			UploadFailed(ReadLine("folder"), ReadLine("file"));
		}

		private string ReadLine(string message)
		{
			Console.Write("{0}> ", message);
			return Console.ReadLine();
		}

		#endregion
	}
}
