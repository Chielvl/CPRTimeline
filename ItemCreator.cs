using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

namespace TimeLine
{
    class ItemCreator
    {
        private static bool isWhiteOnRed;
        FindNameReferenceInText findNames;

        public ItemCreator()
        {
            findNames = new FindNameReferenceInText();
        }

        public StackPanel StackPanelCreator(Event eventToAdd)
        {
            if(eventToAdd == null)
                return null;
            string name = eventToAdd.GetDate().Replace('-', '_');
            StackPanel eventContainer = new StackPanel();
            TextBlock dateBox = new TextBlock();
            Button deleteButton = new Button();

            eventContainer.Width = 400;
            eventContainer.Height = 980;
            eventContainer.Margin = new Thickness(10);
            eventContainer.Name = "container" + name;
            eventContainer.CanVerticallyScroll = true;

            dateBox.Name = "datebox" + name;
            dateBox.Margin = new Thickness(30);
            dateBox.Height = 100;
            dateBox.VerticalAlignment = VerticalAlignment.Stretch;
            dateBox.FontWeight = FontWeights.Bold;
            dateBox.Text = eventToAdd.GetDate();
            
            ScrollViewer descScroller = findNames.SearchTextForNames(eventContainer, eventToAdd.GetDescription());
            descScroller.Name = "descScroller" + name;

            deleteButton.Name = "delete" + name;
            deleteButton.Width = 150;
            deleteButton.Height = 50;
            deleteButton.HorizontalAlignment = HorizontalAlignment.Right;
            deleteButton.VerticalAlignment = VerticalAlignment.Top;
            deleteButton.Margin = new Thickness(10);
            deleteButton.Content = $"Delete \'{eventToAdd.GetDate()}\'";

            deleteButton.Click += DelegateMethod;
            eventContainer.Children.Add(dateBox);
            eventContainer.Children.Add(deleteButton);
            eventContainer.Children.Add(descScroller);
            if (isWhiteOnRed)
            {
                eventContainer.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                dateBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                descScroller.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            }
            isWhiteOnRed = !isWhiteOnRed;

            return eventContainer;
        }
        public static void DelegateMethod(object sender, RoutedEventArgs e)
        {
            string name = sender.ToString();

            name = name.Substring(name.IndexOf("'")).Replace('\'', ' ').Trim();

            ManageFile.GetInstance().RemoveEvent(name);
        }

    }
}
