using System.Windows;

namespace TaskManager.Client.Views.AddWindows;

public partial class CreateOrUpdateUserWindow : Window
{
    public CreateOrUpdateUserWindow()
    {
        InitializeComponent();
    }
    
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}