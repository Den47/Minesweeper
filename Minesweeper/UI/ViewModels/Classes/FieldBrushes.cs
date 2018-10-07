using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Minesweeper.UI.ViewModels.Classes
{
	internal static class FieldBrushes
	{
		public static Brush Brush0 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xF1, 0xF1, 0xF1));

		public static Brush Brush1 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xE1, 0xFF, 0xE0));

		public static Brush Brush2 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xE0, 0xF9, 0xFF));

		public static Brush Brush3 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xF9, 0xFF, 0xB6));

		public static Brush Brush4 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xE0, 0xB6));

		public static Brush Brush5 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xCD, 0xB6));

		public static Brush Brush6 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xB6, 0xB6));

		public static Brush Brush7 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x98, 0x98));

		public static Brush Brush8 { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x88, 0x88));

		public static Brush BrushMined { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0x5D, 0x5D));

		public static Brush BrushClose { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xDE, 0xDE, 0xDE));
	}
}
