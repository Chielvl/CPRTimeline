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
        
        private string[] timeReferences = { "volgende", "dagen later", "weken later", "na ", "dag erna", "week erna", "maandag", "dinsdag", "woensdag", "donderdag", "vrijdag", "zaterdag", "zondag" };
        private int numberOfEntries;
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

        public void ReadText(string inputText, DateTime startingDate, string character)
        {
            if(startingDate == DateTime.MinValue)
            {
                MessageBox.Show("Vul een datum in, aub", "Vul datum in", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            offSet = startingDate;
            if (inputText != null)
            {
                numberOfEntries++;
                StringBuilder sub = new StringBuilder(inputText);

                int timeRef = FindTimeReferences(inputText, startingDate);
                if (timeRef > 0)
                {
                    sub.Length = sub.ToString().LastIndexOf(".", timeRef) > 0 ? sub.ToString().LastIndexOf(".", timeRef) + 1 : sub.Length;
                }
                MessageBox.Show(sub.ToString());
                mainWindow.datePicker.SelectedDate = mainWindow.datePicker.DisplayDate =CalculateNextDate.GetInstance().GetDate();
                Event newEvent = new Event(CalculateNextDate.GetInstance().GetDate(), $" <--- { character } ---> \n{ sub}");
                MessageBox.Show(newEvent.ToString());
                ManageFile.GetInstance().AddEvent(newEvent);
                
                if (inputText.Length - sub.Length > 1)
                {
                    try
                    {
                        ReadText(inputText.Substring(sub.Length + 1).Trim(), offSet, character);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"string length {inputText.Length}\n" +
                            $"sub length {sub.Length}\n" +
                            ex.ToString());
                    }
                }
            }
            
            ManageFile.GetInstance().Save();
            numberOfEntries = 0;
        }

        public int FindTimeReferences(string inputText, DateTime currentDate)
        {
            int firstRef = inputText.Length;
            int secondRef = 0;
            StringBuilder referenceString = new StringBuilder();
            StringBuilder referenceString2 = new StringBuilder();
            foreach (string tr in timeReferences)
            {
                if (inputText.ToLower().Contains(tr) && inputText.IndexOf(tr) < firstRef)
                {
                    secondRef = firstRef;
                    referenceString2 = referenceString;
                    referenceString.Clear().Append(tr);
                    firstRef = inputText.ToLower().IndexOf(tr);
                    if (tr.Equals("na") || tr.Equals("volgende"))
                    {
                        foreach(string naVolgende in new string[] {"dag"})
                        {
                            if (inputText.Substring(firstRef, inputText.IndexOf(".") - firstRef).Contains(naVolgende))
                            {
                                MessageBox.Show("volgende of na " + tr + " " + naVolgende);
                            }
                        }
                        
                    }
                }
                else
                {
                    continue;
                }
            }
            
            if (numberOfEntries > 1 && secondRef != inputText.Length)
            {
                
                offSet = CalculateNextDate.GetInstance().CalculateOffSet(inputText.ToLower(), referenceString2.ToString().ToLower(), currentDate);
            
                return secondRef;
            }

            offSet = CalculateNextDate.GetInstance().CalculateOffSet(inputText.ToLower(), referenceString.ToString().ToLower(), currentDate);
            
            return firstRef;
        }
    }
}
