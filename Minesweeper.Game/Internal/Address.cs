namespace Minesweeper.Game.Internal
{
	internal class Address
	{
		public Address(int row, int column)
		{
			Row = row;
			Column = column;
		}

		public int Column { get; }

		public int Row { get; }
	}
}
