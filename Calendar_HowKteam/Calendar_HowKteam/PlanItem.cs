using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Calendar_HowKteam
{
    public class PlanItem
    {
        
        private DateTime date;
        public DateTime Date { get => date; set => date = value; }

        private string job;
        public string Job { get => job; set => job = value; }

        private Point formTime;
        public Point FormTime { get => formTime; set => formTime = value; }

        private Point toTime;
        public Point ToTime { get => toTime; set => toTime = value; }
       
        private string status;
        public string Status { get => status; set => status = value; }
        

        public static List<string> ListStatus = new List<string>() {"DONE","DOING","COMING","MISSED" };

        public enum EPlanItem
        {
            DONE,
            DOING,
            COMING,
            MISSED
        }

    }
}
