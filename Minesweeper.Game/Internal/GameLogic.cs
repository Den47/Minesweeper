using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Game.Internal
{
	internal class GameLogic
	{
		private static readonly Random _random = new Random();

		private readonly Field _field;

		public GameState State { get; private set; }

		private GameLogic(Field field)
		{
			_field = field;
		}

		public static GameLogic CreateNewGame(Options options, Address firstClickAddress)
		{
			var field = new Field(options);

			GenerateField(field);
			GenerateMines(field, firstClickAddress);

			return new GameLogic(field);
		}

		public void Mark(int row, int column)
		{
			var cell = _field[row, column];
			if (cell.IsOpen)
				return;

			cell.IsMarked = !cell.IsMarked;
		}

		public (GameState, IEnumerable<CellBase>) OpenCell(int row, int column)
		{
			var result = Open(row, column);
			State = result.Item1;
			return result;
		}

		private bool IsCompleted() => _field.CheckCompletion();

		public void ResetChecking()
		{
			foreach (var cells in _field)
				((Cell)cells).IsChecked = false;
		}

		private (GameState, IEnumerable<CellBase>) Open(int row, int column)
		{
			var cell = _field[row, column];

			if (cell.IsMarked)
				return (GameState.Playing, null);

			if (cell.IsMined)
				return (GameState.Failed, _field.GetMinedList());

			ResetChecking();

			var listOpenned = new List<CellBase>();

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

				listOpenned.Add(cell.Copy());

				if (IsCompleted())
					return (GameState.Success, listOpenned);

				if (cell.Count == 0)
					return Open(cell.Neighbors, listOpenned);
			}

			return (GameState.Playing, listOpenned);
		}

		private (GameState, IEnumerable<CellBase>) Open(IEnumerable<Cell> cells, List<CellBase> listOpenned)
		{
			foreach (var cell in cells)
			{
				if (cell.IsChecked || cell.IsMarked)
					continue;

				listOpenned.Add(cell.Copy());

				if (cell.IsMined)
					return (GameState.Failed, _field.GetMinedList());

				cell.IsOpen = true;
				cell.IsChecked = true;

				if (cell.Count == 0)
					Open(cell.Neighbors, listOpenned);
			}

			return IsCompleted() ? (GameState.Success, listOpenned) : (GameState.Playing, listOpenned);
		}

		private static void GenerateField(Field field)
		{
			for (int i = 0; i < field.Options.Height; i++)
			{
				for (int j = 0; j < field.Options.Width; j++)
				{
					var list = new List<Cell>();

					if (i + 1 < field.Options.Height)
						list.Add(field[i + 1, j]);
					if (i - 1 >= 0)
						list.Add(field[i - 1, j]);
					if (j + 1 < field.Options.Width)
						list.Add(field[i, j + 1]);
					if (j - 1 >= 0)
						list.Add(field[i, j - 1]);
					if (i + 1 < field.Options.Height && j + 1 < field.Options.Width)
						list.Add(field[i + 1, j + 1]);
					if (i - 1 >= 0 && j - 1 >= 0)
						list.Add(field[i - 1, j - 1]);
					if (i + 1 < field.Options.Height && j - 1 >= 0)
						list.Add(field[i + 1, j - 1]);
					if (i - 1 >= 0 && j + 1 < field.Options.Width)
						list.Add(field[i - 1, j + 1]);

					field[i, j].UpdateNeighbors(list);
				}
			}
		}

		private static void GenerateMines(Field field, Address address)
		{
			var count = 0;

			while (count < field.Options.MinesCount)
			{
				var i = _random.Next(field.Options.Height);
				var j = _random.Next(field.Options.Width);

				if (address.Row == i && address.Column == j)
					continue;

				if (field[i, j].IsMined)
				{
					continue;
				}
				else
				{
					field[i, j].IsMined = true;
					count++;
				}
			}
		}
	}
}
