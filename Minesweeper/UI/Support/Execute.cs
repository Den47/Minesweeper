using System;
using Windows.UI.Core;

namespace Minesweeper.UI.Support
{
	public static class Execute
	{
		public static void OnUIThread(Action action)
		{
			var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
				CoreDispatcherPriority.Normal,
				() => { action?.Invoke(); });
		}
	}
}
