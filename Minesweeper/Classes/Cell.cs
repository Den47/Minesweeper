using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Classes
{
	public class Cell
	{
		public Cell(int column, int row)
		{
			Column = column;
			Row = row;
			Cells = new List<Cell>();
		}

		public int Column { get; }

		public int Row { get; }

		public bool IsMined { get; set; }

		public bool IsOpen { get; set; }

		public bool IsChecked { get; set; }

		public List<Cell> Cells { get; }

		public int Count => IsMined ? int.MaxValue : (Cells?.Count(x => x.IsMined) ?? 0);

		public override string ToString() => IsMined ? "*" : Count.ToString();
	}
}
