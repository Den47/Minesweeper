using Minesweeper.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

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

		private bool _generated;

		public MainPage()
		{
			this.InitializeComponent();

			Init();
		}

		private void Init()
		{
			_generated = false;

			GenerateField();
			GenerateCells();
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

					var button = new Button
					{
						DataContext = cell,
						Style = CellButtonStyle
					};

					button.Click += CellButton_Click;
					button.RightTapped += CellButton_RightTapped;

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

		private void GenerateMines(Cell tappedCell)
		{
			var count = 0;
			while (count < _minesCount)
			{
				var i = _random.Next(field.RowDefinitions.Count);
				var j = _random.Next(field.ColumnDefinitions.Count);

				if (tappedCell != null && _minesCount > 0 && _cellsList.Count > 1 &&
					tappedCell.Row == i && tappedCell.Column == j)
					continue;

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

			SetCellBackgrounds();

			_generated = true;
		}

		private void SetCellBackgrounds()
		{
			foreach (var item in _cellsList)
			{
				switch (item.Count)
				{
					case 0:
						item.Background = new SolidColorBrush(Colors.WhiteSmoke);
						break;
					default:
						item.Background = new SolidColorBrush(Colors.PaleGreen);
						break;
				}

				if (item.IsMined)
					item.Background = new SolidColorBrush(Colors.PaleVioletRed);

				item.UpdateBindings();
			}
		}

		private void CellButton_Click(object sender, RoutedEventArgs e)
		{
			var cell = ((FrameworkElement)sender).DataContext as Cell;
			if (cell == null)
				return;

			if (!_generated)
				GenerateMines(cell);

			if (cell.IsMarked)
				return;

			foreach (var cells in _cellsList)
				cells.IsChecked = false;

			if (cell.IsOpen)
			{
				var notMarkedCells = cell.Cells.Where(x => !x.IsMarked);
				if (notMarkedCells.Count() == cell.Cells.Count - cell.Count)
					Open(notMarkedCells);
			}
			else
			{
				cell.IsOpen = true;
				cell.IsChecked = true;

				if (cell.Count == 0)
					Open(cell.Cells);
			}
		}

		private void CellButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var cell = ((FrameworkElement)sender).DataContext as Cell;
			if (cell == null)
				return;

			if (cell.IsOpen)
				return;

			cell.IsMarked = !cell.IsMarked;
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
