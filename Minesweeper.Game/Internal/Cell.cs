using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Game.Internal
{
	internal class Cell
	{
		public Cell(int row, int column)
		{
			Row = row;
			Column = column;
			Neighbors = new List<Cell>();
		}

		public int Column { get; }

		public int Row { get; }

		public bool IsOpen { get; set; }

		public bool IsChecked { get; set; }

		public bool IsMarked { get; set; }

		public bool IsMined { get; set; }

		public IReadOnlyList<Cell> Neighbors { get; private set; }

		public int Count => Neighbors.Count(x => x.IsMined);

		public void UpdateNeighbors(IEnumerable<Cell> cells)
		{
			Neighbors = new List<Cell>(cells);
		}
	}
}
