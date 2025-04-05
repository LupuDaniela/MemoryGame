using MemoryGame.ViewModel;
using MemoryGame.Services;
using MemoryGame.View;
using System.Windows;
using MemoryGame.Model;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace MemoryGame
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (DataContext is UserViewModel vm)
            {
                vm.SaveUsers();
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new GameWindowSettings();
            bool? result = settingsDialog.ShowDialog();
            if (result == true)
            {
                string categoryImage = settingsDialog.SelectedCategoryPath;
                int rows = settingsDialog.Rows;
                int columns = settingsDialog.Columns;
                int timerDuration = settingsDialog.TimerDurationInSeconds;

                if (DataContext is UserViewModel vm && vm.SelectedUser != null)
                {
                    User currentUser = vm.SelectedUser;
                    GameWindow gameWindow = new GameWindow(categoryImage, rows, columns, timerDuration, currentUser);
                    gameWindow.Show();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please select a user profile first.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            string currentUser = "";
            if (DataContext is UserViewModel vm && vm.SelectedUser != null)
            {
                currentUser = vm.SelectedUser.Username;
            }
            if (string.IsNullOrEmpty(currentUser))
            {
                MessageBox.Show("Please select a user profile first.", "Load Game", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string savesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            if (!System.IO.Directory.Exists(savesFolder))
            {
                MessageBox.Show("Nu există jocuri salvate pentru acest jucător.", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string searchPattern = $"game_{currentUser}_*.xml";
            string[] savedGames = Directory.GetFiles(savesFolder, searchPattern);

            if (savedGames.Length == 0)
            {
                MessageBox.Show("Nu există jocuri salvate pentru acest jucător.", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SavedGamesWindow savedGamesWindow = new SavedGamesWindow(savedGames);
            savedGamesWindow.ShowDialog();
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statsWindow = new StatisticsWindow();
            statsWindow.ShowDialog();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            FrontPage frontPage = new FrontPage();
            frontPage.Show();
            this.Close();
        }

    }
}
