using System;
using System.Collections.Generic;
using System.Text;

namespace Calendar_HowKteam
{
    [Serializable]
    public class PlanData
    {
        private List<PlanItem> job;

        public List<PlanItem> Job { get => job; set => job = value; }

        private int starUp;
        public int StarUp { get => starUp; set => starUp = value; }

        private int notifyCk;
        public int NotifyCk { get => notifyCk; set => notifyCk = value; }
    }
}
