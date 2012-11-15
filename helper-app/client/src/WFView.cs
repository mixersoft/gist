using System;

namespace Snaphappi
{
	public class WFView : IWFView
	{
		#region data

		private readonly ITaskControlService controlService;

		#endregion // data

		#region interface

		public WFView(ITaskControlService controlService)
		{
			this.controlService = controlService;
		}

		#endregion // interface

		#region IWFView Members

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

		#endregion // IWFView Members
	}
}
