namespace Minesweeper.Game.DTO
{
	public class CellDto
	{
		public CellDto(int row, int column, int count, bool isMined)
		{
			Row = row;
			Column = column;
			Count = count;
			IsMined = isMined;
		}

		public int Column { get; }

		public int Row { get; }

		public int Count { get; }

		public bool IsMined { get; }
	}
}
