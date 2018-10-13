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

		public List<Cell> Neighbors { get; }

		public int Count => Neighbors.Count(x => x.IsMined);
	}
}
