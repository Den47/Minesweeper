using Minesweeper.Game.DTO;
using Minesweeper.Game.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Minesweeper.Game
{
	public class Process
	{
		private GameLogic _currentGame;

		public Process()
		{
		}

		public GameState GameState => _currentGame?.State ?? GameState.Undefined;

		/// <summary>
		/// Creates a new game and opens the first cell
		/// </summary>
		public Task<OpenResultDto> StartAsync(int width, int height, int minesCount, int firstClickRow, int firstClickColumn)
		{
			var options = Options.Create(width, height, minesCount);

			if (!options.IsInRange(firstClickRow, firstClickColumn))
				throw new ArgumentOutOfRangeException("firstClickAddress");

			var address = new Address(firstClickColumn, firstClickRow);

			_currentGame = GameLogic.CreateNewGame(options, address);

			return OpenAsync(firstClickRow, firstClickColumn);
		}

		/// <summary>
		/// Opens a cell manually
		/// </summary>
		public Task<OpenResultDto> OpenAsync(int row, int column)
		{
			var result = _currentGame.Open(row, column);
			var list = result.Item2.Select(x => new AddressDto(x.Column, x.Row));
			return Task.FromResult(new OpenResultDto(result.Item1, list));
		}

		/// <summary>
		/// Marks a cell manually
		/// </summary>
		public void MarkWithFlag(int row, int column)
		{
			_currentGame.Mark(row, column);
		}
	}
}
