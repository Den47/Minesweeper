using Minesweeper.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Minesweeper.UI.Views
{
	public sealed partial class GameView
	{
		private List<Button> _cells;
		private List<CellViewModel> _tiles;

		private int _currentCellSize;

		public GameView()
		{
			this.InitializeComponent();
		}

		private void ViewModel_FieldCreated(int width, int height)
		{
			var fieldChanged = false;

			if (field.RowDefinitions.Count != height)
			{
				fieldChanged = true;
				field.RowDefinitions.Clear();
				for (int i = 0; i < height; i++)
					field.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
			}

			if (field.ColumnDefinitions.Count != width)
			{
				fieldChanged = true;
				field.ColumnDefinitions.Clear();
				for (int i = 0; i < width; i++)
					field.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
			}

			if (fieldChanged)
			{
				field.Children.Clear();

				_cells = new List<Button>();
				_tiles = new List<CellViewModel>();

				for (int i = 0; i < field.RowDefinitions.Count; i++)
				{
					for (int j = 0; j < field.ColumnDefinitions.Count; j++)
					{
						var tile = new CellViewModel(i, j);

						var button = new Button
						{
							DataContext = tile,
							Height = _currentCellSize,
							Width = _currentCellSize,
							Style = CellButtonStyle
						};

						button.Command = ViewModel.OpenTileCommand;
						button.CommandParameter = button.DataContext as CellViewModel;
						button.RightTapped += CellButton_RightTapped;

						Grid.SetRow(button, i);
						Grid.SetColumn(button, j);

						field.Children.Add(button);

						_cells.Add(button);
						_tiles.Add(tile);
					}
				}
			}
			else
			{
				_tiles.ForEach(x => x.ResetState());

				foreach (var button in _cells)
				{
					button.IsTabStop = true;
					button.Command = ViewModel.OpenTileCommand;
				}
			}

			ViewModel.SetCells(_cells, _tiles);

			UpdateCellSize(viewBox.ActualWidth, viewBox.ActualHeight);
		}

		private void CellButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			ViewModel.Mark((CellViewModel)((FrameworkElement)sender).DataContext);
		}

		private void ViewBox_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateCellSize(e.NewSize.Width, e.NewSize.Height);
		}

		private void UpdateCellSize(double width, double height)
		{
			if (_cells == null)
				return;

			var bounds = new Size(
				width - field.Padding.Left - field.Padding.Right,
				height - field.Padding.Top - field.Padding.Bottom);

			var size = (int)Math.Min(bounds.Width / ViewModel.FieldWidth, bounds.Height / ViewModel.FieldHeight) - 2;
			if (size == _currentCellSize)
				return;

			_currentCellSize = size;

			// default cell size = 30
			// default font size = 16
			var fontSize = (int)((double)size / 30 * 16);
			if (FontSize != fontSize)
				FontSize = fontSize;

			foreach (var cell in _cells)
			{
				cell.Width = cell.Height = size;
			}
		}

		private async void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			if (e.NewState == menuState)
			{
				ViewModel.LockMenu(true);
				await Task.Delay(1000);
				NavigateToMenu();
			}
		}

		private void NavigateToMenu()
		{
			var parameter = new GameNavigationParameter();
			parameter.StartCallback = () => RestartGame(parameter);
			parameter.CloseCallback = () => ClearFrame();
			frame.Navigate(typeof(GameMenuView), parameter);
		}

		private void RestartGame(GameNavigationParameter payload)
		{
			ViewModel.FieldWidth = payload.Width;
			ViewModel.FieldHeight = payload.Height;
			ViewModel.MinesCount = payload.Mines;
			ViewModel.Restart();
		}

		private void ClearFrame()
		{
			ViewModel.LockMenu(false);
			frame.Content = null;
		}

		private void Self_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= Self_Loaded;

			ViewModel_FieldCreated(ViewModel.FieldWidth, ViewModel.FieldHeight);

			ViewModel.FieldCreated += ViewModel_FieldCreated;
		}
	}

	internal class GameNavigationParameter
	{
		public Action StartCallback { get; set; }
		public Action CloseCallback { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Mines { get; set; }
	}
}
