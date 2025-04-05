using System.Windows;

namespace MemoryGame.View
{
    public partial class FrontPage : Window
    {
        public FrontPage()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Memory Game Help:\n\n" +
                "1. Apasa 'Start Game' sa te inregistrezi\n" +
                "2. Selecteaza sau creaza un profil\n" +
                "3. Alege 'Play' sa incepi jocul\n" +
                "4. Potriveste perechi de carti sa castigi\n\n" +
                "Tips:\n" +
                "- Tine minte pozitiile cartiilor \n" +
                "- Ai grija la timp\n" +
                "- Incearca sa gasesti cat mai multe perechi ",
                "Help",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Gaia's Echoes v1.0\n\n" +
                "Developed by [Lupu Daniela]\n\n" +
                "Features:\n" +
                "- Profil pentru user \n" +
                "- Numar de carti custom pentru nivele de dificultate diferite\n" +
                "- Urmareste si salveaza progresul jocului\n\n" +
                "© 2024 Gaia's Echoes",
                "About",
                MessageBoxButton.OK,
                
                MessageBoxImage.Information
            );
        }
    }
}