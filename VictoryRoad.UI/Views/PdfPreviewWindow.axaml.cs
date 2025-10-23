using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace VictoryRoad.UI.Views;

public partial class PdfPreviewWindow : Window
{
    private readonly byte[] _pdfBytes;
    private readonly string _suggestedFileName;
    private string? _tempFilePath;

    public PdfPreviewWindow(byte[] pdfBytes, string suggestedFileName)
    {
        InitializeComponent();
        _pdfBytes = pdfBytes;
        _suggestedFileName = suggestedFileName;

        Loaded += OnWindowLoaded;
        Closed += OnWindowClosed;
    }

    private async void OnWindowLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            var webView = this.FindControl<WebViewControl.WebView>("PdfWebView");
            if (webView == null)
            {
                await ShowMessageBox("Error", "WebView control not found");
                return;
            }

            _tempFilePath = Path.Combine(Path.GetTempPath(), $"victory-road-{Guid.NewGuid()}.pdf");
            await File.WriteAllBytesAsync(_tempFilePath, _pdfBytes);

            var fileUri = new Uri(_tempFilePath);
            webView.LoadUrl(fileUri.AbsoluteUri);
        }
        catch (Exception ex)
        {
            await ShowMessageBox("Error", $"Failed to load PDF: {ex.Message}");
        }
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        CleanupTempFile();
    }

    private async void SaveAs_Click(object? sender, RoutedEventArgs e)
    {
        var options = new FilePickerSaveOptions
        {
            Title = "Save Deck List PDF",
            DefaultExtension = "pdf",
            SuggestedFileName = _suggestedFileName,
            FileTypeChoices =
            [
                new FilePickerFileType("PDF Files")
                {
                    Patterns = ["*.pdf"]
                }
            ]
        };

        var file = await StorageProvider.SaveFilePickerAsync(options);

        if (file != null)
        {
            try
            {
                await File.WriteAllBytesAsync(file.Path.LocalPath, _pdfBytes);
                await ShowMessageBox("Success", $"Deck list saved to {file.Name}");
            }
            catch (Exception ex)
            {
                await ShowMessageBox("Error", $"Failed to save PDF: {ex.Message}");
            }
        }
    }

    private async void Print_Click(object? sender, RoutedEventArgs e)
    {
        if (_tempFilePath == null)
            return;

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                PrintOnWindows(_tempFilePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                PrintOnLinux(_tempFilePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                PrintOnMac(_tempFilePath);
            }
        }
        catch (Exception ex)
        {
            await ShowMessageBox("Error", $"Failed to print: {ex.Message}");
        }
    }

    private void PrintOnWindows(string filePath)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true,
            Verb = "print"
        };
        Process.Start(processStartInfo);
    }

    private void PrintOnLinux(string filePath)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "lp",
            Arguments = $"\"{filePath}\"",
            UseShellExecute = false
        };
        Process.Start(processStartInfo);
    }

    private void PrintOnMac(string filePath)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "lpr",
            Arguments = $"\"{filePath}\"",
            UseShellExecute = false
        };
        Process.Start(processStartInfo);
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CleanupTempFile()
    {
        try
        {
            if (_tempFilePath != null && File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }
        catch
        {
        }
    }

    private async Task ShowMessageBox(string title, string message)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        Margin = new Avalonia.Thickness(0, 0, 0, 20)
                    },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Width = 100
                    }
                }
            }
        };

        var button = (messageBox.Content as StackPanel)?.Children.OfType<Button>().First();
        if (button != null)
        {
            button.Click += (_, _) => messageBox.Close();
        }

        await messageBox.ShowDialog(this);
    }
}
