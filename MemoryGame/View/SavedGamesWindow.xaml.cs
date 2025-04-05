using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using MemoryGame.Model;

namespace MemoryGame.View
{
    public partial class SavedGamesWindow : Window
    {
        private string[] savedGames;

        public SavedGamesWindow(string[] savedGames)
        {
            InitializeComponent();
            this.savedGames = savedGames;
            listBoxSavedGames.ItemsSource = savedGames.Select(Path.GetFileName);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSavedGames.SelectedItem != null)
            {
                string selectedFileName = listBoxSavedGames.SelectedItem.ToString();
                string selectedFile = savedGames.FirstOrDefault(f => Path.GetFileName(f) == selectedFileName);
                if (selectedFile != null)
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(GameState));
                        using (FileStream stream = new FileStream(selectedFile, FileMode.Open))
                        {
                            GameState savedState = (GameState)serializer.Deserialize(stream);
                            if (savedState == null || savedState.Cards == null)
                            {
                                MessageBox.Show("The saved game is corrupted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            User currentUser = new User { Username = savedState.PlayerName };
                            GameWindow gameWindow = new GameWindow(savedState, currentUser);
                            gameWindow.Show();
                        }
                        this.Close();
                        Application.Current.MainWindow.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selectează o salvare din listă.", "Avertisment", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
