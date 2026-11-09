namespace WPFGallery.Models;

public partial class User : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    public partial string? FirstName { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    public partial string? LastName { get; set; }

    [ObservableProperty]
    public partial string? Company { get; set; }

    [ObservableProperty]
    public partial string? Address { get; set; }

    [ObservableProperty]
    public partial bool IsNewGraduate { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageKey))]
    public partial string ImageId { get; set; } = "91";

    [ObservableProperty]
    public partial int Age { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateOfJoiningOffset))]
    public partial DateTime DateOfJoining { get; set; }

    private string _deletedname = string.Empty;
    public string Deletedname
    {
        get => _deletedname;
        set
        {
            _deletedname = value;
            OnPropertyChanged();
        }
    }

    public string Name => $"{FirstName} {LastName}";
    public override string ToString() => Name;
    public string ImageKey => $"p{ImageId}";

    public DateTimeOffset? DateOfJoiningOffset
    {
        get => new DateTimeOffset(DateOfJoining);
        set
        {
            if (value is { } date) DateOfJoining = date.DateTime;
        }
    }

    public User(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public User(User user) : this(user.ImageId, user.FirstName, user.LastName,
        user.Company, user.Address, user.Age, user.DateOfJoining, user.IsNewGraduate)
    { }

    public User(string imageID, string? firstName, string? lastName, string? company,
        string? address, int age, DateTime doj, bool isNewGraduate = false)
    {
        ImageId = imageID;
        FirstName = firstName;
        LastName = lastName;
        Company = company;
        Address = address;
        IsNewGraduate = isNewGraduate;
        Age = age;
        DateOfJoining = doj;
    }
}
