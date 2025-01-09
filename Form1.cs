
using Guna.UI2.WinForms;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;


namespace PrayTimer
{
    public partial class Form1 : Form
    {
        enum enSun { Sunrise  , Sunset} 
        struct stPrayerTime {public int Hourse ; public int Minutes; };
        stPrayerTime stprayerTime = new stPrayerTime();
        WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();


        TimeSpan CountDown = new TimeSpan();
        DateTime PrayerTime = new DateTime();
        DateTime CurrntTime = DateTime.Now;
      


        DateTime TimeNow()
        {
            DateTime Time = DateTime.Now;
            return Time;
        }
        void DisplayPraySound(bool Display = false)
        {
            player.URL =@"C:\Users\chmik\Desktop\صلاتي\Azan.mp3";

            player.controls.play();

           
            if (!Display)
                player.controls.stop();
            return;

        }
        void ShowSun(enSun Sun)
        {
            if(Sun == enSun.Sunrise)
            {
                pcbxSunRays.Visible = true;
                PcSunD.Visible = false;

            }
            if (Sun == enSun.Sunset)
            {
                PcSunD.Visible = true;
                pcbxSunRays.Visible = false; 
            }
        }
        string HijriMonthName(int Month )
        {
            string[] hijriMonthNames = new string[]
            {
            "محرم", "صفر", "ربيع الأول", "ربيع الآخر",
            "جمادى الأولى", "جمادى الآخرة", "رجب", "شعبان",
            "رمضان", "شوال", "ذو القعدة", "ذو الحجة"
            };
            return hijriMonthNames[Month - 1];
        }
        void HijriCalendar(DateTime CurrntDate)
        {
            HijriCalendar hijriCalendar = new HijriCalendar();
            lblHijriYear.Text = hijriCalendar.GetYear(CurrntDate).ToString();
            lblHijrMonthName.Text =  HijriMonthName(hijriCalendar.GetMonth(CurrntDate));
            lblHijrDayOfMonth.Text =  hijriCalendar.GetDayOfMonth(CurrntDate).ToString();
            CultureInfo arabicCulture = new CultureInfo("ar-SA");
            lblHijrDayOfWeek.Text = CurrntDate.ToString("ddd", arabicCulture);
        }
        void EnCalendar(DateTime CurrentDate)
        {
            lblEnDayOfWeek.Text = CurrentDate.DayOfWeek.ToString();
            lblEnDayOfMonth.Text = CurrentDate.Day.ToString();
            lblEnMonthName.Text = CurrentDate.ToString("MMMM");
            lblEnYear.Text = CurrentDate.Year.ToString();
        }
        bool IsPrayTimeFor(System.Windows.Forms.Label lblPraytime )
        {
          
            stprayerTime.Hourse = Convert.ToInt32(lblPraytime.Text.Substring(0,2));
            stprayerTime.Minutes = Convert.ToInt32(lblPraytime.Text.Substring(3, 2));
            TimeSpan T = new TimeSpan(stprayerTime.Hourse, stprayerTime.Minutes, 0);

            PrayerTime = DateTime.Today.Add(T);
            CurrntTime =  DateTime.Now;
            if (PrayerTime < CurrntTime)
            {
                return false;
            }
            else 
            return true;
           
        }
        void ResetToNextDayPrayTime()
        {
            stprayerTime.Hourse = Convert.ToInt32(AlfajrTime.Text.Substring(0, 2));
            stprayerTime.Minutes = Convert.ToInt32(AlfajrTime.Text.Substring(3, 2));
            PrayerTime = DateTime.Now.AddDays(1);
            PrayerTime = new DateTime(PrayerTime.Year, PrayerTime.Month, PrayerTime.Day, stprayerTime.Hourse, stprayerTime.Minutes , 0); 
        }
        void GoToNextPayerTime()
        {
            
            if (IsPrayTimeFor(AlfajrTime))
            {
                lblPrayNameProgess.Text = AlFajrlbl.Text;
                ShowSun(enSun.Sunrise);
                return; 
            }
            if (IsPrayTimeFor(AlDohurTime ))
            {
                lblPrayNameProgess.Text = AlDouhrlbl.Text;
                ShowSun(enSun.Sunrise);
               
                return;
            }
            if (IsPrayTimeFor(AlAsarTime))
            {
                lblPrayNameProgess.Text = AlAsarlbl.Text;
                ShowSun(enSun.Sunrise);
                return;
            }
            if (IsPrayTimeFor(AlMagraibTime))
            {
                lblPrayNameProgess.Text = AlMagriblbl.Text;
                ShowSun(enSun.Sunset);
                return;
            }
            if (IsPrayTimeFor(AlEshaaTime))
            {
                lblPrayNameProgess.Text = LblAlishaa.Text;
                ShowSun(enSun.Sunset);
                PrayerTime.AddDays(1); 
                GoToNextPayerTime();
                return;
            }
            ResetToNextDayPrayTime();


        }  
        void CalculateTimeDifferece(DateTime CurrntTime)
        {
           
             CountDown = PrayerTime - CurrntTime;

            if(stprayerTime.Hourse == 0 && stprayerTime.Minutes == 0 )
            {
                GoToNextPayerTime();
               
            }

             circularProgressBar1.Text = CountDown.Hours.ToString() + ":" + CountDown.Minutes.ToString() + ":" +
             CountDown.Seconds.ToString() ;
             circularProgressBar1.Style = ProgressBarStyle.Marquee;
             CountDown = PrayerTime - CurrntTime;
             if (CountDown.TotalSeconds < 0 )
             {

                timertoCheckLblsPrayTime.Start(); 
               
             }

           
        }


         public Form1()
         {
           
            InitializeComponent();
         }


        private void Form1_Load(object sender, EventArgs e)
        {
            
            HijriCalendar(CurrntTime);
            EnCalendar(CurrntTime);
            timer1.Start();
           
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
                      
            lblMainTime.Text = TimeNow().ToString("T");
            CalculateTimeDifferece(TimeNow());
            
        }
        private void Bell_OnClick(object sender, EventArgs e)
        {
            ((Guna2CircleButton)sender).Checked = ! ((Guna2CircleButton)sender).Checked;
            if (((Guna2CircleButton)sender).Checked)
             ((Guna2CircleButton)sender).Image = Properties.Resources.bell;
            else
                ((Guna2CircleButton)sender).Image = Properties.Resources.silent;
            DisplayPraySound(false); 
        }
        private void timertoCheckLblsPrayTime_Tick(object sender, EventArgs e)
        {
            timertoCheckLblsPrayTime.Stop();
            Thread.Sleep(20);
            DisplayPraySound(true);
            GoToNextPayerTime();
        }



    }
}
