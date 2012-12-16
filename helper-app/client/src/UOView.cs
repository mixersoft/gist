using System;

namespace Snaphappi
{
	public class UOView : IUOView
	{
		#region data

		private readonly ITaskControlService controlService;

		#endregion

		#region interface

		public UOView(ITaskControlService controlService)
		{
			this.controlService = controlService;
		}

		#endregion

		#region IUOView Members

		public void ReportFileNotFound(string folderPath, string filePath)
		{
			controlService.ReportFileNotFound(folderPath, filePath);
		}

		public void ReportUploadFailed(string folderPath, string filePath)
		{
			controlService.ReportUploadFailed(folderPath, filePath);
		}

		#endregion
	}
}
