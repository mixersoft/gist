using System;

namespace Snaphappi
{
	public class UploadTarget
	{
		public readonly string FilePath;
		public readonly int    ExifDateTime;
		public readonly int    ImageID;

		public UploadTarget(string filePath, int exifDateTime, int imageID)
		{
			FilePath     = filePath;
			ExifDateTime = exifDateTime;
			ImageID      = imageID;
		}
	}
}