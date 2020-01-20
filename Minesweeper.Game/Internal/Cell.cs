using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Game.Internal
{
	internal class Cell : CellBase
	{
		public Cell(int row, int column) : base(row, column)
		{
			Neighbors = new List<Cell>();
		}

		public bool IsOpen { get; set; }

		public bool IsChecked { get; set; }

		public bool IsMarked { get; set; }

		public IReadOnlyList<Cell> Neighbors { get; private set; }

		public override int Count => Neighbors.Count(x => x.IsMined);

		public void UpdateNeighbors(IEnumerable<Cell> cells)
		{
			Neighbors = new List<Cell>(cells);
		}
	}
}
