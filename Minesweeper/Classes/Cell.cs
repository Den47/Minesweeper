using Minesweeper.Support;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Minesweeper.Classes
{
	internal class Cell : PropertyChangedBase
	{
		private bool _isOpen;
		private bool _isMarked;

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
					NotifyOfPropertyChange(nameof(IsMarkVisible));
				}
			}
		}

		public bool IsMarked
		{
			get => _isMarked;
			set
			{
				if (_isMarked != value)
				{
					_isMarked = value;
					NotifyOfPropertyChange(nameof(IsMarked));
					NotifyOfPropertyChange(nameof(IsMarkVisible));
				}
			}
		}

		public string Content
		{
			get
			{
				if (IsMined)
					return "Ж";

				var count = Count;
				return count == 0 ? string.Empty : count.ToString();
			}
		}

		public bool IsMarkVisible => IsMarked && !IsOpen;

		public Brush Background { get; set; }

		public List<Cell> Cells { get; }

		public int Count => IsMined ? int.MaxValue : (Cells?.Count(x => x.IsMined) ?? 0);

		public Button Button { get; set; }

		public void UpdateBindings()
		{
			NotifyOfPropertyChange(nameof(Count));
			NotifyOfPropertyChange(nameof(Content));
			NotifyOfPropertyChange(nameof(Background));
		}

		public override string ToString() => Content;
	}
}
