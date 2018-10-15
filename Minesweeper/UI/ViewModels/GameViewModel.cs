using Minesweeper.Game;
using Minesweeper.Settings;
using Minesweeper.UI.Support;
using Minesweeper.UI.ViewModels.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
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

		private int _fieldWidth;
		private int _fieldHeight;
		private int _minesCount;
		private int _flagsCount;

		private bool _fieldIsActive;

		private Brush _fieldBackground;

		private List<Button> _cells;
		private List<TileViewModel> _tiles;

		public GameViewModel()
		{
			RestartCommand = new RelayCommand(Restart);
			OpenTileCommand = new RelayCommand<TileViewModel>(OpenTile);

			FieldWidth = LocalSettings.Width;
			FieldHeight = LocalSettings.Height;
			FlagsCount = MinesCount = LocalSettings.MinesCount;

			_gameProcess = new Process();
			_gameProcess.GameStateChanged += GameProcess_GameStateChanged;
			_gameProcess.FieldCreated += GameProcess_FieldCreated;
			_gameProcess.CellOpenned += GameProcess_CellOpenned;
			_gameProcess.MinesUpdated += GameProcess_MinesUpdated;
			_gameProcess.Restart(FieldWidth, FieldHeight, MinesCount);
		}

		public event Action<int, int> FieldCreated;

		public int FieldWidth
		{
			get => _fieldWidth;
			set
			{
				if (_fieldWidth != value)
				{
					_fieldWidth = value;
					LocalSettings.Width = value;
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
					LocalSettings.Height = value;
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
					LocalSettings.MinesCount = value;
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

		public RelayCommand RestartCommand { get; }

		public RelayCommand<TileViewModel> OpenTileCommand { get; }

		public void SetCells(List<Button> buttons, List<TileViewModel> tiles)
		{
			_cells = buttons;
			_tiles = tiles;
		}

		public void Mark(TileViewModel tile)
		{
			if (tile.IsOpen)
				return;

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

		private void GameProcess_FieldCreated(int width, int height, int minesCount)
		{
			Execute.OnUIThread(() =>
			{
				FlagsCount = MinesCount;
				FieldCreated?.Invoke(width, height);
			});
		}

		private void GameProcess_CellOpenned(IReadOnlyList<Point> listOpenned)
		{
			var list = new List<TileViewModel>();

			foreach (var point in listOpenned)
			{
				var tile = _tiles.First(x => x.Column == point.X && x.Row == point.Y);
				list.Add(tile);
			}

			Execute.OnUIThread(() =>
			{
				foreach (var item in list)
					item.IsOpen = true;
			});
		}

		private void GameProcess_MinesUpdated()
		{
			Execute.OnUIThread(() =>
			{
				foreach (var item in _tiles)
				{
					var count = _gameProcess.GetCount(item.Row, item.Column);
					switch (count)
					{
						case 0:
							item.Background = FieldBrushes.Brush0;
							break;
						case 1:
							item.Background = FieldBrushes.Brush1;
							break;
						case 2:
							item.Background = FieldBrushes.Brush2;
							break;
						case 3:
							item.Background = FieldBrushes.Brush3;
							break;
						case 4:
							item.Background = FieldBrushes.Brush4;
							break;
						case 5:
							item.Background = FieldBrushes.Brush5;
							break;
						case 6:
							item.Background = FieldBrushes.Brush6;
							break;
						case 7:
							item.Background = FieldBrushes.Brush7;
							break;
						default:
							item.Background = FieldBrushes.Brush8;
							break;
					}

					item.IsMined = _gameProcess.IsMined(item.Row, item.Column);

					item.Count = count;

					if (item.IsMined)
						item.Background = FieldBrushes.BrushMined;

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
			}
		}

		private void RunFailedState()
		{
			_tiles.Where(x => x.IsMined).ToList().ForEach(x => x.IsOpen = true);

			FieldBackground = _failedBackgroundBrush;
			FieldIsActive = false;
			foreach (var cell in _cells)
			{
				cell.IsTabStop = false;
				cell.Command = null;
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
