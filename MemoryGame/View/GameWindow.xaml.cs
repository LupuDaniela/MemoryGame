using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;
using System.Xml.Serialization;
using MemoryGame.Model;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace MemoryGame.View
{
    public partial class GameWindow : Window
    {
        private List<Card> cards;
        private Card firstCard;
        private Card secondCard;
        private bool isProcessing = false;
        private int moves = 0;
        private int pairsFound = 0;
        private int totalPairs;
        private DispatcherTimer gameTimer;
        private TimeSpan elapsedTime;
        private string selectedCategoryPath;
        private int rows;
        private int columns;
        private int timerDurationInSeconds;

        private DateTime gameStartTime;
        private User currentPlayer; 

        public int BoardRows
        {
            get { return (int)GetValue(BoardRowsProperty); }
            set { SetValue(BoardRowsProperty, value); }
        }
        public static readonly DependencyProperty BoardRowsProperty =
            DependencyProperty.Register("BoardRows", typeof(int), typeof(GameWindow), new PropertyMetadata(4));

        public int BoardColumns
        {
            get { return (int)GetValue(BoardColumnsProperty); }
            set { SetValue(BoardColumnsProperty, value); }
        }
        public static readonly DependencyProperty BoardColumnsProperty =
            DependencyProperty.Register("BoardColumns", typeof(int), typeof(GameWindow), new PropertyMetadata(4));

        public GameWindow(string categoryPath, int rows, int columns, int timerDurationInSeconds, User currentUser)
        {
            InitializeComponent();

            this.selectedCategoryPath = categoryPath;
            this.rows = rows;
            this.columns = columns;
            this.timerDurationInSeconds = timerDurationInSeconds;
            this.currentPlayer = currentUser; 

            BoardRows = rows;
            BoardColumns = columns;

            totalPairs = (rows * columns) / 2;
            InitializeTimer();
            InitializeGame();
        }

        public GameWindow(GameState savedState, User currentUser)
        {
            InitializeComponent();

            if (savedState == null)
            {
                MessageBox.Show("Eroare: Obiectul GameState (savedState) este null! Nu se poate încărca jocul.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (currentUser == null)
            {
                MessageBox.Show("Eroare: Utilizatorul curent (currentUser) este null! Nu se poate încărca jocul.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.selectedCategoryPath = savedState.CategoryPath;
            this.rows = savedState.Rows;
            this.columns = savedState.Columns;
            this.timerDurationInSeconds = savedState.TimerDurationInSeconds;
            this.currentPlayer = currentUser;  
            BoardRows = rows;
            BoardColumns = columns;

            totalPairs = (rows * columns) / 2;
            InitializeTimer();
            LoadSavedGame(savedState);
        }

        private void InitializeTimer()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += Timer_Tick;
            elapsedTime = TimeSpan.FromSeconds(timerDurationInSeconds);
            UpdateTimeDisplay();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimeDisplay();
            if (elapsedTime.TotalSeconds <= 0)
            {
                GameOver();
            }
        }

        private void UpdateTimeDisplay()
        {
            txtTime.Text = elapsedTime.ToString(@"mm\:ss");
        }

        private void InitializeGame()
        {
            gameStartTime = DateTime.Now; 

            firstCard = null;
            secondCard = null;
            isProcessing = false;
            moves = 0;
            pairsFound = 0;
            elapsedTime = TimeSpan.FromSeconds(timerDurationInSeconds);
            UpdateTimeDisplay();
            txtMoves.Text = "0";

            var imagePaths = LoadImagePaths();
            cards = CreateCardPairs(imagePaths);
            ShuffleCards(cards);
            gameBoard.ItemsSource = cards;
            foreach (var card in cards)
            {
                card.IsFlipped = true;
            }

            DispatcherTimer revealTimer = new DispatcherTimer();
            revealTimer.Interval = TimeSpan.FromSeconds(1);
            revealTimer.Tick += (s, e) =>
            {
                revealTimer.Stop();
                foreach (var card in cards)
                {
                    card.IsFlipped = false;
                }
                gameTimer.Start();
            };
            revealTimer.Start();
        }

        private List<string> LoadImagePaths()
        {
            string imageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", selectedCategoryPath);
            if (!Directory.Exists(imageFolder))
            {
                MessageBox.Show($"Folderul nu a fost găsit: {imageFolder}");
                return new List<string>();
            }
            string[] files = Directory.GetFiles(imageFolder, "*.png");
            if (files.Length < totalPairs)
            {
                MessageBox.Show("Număr insuficient de imagini în categoria selectată.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<string>();
            }
            return files.Take(totalPairs)
                        .Select(path => new Uri(path).AbsoluteUri)
                        .ToList();
        }

        private List<Card> CreateCardPairs(List<string> imagePaths)
        {
            List<Card> cardList = new List<Card>();
            var selectedImages = imagePaths.Take(totalPairs).ToList();
            int index = 0;
            foreach (var imagePath in selectedImages)
            {
                cardList.Add(new Card { ImagePath = imagePath, Index = index++ });
                cardList.Add(new Card { ImagePath = imagePath, Index = index++ });
            }
            return cardList;
        }

        private void ShuffleCards(List<Card> cardList)
        {
            Random random = new Random();
            for (int i = cardList.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = cardList[i];
                cardList[i] = cardList[j];
                cardList[j] = temp;
            }
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessing)
                return;

            Button button = sender as Button;
            Card card = button?.DataContext as Card;
            if (card == null || card.IsMatched || card.IsFlipped)
                return;

            card.IsFlipped = true;

            if (firstCard == null)
                firstCard = card;
            else if (secondCard == null)
            {
                secondCard = card;
                moves++;
                txtMoves.Text = moves.ToString();
                isProcessing = true;
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                if (firstCard.ImagePath == secondCard.ImagePath)
                {
                    firstCard.IsMatched = true;
                    secondCard.IsMatched = true;
                    pairsFound++;
                    if (pairsFound == totalPairs)
                    {
                        GameCompleted();
                    }
                }
                else
                {
                    firstCard.IsFlipped = false;
                    secondCard.IsFlipped = false;
                }
                firstCard = null;
                secondCard = null;
                isProcessing = false;
            };
            timer.Start();
        }

        private void LoadSavedGame(GameState savedState)
        {
            gameStartTime = DateTime.Now; 

            firstCard = null;
            secondCard = null;
            isProcessing = false;
            moves = savedState.Moves;
            txtMoves.Text = moves.ToString();

            elapsedTime = savedState.ElapsedTime;
            UpdateTimeDisplay();

            if (savedState.Cards == null)
            {
                MessageBox.Show("Eroare: Jocul salvat este corupt (lista de carduri lipsește).",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            cards = savedState.Cards;
            gameBoard.ItemsSource = cards;

            foreach (var card in cards)
            {
                if (!card.IsMatched)
                {
                    card.IsFlipped = false;
                }
            }

            pairsFound = cards.Count(c => c.IsMatched) / 2;
            gameTimer.Start();
        }

        private void GameCompleted()
        {
            gameTimer.Stop();
            MessageBox.Show($"Congratulations! You completed the game!\nTime: {txtTime.Text}\nMoves: {moves}",
                "Game Completed", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdatePlayerStatistics(true);
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show("Time's up! Game Over.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
            foreach (var card in cards)
            {
                if (!card.IsMatched)
                    card.IsFlipped = true;
            }
            isProcessing = true;

            UpdatePlayerStatistics(false);
        }

        public void NewGame_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            InitializeGame();
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();

            string customName = Interaction.InputBox("Introdu un nume pentru salvarea jocului:", "Salvare Joc", "SalvareJoc");

            if (string.IsNullOrWhiteSpace(customName))
            {
                customName = "SalvareJoc";
            }

            string playerName = currentPlayer != null ? currentPlayer.Username : "Player";

            GameState state = new GameState
            {
                CategoryPath = selectedCategoryPath,
                Rows = rows,
                Columns = columns,
                Cards = cards,
                ElapsedTime = elapsedTime,
                TimeSpent = DateTime.Now - gameStartTime,
                Moves = moves,
                TimerDurationInSeconds = timerDurationInSeconds,
                PlayerName = playerName
            };

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            string savesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            if (!System.IO.Directory.Exists(savesFolder))
            {
                System.IO.Directory.CreateDirectory(savesFolder);
            }

            string savePath = System.IO.Path.Combine(savesFolder, $"game_{playerName}_{customName}_{timestamp}.xml");

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameState));
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    serializer.Serialize(stream, state);
                }
                MessageBox.Show("Game saved successfully!", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void UpdatePlayerStatistics(bool won)
        {
            try
            {
                if (currentPlayer == null)
                {
                    MessageBox.Show("Eroare: Jucătorul curent nu este setat!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string statsPath = "statistics.json";
                List<PlayerStatistic> statistics = new List<PlayerStatistic>();

                if (File.Exists(statsPath))
                {
                    string json = File.ReadAllText(statsPath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        statistics = JsonConvert.DeserializeObject<List<PlayerStatistic>>(json) ?? new List<PlayerStatistic>();
                    }
                }

                var playerStat = statistics.FirstOrDefault(s => s.Username == currentPlayer.Username);
                if (playerStat == null)
                {
                    playerStat = new PlayerStatistic
                    {
                        Username = currentPlayer.Username,
                        GamesPlayed = 0,
                        GamesWon = 0
                    };
                    statistics.Add(playerStat);
                }

                playerStat.GamesPlayed++;
                if (won)
                    playerStat.GamesWon++;

                string updatedJson = JsonConvert.SerializeObject(statistics, Formatting.Indented);
                File.WriteAllText(statsPath, updatedJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
