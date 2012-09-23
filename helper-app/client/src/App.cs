using System;

namespace Snaphappi
{
	public class App : IApp
	{
		#region interface

		public void LoadUR()
		{
			if (LoadUploadResampled != null)
				LoadUploadResampled();
		}

		public void LoadUO()
		{
			if (LoadUploadOriginals != null)
				LoadUploadOriginals();
		}

		#endregion

		#region IApp Members

		public event Action LoadUploadResampled;

		public event Action LoadUploadOriginals;

		public void Quit()
		{
			Environment.Exit(0);
		}

		#endregion
	}
}
