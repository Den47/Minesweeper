namespace Minesweeper.Game.DTO
{
	public class CellDto
	{
		public CellDto(int column, int row, int value)
		{
			Column = column;
			Row = row;
			Value = value;
		}

		public int Column { get; }

		public int Row { get; }

		public int Value { get; }
	}
}
