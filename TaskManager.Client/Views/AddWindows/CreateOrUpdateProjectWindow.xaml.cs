using System.Windows;

namespace TaskManager.Client.Views.AddWindows;

public partial class CreateOrUpdateProjectWindow : Window
{
    public CreateOrUpdateProjectWindow()
    {
        InitializeComponent();
    }
    
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}