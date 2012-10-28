using System;

namespace Snaphappi
{
	public class App : IApp
	{
		#region interface

		public void Load()
		{
			if (Loaded != null)
				Loaded();
		}

		public void LoadUO()
		{
			if (LoadUploadOriginals != null)
				LoadUploadOriginals();
		}

		#endregion

		#region IApp Members

		public event Action Loaded;

		public event Action LoadUploadOriginals;

		public void Quit()
		{
			Environment.Exit(0);
		}

		#endregion
	}
}
