using System;

namespace Minesweeper.Game.Internal
{
	internal class Options
	{
		private const int MIN_WIDTH = 2;
		private const int MIN_HEIGHT = 2;
		private const int MAX_WIDTH = 40;
		private const int MAX_HEIGHT = 20;

		public int Width { get; }

		public int Height { get; }

		public int MinesCount { get; }

		private Options(int width, int height, int minesCount)
		{
			Width = width;
			Height = height;
			MinesCount = minesCount;
		}

		public static Options Create(int width, int height, int minesCount)
		{
			if (width < MIN_WIDTH)
				throw new ArgumentOutOfRangeException(nameof(width), $"Min width is {MIN_WIDTH}.");
			if (width > MAX_WIDTH)
				throw new ArgumentOutOfRangeException(nameof(width), $"Max width is {MAX_WIDTH}.");
			if (height < MIN_HEIGHT)
				throw new ArgumentOutOfRangeException(nameof(height), $"Min height is {MIN_HEIGHT}.");
			if (height > MAX_HEIGHT)
				throw new ArgumentOutOfRangeException(nameof(height), $"Max height is {MIN_HEIGHT}.");
			if (minesCount < 0)
				throw new ArgumentOutOfRangeException(nameof(minesCount));
			if (minesCount > width * height)
				throw new ArgumentOutOfRangeException(nameof(minesCount));

			return new Options(width, height, minesCount);
		}

		public bool IsInRange(int row, int column)
		{
			return column >= MIN_WIDTH && column <= Width && row >= MIN_HEIGHT && row <= Height;
		}
	}
}
