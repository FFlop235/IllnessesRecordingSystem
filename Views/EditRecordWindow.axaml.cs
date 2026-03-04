using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IllnessesRecordingSystem.Models;
using IllnessesRecordingSystem.ViewModels;

namespace IllnessesRecordingSystem.Views;

public partial class EditRecordWindow : Window
{
    public EditRecordWindow(IllnessRecordView Record)
    {
        InitializeComponent();
        DataContext = new EditRecordViewModel(Record, Close);
    }
}