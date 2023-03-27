using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Calendar_HowKteam
{
    public partial class fCalender : Form
    {
        #region Properties
       

        private string filePath = "data.xml";

        private int appTime;
        public int AppTime { get => appTime; set => appTime = value; }
        private List<List<Button>> matrix;
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }
        
        private PlanData job;
        public PlanData Job { get => job; set => job = value; }
        
        

        private List<string> dateOfWeek = new List<string>() {"Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday"};/// <summary>
        /// this code line must be write correctly or code will bug !!!
        /// </summary>

        #endregion
        public fCalender()
        {
            InitializeComponent();
            
            
            tmNotify.Start();
            AppTime = 0;
            LoadMatrix();

            try
            {
                Job = DeserializeFromXML(filePath) as PlanData;
            }
            catch {
                SetDefaultJob();
            }
            setUI();
        }

        void setUI()//set checkbox từ dữ liệu đã lưu
        {
           
            if (Job.NotifyCk == 0)
                cbNotify.Checked = false;
            else if(Job.NotifyCk == 1)
                cbNotify.Checked = true;

            if (Job.StarUp == 0)
                ckbStartUp.Checked = false;
            else if (Job.StarUp == 1)
                ckbStartUp.Checked = true;
        }
        void SetDefaultJob()//đặt công việc mặc định để testư
        {
            Job = new PlanData();
            Job.Job = new List<PlanItem>();
            Job.NotifyCk = 0;
            Job.StarUp = 0;
            Job.Job.Add(new PlanItem()
            {
                Date = DateTime.Now,
                FormTime = new Point(4,0),
                ToTime = new Point(5,0),
                Job = "Test",
                Status = PlanItem.ListStatus[(int)PlanItem.EPlanItem.COMING]
            });
            Job.Job.Add(new PlanItem()
            {
                Date = DateTime.Now,
                FormTime = new Point(4, 0),
                ToTime = new Point(5, 0),
                Job = "Test2",
                Status = PlanItem.ListStatus[(int)PlanItem.EPlanItem.DOING]
            });
            Job.Job.Add(new PlanItem()
            {
                Date = DateTime.Now,
                FormTime = new Point(2, 0),
                ToTime = new Point(6, 0),
                Job = "Test3",
                Status = PlanItem.ListStatus[(int)PlanItem.EPlanItem.DONE]
            });

        }
        void LoadMatrix()//load bảng nút
        {

            Matrix = new List<List<Button>>();

            Button oldBtn = new Button() { Width = 0, Height = 0, Location = new Point(-Cons.margin, 0) };
            for(int i = 0; i<Cons.DayOfColumn ; i++)
            {
                Matrix.Add(new List<Button>());
                for(int j = 0; j< Cons.DayOfWeek; j++)
                {
                    Button btn = new Button() { Width = Cons.DateButtonWidth + Cons.margin, Height = Cons.DateButtonHeigh };
                    btn.Location = new Point(oldBtn.Location.X + oldBtn.Width + Cons.margin, oldBtn.Location.Y);
                    btn.Click += Btn_Click;
                    pnlMatrix.Controls.Add(btn);
                    Matrix[i].Add(btn);

                    oldBtn = btn;
                }
                oldBtn = new Button() { Width = 0, Height = 0, Location = new Point(-Cons.margin, oldBtn.Location.Y + Cons.DateButtonHeigh) };
            }
            setDefaultDate();
        }

        private void Btn_Click(object sender, EventArgs e)//sự kiện nhấn nút
        {
            if (String.IsNullOrEmpty((sender as Button).Text))
                return;
            DailyPlan daily = new DailyPlan(new DateTime(dtpkDate.Value.Year,dtpkDate.Value.Month, Convert.ToInt32((sender as Button).Text)),Job);
            daily.ShowDialog();
        }

        int dayOfMonth(DateTime date)
        {
            switch (date.Month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2:
                    if ((date.Year % 4 == 0 && date.Year % 100 != 0) || date.Year % 400 == 0)
                        return 29;
                    else
                        return 28;
                default:
                    return 30;

            }

        }
        void AddNumberIntoMatrixByDate(DateTime date)
        {
            ClearMatrixValue();
            DateTime useDate = new DateTime(date.Year,date.Month, 1);
           
            int line = 0;

            for(int i = 1; i <= dayOfMonth(date); i++)
            {
                int column = dateOfWeek.IndexOf(useDate.DayOfWeek.ToString());
                Button btn = Matrix[line][column];
                btn.Text = i.ToString();

                if(isEqualDate(useDate,DateTime.Now))
                {
                    btn.BackColor = Color.Yellow;
                }

                if (isEqualDate(useDate, date))
                {
                    btn.BackColor = Color.Aqua;
                }

                if (column >= 6)
                    line++;

                useDate = useDate.AddDays(1);
            }
        }

        bool isEqualDate(DateTime dateA, DateTime dateB)
        {
            return dateA.Year == dateB.Year && dateA.Month == dateB.Month && dateA.Day == dateB.Day;
        }

        void ClearMatrixValue()
        {
            for(int i = 0; i< Matrix.Count; i++)
            {
                for (int j = 0; j < Matrix[i].Count; j++)
                {
                    Button btn = Matrix[i][j];
                    btn.Text = "";
                    btn.BackColor = Color.WhiteSmoke;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
        
        void setDefaultDate()
        {
            dtpkDate.Value = DateTime.Now;
        }

        private void dtpkDate_ValueChanged(object sender, EventArgs e)
        {
            AddNumberIntoMatrixByDate((sender as DateTimePicker).Value);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            dtpkDate.Value = dtpkDate.Value.AddMonths(-1);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            dtpkDate.Value = dtpkDate.Value.AddMonths(1);
        }

        private void btnToDay_Click(object sender, EventArgs e)
        {
            setDefaultDate();
        }

        private void SerializeToXML(object data,string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            XmlSerializer sr = new XmlSerializer(typeof(PlanData));

            sr.Serialize(fs,data);

            fs.Close();

        }

        private object DeserializeFromXML(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                XmlSerializer sr = new XmlSerializer(typeof(PlanData));

                object result = sr.Deserialize(fs);

                fs.Close();
                return result;
            }
            catch (Exception e)
            {
                fs.Close();
                throw new NotImplementedException();
            }
           
        }

        private void fCalender_FormClosed(object sender, FormClosedEventArgs e)
        {
            SerializeToXML(Job,filePath);
        }

        private void tmNotify_Tick(object sender, EventArgs e)
        {
            if (!cbNotify.Checked) // check checkbox
                return;

            ++AppTime;
            if (AppTime < Cons.notifyTime)
                return;

            if (Job == null || Job.Job == null)
                return;

            DateTime currentDate = DateTime.Now;
            List<PlanItem> todayJob = Job.Job.Where(p=>p.Date.Year==currentDate.Year && p.Date.Month == currentDate.Month && p.Date.Day==currentDate.Day).ToList();
            Notify.ShowBalloonTip(Cons.notifyTimeOut,"Lịch công việc",String.Format("Bạn có {0} việc trong ngày hôm nay",todayJob.Count),ToolTipIcon.Info);
            AppTime = 0;
        }

        private void nmNotify_ValueChanged(object sender, EventArgs e)
        {
            Cons.notifyTime = (int)nmNotify.Value;
        }

        private void cbNotify_CheckedChanged(object sender, EventArgs e)
        {
            nmNotify.Enabled = cbNotify.Checked;
            if(cbNotify.Checked)
            {
                Job.NotifyCk = 1;
            }
            else
            {
                Job.NotifyCk = 0;
            }    
        }

        private void ckbStartUp_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\LapLich");
            //mo registry khoi dong cung win
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue;
            if (ckbStartUp.Checked)
            {
                
                keyvalue = "1";
                Job.StarUp = 1;
                

            }
            else
            {
                keyvalue = "0";
                Job.StarUp = 0;
            }

            try
            {
                //chen gia tri key
                regkey.SetValue("Index", keyvalue);
                //regstart.SetValue("taoregistrytronghethong", "E:\\Studing\\Bai Tap\\CSharp\\Channel 4\\bai temp\\tao registry trong he thong\\tao registry trong he thong\\bin\\Debug\\tao registry trong he thong.exe");
                regstart.SetValue("LapLich", Application.StartupPath + "\\Calender_HowKteam.exe");
                ////dong tien trinh ghi key
                //regkey.Close();
            }
            catch (System.Exception ex)
            {
            }
        }
    }
}
