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

		public void ReportFileNotFound(int imageID)
		{
			controlService.ReportFileNotFoundByID(imageID);
		}

		public void ReportUploadFailed(int imageID)
		{
			controlService.ReportUploadFailedByID(imageID);
		}

		#endregion
	}
}
