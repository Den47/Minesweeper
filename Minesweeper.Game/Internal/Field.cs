using System.Collections;
using System.Collections.Generic;

namespace Minesweeper.Game.Internal
{
	internal class Field : IEnumerable
	{
		private readonly Cell[,] _cells;

		public Field(Options options)
		{
			Options = options;

			_cells = new Cell[Options.Height, Options.Width];

			for (int i = 0; i < Options.Height; i++)
			{
				for (int j = 0; j < Options.Width; j++)
				{
					_cells[i, j] = new Cell(i, j);
				}
			}
		}

		public Options Options { get; }

		public Cell this[int row, int column]
		{
			get => _cells[row, column];
		}

		public bool CheckCompletion()
		{
			var notOpen = 0;
			var hasMine = 0;

			for (int i = 0; i < Options.Height; i++)
			{
				for (int j = 0; j < Options.Width; j++)
				{
					if (!_cells[i, j].IsOpen)
						notOpen++;
					if (_cells[i, j].IsMined)
						hasMine++;
				}
			}

			return notOpen == hasMine;
		}

		public IEnumerable<CellBase> GetMinedList()
		{
			for (int i = 0; i < Options.Height; i++)
				for (int j = 0; j < Options.Width; j++)
					if (_cells[i, j].IsMined)
						yield return _cells[i, j];
		}

		public IEnumerator GetEnumerator()
		{
			return _cells.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _cells.GetEnumerator();
		}
	}
}
