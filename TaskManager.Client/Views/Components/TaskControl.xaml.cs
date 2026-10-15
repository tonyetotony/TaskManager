using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Views.Components;

public partial class TaskControl : UserControl
{
    private SolidColorBrush _originalBorderBrush;
    private SolidColorBrush _originalBackground;
    
    public static readonly DependencyProperty IsAdminOrEditorProperty =
        DependencyProperty.Register(nameof(IsAdminOrEditor), typeof(bool), typeof(TaskControl));

    public bool IsAdminOrEditor
    {
        get => (bool)GetValue(IsAdminOrEditorProperty);
        set
        {
            SetValue(IsAdminOrEditorProperty, value);
            UpdateMenuItemVisibility();
        }
    }

    private void UpdateMenuItemVisibility()
    {
        if (EditMenuItem != null)
            EditMenuItem.Visibility = IsAdminOrEditor ? Visibility.Visible : Visibility.Collapsed;
        if (DeleteMenuItem != null)
            DeleteMenuItem.Visibility = IsAdminOrEditor ? Visibility.Visible : Visibility.Collapsed;
    }
    
    public event EventHandler<TaskClient> EditRequested;
    public event EventHandler<TaskClient> DeleteRequested;
    
    public TaskControl(TaskClient task)
    {
        InitializeComponent();
        DataContext = task;
    }

    private void TaskBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is TaskClient taskClient)
        {
            var data = new DataObject("TaskData", taskClient);
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
        }
    }

    private void TaskBorder_MouseEnter(object sender, MouseEventArgs e)
    {
        var border = sender as System.Windows.Controls.Border;
        if (border != null)
        {
            _originalBorderBrush = (SolidColorBrush)border.BorderBrush;
            _originalBackground = (SolidColorBrush)border.Background;
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(6, 150, 215));
            border.Background = new SolidColorBrush(Color.FromRgb(232, 244, 248));
        }
    }

    private void TaskBorder_MouseLeave(object sender, MouseEventArgs e)
    {
        var border = sender as System.Windows.Controls.Border;
        if (border != null && _originalBorderBrush != null)
        {
            border.BorderBrush = _originalBorderBrush;
            border.Background = _originalBackground;
        }
    }
    
    private void EditMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is TaskClient task)
        {
            EditRequested?.Invoke(this, task);
        }
    }
    
    private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is TaskClient task)
        {
            DeleteRequested?.Invoke(this, task);
        }
    }
}