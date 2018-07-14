using Minesweeper.Classes;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Minesweeper
{
	public sealed partial class MainPage : Page
	{
		private readonly Random _random = new Random();

		private Cell[,] _cells;
		private List<Cell> _cellsList;

		private int _width = 24;
		private int _height = 12;
		private int _minesCount = 40;

		public MainPage()
		{
			this.InitializeComponent();
			GenerateField();
			GenerateCells();
			GenerateMines();
		}

		private void GenerateField()
		{
			field.Children.Clear();
			field.ColumnDefinitions.Clear();
			field.RowDefinitions.Clear();

			for (int i = 0; i < _height; i++)
				field.RowDefinitions.Add(new RowDefinition());
			for (int i = 0; i < _width; i++)
				field.ColumnDefinitions.Add(new ColumnDefinition());
		}

		private void GenerateCells()
		{
			_cells = new Cell[_height, _width];
			_cellsList = new List<Cell>();

			for (int i = 0; i < field.RowDefinitions.Count; i++)
			{
				for (int j = 0; j < field.ColumnDefinitions.Count; j++)
				{
					var cell = new Cell(i, j);
					_cells[i, j] = cell;
					_cellsList.Add(cell);

					var button = new ToggleButton
					{
						Style = CellButtonStyle,
						DataContext = cell
					};

					button.Click += CellButton_Click;

					Grid.SetRow(button, i);
					Grid.SetColumn(button, j);

					field.Children.Add(button);

					cell.Button = button;
				}
			}

			for (int i = 0; i < field.RowDefinitions.Count; i++)
			{
				for (int j = 0; j < field.ColumnDefinitions.Count; j++)
				{
					var rows = field.RowDefinitions.Count;
					var cols = field.ColumnDefinitions.Count;
					var list = new List<Cell>();

					if (i + 1 < rows)
						list.Add(_cells[i + 1, j]);
					if (i - 1 >= 0)
						list.Add(_cells[i - 1, j]);
					if (j + 1 < cols)
						list.Add(_cells[i, j + 1]);
					if (j - 1 >= 0)
						list.Add(_cells[i, j - 1]);

					if (i + 1 < rows && j + 1 < cols)
						list.Add(_cells[i + 1, j + 1]);
					if (i - 1 >= 0 && j - 1 >= 0)
						list.Add(_cells[i - 1, j - 1]);
					if (i + 1 < rows && j - 1 >= 0)
						list.Add(_cells[i + 1, j - 1]);
					if (i - 1 >= 0 && j + 1 < cols)
						list.Add(_cells[i - 1, j + 1]);

					_cells[i, j].Cells.AddRange(list);
				}
			}
		}

		private void GenerateMines()
		{
			var count = 0;
			while (count < _minesCount)
			{
				var i = _random.Next(field.RowDefinitions.Count);
				var j = _random.Next(field.ColumnDefinitions.Count);

				if (_cells[i, j].IsMined)
				{
					continue;
				}
				else
				{
					_cells[i, j].IsMined = true;
					count++;
				}
			}
		}

		private void CellButton_Click(object sender, RoutedEventArgs e)
		{
			var cell = ((FrameworkElement)sender).DataContext as Cell;
			if (cell == null)
				return;

			foreach (var cells in _cellsList)
				cells.IsChecked = false;

			cell.IsOpen = true;

			if (cell.Count == 0)
				Open(cell.Cells);
		}

		private void Open(IEnumerable<Cell> cells)
		{
			foreach (var cell in cells)
			{
				if (cell.IsChecked)
					continue;

				if (cell.IsMined)
					return;

				cell.IsOpen = true;
				cell.IsChecked = true;

				if (cell.Count == 0)
					Open(cell.Cells);
			}
		}
	}
}
