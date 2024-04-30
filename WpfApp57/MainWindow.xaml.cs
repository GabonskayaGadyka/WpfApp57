using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp57
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TaskDbContext _dbContext;
        private BindingList<Task> _tasks;

        public MainWindow()
        {
            InitializeComponent();

            _dbContext = new TaskDbContext();
            _tasks = new BindingList<Task>(_dbContext.Tasks.ToList());
            DataContext = this;
            TaskListView.ItemsSource = _tasks; 
        }

        private void TaskTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AddTask();
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            AddTask();
        }

        private void AddTask()
        {
            string taskDescription = TaskTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(taskDescription))
            {
                Task task = new Task { Description = taskDescription };
                _dbContext.Tasks.Add(task);
                _dbContext.SaveChanges();
                _tasks.Add(task); 
                TaskTextBox.Clear();
            }
        }

        private void CompletedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var task = checkBox.DataContext as Task;
            task.IsCompleted = checkBox.IsChecked ?? false;
            _dbContext.SaveChanges();
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var task = button.DataContext as Task;

            var editTaskWindow = new Window1(task);
            if (editTaskWindow.ShowDialog() == true)
            {
                _dbContext.Tasks.Attach(task);
                _dbContext.Entry(task).State = EntityState.Modified;

                _dbContext.SaveChanges();
                int index = _tasks.IndexOf(task);
                _tasks.RemoveAt(index);
                _tasks.Insert(index, task);
            }
        }


        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var task = button.DataContext as Task;

            _dbContext.Tasks.Remove(task);
            _dbContext.SaveChanges();
            _tasks.Remove(task);
        }
    }
}