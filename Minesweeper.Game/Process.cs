using Minesweeper.Game.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Game
{
	public class Process
	{
		public const int MAX_WIDTH = 40;
		public const int MAX_HEIGHT = 20;

		private readonly Random _random = new Random();

		private bool _fieldGenerated;

		private int _width;
		private int _height;
		private int _minesCount;

		private Cell[,] _cells;
		private List<Cell> _cellsList;

		public Process()
		{
			_width = MAX_WIDTH;
			_height = MAX_HEIGHT;
			_minesCount = MAX_WIDTH * MAX_WIDTH / 5;
		}

		public event Action<GameState> GameStateChanged;

		public event Action<int, int, int> FieldCreated;

		public event Action<int, int> CellOpenned;

		public event Action MinesUpdated;

		public GameState GameState { get; private set; }

		public object GetCell(int row, int column)
		{
			// todo: check args
			return _cells[row, column];
		}

		public bool IsMarked(int row, int column)
		{
			// todo: check args
			return _cells[row, column].IsMarked;
		}

		public bool IsMined(int row, int column)
		{
			// todo: check args
			return _cells[row, column].IsMined;
		}

		public int GetCount(int row, int column)
		{
			// todo: check args
			return _cells[row, column].Count;
		}

		public void Restart(int width, int height, int minesCount)
		{
			Task.Run(() =>
			{
				SetParameters(width, height, minesCount);

				if (GameState == GameState.Generating)
					GenerateField();
			});
		}

		public void Open(int row, int column)
		{
			var cell = _cells[row, column];

			if (!_fieldGenerated)
				GenerateMines(cell);

			if (cell.IsMarked)
				return;

			if (cell.IsMined)
			{
				SetState(GameState.Failed);
				return;
			}

			ResetChecking();

			if (cell.IsOpen)
			{
				var notMarkedCells = cell.Neighbors.Where(x => !x.IsMarked);
				if (notMarkedCells.Count() == cell.Neighbors.Count - cell.Count)
					Open(notMarkedCells);
			}
			else
			{
				cell.IsOpen = true;
				cell.IsChecked = true;

				CellOpenned?.Invoke(cell.Row, cell.Column);

				CheckCompletedState();

				if (cell.Count == 0)
					Open(cell.Neighbors);
			}
		}

		public void Mark(int row, int column)
		{
			var cell = _cells[row, column];

			if (cell.IsOpen)
				return;

			cell.IsMarked = !cell.IsMarked;
		}

		private void GenerateMines(Cell tappedCell)
		{
			var count = 0;
			while (count < _minesCount)
			{
				var i = _random.Next(_height);
				var j = _random.Next(_width);

				if (tappedCell != null && _minesCount > 0 && _cellsList.Count > 1 &&
					tappedCell.Row == i && tappedCell.Column == j)
					continue;

				if (_cells[i, j].IsMined)
				{
					continue;
				}
				else
				{
					_cells[i, j].IsMined = true;
					count++;
				}
			}

			MinesUpdated?.Invoke();

			_fieldGenerated = true;
		}

		private void SetParameters(int width, int height, int minesCount)
		{
			if (width <= 0 || height <= 0 || width > MAX_WIDTH || height > MAX_HEIGHT || minesCount < 0 || minesCount >= width * height)
			{
				GameState = GameState.Undefined;
			}
			else
			{
				GameState = GameState.Generating;
				_width = width;
				_height = height;
				_minesCount = minesCount;
			}
		}

		private void GenerateField()
		{
			_fieldGenerated = false;
			_cells = new Cell[_height, _width];
			_cellsList = new List<Cell>();

			for (int i = 0; i < _height; i++)
			{
				for (int j = 0; j < _width; j++)
				{
					var cell = new Cell(i, j);
					_cells[i, j] = cell;
					_cellsList.Add(cell);
				}
			}

			for (int i = 0; i < _height; i++)
			{
				for (int j = 0; j < _width; j++)
				{
					var list = new List<Cell>();

					if (i + 1 < _height)
						list.Add(_cells[i + 1, j]);
					if (i - 1 >= 0)
						list.Add(_cells[i - 1, j]);
					if (j + 1 < _width)
						list.Add(_cells[i, j + 1]);
					if (j - 1 >= 0)
						list.Add(_cells[i, j - 1]);
					if (i + 1 < _height && j + 1 < _width)
						list.Add(_cells[i + 1, j + 1]);
					if (i - 1 >= 0 && j - 1 >= 0)
						list.Add(_cells[i - 1, j - 1]);
					if (i + 1 < _height && j - 1 >= 0)
						list.Add(_cells[i + 1, j - 1]);
					if (i - 1 >= 0 && j + 1 < _width)
						list.Add(_cells[i - 1, j + 1]);

					_cells[i, j].Neighbors.AddRange(list);
				}
			}

			FieldCreated?.Invoke(_width, _height, _minesCount);

			SetState(GameState.Ready);
		}

		private void Open(IEnumerable<Cell> cells)
		{
			foreach (var cell in cells)
			{
				if (cell.IsChecked || cell.IsMarked)
					continue;

				if (cell.IsMined)
				{
					SetState(GameState.Failed);
					return;
				}

				cell.IsOpen = true;
				cell.IsChecked = true;

				CellOpenned?.Invoke(cell.Row, cell.Column);

				if (cell.Count == 0)
					Open(cell.Neighbors);
			}

			CheckCompletedState();
		}

		private void ResetChecking()
		{
			foreach (var cells in _cellsList)
				cells.IsChecked = false;
		}

		private void SetState(GameState state)
		{
			GameState = state;
			GameStateChanged?.Invoke(state);
		}

		private void CheckCompletedState()
		{
			var closeCount = _cellsList.Count(x => !x.IsOpen);
			var minedCount = _cellsList.Count(x => x.IsMined);
			if (closeCount == minedCount)
			{
				SetState(GameState.Success);
			}
		}
	}
}
