using CommunityToolkit.Mvvm.ComponentModel;

namespace WPFGallery.Models;

public partial class User : ObservableObject
{
    [ObservableProperty, NotifyPropertyChangedFor(nameof(Name))]
    public partial string FirstName { get; set; } = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(Name))]
    public partial string LastName { get; set; } = "";
    [ObservableProperty]
    public partial string Company { get; set; } = "";
    [ObservableProperty]
    public partial string Address { get; set; } = "";
    [ObservableProperty]
    public partial double Age { get; set; } = 25;
    [ObservableProperty]
    public partial DateTimeOffset DateOfJoining { get; set; } = DateTimeOffset.Now.Date;
    [ObservableProperty]
    public partial bool IsNewGraduate { get; set; }
    public string ImageId { get; init; } = "91";
    public string Name => $"{FirstName} {LastName}".Trim();
    public string ImageSource => $"ms-appx:///Assets/Gallery/UserDashboard/{ImageId}-100x100.jpg";

    public User Copy() => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Company = Company,
        Address = Address,
        Age = Age,
        DateOfJoining = DateOfJoining,
        IsNewGraduate = IsNewGraduate,
        ImageId = ImageId
    };
}
