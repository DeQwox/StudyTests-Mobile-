using Avalonia.Controls;
using Avalonia.Controls.Templates;
using OnlineTestingClient.ViewModels;
using System;
using Avalonia.Media;

namespace OnlineTestingClient;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = "Data is null" };

        var vmType = data.GetType();

        var viewTypeName = vmType.FullName!
            .Replace(".ViewModels.", ".Views.")
            .Replace("ViewModel", "View");

        var viewType = vmType.Assembly.GetType(viewTypeName);

        if (viewType != null)
        {
            try
            {
                var view = (Control)Activator.CreateInstance(viewType)!;

                // ЦЕ ВАЖЛИВО! Без цього DataContext не встановлюється → команди не працюють
                view.DataContext = data;

                return view;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null) msg += "\nInner: " + ex.InnerException.Message;

                return new TextBlock
                {
                    Text = $"Create Error:\n{msg}",
                    Foreground = Brushes.Yellow
                };
            }
        }

        return new TextBlock
        {
            Text = $"Not Found: {viewTypeName}",
            Foreground = Brushes.Red
        };
    }

    public bool Match(object? data) => data is ViewModelBase;
}
