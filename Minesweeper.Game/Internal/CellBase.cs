namespace Minesweeper.Game.Internal
{
	internal class CellBase
	{
		public CellBase(int row, int column)
		{
			Row = row;
			Column = column;
		}

		public CellBase(int row, int column, int count, bool isMined) : this(row, column)
		{
			Count = count;
			IsMined = isMined;
		}

		public int Column { get; }

		public int Row { get; }

		public virtual int Count { get; }

		public bool IsMined { get; set; }

		public CellBase Copy()
		{
			return new CellBase(Row, Column, Count, IsMined);
		}
	}
}
