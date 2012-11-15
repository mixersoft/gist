using System;

namespace Snaphappi
{
	public class URView : IURView
	{
		#region data

		private readonly ITaskControlService controlService;

		#endregion

		#region interface

		public URView(ITaskControlService controlService)
		{
			this.controlService = controlService;
		}

		#endregion

		#region IURView Members

		public void ReportFileCount(string folderPath, int count)
		{
			controlService.ReportFolderFileCount(folderPath, count);
		}

		public void ReportFolderNotFound(string folderPath)
		{
			controlService.ReportFolderNotFound(folderPath);
		}

		public void ReportFolderUploadComplete(string folderPath)
		{
			controlService.ReportFolderUploadComplete(folderPath);
		}

		public void ReportUploadFailed(string folderPath, string filePath)
		{
			controlService.ReportUploadFailed(folderPath, filePath);
		}

		#endregion
	}
}
