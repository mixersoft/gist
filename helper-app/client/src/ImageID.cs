using System;

namespace Snaphappi
{
	public class ImageID
	{
		public readonly string Data;

		public ImageID(string data)
		{
			Data = data;
		}

		public override bool Equals(object obj)
		{
			return obj is ImageID && Equals((ImageID)obj);
		}

		public bool Equals(ImageID id)
		{
			return Data == id.Data;
		}

		public override int GetHashCode()
		{
			return Data.GetHashCode();
		}

		public override string ToString()
		{
			return Data.ToString();
		}
	}
}
