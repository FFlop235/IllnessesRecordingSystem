using Avalonia.Controls;
using Avalonia.Input;
using IllnessesRecordingSystem.ViewModels;

namespace IllnessesRecordingSystem.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void EditRecordDoubleTap(object? sender, TappedEventArgs e)
    {
        (DataContext as MainWindowViewModel).EditRecord();
    }
}