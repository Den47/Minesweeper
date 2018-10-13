using Minesweeper.UI.Support;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Minesweeper.UI.ViewModels
{
	public class TileViewModel : PropertyChangedBase
	{
		private int _count;
		private bool _isOpen;
		private bool _isMarked;
		private bool _isMined;

		public TileViewModel(int row, int column)
		{
			Row = row;
			Column = column;
			Cells = new List<TileViewModel>();
		}

		public int Column { get; }

		public int Row { get; }

		public bool IsChecked { get; set; }

		public int Count
		{
			get => _count;
			set
			{
				if (_count != value)
				{
					_count = value;
					NotifyOfPropertyChange(nameof(Count));
				}
			}
		}

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

		public bool IsMined
		{
			get => _isMined;
			set
			{
				if (_isMined != value)
				{
					_isMined = value;
					NotifyOfPropertyChange(nameof(IsMined));
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

		public List<TileViewModel> Cells { get; private set; }

		public Button Button { get; set; }

		public void ResetState()
		{
			IsMined = false;
			IsOpen = false;
			IsMarked = false;
			Count = 0;
			Background = null;
		}

		public void UpdateBindings()
		{
			NotifyOfPropertyChange(nameof(Count));
			NotifyOfPropertyChange(nameof(Content));
			NotifyOfPropertyChange(nameof(Background));
		}

		public override string ToString() => Content;
	}
}
