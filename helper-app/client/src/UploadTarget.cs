using System;

namespace Snaphappi
{
	public class UploadTarget
	{
		public readonly string  FilePath;
		public readonly int     ExifDateTime;
		public readonly ImageID ImageID;

		public UploadTarget(string filePath, int exifDateTime, ImageID imageID)
		{
			FilePath     = filePath;
			ExifDateTime = exifDateTime;
			ImageID      = imageID;
		}
	}
}