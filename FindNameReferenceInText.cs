using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace TimeLine
{
    class FindNameReferenceInText
    {
        string regExCollection = @"Archer|Beetle|Doc|Kintsugi";
        TextBlock descBox;

        public ScrollViewer SearchTextForNames(StackPanel eventStackPanel, string inputString)
        {
            if (inputString.Length <= 0)
                return new ScrollViewer();

            ScrollViewer scroller = new ScrollViewer();
            descBox = new TextBlock();
            scroller.CanContentScroll = true;
            scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scroller.Width = 380;
            descBox.Width = 300;
            scroller.Height = 750;
            descBox.TextWrapping = TextWrapping.WrapWithOverflow;
            descBox.TextAlignment = TextAlignment.Justify;


            MatchCollection matches = Regex.Matches(inputString, regExCollection);
            if (matches.Count > 0)
            {
                int subBeginning = 0;
                foreach (Match match in matches)
                {
                    descBox.Inlines.Add(inputString.Substring(subBeginning, match.Index - subBeginning));
                    MakeLink(match);
                    subBeginning = match.Length + match.Index;
                }
                descBox.Inlines.Add(inputString.Substring(subBeginning));
            }
            else
            {
                descBox.Inlines.Add(inputString);
            }

            scroller.Content = descBox;
            return scroller;
        }

        private void MakeLink(Match name)
        {
            Run run = new Run(name.ToString());
            Hyperlink link = new Hyperlink(run);
            link.TargetName = "_blank";
            link.Name = "Name";
            link.Tag = name;
            link.NavigateUri = new Uri("http://www.google.com");
            descBox.Inlines.Add(link);
        }

    }
}
