using Minesweeper.UI.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Minesweeper.UI.Views
{
	public sealed partial class GameView
	{
		private List<Button> _cells;
		private List<TileViewModel> _tiles;

		public GameView()
		{
			this.InitializeComponent();
		}

		private void ViewModel_FieldCreated(int width, int height)
		{
			var fieldChanged = false;

			if (field.RowDefinitions.Count != height)
			{
				fieldChanged = true;
				field.RowDefinitions.Clear();
				for (int i = 0; i < height; i++)
					field.RowDefinitions.Add(new RowDefinition());
			}

			if (field.ColumnDefinitions.Count != width)
			{
				fieldChanged = true;
				field.ColumnDefinitions.Clear();
				for (int i = 0; i < width; i++)
					field.ColumnDefinitions.Add(new ColumnDefinition());
			}

			if (fieldChanged)
			{
				field.Children.Clear();

				_cells = new List<Button>();
				_tiles = new List<TileViewModel>();

				for (int i = 0; i < field.RowDefinitions.Count; i++)
				{
					for (int j = 0; j < field.ColumnDefinitions.Count; j++)
					{
						var tile = new TileViewModel(i, j);

						var button = new Button
						{
							DataContext = tile,
							Style = CellButtonStyle
						};

						button.Command = ViewModel.OpenTileCommand;
						button.CommandParameter = button.DataContext as TileViewModel;
						button.RightTapped += CellButton_RightTapped;

						Grid.SetRow(button, i);
						Grid.SetColumn(button, j);

						field.Children.Add(button);

						_cells.Add(button);
						_tiles.Add(tile);
					}
				}
			}
			else
			{
				_tiles.ForEach(x => x.ResetState());

				foreach (var button in _cells)
				{
					button.IsTabStop = true;
					button.Command = ViewModel.OpenTileCommand;
				}
			}

			ViewModel.SetCells(_cells, _tiles);
		}

		private void CellButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			ViewModel.Mark((TileViewModel)((FrameworkElement)sender).DataContext);
		}
	}
}
