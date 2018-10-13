using Minesweeper.Game.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Minesweeper.Game
{
	public class Process
	{
		private Field _field;

		public Process()
		{
		}

		public event Action<GameState> GameStateChanged;

		public event Action<int, int, int> FieldCreated;

		public event Action<IReadOnlyList<Point>> CellOpenned;

		public event Action MinesUpdated;

		public GameState GameState { get; private set; }

		public bool IsMined(int row, int column) => _field[row, column].IsMined;

		public int GetCount(int row, int column) => _field[row, column].Count;

		public void Restart(int width, int height, int minesCount)
		{
			Task.Run(() =>
			{
				_field = Field.GenerateNew(width, height, minesCount);
				FieldCreated?.Invoke(_field.Width, _field.Height, _field.MinesCount);
				SetState(GameState.Ready);
			});
		}

		public void Open(int row, int column)
		{
			Task.Run(() =>
			{
				var fieldIsMined = _field.FieldIsMined;
				var newState = _field.Open(row, column, out var points);

				if (fieldIsMined != _field.FieldIsMined)
					MinesUpdated?.Invoke();

				SetState(newState);
				CellOpenned?.Invoke(points);
			});
		}

		public void Mark(int row, int column)
		{
			_field.Mark(row, column);
		}

		private void SetState(GameState state)
		{
			GameState = state;
			GameStateChanged?.Invoke(state);
		}
	}
}
