using Minesweeper.Game.Classes;
using System.Collections.Generic;

namespace Minesweeper.Game.Internal
{
	internal class GameLogic
	{
		private Field _field;

		public GameState State { get; private set; }

		private GameLogic()
		{
		}

		public static GameLogic CreateNewGame(Options options, Address firstClickAddress)
		{
			var game = new GameLogic();
			game._field = Field.GenerateNew(options, firstClickAddress);
			return game;
		}

		public void Mark(int row, int column)
		{
			_field.Mark(row, column);
		}

		public (GameState, IEnumerable<Address>) Open(int row, int column)
		{
			State = _field.Open(row, column, out var points);
			return (State, points);
		}
	}
}
