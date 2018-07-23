using Minesweeper.UI.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Minesweeper.UI.Views
{
	public sealed partial class GameView
	{
		public GameView()
		{
			this.InitializeComponent();
			Loaded += GameView_Loaded;
		}

		public GameViewModel ViewModel => DataContext as GameViewModel;

		private void GameView_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= GameView_Loaded;

			if (ViewModel != null)
			{
				ViewModel.FieldUpdated += ViewModel_FieldUpdated;

				ViewModel.Brush0 = Brush0;
				ViewModel.Brush1 = Brush1;
				ViewModel.Brush2 = Brush2;
				ViewModel.Brush3 = Brush3;
				ViewModel.Brush4 = Brush4;
				ViewModel.Brush5 = Brush5;
				ViewModel.Brush6 = Brush6;
				ViewModel.Brush7 = Brush7;
				ViewModel.Brush8 = Brush8;
				ViewModel.BrushClose = BrushClose;
				ViewModel.BrushMined = BrushMined;
			}
		}

		private void ViewModel_FieldUpdated(int width, int height)
		{
			field.Children.Clear();
			field.ColumnDefinitions.Clear();
			field.RowDefinitions.Clear();

			for (int i = 0; i < height; i++)
				field.RowDefinitions.Add(new RowDefinition());
			for (int i = 0; i < width; i++)
				field.ColumnDefinitions.Add(new ColumnDefinition());

			var _cells = new List<Button>();

			for (int i = 0; i < field.RowDefinitions.Count; i++)
			{
				for (int j = 0; j < field.ColumnDefinitions.Count; j++)
				{
					var button = new Button
					{
						DataContext = new TileViewModel(i, j),
						Style = CellButtonStyle
					};

					button.Command = ViewModel.OpenTileCommand;
					button.CommandParameter = button.DataContext as TileViewModel;
					button.RightTapped += CellButton_RightTapped;

					Grid.SetRow(button, i);
					Grid.SetColumn(button, j);

					field.Children.Add(button);

					_cells.Add(button);
				}
			}

			ViewModel.SetCells(_cells);
		}

		private void CellButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			ViewModel?.Mark(((FrameworkElement)sender).DataContext as TileViewModel);
		}
	}
}
