using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TaskManager.Client.Models;

namespace TaskManager.Client.Views.Components;

public partial class TaskControl : UserControl
{
    private SolidColorBrush _originalBorderBrush;
    private SolidColorBrush _originalBackground;
    
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
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            border.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
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
}