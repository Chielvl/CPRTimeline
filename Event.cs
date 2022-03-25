using System;

namespace TimeLine
{
    [Serializable]
    public class Event
    {
        private DateTime Date;
        private string Description;

        public Event()
        {

        }

        public Event(DateTime Date, string Description)
        {
            this.Date = Date;
            this.Description = Description;
        }

        public string GetDate()
        {
            return Date.ToString("dd/MM/yyyy");
        }
        public void SetDate(DateTime Date)
        {
            this.Date = Date;
        }
        public string GetDescription()
        {
            return Description;
        }
        public void SetDescription(string Description)
        {
            if(this.Description != null)
            {
                if (this.Description.Equals(Description))
                {
                    return;
                }
                else
                    this.Description += "\n\n" + Description;
            }
            this.Description = Description;
        }

        public void AddDescription(string Description)
        {
            if (this.Description.Equals(Description))
            {
                return;
            }
            else
                this.Description += "\n\n" + Description;
         
        }

        public override string ToString()
        {

            return ($"On {GetDate()} the following happened:  {GetDescription()}");
        }

    }
}
