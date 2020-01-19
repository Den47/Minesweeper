namespace Minesweeper.Game.DTO
{
	public class AddressDto
	{
		public AddressDto(int column, int row)
		{
			Column = column;
			Row = row;
		}

		public int Column { get; }

		public int Row { get; }
	}
}
