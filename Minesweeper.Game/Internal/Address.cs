namespace Minesweeper.Game.Internal
{
	internal class Address
	{
		public Address(int column, int row)
		{
			Column = column;
			Row = row;
		}

		public int Column { get; }

		public int Row { get; }
	}
}
