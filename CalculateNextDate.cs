﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TimeLine
{
    public class CalculateNextDate
    {
        private static string[] days = { "maandag", "dinsdag", "woensdag", "donderdag", "vrijdag", "zaterdag", "zondag", "weekend" };
        private static CalculateNextDate instance;
        private static DateTime date;

        public static CalculateNextDate GetInstance()
        {
            if(instance == null)
                instance = new CalculateNextDate();

            return instance;
        }

        public DateTime GetDate()
        {
            return date;
        }

        //get the piece of text, the reference and the current date to offset
        public DateTime CalculateOffSet(string inputString, string timeRef, DateTime currentDate)
        {
            date = currentDate;

            if (inputString != null)
            {
                MessageBox.Show("time ref " +timeRef);
                int indexOfTimeRef = inputString.IndexOf(timeRef);
                int sentenceBeginning = inputString.LastIndexOf(".", indexOfTimeRef) > 0? inputString.LastIndexOf(".", indexOfTimeRef): 0;
                int sentenceEnd = inputString.IndexOf(".", sentenceBeginning +1) > 0? inputString.IndexOf(".", sentenceBeginning + 1): inputString.Length;
                inputString = inputString.Substring(sentenceBeginning, sentenceEnd-sentenceBeginning).Trim();
                //below referes to the next day
                if (timeRef.Equals("volgende dag")
                    || timeRef.Equals("volgende ochtend")
                    || timeRef.Equals("volgende middag")
                    || timeRef.Equals("volgende avond")
                    || timeRef.Equals("volgende nacht")
                    || timeRef.Equals(" dag erna")
                    )
                {
                    MessageBox.Show("Voeg 1 dag toe");
                    date =  currentDate.AddDays(1.0);
                }//refers to multiple days have passed
                else if (timeRef.Equals("dagen"))
                {
                    double numberOfDays = GetNumberFromString(inputString);
                    date =  currentDate.AddDays(numberOfDays);
                }
                else if (timeRef.Equals("weken later"))
                {//refers to multiple weeks have passed
                    double numberOfWeeks = GetNumberFromString(inputString);
                    date =  currentDate.AddDays(numberOfWeeks * 7.0);
                }//refers to the next week
                else if (timeRef.Equals("volgende week") || timeRef.Equals("week erna"))
                {
                    date =  currentDate.AddDays(7.0);
                }
                else if (timeRef.Equals("na ") || timeRef.Equals("volgende"))
                {//gets the day of the week and increments till the next occurence of the named day
                    foreach (string day in days)
                    {
                        if (inputString.Contains(day))
                        {
                            timeRef = day;
                            MessageBox.Show(timeRef);
                        }
                        else
                            continue;
                    }
                    switch (timeRef)
                    {
                        case "maandag": GetNextWeekDay(date =  currentDate.AddDays(1.0), DayOfWeek.Monday); break;
                        case "dinsdag": GetNextWeekDay(date =  currentDate.AddDays(1.0), DayOfWeek.Tuesday); break;
                        case "woensdag": GetNextWeekDay(date =  currentDate.AddDays(1.0), DayOfWeek.Wednesday); break;
                        case "donderdag": GetNextWeekDay(date =  currentDate.AddDays(1.0), DayOfWeek.Thursday); break;
                        case "vrijdag": GetNextWeekDay(date =  currentDate.AddDays(1.0), DayOfWeek.Friday); break;
                        case "weekend":
                        case "zaterdag": GetNextWeekDay(date =  currentDate.AddDays(1.0), DayOfWeek.Saturday); break;
                        case "zondag": GetNextWeekDay(date =  currentDate.AddDays(1.0), DayOfWeek.Sunday); break;
                    }
                    
                    if (timeRef.Contains("weekend")) date =  currentDate.AddDays(1.0);//add another day to account for the weekend
                    //offSet =  currentDate.AddDays(1.0); //add a day to account for the word after (na)
                }
            }

            return date;
        }

        private static double GetNumberFromString(string input)
        {
            if (double.TryParse((Regex.Match(input, @"\d+").Value), out double result))
            {
                return result;
            }
            else
            {
                switch (input)
                {
                    default:
                    case string a when a.Contains("een"): return 1.0;
                    case string a when a.Contains("twee"): return 2.0;
                    case string a when a.Contains("drie"): return 3.0;
                    case string a when a.Contains("vier"): return 4.0;
                    case string a when a.Contains("vijf"): return 5.0;
                    case string a when a.Contains("zes"): return 6.0;
                    case string a when a.Contains("zeven"): return 7.0;
                    case string a when a.Contains("acht"): return 8.0;
                    case string a when a.Contains("negen"): return 9.0;
                    case string a when a.Contains("tien"): return 10.0;
                }
            }
        }

        private static DateTime GetNextWeekDay(DateTime currentDate, DayOfWeek targetDay)
        {
            MessageBox.Show(currentDate.DayOfWeek + " volgende " + targetDay);
            int daysToAdd = ((int)targetDay - (int)currentDate.DayOfWeek + 7) % 7 ;
            return currentDate =  currentDate.AddDays(daysToAdd);
        }
    }
}
