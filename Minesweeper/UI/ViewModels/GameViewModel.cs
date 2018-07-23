using Minesweeper.Game;
using Minesweeper.UI.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Minesweeper.UI.ViewModels
{
	public class GameViewModel : PropertyChangedBase
	{
		private readonly Brush _defaultBackgroundBrush = new SolidColorBrush(Colors.Transparent);
		private readonly Brush _completedBackgroundBrush = new SolidColorBrush(Colors.PaleGreen);
		private readonly Brush _failedBackgroundBrush = new SolidColorBrush(Colors.PaleVioletRed);

		private readonly Process _gameProcess;

		private int _fieldWidth = 32;
		private int _fieldHeight = 16;
		private int _minesCount = 20;
		private int _flagsCount = 20;

		private bool _fieldIsActive;

		private Brush _fieldBackground;

		private List<Button> _cells;

		public GameViewModel()
		{
			RestartCommand = new RelayCommand(Restart);
			OpenTileCommand = new RelayCommand<TileViewModel>(OpenTile);

			_gameProcess = new Process();
			_gameProcess.GameStateChanged += GameProcess_GameStateChanged;
			_gameProcess.FieldCreated += GameProcess_FieldUpdated;
			_gameProcess.CellOpenned += GameProcess_CellOpenned;
			_gameProcess.MinesUpdated += GameProcess_MinesUpdated;
			_gameProcess.Restart(FieldWidth, FieldHeight, MinesCount);
		}

		public event Action<int, int> FieldUpdated;

		public int FieldWidth
		{
			get => _fieldWidth;
			set
			{
				if (_fieldWidth != value)
				{
					_fieldWidth = value;
					NotifyOfPropertyChange(nameof(FieldWidth));
				}
			}
		}

		public int FieldHeight
		{
			get => _fieldHeight;
			set
			{
				if (_fieldHeight != value)
				{
					_fieldHeight = value;
					NotifyOfPropertyChange(nameof(FieldHeight));
				}
			}
		}

		public int MinesCount
		{
			get => _minesCount;
			set
			{
				if (_minesCount != value)
				{
					_minesCount = value;
					NotifyOfPropertyChange(nameof(MinesCount));
				}
			}
		}

		public int FlagsCount
		{
			get => _flagsCount;
			set
			{
				if (_flagsCount != value)
				{
					_flagsCount = value;
					NotifyOfPropertyChange(nameof(FlagsCount));
				}
			}
		}

		public bool FieldIsActive
		{
			get => _fieldIsActive;
			set
			{
				if (_fieldIsActive != value)
				{
					_fieldIsActive = value;
					NotifyOfPropertyChange(nameof(FieldIsActive));
				}
			}
		}

		public Brush FieldBackground
		{
			get => _fieldBackground;
			set
			{
				if (_fieldBackground != value)
				{
					_fieldBackground = value;
					NotifyOfPropertyChange(nameof(FieldBackground));
				}
			}
		}

		public Brush Brush0 { get; set; }

		public Brush Brush1 { get; set; }

		public Brush Brush2 { get; set; }

		public Brush Brush3 { get; set; }

		public Brush Brush4 { get; set; }

		public Brush Brush5 { get; set; }

		public Brush Brush6 { get; set; }

		public Brush Brush7 { get; set; }

		public Brush Brush8 { get; set; }

		public Brush BrushMined { get; set; }

		public Brush BrushClose { get; set; }

		public RelayCommand RestartCommand { get; }

		public RelayCommand<TileViewModel> OpenTileCommand { get; }

		public void SetCells(List<Button> buttons)
		{
			_cells = buttons;
		}

		public void Mark(TileViewModel tile)
		{
			tile.IsMarked = !tile.IsMarked;
			_gameProcess.Mark(tile.Row, tile.Column);

			if (tile.IsMarked)
				FlagsCount--;
			else
				FlagsCount++;
		}

		private void GameProcess_GameStateChanged(GameState state)
		{
			switch (state)
			{
				case GameState.Undefined:
					Execute.OnUIThread(RunDisableState);
					break;
				case GameState.Generating:
					Execute.OnUIThread(RunDisableState);
					break;
				case GameState.Ready:
					Execute.OnUIThread(RunActiveState);
					break;
				case GameState.Playing:
					Execute.OnUIThread(RunActiveState);
					break;
				case GameState.Success:
					Execute.OnUIThread(RunSuccessState);
					break;
				case GameState.Failed:
					Execute.OnUIThread(RunFailedState);
					break;
				default:
					break;
			}
		}

		private void GameProcess_FieldUpdated(int width, int height, int minesCount)
		{
			Execute.OnUIThread(() =>
			{
				FlagsCount = MinesCount;
				FieldUpdated?.Invoke(width, height);
			});
		}

		private void GameProcess_CellOpenned(int row, int column)
		{
			Execute.OnUIThread(() =>
			{
				var item = _cells.Select(x => x.DataContext).OfType<TileViewModel>().FirstOrDefault(x => x.Row == row && x.Column == column);
				if (item != null)
					item.IsOpen = true;
			});
		}

		private void GameProcess_MinesUpdated()
		{
			Execute.OnUIThread(() =>
			{
				foreach (var button in _cells)
				{
					var item = (TileViewModel)(button.DataContext);
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

		private void RunActiveState()
		{
			FieldBackground = _defaultBackgroundBrush;
			FieldIsActive = true;
		}

		private void RunDisableState()
		{
			FieldBackground = _defaultBackgroundBrush;
			FieldIsActive = false;
		}

		private void RunSuccessState()
		{
			FieldBackground = _completedBackgroundBrush;
			FieldIsActive = false;
			foreach (var cell in _cells)
			{
				cell.IsTabStop = false;
				cell.Command = null;
				//cell.RightTapped -= CellButton_RightTapped;
			}
		}

		private void RunFailedState()
		{
			_cells.Select(x => x.DataContext).OfType<TileViewModel>().Where(x => x.IsMined).ToList().ForEach(x => x.IsOpen = true);

			FieldBackground = _failedBackgroundBrush;
			FieldIsActive = false;
			foreach (var cell in _cells)
			{
				cell.IsTabStop = false;
				cell.Command = null;
				//cell.RightTapped -= CellButton_RightTapped;
			}
		}

		private void Restart()
		{
			_gameProcess.Restart(FieldWidth, FieldHeight, MinesCount);
		}

		private void OpenTile(TileViewModel tile)
		{
			_gameProcess.Open(tile.Row, tile.Column);
		}
	}
}
