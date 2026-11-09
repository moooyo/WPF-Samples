using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPFGallery.Models;

namespace WPFGallery.ViewModels;

public partial class UserDashboardViewModel : ObservableObject
{
    public ObservableCollection<User> Users { get; } = [];
    [ObservableProperty]
    public partial User? SelectedUser { get; set; }
    [ObservableProperty]
    public partial User Draft { get; set; } = new();
    [ObservableProperty, NotifyPropertyChangedFor(nameof(CanSelect))]
    public partial bool IsEditing { get; set; }
    [ObservableProperty]
    public partial string Status { get; set; } = "";
    public bool CanSelect => !IsEditing;
    public bool CanEdit => SelectedUser is not null && !IsEditing;
    public DateTimeOffset DraftJoiningDate
    {
        get => Draft.DateOfJoining;
        set => Draft.DateOfJoining = value;
    }
    partial void OnDraftChanged(User value) => OnPropertyChanged(nameof(DraftJoiningDate));

    public UserDashboardViewModel()
    {
        string[] names = ["John", "Adrianna", "Lucas", "Phoebe", "Winston", "Marissa"];
        string[] surnames = ["Doe", "Cisneros", "Marsh", "Munoz", "Tapia", "Bartlett"];
        string[] images = ["64", "65", "91", "103", "177", "334"];
        for (int index = 0; index < 20; index++)
        {
            Users.Add(new User
            {
                FirstName = names[index % names.Length],
                LastName = surnames[index % surnames.Length],
                Company = index % 2 == 0 ? "Luminary Nexus" : "Horizon Ventures",
                Address = "Seattle, United States",
                Age = 21 + index,
                ImageId = images[index % images.Length],
                DateOfJoining = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).AddDays(index * 30),
                IsNewGraduate = index % 3 == 0
            });
        }
        SelectedUser = Users[0];
    }

    partial void OnSelectedUserChanged(User? value)
    {
        Draft = value?.Copy() ?? new User();
        UpdateCommands();
    }
    partial void OnIsEditingChanged(bool value) => UpdateCommands();

    private void UpdateCommands()
    {
        OnPropertyChanged(nameof(CanEdit));
        AddCommand.NotifyCanExecuteChanged();
        EditCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
        RemoveCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanSelect))]
    private void Add()
    {
        SelectedUser = null;
        Draft = new User { FirstName = "New", LastName = "User" };
        IsEditing = true;
        Status = "Enter the new user's details, then save or cancel.";
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Edit()
    {
        Draft = SelectedUser!.Copy();
        IsEditing = true;
        Status = "Editing user.";
    }

    [RelayCommand(CanExecute = nameof(IsEditing))]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(Draft.FirstName) || string.IsNullOrWhiteSpace(Draft.LastName)
            || !double.IsFinite(Draft.Age) || Draft.Age < 0 || Draft.Age > 130)
        {
            Status = "Enter a first name, a last name, and an age from 0 to 130.";
            return;
        }
        User user = Draft.Copy();
        if (SelectedUser is null)
        {
            Users.Add(user);
        }
        else
        {
            Users[Users.IndexOf(SelectedUser)] = user;
        }
        IsEditing = false;
        SelectedUser = user;
        Status = $"Saved {user.Name}.";
    }

    [RelayCommand(CanExecute = nameof(IsEditing))]
    private void Cancel()
    {
        IsEditing = false;
        Draft = SelectedUser?.Copy() ?? new User();
        Status = "Changes cancelled.";
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Remove()
    {
        User user = SelectedUser!;
        int index = Users.IndexOf(user);
        Users.Remove(user);
        SelectedUser = Users.Count == 0 ? null : Users[Math.Min(index, Users.Count - 1)];
        Status = $"Deleted {user.Name}.";
    }
}
