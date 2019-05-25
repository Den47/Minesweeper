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

		/// <summary>
		/// Width, Height, MinesCount
		/// </summary>
		public event Action<int, int, int> FieldCreated;

		/// <summary>
		/// Returns openned cells after open some cell
		/// </summary>
		public event Action<IReadOnlyList<Point>> CellOpenned;

		/// <summary>
		/// Raised after full generating field
		/// </summary>
		public event Action MinesUpdated;

		/// <summary>
		/// Current game state
		/// </summary>
		public GameState GameState { get; private set; }

		/// <summary>
		/// Check the cell for mines
		/// </summary>
		public bool IsMined(int row, int column)
		{
			return _field[row, column].IsMined;
		}

		/// <summary>
		/// Gets the number of mines around
		/// </summary>
		public int GetMinesCount(int row, int column)
		{
			return _field[row, column].Count;
		}

		/// <summary>
		/// Creates a new game field
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="minesCount"></param>
		public void Restart(int width, int height, int minesCount)
		{
			Task.Run(() =>
			{
				_field = Field.GenerateNew(width, height, minesCount);
				FieldCreated?.Invoke(_field.Width, _field.Height, _field.MinesCount);
				UpdateGameState(GameState.Ready);
			});
		}

		/// <summary>
		/// Opens the cell manually
		/// </summary>
		public void Open(int row, int column)
		{
			Task.Run(() =>
			{
				var fieldIsMined = _field.FieldIsMined;
				var newState = _field.Open(row, column, out var points);

				if (fieldIsMined != _field.FieldIsMined)
					MinesUpdated?.Invoke();

				UpdateGameState(newState);
				CellOpenned?.Invoke(points);
			});
		}

		/// <summary>
		/// Marks the cell manually
		/// </summary>
		public void MarkWithFlag(int row, int column)
		{
			_field.Mark(row, column);
		}

		private void UpdateGameState(GameState state)
		{
			GameState = state;
			GameStateChanged?.Invoke(state);
		}
	}
}
