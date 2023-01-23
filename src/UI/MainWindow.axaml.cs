using Avalonia.Controls;

namespace Zenki.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new Data();
    }
}