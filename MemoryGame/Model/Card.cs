using System.ComponentModel;

public class Card : INotifyPropertyChanged
{
    private bool isFlipped;
    private bool isMatched;

    public string ImagePath { get; set; }
    public int Index { get; set; }

    public bool IsFlipped
    {
        get => isFlipped;
        set
        {
            if (isFlipped != value)
            {
                isFlipped = value;
                OnPropertyChanged(nameof(IsFlipped));
            }
        }
    }

    public bool IsMatched
    {
        get => isMatched;
        set
        {
            if (isMatched != value)
            {
                isMatched = value;
                OnPropertyChanged(nameof(IsMatched));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
