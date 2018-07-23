using Minesweeper.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Minesweeper.UI.Views
{
	public sealed partial class GameView
	{
		private readonly Game.Process _gameProcess;

		private List<Button> _cells;

		public GameView()
		{
			this.InitializeComponent();

			_gameProcess = new Game.Process();
			_gameProcess.GameStateChanged += GameProcess_GameStateChanged;
			_gameProcess.FieldCreated += GameProcess_FieldUpdated;
			_gameProcess.CellOpenned += GameProcess_CellOpenned;
			_gameProcess.MinesUpdated += GameProcess_MinesUpdated;
			_gameProcess.Restart((int)WidthSlider.Value, (int)HeightSlider.Value, (int)MinesSlider.Value);
		}

		private void GameProcess_GameStateChanged(Game.GameState state)
		{
			switch (state)
			{
				case Game.GameState.Undefined:
					ExecuteOnUIThread(RunDisableState);
					break;
				case Game.GameState.Generating:
					ExecuteOnUIThread(RunDisableState);
					break;
				case Game.GameState.Ready:
					ExecuteOnUIThread(RunActiveState);
					break;
				case Game.GameState.Playing:
					ExecuteOnUIThread(RunActiveState);
					break;
				case Game.GameState.Success:
					ExecuteOnUIThread(RunSuccessState);
					break;
				case Game.GameState.Failed:
					ExecuteOnUIThread(RunFailedState);
					break;
				default:
					break;
			}
		}

		private void GameProcess_FieldUpdated(int width, int height, int minesCount)
		{
			ExecuteOnUIThread(() =>
			{
				field.Children.Clear();
				field.ColumnDefinitions.Clear();
				field.RowDefinitions.Clear();

				for (int i = 0; i < height; i++)
					field.RowDefinitions.Add(new RowDefinition());
				for (int i = 0; i < width; i++)
					field.ColumnDefinitions.Add(new ColumnDefinition());

				_cells = new List<Button>();

				for (int i = 0; i < field.RowDefinitions.Count; i++)
				{
					for (int j = 0; j < field.ColumnDefinitions.Count; j++)
					{
						var button = new Button
						{
							DataContext = new Cell(i, j),
							Style = CellButtonStyle
						};

						button.Click += CellButton_Click;
						button.RightTapped += CellButton_RightTapped;

						Grid.SetRow(button, i);
						Grid.SetColumn(button, j);

						field.Children.Add(button);

						_cells.Add(button);
					}
				}

				FlagsCounterTextBlock.Text = minesCount.ToString();
			});
		}

		private void GameProcess_CellOpenned(int row, int column)
		{
			ExecuteOnUIThread(() =>
			{
				var item = _cells.Select(x => x.DataContext).OfType<Cell>().FirstOrDefault(x => x.Row == row && x.Column == column);
				if (item != null)
					item.IsOpen = true;
			});
		}

		private void GameProcess_MinesUpdated()
		{
			ExecuteOnUIThread(() =>
			{
				foreach (var button in _cells)
				{
					var item = (Cell)(button.DataContext);
					var count = _gameProcess.GetCount(item.Row, item.Column);
					switch (count)
					{
						case 0:
							item.Background = Brush0;
							break;
						case 1:
							item.Background = Brush1;
							break;
						case 2:
							item.Background = Brush2;
							break;
						case 3:
							item.Background = Brush3;
							break;
						case 4:
							item.Background = Brush4;
							break;
						case 5:
							item.Background = Brush5;
							break;
						case 6:
							item.Background = Brush6;
							break;
						case 7:
							item.Background = Brush7;
							break;
						default:
							item.Background = Brush8;
							break;
					}

					item.IsMined = _gameProcess.IsMined(item.Row, item.Column);

					item.Count = count;

					if (item.IsMined)
						item.Background = BrushMined;

					item.UpdateBindings();
				}
			});
		}

		private void CellButton_Click(object sender, RoutedEventArgs e)
		{
			var cell = ((FrameworkElement)sender).DataContext as Cell;
			if (cell == null)
				return;

			Task.Run(() => _gameProcess.Open(cell.Row, cell.Column));
		}

		private void CellButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var cell = ((FrameworkElement)sender).DataContext as Cell;
			if (cell == null)
				return;

			cell.IsMarked = !cell.IsMarked;
			_gameProcess.Mark(cell.Row, cell.Column);

			if (cell.IsMarked)
				FlagsCounterTextBlock.Text = (int.Parse(FlagsCounterTextBlock.Text) - 1).ToString();
			else
				FlagsCounterTextBlock.Text = (int.Parse(FlagsCounterTextBlock.Text) + 1).ToString();
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			_gameProcess.Restart((int)WidthSlider.Value, (int)HeightSlider.Value, (int)MinesSlider.Value);
		}

		private void RunActiveState()
		{
			field.Background = new SolidColorBrush(Colors.Transparent);
			field.IsHitTestVisible = true;
		}

		private void RunDisableState()
		{
			field.Background = new SolidColorBrush(Colors.Transparent);
			field.IsHitTestVisible = false;
		}

		private void RunSuccessState()
		{
			field.Background = BrushCompleted;
			field.IsHitTestVisible = false;
			foreach (var cell in _cells)
			{
				cell.IsTabStop = false;
				cell.Click -= CellButton_Click;
				cell.RightTapped -= CellButton_RightTapped;
			}
		}

		private void RunFailedState()
		{
			_cells.Select(x => x.DataContext).OfType<Cell>().Where(x => x.IsMined).ToList().ForEach(x => x.IsOpen = true);

			field.Background = BrushFailed;
			field.IsHitTestVisible = false;
			foreach (var cell in _cells)
			{
				cell.IsTabStop = false;
				cell.Click -= CellButton_Click;
				cell.RightTapped -= CellButton_RightTapped;
			}
		}

		private void ExecuteOnUIThread(Action action)
		{
			var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				action();
			});
		}
	}
}
