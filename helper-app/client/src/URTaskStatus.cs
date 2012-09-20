using System;

namespace Snaphappi
{
	public class UploadResampledTaskStatus
	{
		public readonly bool IsCancelled;

		public UploadResampledTaskStatus(bool isCancelled)
		{
			IsCancelled = isCancelled;
		}
	}
}
