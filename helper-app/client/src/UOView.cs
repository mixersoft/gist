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

		public void ReportFileNotFound(ImageID imageID)
		{
			controlService.ReportFileNotFoundByID(imageID);
		}

		public void ReportUploadFailed(ImageID imageID)
		{
			controlService.ReportUploadFailedByID(imageID);
		}

		#endregion
	}
}
