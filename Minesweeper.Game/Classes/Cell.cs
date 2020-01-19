using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Game.Classes
{
	internal class Cell
	{
		public Cell(int row, int column)
		{
			Neighbors = new List<Cell>();
			Row = row;
			Column = column;
		}

		public int Column { get; }

		public int Row { get; }

		public bool IsMined { get; set; }

		public bool IsOpen { get; set; }

		public bool IsChecked { get; set; }

		public bool IsMarked { get; set; }

		public IReadOnlyList<Cell> Neighbors { get; private set; }

		public int Count { get; private set; }

		public void UpdateNeighbors(IEnumerable<Cell> cells)
		{
			Neighbors = new List<Cell>(cells);
			Count = Neighbors.Count(x => x.IsMined);
		}
	}
}
