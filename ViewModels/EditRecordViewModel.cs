using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IllnessesRecordingSystem.DB;
using IllnessesRecordingSystem.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace IllnessesRecordingSystem.ViewModels;

public partial class EditRecordViewModel: ViewModelBase
{
    [ObservableProperty]
    IllnessRecordView _selectedIllnessRecord;

    private EmployeeRepository _employeeRepository;
    private IllnessTypeRepository _illnessTypeRepository;
    private IllnessRecordRepository _illnessRecordRepository;
    private DepartmentRepository _departmentRepository;
    
    [ObservableProperty]
    private List<Employee> _employees;
    
    [ObservableProperty]
    private List<IllnessType> _illnessTypes;
    
    [ObservableProperty]
    private Employee? _selectedEmployee;
    
    [ObservableProperty]
    private IllnessType? _selectedIllnessType;
    
    [ObservableProperty]
    private DateTimeOffset _startDate;
    
    [ObservableProperty]
    private DateTimeOffset _endDate;

    [ObservableProperty]
    private string _diagnosisNote;
    
    private Action<object?> close;

    public EditRecordViewModel(IllnessRecordView selectedIllnessRecord, Action<object?> close)
    {
        _illnessRecordRepository = new IllnessRecordRepository();
        _employeeRepository = new EmployeeRepository();
        _departmentRepository = new DepartmentRepository();
        _illnessTypeRepository = new IllnessTypeRepository();

        Employees = _employeeRepository.GetAll();
        IllnessTypes = _illnessTypeRepository.GetAll();
        
        var employeeId = _illnessRecordRepository.GetEmployeeIdByName(selectedIllnessRecord.EmployeeName);
        SelectedEmployee = Employees.FirstOrDefault(e => e.Id == employeeId);
        
        var illnessTypeId = _illnessRecordRepository.GetIllnessTypeIdByName(selectedIllnessRecord.IllnessType);
        SelectedIllnessType = IllnessTypes.FirstOrDefault(it => it.Id == illnessTypeId);
        
        StartDate = selectedIllnessRecord.StartDate;
        EndDate = selectedIllnessRecord.EndDate;
        DiagnosisNote = selectedIllnessRecord.DiagnosisNote ?? string.Empty;
        
        SelectedIllnessRecord = selectedIllnessRecord;
        this.close = close;
    }
    
    [RelayCommand]
    public void Close()
    {
        GetWindow().Close();
    }
    
    Window GetWindow()
    {
        if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows)
            {
                if (window.DataContext == this)
                {
                    return window;
                }
            }
        }

        return null;
    }

    [RelayCommand]
    public async void SaveChanges()
    {
        if (SelectedEmployee == null || string.IsNullOrWhiteSpace(SelectedEmployee.FullName))
        {
            var errorBox = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", "Укажите ФИО сотрудника", ButtonEnum.Ok);
            await errorBox.ShowWindowDialogAsync(GetWindow());
            return;
        }
        
        if (SelectedIllnessType == null || string.IsNullOrWhiteSpace(SelectedIllnessType.Name))
        {
            var errorBox = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", "Укажите тип болезни", ButtonEnum.Ok);
            await errorBox.ShowWindowDialogAsync(GetWindow());
            return;
        }

        if (EndDate < StartDate)
        {
            var errorBox = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", "Дата окончания не может быть раньше даты начала", ButtonEnum.Ok);
            await errorBox.ShowWindowDialogAsync(GetWindow());
            return;
        }
        
        SelectedIllnessRecord.EmployeeName = SelectedEmployee.FullName;
        SelectedIllnessRecord.DepartmentName = _departmentRepository.GetById(SelectedEmployee.DepartmentId).Name;
        SelectedIllnessRecord.IllnessType = SelectedIllnessType.Name;
        SelectedIllnessRecord.StartDate = StartDate.DateTime;
        SelectedIllnessRecord.EndDate = EndDate.DateTime;
        SelectedIllnessRecord.DurationDays = (EndDate.DateTime - StartDate.DateTime).Days;
        SelectedIllnessRecord.DiagnosisNote = DiagnosisNote;
        
        _illnessRecordRepository.Update(SelectedIllnessRecord);
        
        GetWindow().Close();
    }
}