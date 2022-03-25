using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeLine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isWhiteOnRed;
        public DatePicker datePicker;
        TextBox eventDesc;
        TextBox eventChar;
        TextBox notBox;
        StackPanel timeline;

        DateTime date = DateTime.Today;
        public MainWindow()
        {
            InitializeComponent();
            ManageFile.GetInstance().mainWindow = this;
            ManageFile.GetInstance().Load();
            TextReader.GetInstance().mainWindow = this;
            datePicker = (DatePicker)FindName("Date");
            eventDesc = (TextBox)FindName("EventDesc");
            eventChar = (TextBox)FindName("Character");
            notBox = (TextBox)FindName("NotificationBox");
            timeline = (StackPanel)FindName("TimeLineStackPanel");
            if (timeline.Children.Count > 0)
            {
                string date = timeline.Children.OfType<StackPanel>().FirstOrDefault().Name.Substring(9).Replace('_', '-');
                this.date = DateTime.Parse(date);
            }
            datePicker.SelectedDate = this.date;
            datePicker.DisplayDate = this.date;
        }
        public void UpdateNotificiationWindow(string msg)
        {
            notBox.Text = msg;
        }
        public void AddItemToTimeLine(Event e)
        {
            StackPanel timeline = (StackPanel)FindName("TimeLineStackPanel");
            StackPanel eventContainer = new StackPanel();
            TextBlock dateBox = new TextBlock();
            TextBlock descBox = new TextBlock();
            Button deleteButton = new Button();

            string name = e.GetDate().Replace('-', '_');

            eventContainer.Width = 400;
            eventContainer.Height = 980;
            eventContainer.Margin = new Thickness(10);
            eventContainer.Name = "container" + name;

            dateBox.Name = "datebox" + name;
            dateBox.Margin = new Thickness(30);
            dateBox.Height = 300;
            dateBox.VerticalAlignment = VerticalAlignment.Stretch;
            dateBox.Text = e.GetDate();
            dateBox.FontWeight = FontWeights.Bold;

            descBox.Width = 250;
            descBox.Height = 550;
            descBox.Padding = new Thickness(0);
            descBox.Name = "descBox" + name;
            descBox.Text = e.GetDescription();
            descBox.TextWrapping = TextWrapping.WrapWithOverflow;

            deleteButton.Content = $"Delete \'{e.GetDate()}\'";
            deleteButton.Width = 150;
            deleteButton.Height = 50;
            deleteButton.HorizontalAlignment = HorizontalAlignment.Right;
            deleteButton.VerticalAlignment = VerticalAlignment.Bottom;
            deleteButton.Margin = new Thickness(10);

            deleteButton.Click += DelegateMethod;

            timeline.Children.Add(eventContainer);
            eventContainer.Children.Add(dateBox);
            eventContainer.Children.Add(descBox);
            eventContainer.Children.Add(deleteButton);

            if (isWhiteOnRed)
            {
                eventContainer.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                dateBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                descBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            }
            isWhiteOnRed = !isWhiteOnRed;

            if (DateTime.Parse(e.GetDate()) == DateTime.MinValue)
            {
                //deleteButton.Visibility = Visibility.Hidden;
                dateBox.Visibility = Visibility.Hidden;
            }
        }

        public void AddEntryToDictionary()
        {
            if (DisplayErrorMsg())
                return;
            date = datePicker.SelectedDate.Value;
            Event e = new Event(date, eventDesc.Text);
            TextReader.GetInstance().ReadText(eventDesc.Text.Trim(), date,eventChar.Text.Trim());
        }

        private void TimelineEventAdderSwitcher(object sender, RoutedEventArgs e)
        {
            ScrollViewer timelineHolder = (ScrollViewer)FindName("TimeLineHolder");
            StackPanel entryPanel = (StackPanel)FindName("AddEntryPanel");
            Button switcherButton = (Button)FindName("SwitcherButton");
            if (timelineHolder.Visibility == Visibility.Visible)
            {
                switcherButton.Content = "Bekijk tijdlijn";
                timelineHolder.Visibility = Visibility.Hidden;
                entryPanel.Visibility = Visibility.Visible;
            }
            else
            {
                switcherButton.Content = "Voeg event toe";
                timelineHolder.Visibility = Visibility.Visible;
                entryPanel.Visibility = Visibility.Hidden;
                ManageFile.GetInstance().Load();
            }
        }

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            AddEntryToDictionary();
        }


        public delegate void Del(object sender, RoutedEventArgs e,string message);

        public static void DelegateMethod(object sender, RoutedEventArgs e)
        {
            string name = sender.ToString();

            name = name.Substring(name.IndexOf("'")).Replace('\'', ' ').Trim();

            ManageFile.GetInstance().RemoveEvent(name);
        }

        public void ClearTimeLine()
        {
            isWhiteOnRed = false;
            StackPanel timeline = (StackPanel)FindName("TimeLineStackPanel");
            timeline.Children.RemoveRange(0, timeline.Children.Count);
        }

        public void ClearEntryField()
        {
            eventDesc.Text = "";
            datePicker.DisplayDate = DateTime.Today;
            datePicker.SelectedDate = DateTime.Today;
        }

        public bool DisplayErrorMsg()
        {
            StringBuilder errorMsg = new StringBuilder();
            if (datePicker.SelectedDate == null)
                errorMsg.AppendLine("Geef een datum op.");
            if (eventDesc.Text == "Geef het event een omschrijving")
                errorMsg.AppendLine("Geef een omschrijving op");
            if (eventChar.Text == "Wie is dit overkomen?")
                errorMsg.AppendLine("Geef een karakter op");
            if (errorMsg.ToString() != "")
            {
                MessageBox.Show(errorMsg.ToString());
                return true;
            }
            return false;
        }
    }
}
