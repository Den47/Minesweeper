using Minesweeper.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Minesweeper.UI.Views
{
	public sealed partial class GameMenuView : Page
	{
		public bool IsExitStateActive
		{
			get { return (bool)GetValue(IsExitStateActiveProperty); }
			set { SetValue(IsExitStateActiveProperty, value); }
		}

		public static readonly DependencyProperty IsExitStateActiveProperty =
			DependencyProperty.Register("IsExitStateActive", typeof(bool), typeof(GameMenuView), new PropertyMetadata(false));

		private GameNavigationParameter _parameter;

		public GameMenuView()
		{
			this.InitializeComponent();

			FieldWidth = LocalSettings.Width;
			FieldHeight = LocalSettings.Height;
			FieldMines = LocalSettings.MinesCount;
		}

		public int FieldWidth { get; set; }

		public int FieldHeight { get; set; }

		public int FieldMines { get; set; }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_parameter = (GameNavigationParameter)e.Parameter;
			base.OnNavigatedTo(e);
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			_parameter.Width = FieldWidth;
			_parameter.Height = FieldHeight;
			_parameter.Mines = FieldMines;
			_parameter.StartCallback();
			IsExitStateActive = true;
		}

		private void ExitAnimation_Completed(object sender, object e)
		{
			_parameter.CloseCallback();
		}
	}
}
