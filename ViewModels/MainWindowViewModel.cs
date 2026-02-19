using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IlnessesRecordingSystem.DB;
using IlnessesRecordingSystem.Models;

namespace IlnessesRecordingSystem.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<IllnessRecordViem> _illnessRecords = new();

    [ObservableProperty] private int _currentPageSize;
    [ObservableProperty] private List<int> pageSizes;
    [ObservableProperty] private string _pageInfo;
    
    private int _currentPage = 1;
    private int _totalPages;

    public MainWindowViewModel()
    {
        PageSizes = new List<int>([5, 10, 20]);
        CurrentPageSize = PageSizes.First();
    }

    partial void OnCurrentPageSizeChanged(int value)
    {
        CalculatePages();
    }

    void CalculatePages()
    {
        using var db = new IllnessRecordRepository();
        var rowsCount = db.GetRowsCount();
        _totalPages = (int)Math.Ceiling((double)rowsCount / CurrentPageSize);
        ShowFirstPage();
    }

    void ShowPage(int pageIndex)
    {
        _currentPage = pageIndex;
        using var db = new IllnessRecordRepository();
        IllnessRecords.Clear();
        var rows = db.GetPage(pageIndex, CurrentPageSize);
        rows.ForEach(i => IllnessRecords.Add(i));
        PageInfo = $"Страница {_currentPage} из {_totalPages}";
    }

    [RelayCommand]
    private void ShowFirstPage()
    {
        ShowPage(1);
    }

    [RelayCommand]
    private void ShowLastPage()
    {
        ShowPage(_totalPages);
    }

    [RelayCommand]
    private void ShowNextPage()
    {
        if (_currentPage < _totalPages)
            ShowPage(_currentPage + 1);
    }
    
    [RelayCommand]
    private void ShowPreviousPage()
    {
        if (_currentPage > 1)
        {
            ShowPage(_currentPage - 1);
        }
    }
}