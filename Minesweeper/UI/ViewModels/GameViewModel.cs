using Minesweeper.Game;
using Minesweeper.Game.Public;
using Minesweeper.Settings;
using Minesweeper.UI.Support;
using Minesweeper.UI.ViewModels.Classes;
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

		private GameState _gameState;

		private int _fieldWidth;
		private int _fieldHeight;
		private int _minesCount;
		private int _flagsCount;

		private bool _fieldIsActive;
		private bool _showMenuForce;

		private Brush _fieldBackground;

		private List<Button> _cells;
		private List<CellViewModel> _tiles;

		public GameViewModel()
		{
			RestartCommand = new RelayCommand(Restart);
			OpenTileCommand = new RelayCommand<CellViewModel>(OpenTile);

			FieldWidth = LocalSettings.Width;
			FieldHeight = LocalSettings.Height;
			FlagsCount = MinesCount = LocalSettings.MinesCount;

			_gameProcess = new Process();

			Restart();
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

		public bool IsMenuVisible => _showMenuForce || _gameProcess.CurrentGameState == GameState.Failed || _gameProcess.CurrentGameState == GameState.Success;

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

		public RelayCommand<CellViewModel> OpenTileCommand { get; }

		public void SetCells(List<Button> buttons, List<CellViewModel> tiles)
		{
			_cells = buttons;
			_tiles = tiles;
		}

		public void Mark(CellViewModel tile)
		{
			if (tile.IsOpen)
				return;

			tile.IsMarked = !tile.IsMarked;
			_gameProcess.MarkWithFlag(tile.Row, tile.Column);

			if (tile.IsMarked)
				FlagsCount--;
			else
				FlagsCount++;
		}

		public void Restart()
		{
			GameProcess_GameStateChanged(GameState.Ready);
			FlagsCount = MinesCount;
			FieldCreated?.Invoke(FieldWidth, FieldHeight);
		}

		public void LockMenu(bool isVisible)
		{
			_showMenuForce = isVisible;

			Execute.OnUIThread(() =>
			{
				NotifyOfPropertyChange(nameof(IsMenuVisible));
			});
		}

		private void GameProcess_GameStateChanged(GameState state)
		{
			if (_gameState == state)
				return;

			_gameState = state;

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

			Execute.OnUIThread(() =>
			{
				NotifyOfPropertyChange(nameof(IsMenuVisible));
			});
		}

		private void GameProcess_MinesUpdated(CellResult cell)
		{
			var item = _tiles.FirstOrDefault(x => x.Column == cell.Column && x.Row == cell.Row);
			if (item == null)
				return;

			Execute.OnUIThread(() =>
			{
				item.IsMined = cell.IsMined;
				item.Count = cell.Count;

				if (item.IsMined)
				{
					item.Background = FieldBrushes.BrushMined;
				}
				else
				{
					switch (item.Count)
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
				}

				item.UpdateBindings();
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

		private async void OpenTile(CellViewModel tile)
		{
			OpenResult response;

			if (_gameState == GameState.Ready)
				response = await _gameProcess.StartAsync(FieldWidth, FieldHeight, MinesCount, tile.Row, tile.Column);
			else
				response = await _gameProcess.OpenAsync(tile.Row, tile.Column);

			GameProcess_GameStateChanged(response.State);

			if (response.OpennedCells != null)
			{
				var list = new List<CellViewModel>();

				foreach (var item in response.OpennedCells)
				{
					tile = _tiles.First(x => x.Column == item.Column && x.Row == item.Row);
					list.Add(tile);

					GameProcess_MinesUpdated(item);
				}

				Execute.OnUIThread(() =>
				{
					foreach (var item in list)
						item.IsOpen = true;
				});
			}
		}
	}
}
