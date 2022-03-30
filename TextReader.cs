using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace TimeLine
{
    public class TextReader
    {
        private static TextReader instance;

        private Regex regEx = new Regex(@"(volgende (ochtend|avond|[a-z]*dag|week))|((na)\s([a-z]*|\d)\s?(([a-z]{3,6})?[a-z]* weekend|dagen|week|weken|dag))|(([a-z]*|\d){1}\s(dag|dagen|week|weken)\s(later))|((([a-z]{3,6})?dag|weekend)\s(erop|erna|daarop))", RegexOptions.IgnoreCase);

        DateTime offSet;
        public MainWindow mainWindow;
        private TextReader()
        {
        }

        public static TextReader GetInstance()
        {
            if (instance == null)
                instance = new TextReader();
            return instance;
        }

        public void SetDate(DateTime date)
        {
            offSet = date;
        }

        public void ReadText(string input, DateTime startingDate, string character)
        {
            
            MatchCollection matches = Regex.Matches(input.ToLower(), regEx.ToString(), RegexOptions.IgnoreCase);
            //if there are no matches, make a single entry;
            if (matches.Count == 0)
            {
                ManageFile.GetInstance().AddEvent(new Event(startingDate, $"<--- { character } --->\n {input}"));

            }
            else
            {
                offSet = startingDate;
                int subBeginning, subEnding;
                subBeginning = subEnding = -1;
                //Loop through array of matches to get all text parts. Start loop at -1 to account for the time reference to appear later in the text.
                for (int i = -1; i < matches.Count; i++)
                {
                    string timeRef;
                    if (i == -1)
                    {
                        //if the first sentence already contains the time reference, skip the -1 sentence;
                        if (input.LastIndexOf('.', matches[0].Index) < 0)
                            continue;
                        subBeginning = input.LastIndexOf('.', matches[0].Index);
                        subEnding = matches.Count > 0 ? input.LastIndexOf('.', matches[0].Index) : input.Length;
                        timeRef = null;
                    }
                    else
                    {
                        subBeginning = input.LastIndexOf('.', matches[0].Index) > 0 ? input.LastIndexOf('.', matches[i].Index) + 1 : 0;
                        subEnding = i + 1 >= matches.Count ? input.Length : input.LastIndexOf('.', matches[i + 1].Index);
                        timeRef = matches[i].ToString();
                    }
                    string eventString = $"<--- { character } --->\n {input.Substring(subBeginning, subEnding - subBeginning).Trim()} ";
                    offSet = CalculateNextDate.GetInstance().CalculateOffSet(eventString, timeRef, offSet);
                    ManageFile.GetInstance().AddEvent(new Event(offSet, eventString));
                }
            }
            ManageFile.GetInstance().Save();
            mainWindow.UpdateDate(offSet.ToString("dd/MM/yyyy"));
        }
    }
}
