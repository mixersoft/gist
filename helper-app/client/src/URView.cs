using System;

namespace Snaphappi
{
	public class URView : IURView
	{
		#region data

		private readonly IURTaskControlService task;

		#endregion

		#region interface

		public URView(IURTaskControlService taskControl)
		{
			this.task = taskControl;
		}

		#endregion

		#region IURView Members

		public void ReportFolderNotFound(string folderPath)
		{
			task.ReportFolderNotFound(folderPath);
		}

		public void ReportFileCount(string folderPath, int count)
		{
			task.ReportFolderFileCount(folderPath, count);
		}

		public void ReportUploadFailed(string folderPath, string filePath)
		{
			task.ReportUploadFailed(folderPath, filePath);
		}

		public void ReportFolderUploadComplete(string folderPath)
		{
			task.ReportFolderUploadComplete(folderPath);
		}

		#endregion
	}
}
