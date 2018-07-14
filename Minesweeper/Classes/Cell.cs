using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls.Primitives;

namespace Minesweeper.Classes
{
	public class Cell
	{
		private bool _isOpen;

		public Cell(int column, int row)
		{
			Column = column;
			Row = row;
			Cells = new List<Cell>();
		}

		public int Column { get; }

		public int Row { get; }

		public bool IsMined { get; set; }

		public bool IsOpen
		{
			get => _isOpen;
			set
			{
				if (_isOpen != value)
				{
					_isOpen = value;
					if (value && Button != null)
						Button.IsChecked = true;
				}
			}
		}

		public bool IsChecked { get; set; }

		public List<Cell> Cells { get; }

		public int Count => IsMined ? int.MaxValue : (Cells?.Count(x => x.IsMined) ?? 0);

		public ToggleButton Button { get; set; }

		public override string ToString() => IsMined ? "*" : Count.ToString();
	}
}
