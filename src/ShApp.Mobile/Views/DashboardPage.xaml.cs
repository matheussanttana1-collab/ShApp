using ShApp.Mobile.ViewModels;

namespace ShApp.Mobile.Views;

public partial class DashboardPage : ContentPage
{
	private static readonly string[] HeatColorKeys =
	[
		"ObHeat0",
		"ObHeat1",
		"ObHeat2",
		"ObHeat3",
		"ObHeat4",
	];

	// Cada array representa uma semana (coluna), de Segunda a Domingo (7 linhas).
	// -1 = dia futuro/fora do mês (célula vazia).
	// TODO: heatmap de frequência ainda não é alimentado por dados reais (ver DashboardViewModel).
	private static readonly int[][] FrequencyWeeks =
	[
		[0, 0, 1, 2, 3, 0, 4],
		[2, 0, 1, 3, 4, 0, 4],
		[2, 3, 0, 1, 2, 4, 0],
		[2, 3, 0, 2, 4, 0, 1],
		[2, 3, -1, -1, -1, -1, -1],
	];

	private const int TodayWeek = 2;
	private const int TodayDay = 5;

	private static readonly string[] DayLabels = ["Seg", "Qua", "Sex", "Dom"];
	private static readonly int[] DayLabelRows = [0, 2, 4, 6];

	private readonly DashboardViewModel _viewModel;

	public DashboardPage(DashboardViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		BuildDayLabels();
		BuildHeatmap();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_ = _viewModel.LoadAsync();
	}

	private void BuildDayLabels()
	{
		for (var i = 0; i < DayLabels.Length; i++)
		{
			var label = new Label
			{
				Text = DayLabels[i],
				FontFamily = "JetBrainsMonoRegular",
				FontSize = 9,
				TextColor = (Color)Application.Current!.Resources["ObOnSurfaceSubtle"],
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.End,
			};
			Grid.SetRow(label, DayLabelRows[i]);
			DayLabelsGrid.Children.Add(label);
		}
	}

	private void BuildHeatmap()
	{
		const double cellSize = 13;

		for (var i = 0; i < 7; i++)
			HeatmapGrid.RowDefinitions.Add(new RowDefinition(cellSize));

		for (var week = 0; week < FrequencyWeeks.Length; week++)
		{
			HeatmapGrid.ColumnDefinitions.Add(new ColumnDefinition(cellSize));

			for (var day = 0; day < 7; day++)
			{
				var level = FrequencyWeeks[week][day];
				var isToday = week == TodayWeek && day == TodayDay;

				var cell = new Border
				{
					WidthRequest = cellSize,
					HeightRequest = cellSize,
					StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 3 },
					StrokeThickness = isToday ? 1.5 : 0,
					Stroke = isToday ? (Color)Application.Current!.Resources["ObOnSurface"] : Colors.Transparent,
					Background = GetHeatBrush(level),
				};

				Grid.SetRow(cell, day);
				Grid.SetColumn(cell, week);
				HeatmapGrid.Children.Add(cell);
			}
		}
	}

	private static Brush GetHeatBrush(int level)
	{
		var key = level < 0 ? "ObHeatEmpty" : HeatColorKeys[level];
		var color = (Color)Application.Current!.Resources[key];
		return new SolidColorBrush(color);
	}

	private async void OnStartWorkoutClicked(object? sender, EventArgs e)
	{
		if (sender is Button button)
		{
			button.Scale = 0.98;
			await button.ScaleToAsync(1, 90, Easing.CubicOut);
		}

		if (Shell.Current is not null)
			await Shell.Current.GoToAsync("sessao-treino");
	}

	private async void OnPerfilIconTapped(object? sender, TappedEventArgs e)
	{
		if (Shell.Current is not null)
			await Shell.Current.GoToAsync("//perfil-page");
	}
}
