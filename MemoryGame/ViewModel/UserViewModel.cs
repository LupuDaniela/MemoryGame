using MemoryGame.Model;
using MemoryGame.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace MemoryGame.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private const string DataFilePath = "users.json";
        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged(nameof(SelectedUser));

                    if (value != null)
                    {
                        IsImageSelectionVisible = false;
                        ShowPhoto = true;
                    }
                    else
                    {
                        ShowPhoto = false;
                    }

                    OnPropertyChanged(nameof(IsDeleteUserEnabled));
                    OnPropertyChanged(nameof(IsPlayEnabled));
                }
            }
        }

        private string _newUserImagePath;
        public string NewUserImagePath
        {
            get => _newUserImagePath;
            set
            {
                if (_newUserImagePath != value)
                {
                    _newUserImagePath = value;
                    OnPropertyChanged(nameof(NewUserImagePath));
                }
            }
        }

        private string _newUsername;
        public string NewUsername
        {
            get => _newUsername;
            set
            {
                if (_newUsername != value)
                {
                    _newUsername = value;
                    OnPropertyChanged(nameof(NewUsername));
                }
            }
        }

        public ObservableCollection<string> AvailableImages { get; } = new ObservableCollection<string>
        {
            "/Images/Players/Aphrodite-removebg-preview.png",
            "/Images/Players/Apollo-removebg-preview.png",
            "/Images/Players/Ares-removebg-preview.png",
            "/Images/Players/Artemis-removebg-preview.png",
            "/Images/Players/Demeter-removebg-preview.png",
            "/Images/Players/Hades-removebg-preview.png",
            "/Images/Players/Hera-removebg-preview.png",
            "/Images/Players/Zeus-removebg-preview.png"
        };

        private int _currentImageIndex;
        public int CurrentImageIndex
        {
            get => _currentImageIndex;
            set
            {
                if (_currentImageIndex != value)
                {
                    _currentImageIndex = value;
                    OnPropertyChanged(nameof(CurrentImageIndex));
                    if (AvailableImages.Count > 0)
                        NewUserImagePath = AvailableImages[_currentImageIndex];
                }
            }
        }

        private bool _isImageSelectionVisible;
        public bool IsImageSelectionVisible
        {
            get => _isImageSelectionVisible;
            set
            {
                if (_isImageSelectionVisible != value)
                {
                    _isImageSelectionVisible = value;
                    OnPropertyChanged(nameof(IsImageSelectionVisible));
                }
            }
        }

        private bool _showPhoto;
        public bool ShowPhoto
        {
            get => _showPhoto;
            set
            {
                if (_showPhoto != value)
                {
                    _showPhoto = value;
                    OnPropertyChanged(nameof(ShowPhoto));
                }
            }
        }

        public bool IsDeleteUserEnabled => SelectedUser != null;
        public bool IsPlayEnabled => SelectedUser != null;

        public ICommand ToggleNewUserPanelCommand { get; }
        public ICommand ConfirmAddUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand PreviousUserCommand { get; }
        public ICommand NextUserCommand { get; }

        public UserViewModel()
        {
            ToggleNewUserPanelCommand = new RelayCommand(ToggleNewUserPanel);
            ConfirmAddUserCommand = new RelayCommand(AddUser, CanAddUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, () => IsDeleteUserEnabled);
            NextImageCommand = new RelayCommand(NextImage, () => AvailableImages.Count > 0);
            PreviousImageCommand = new RelayCommand(PreviousImage, () => AvailableImages.Count > 0);

            CurrentImageIndex = 0;
            IsImageSelectionVisible = false;
            _newUsername = string.Empty;
            _newUserImagePath = AvailableImages[0];
            ShowPhoto = false;

            LoadUsers();
        }

        private void ToggleNewUserPanel()
        {
            SelectedUser = null;
            NewUsername = string.Empty;
            CurrentImageIndex = 0;
            IsImageSelectionVisible = !IsImageSelectionVisible;

            if (IsImageSelectionVisible)
                ShowPhoto = false;
        }

        private void AddUser()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewUserImagePath))
                return;

            var newUser = new User
            {
                Username = NewUsername,
                ProfileImagePath = NewUserImagePath
            };

            Users.Add(newUser);

            NewUsername = string.Empty;
            CurrentImageIndex = 0;
            IsImageSelectionVisible = false;
        }

        private bool CanAddUser()
        {
            return !string.IsNullOrWhiteSpace(NewUsername) &&
                   !string.IsNullOrWhiteSpace(NewUserImagePath);
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                SelectedUser = null;
            }
        }


        private void NextImage()
        {
            if (AvailableImages.Count == 0)
                return;
            CurrentImageIndex = (CurrentImageIndex + 1) % AvailableImages.Count;
        }

        private void PreviousImage()
        {
            if (AvailableImages.Count == 0)
                return;
            CurrentImageIndex = (CurrentImageIndex - 1 + AvailableImages.Count) % AvailableImages.Count;
        }


        private bool CanNavigateUsers() => Users.Count > 1;

        public void SaveUsers()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(Users, options);
                File.WriteAllText(DataFilePath, json);
            }
            catch { }
        }

        public void LoadUsers()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    var json = File.ReadAllText(DataFilePath);
                    var loadedUsers = JsonSerializer.Deserialize<ObservableCollection<User>>(json);
                    if (loadedUsers != null)
                    {
                        Users.Clear();
                        foreach (var user in loadedUsers)
                            Users.Add(user);
                    }
                }
            }
            catch { }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
