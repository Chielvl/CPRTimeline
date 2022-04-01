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
        public DatePicker datePicker;
        TextBox eventDesc;
        ComboBox combo;
        TextBox notBox;
        StackPanel timeline;
        readonly string[] characters = { "Full Party", "Archer", "Beetle", "Doc", "Kintsugi" };
        ItemCreator creator;
        FindTimeReferenceInText findTimeRef;

        DateTime date = DateTime.Today;
        public MainWindow()
        {
            InitializeComponent();
            creator = new ItemCreator();
            ManageFile.GetInstance().mainWindow = this;
            ManageFile.GetInstance().Load();
            findTimeRef = new FindTimeReferenceInText(this);
            combo = (ComboBox)FindName("CharacterComboBox");
            combo.ItemsSource = characters;
            combo.SelectedIndex = 0;
            datePicker = (DatePicker)FindName("Date");
            eventDesc = (TextBox)FindName("EventDesc");
            notBox = (TextBox)FindName("NotificationBox");
            timeline = (StackPanel)FindName("TimeLineStackPanel");
            UpdateDate();
            
        }

        public void UpdateDate(string date = "")
        {
            if (date == "")
                _ = DateTime.Now.ToString();
            else
            {
                date = timeline.Children.OfType<StackPanel>().FirstOrDefault().Name.Substring(9).Replace('_', '-');
                this.date = DateTime.Parse(date);
            }
            datePicker.SelectedDate = this.date;
            datePicker.DisplayDate = this.date;
        }
        public void UpdateNotificiationWindow(string msg)
        {
            notBox.Text = msg;
        }

        public void AddItemToTimeLine(Event eventToAdd)
        {
            if(creator == null)
                creator = new ItemCreator();
            StackPanel timeline = (StackPanel)FindName("TimeLineStackPanel");
            StackPanel addedItemStackPanel = creator.StackPanelCreator(eventToAdd);
            timeline.Children.Add(addedItemStackPanel);
        }

        public void AddEntryToDictionary()
        {
            if (DisplayErrorMsg())
                return;
            date =
                datePicker.SelectedDate.Value;
            Event e = new Event(date, eventDesc.Text);
            findTimeRef.SearchTextForTimeReferences(eventDesc.Text.Trim(), date,combo.SelectedValue.ToString().Trim());
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

        public void ClearTimeLine()
        {
            StackPanel timeline = (StackPanel)FindName("TimeLineStackPanel");
            timeline.Children.RemoveRange(0, timeline.Children.Count);
        }

        public void ClearEntryField(TextBox textBoxToClear)
        {
            textBoxToClear.Clear();
        }

        public void ClearDefaultDescription(object sender, RoutedEventArgs e)
        {
            if(eventDesc.Text == "Geef het event een omschrijving.")
                ClearEntryField(eventDesc);
        }

        public bool DisplayErrorMsg()
        {
            StringBuilder errorMsg = new StringBuilder();
            if (datePicker.SelectedDate == null)
                errorMsg.AppendLine("Geef een datum op.");
            if (eventDesc.Text == "Geef het event een omschrijving.")
                errorMsg.AppendLine("Geef een omschrijving op");
            if (errorMsg.ToString() != "")
            {
                MessageBox.Show(errorMsg.ToString());
                return true;
            }
            return false;
        }
    }
}
