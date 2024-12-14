using System;

namespace Data
{
	[Serializable]
	public class UserData
	{
		public int Level;
		public int CheckPoint;
		public int HighScore;
		public int Health;
		public int Lives;
		public bool IsMusicOn;
		public bool IsSfxOn;
		public float MusicVolume;
		public float SfxVolume;
		public float MasterVolume;
		public float Sensitivity;
		public string Resolution;

		public UserData()
		{
			Level = 1;
			CheckPoint = 0;
			HighScore = 0;
			Health = 100;
			Lives = 3;
			IsMusicOn = true;
			IsSfxOn = true;
			MusicVolume = 0.5f;
			SfxVolume = 0.5f;
			MasterVolume = 0.5f;
			Sensitivity = 0.5f;
			Resolution = "1920x1080";
		}

	}
}
