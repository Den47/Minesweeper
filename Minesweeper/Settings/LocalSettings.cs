using Windows.Storage;

namespace Minesweeper.Settings
{
	public static class LocalSettings
	{
		private const int DEFAULT_WIN_WIDTH = 30;
		private const int DEFAULT_WIN_HEIGHT = 16;
		private const int DEFAULT_WIN_MINES = 99;

		private const string KEY_WIDTH = "Minesweeper.Settings.Width";
		private const string KEY_HEIGHT = "Minesweeper.Settings.Height";
		private const string KEY_MINES_COUNT = "Minesweeper.Settings.MinesCount";

		public static int Width
		{
			get => GetProperty(KEY_WIDTH, DEFAULT_WIN_WIDTH);
			set => SetProperty(KEY_WIDTH, value);
		}

		public static int Height
		{
			get => GetProperty(KEY_HEIGHT, DEFAULT_WIN_HEIGHT);
			set => SetProperty(KEY_HEIGHT, value);
		}

		public static int MinesCount
		{
			get => GetProperty(KEY_MINES_COUNT, DEFAULT_WIN_MINES);
			set => SetProperty(KEY_MINES_COUNT, value);
		}

		private static void SetProperty<T>(string key, T value)
		{
			ApplicationData.Current.LocalSettings.Values[key] = value;
		}

		private static T GetProperty<T>(string key, T defaultValue)
		{
			return ApplicationData.Current.LocalSettings.Values.ContainsKey(key) ? (T)ApplicationData.Current.LocalSettings.Values[key] : defaultValue;
		}
	}
}
