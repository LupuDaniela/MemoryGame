using System.IO;
using System.Windows;
using Newtonsoft.Json;
using MemoryGame.Model;


namespace MemoryGame.View
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            string statsPath = "statistics.json";
            if (File.Exists(statsPath))
            {
                string json = File.ReadAllText(statsPath);
                List<PlayerStatistic> stats = JsonConvert.DeserializeObject<List<PlayerStatistic>>(json);

                stats = stats.Where(s => s.GamesPlayed > 0).ToList();

                dataGridStats.ItemsSource = stats;
            }
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
