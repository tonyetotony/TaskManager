using System.Windows;

namespace TaskManager.Client.Views.AddWindows;

public partial class CreateOrUpdateDeskWindow : Window
{
    public CreateOrUpdateDeskWindow()
    {
        InitializeComponent();
    }
    
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}