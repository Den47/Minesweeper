using Minesweeper.Game.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Game.Classes
{
	internal class Field
	{
		private readonly Options _options;
		private readonly Random _random = new Random();

		private Cell[,] _cells;
		private List<Cell> _cellsList;

		private Field(Options options)
		{
			_options = options;
		}

		public static Field GenerateNew(Options options, Address firstClickAddress)
		{
			var field = new Field(options);
			field.GenerateField();
			field.GenerateMines(firstClickAddress);
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

		public GameState Open(int row, int column, out List<Address> listOpenned)
		{
			listOpenned = new List<Address>();

			var cell = _cells[row, column];

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

				listOpenned.Add(new Address(cell.Column, cell.Row));

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

		private void GenerateField()
		{
			_cells = new Cell[_options.Height, _options.Width];
			_cellsList = new List<Cell>();

			for (int i = 0; i < _options.Height; i++)
			{
				for (int j = 0; j < _options.Width; j++)
				{
					var cell = new Cell(i, j);
					_cells[i, j] = cell;
					_cellsList.Add(cell);
				}
			}

			for (int i = 0; i < _options.Height; i++)
			{
				for (int j = 0; j < _options.Width; j++)
				{
					var list = new List<Cell>();

					if (i + 1 < _options.Height)
						list.Add(_cells[i + 1, j]);
					if (i - 1 >= 0)
						list.Add(_cells[i - 1, j]);
					if (j + 1 < _options.Width)
						list.Add(_cells[i, j + 1]);
					if (j - 1 >= 0)
						list.Add(_cells[i, j - 1]);
					if (i + 1 < _options.Height && j + 1 < _options.Width)
						list.Add(_cells[i + 1, j + 1]);
					if (i - 1 >= 0 && j - 1 >= 0)
						list.Add(_cells[i - 1, j - 1]);
					if (i + 1 < _options.Height && j - 1 >= 0)
						list.Add(_cells[i + 1, j - 1]);
					if (i - 1 >= 0 && j + 1 < _options.Width)
						list.Add(_cells[i - 1, j + 1]);

					_cells[i, j].UpdateNeighbors(list);
				}
			}
		}

		private void GenerateMines(Address address)
		{
			var count = 0;

			while (count < _options.MinesCount)
			{
				var i = _random.Next(_options.Height);
				var j = _random.Next(_options.Width);

				if (address.Row == i && address.Column == j)
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
		}

		private GameState Open(IEnumerable<Cell> cells, List<Address> listOpenned)
		{
			foreach (var cell in cells)
			{
				if (cell.IsChecked || cell.IsMarked)
					continue;

				listOpenned.Add(new Address(cell.Column, cell.Row));

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
	}
}
