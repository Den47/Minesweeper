using System.Collections.Generic;

namespace Minesweeper.Game.DTO
{
	public class OpenResultDto
	{
		public OpenResultDto(GameState state, IEnumerable<CellDto> opennedCells)
		{
			State = state;

			if (opennedCells != null)
				OpennedCells = new List<CellDto>(opennedCells);
		}

		public GameState State { get; }

		public IEnumerable<CellDto> OpennedCells { get; }
	}
}
