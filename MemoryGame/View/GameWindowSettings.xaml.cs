using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MemoryGame.View
{
    public partial class GameWindowSettings : Window
    {
        public string SelectedCategoryPath { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public int TimerDurationInSeconds { get; private set; }

        public GameWindowSettings()
        {
            InitializeComponent();

            rbStandard.IsChecked = true;

            if (lstCategories.Items.Count > 0)
            {
                lstCategories.SelectedIndex = 0;
            }

            Rows = 4;
            Columns = 4;
            TimerDurationInSeconds = 45;
        }

        private void rbStandard_Checked(object sender, RoutedEventArgs e)
        {
            if (txtRows != null && txtColumns != null)
            {
                txtRows.IsEnabled = false;
                txtColumns.IsEnabled = false;
                txtRows.Text = "4";
                txtColumns.Text = "4";
            }
            rbStandard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#324E4A"));

        }


        private void rbCustom_Checked(object sender, RoutedEventArgs e)
        {
            if (txtRows != null && txtColumns != null)
            {
                txtRows.IsEnabled = true;
                txtColumns.IsEnabled = true;
            }
            rbStandard.Background = new SolidColorBrush(Colors.White);
        }


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (lstCategories.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Selection Required",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (lstCategories.SelectedItem is ListBoxItem selectedItem)
            {
                SelectedCategoryPath = selectedItem.Tag.ToString();
            }

            if (!ValidateDimensions())
            {
                return;
            }

            if (rb45Seconds.IsChecked == true)
                TimerDurationInSeconds = 45;
            else if (rb1Minute.IsChecked == true)
                TimerDurationInSeconds = 60;
            else if (rb2Minutes.IsChecked == true)
                TimerDurationInSeconds = 120;
            else if (rb3Minutes.IsChecked == true)
                TimerDurationInSeconds = 180;

            this.DialogResult = true;
            this.Close();
        }


        private bool ValidateDimensions()
        {
            try
            {
                if (rbCustom.IsChecked == true)
                {
                    if (!int.TryParse(txtRows.Text, out int rows) || !int.TryParse(txtColumns.Text, out int columns))
                    {
                        MessageBox.Show("Please enter valid numbers for rows and columns.",
                                        "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (rows < 2 || columns < 2)
                    {
                        MessageBox.Show("Rows and columns must be at least 2.",
                                        "Invalid Size", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if ((rows * columns) % 2 != 0)
                    {
                        MessageBox.Show("Choose numbers between 2 and 6 and the total number of cards (rows × columns) must be even.",
                                        "Invalid Size", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    Rows = rows;
                    Columns = columns;
                }
                else
                {
                    Rows = 4;
                    Columns = 4;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating dimensions: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}