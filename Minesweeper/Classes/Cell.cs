using Minesweeper.Support;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Minesweeper.Classes
{
	internal class Cell : PropertyChangedBase
	{
		private bool _isOpen;

		public Cell(int row, int column)
		{
			Row = row;
			Column = column;
			Cells = new List<Cell>();
		}

		public int Column { get; }

		public int Row { get; }

		public bool IsMined { get; set; }

		public bool IsChecked { get; set; }

		public bool IsOpen
		{
			get => _isOpen;
			set
			{
				if (_isOpen != value)
				{
					_isOpen = value;
					NotifyOfPropertyChange(nameof(IsOpen));
				}
			}
		}

		public List<Cell> Cells { get; }

		public int Count => IsMined ? int.MaxValue : (Cells?.Count(x => x.IsMined) ?? 0);

		public Button Button { get; set; }

		public override string ToString() => IsMined ? "*" : Count.ToString();
	}
}
