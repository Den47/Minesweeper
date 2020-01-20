using System.Collections.Generic;

namespace Minesweeper.Game.Public
{
	public class OpenResult
	{
		internal OpenResult(GameState state, IEnumerable<CellResult> opennedCells)
		{
			State = state;

			if (opennedCells != null)
				OpennedCells = new List<CellResult>(opennedCells);
		}

		public GameState State { get; private set; }

		public IEnumerable<CellResult> OpennedCells { get; }
	}
}
