using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace Minesweeper.Game.Classes
{
	internal class Field
	{
		private const int MIN_WIDTH = 2;
		private const int MIN_HEIGHT = 2;
		private const int MAX_WIDTH = 40;
		private const int MAX_HEIGHT = 20;

		private readonly Random _random = new Random();

		private Cell[,] _cells;
		private List<Cell> _cellsList;

		private Field(int width, int height, int minesCount)
		{
			SetWidth(width);
			SetHeight(height);
			SetMinesCount(minesCount);
		}

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int MinesCount { get; private set; }

		public bool FieldIsMined { get; private set; }

		public static Field GenerateNew(int width, int height, int minesCount)
		{
			var field = new Field(width, height, minesCount);
			field.Generate();
			return field;
		}

		public Cell this[int row, int column]
		{
			get => _cells[row, column];
		}

		public void Mark(int row, int column)
		{
			var cell = _cells[row, column];

			if (cell.IsOpen)
				return;

			cell.IsMarked = !cell.IsMarked;
		}

		public GameState Open(int row, int column, out List<Point> listOpenned)
		{
			listOpenned = new List<Point>();

			var cell = _cells[row, column];

			if (!FieldIsMined)
				GenerateMines(cell);

			if (cell.IsMarked)
				return GameState.Playing;

			if (cell.IsMined)
				return GameState.Failed;

			ResetChecking();

			if (cell.IsOpen)
			{
				var notMarkedCells = cell.Neighbors.Where(x => !x.IsMarked);
				if (notMarkedCells.Count() == cell.Neighbors.Count - cell.Count)
					return Open(notMarkedCells, listOpenned);
			}
			else
			{
				cell.IsOpen = true;
				cell.IsChecked = true;

				listOpenned.Add(new Point(cell.Column, cell.Row));

				if (IsCompleted())
					return GameState.Success;

				if (cell.Count == 0)
					return Open(cell.Neighbors, listOpenned);
			}

			return GameState.Playing;
		}

		private bool IsCompleted()
		{
			var closeCount = _cellsList.Count(x => !x.IsOpen);
			var minedCount = _cellsList.Count(x => x.IsMined);
			return closeCount == minedCount;
		}

		private void Generate()
		{
			FieldIsMined = false;

			_cells = new Cell[Height, Width];
			_cellsList = new List<Cell>();

			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					var cell = new Cell(i, j);
					_cells[i, j] = cell;
					_cellsList.Add(cell);
				}
			}

			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					var list = new List<Cell>();

					if (i + 1 < Height)
						list.Add(_cells[i + 1, j]);
					if (i - 1 >= 0)
						list.Add(_cells[i - 1, j]);
					if (j + 1 < Width)
						list.Add(_cells[i, j + 1]);
					if (j - 1 >= 0)
						list.Add(_cells[i, j - 1]);
					if (i + 1 < Height && j + 1 < Width)
						list.Add(_cells[i + 1, j + 1]);
					if (i - 1 >= 0 && j - 1 >= 0)
						list.Add(_cells[i - 1, j - 1]);
					if (i + 1 < Height && j - 1 >= 0)
						list.Add(_cells[i + 1, j - 1]);
					if (i - 1 >= 0 && j + 1 < Width)
						list.Add(_cells[i - 1, j + 1]);

					_cells[i, j].Neighbors.AddRange(list);
				}
			}
		}

		private void GenerateMines(Cell tappedCell)
		{
			var count = 0;

			while (count < MinesCount)
			{
				var i = _random.Next(Height);
				var j = _random.Next(Width);

				if (tappedCell.Row == i && tappedCell.Column == j)
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

			FieldIsMined = true;
		}

		private GameState Open(IEnumerable<Cell> cells, List<Point> listOpenned)
		{
			foreach (var cell in cells)
			{
				if (cell.IsChecked || cell.IsMarked)
					continue;

				listOpenned.Add(new Point(cell.Column, cell.Row));

				if (cell.IsMined)
					return GameState.Failed;

				cell.IsOpen = true;
				cell.IsChecked = true;

				if (cell.Count == 0)
					Open(cell.Neighbors, listOpenned);
			}

			return IsCompleted() ? GameState.Success : GameState.Playing;
		}

		private void ResetChecking()
		{
			foreach (var cells in _cellsList)
				cells.IsChecked = false;
		}

		private void SetWidth(int width)
		{
			if (width < MIN_WIDTH)
				Width = MIN_WIDTH;
			else if (width > MAX_WIDTH)
				Width = MAX_WIDTH;
			else
				Width = width;
		}

		private void SetHeight(int height)
		{
			if (height < MIN_HEIGHT)
				Height = MIN_HEIGHT;
			else if (height > MAX_HEIGHT)
				Height = MAX_HEIGHT;
			else
				Height = height;
		}

		private void SetMinesCount(int minesCount)
		{
			if (minesCount < 1)
			{
				MinesCount = 1;
			}
			else
			{
				var maxMines = Width * Height - 1;
				if (minesCount > maxMines)
					MinesCount = maxMines;
				else
					MinesCount = minesCount;
			}
		}
	}
}
