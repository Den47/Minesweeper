using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Game.Classes
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

		public bool IsMined { get; set; }

		public bool IsOpen { get; set; }

		public bool IsChecked { get; set; }

		public bool IsMarked { get; set; }

		public List<Cell> Neighbors { get; }

		public int Count => IsMined ? int.MaxValue : (Neighbors?.Count(x => x.IsMined) ?? 0);
	}
}
