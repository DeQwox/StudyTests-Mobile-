using System.Linq;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using OnlineTestingClient.Models;
using OnlineTestingClient.Services;

namespace OnlineTestingClient.ViewModels;

public partial class StatisticsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    public ISeries[] Series { get; set; } = new ISeries[0];
    public Axis[] XAxes { get; set; } = new[] { new Axis { Name = "Тести" } };
    public Axis[] YAxes { get; set; } = new[] { new Axis { Name = "Бали" } };

    public StatisticsViewModel(MainWindowViewModel main)
    {
        _main = main;
    }

    public async Task LoadStatsAsync()
    {
        _main.IsLoading = true;
        var api = new ApiService(AppState.Token);
        var data = await api.GetStatisticsAsync();

        Series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = data.Select(x => x.Score).ToArray(),
                Name = "Середній бал"
            }
        };

        XAxes[0].Labels = data.Select(x => x.TestName ?? $"Test {x.TestId}").ToArray();
        _main.IsLoading = false;
    }
}