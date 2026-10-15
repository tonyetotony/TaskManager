using System.Windows;

namespace TaskManager.Client.Views.AddWindows;

public partial class CreateOrUpdateTaskWindow : Window
{
    public CreateOrUpdateTaskWindow()
    {
        InitializeComponent();
    }
    
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}