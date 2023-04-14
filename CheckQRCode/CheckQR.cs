using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckQRCode
{
    public partial class CheckQR : Form
    {
        public delegate void notifytoform();
        public notifytoform notifytoform1;
        public delegate void ControlAdruinoOnof(string OnorOff);
        public ControlAdruinoOnof mycontrolAdruino;
        //Timer t1;
        //Timer t2;
        public string codelog;
        public string fullpathfile;
        public string myconnectionstring;
        public SerialPort PortBarcode;
        public bool OKNG;
        public int checktime;

        public CheckQR(string code, string pathname)
        {
            
            codelog = code;
            fullpathfile = pathname;
            InitializeComponent();
            OKNG = false;
            checktime = 0;
            myconnectionstring = Directory.GetCurrentDirectory() + "\\Database\\DataCheckQR.db";
            lbl_CSV.Text = codelog;
            fileSystemWatcher1.Path = fullpathfile.Substring(0,fullpathfile.LastIndexOf('\\'));
            fileSystemWatcher1.EnableRaisingEvents = true;
            Locationscreen();
            CheckRecivedBarcode.Start();
            TimerCheckEnd.Start();
        }
        public void Locationscreen()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int sizehei = this.Size.Height;
            int sizewit = this.Size.Width;
            Point p = new Point(screenWidth - sizewit, screenHeight - sizehei);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = p;
        }

        //private void txb_CodeLen_TextChanged(object sender, EventArgs e)
        //{
        //    //t1.Start();
        //    //t1.Tick += T1_Tick;
        //    //if (txb_CodeLen.Text == lbl_CSV.Text && Txb_CodeCheckSheet.Text == lbl_CSV.Text + "p")
        //    //{
        //    //    //dataglobal.countdatacomparenew = 0;
        //    //    if (!Checkdata(lbl_CSV.Text))
        //    //    {
        //    //        insertdata(lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text, "OK", "");
        //    //    }
        //    //    else
        //    //    {
        //    //        updatedata("OK", lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text);
        //    //    }
        //    //    notifytoform1?.Invoke();
        //    //    this.Close();
        //    //}
        //    //if (txb_CodeLen.Text.Length >= 16 && Txb_CodeCheckSheet.Text.Length >= 17 && txb_CodeLen.Text != lbl_CSV.Text && Txb_CodeCheckSheet.Text == lbl_CSV.Text + "p")
        //    //{
        //    //    if (!Checkdata(lbl_CSV.Text))
        //    //    {
        //    //        insertdata(lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text, "NG", "");
        //    //    }
        //    //    else
        //    //    {
        //    //        updatedata("NG", lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text);
        //    //    }
        //    //    notifytoform1?.Invoke();
        //    //    MessageBox.Show("QR code Len sai", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        //    //    //txb_CodeLen.Text = null;
        //    //    //Txb_CodeCheckSheet.Text = null;
        //    //    //txb_CodeLen.Focus();
        //    //    this.Close();

        //    //}
        //    //if (txb_CodeLen.Text.Length >= 16 && Txb_CodeCheckSheet.Text.Length >= 17 && Txb_CodeCheckSheet.Text != lbl_CSV.Text + "p" && txb_CodeLen.Text == lbl_CSV.Text)
        //    //{
        //    //    if (!Checkdata(lbl_CSV.Text))
        //    //    {
        //    //        insertdata(lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text, "NG", "");
        //    //    }
        //    //    else
        //    //    {
        //    //        updatedata("NG", lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text);
        //    //    }
        //    //    notifytoform1?.Invoke();

        //    //    MessageBox.Show("QR code CheckSheet sai", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        //    //    //Txb_CodeCheckSheet.Text = null;
        //    //    //txb_CodeLen.Text = null;
        //    //    //txb_CodeLen.Focus();
        //    //    this.Close();

        //    //}
        //    //if (txb_CodeLen.Text.Length >= 16 && Txb_CodeCheckSheet.Text.Length >= 17 && txb_CodeLen.Text != lbl_CSV.Text && Txb_CodeCheckSheet.Text != lbl_CSV.Text + "p")
        //    //{
        //    //    if (!Checkdata(lbl_CSV.Text))
        //    //    {
        //    //        insertdata(lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text, "NG", "");
        //    //    }
        //    //    else
        //    //    {
        //    //        updatedata("NG", lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text);
        //    //    }
        //    //    notifytoform1?.Invoke();
        //    //    MessageBox.Show("QR code Len va CheckSheet sai", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        //    //    //Txb_CodeCheckSheet.Text = null;
        //    //    //txb_CodeLen.Text = null;
        //    //    //txb_CodeLen.Focus();
        //    //    this.Close();
        //    //}
        //}

        //private void T1_Tick(object sender, EventArgs e)
        //{
        //    //if (txb_CodeLen.Text.Length <14)
        //    //{
        //    //    txb_CodeLen.Text = null;
        //    //}
        //    //else
        //    //{
        //    //   // Txb_CodeCheckSheet.Focus();
        //    //    t1.Stop();
        //    //}
            
        //}

        //private void Txb_CodeCheckSheet_TextChanged(object sender, EventArgs e)
        //{
        //    //t2.Start();
        //    //t2.Tick += T2_Tick;
           
        //}

        //private void T2_Tick(object sender, EventArgs e)
        //{
        //    //if (Txb_CodeCheckSheet.Text.Length<14)
        //    //{
        //    //    Txb_CodeCheckSheet.Text = null;
        //    //}
        //    //else
        //    //{
        //    //    t2.Stop();
        //    //}
           

        //}
        public void Readcontent(string path)
        {
            string cotentfile;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    cotentfile = sr.ReadToEnd();
                }
            }
            string[] arraycontentfile = cotentfile.Split('\r');
            foreach (string line in arraycontentfile)
            {
                if (line.Contains("[EOF]"))
                {
                    timerendcheck.Start();
                   
                }
            }
        }

        private void FileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            Readcontent(fullpathfile);
        }
        public void Insertdata(string CodeMain, string CodeLen, string CodeCheckSheet, string FirstCheck, string ReCheck)
        {
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={myconnectionstring};Version=3;"))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO DataCheckQR ( TimeUpdate, QRMain, QRLen,QRCheckSheet, FirstCheck, ReCheck)  VALUES ('{DateTime.Now.ToString("HH:mm dd/MM/yyyy")}','{CodeMain}','{CodeLen}','{CodeCheckSheet}','{FirstCheck}','{ReCheck}')", conn); 
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public bool Checkdata(string CodeMain)
        {
            string data = "";
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={myconnectionstring};Version=3;"))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand($"SELECT DataCheckQR.QRMain from DataCheckQR WHERE QRMain ='{CodeMain}'", conn);
                data = (string)cmd.ExecuteScalar();
                conn.Close();
            }
            if(data != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Updatedata(string ReCheck, string CodeMain, string CodeLen, string CodeCheckSheet)
        {
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={myconnectionstring};Version=3;"))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand($"UPDATE DataCheckQR SET TimeUpdate ='{DateTime.Now.ToString("HH:mm dd/MM/yyyy")}', QRMain='{CodeMain}',QRLen='{CodeLen}',QRCheckSheet='{CodeCheckSheet}', ReCheck = '{ReCheck}' WHERE QRMain = '{CodeMain}'", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void Endcheck_Tick(object sender, EventArgs e)
        {
            if (OKNG)
            {
                mycontrolAdruino?.Invoke("0");
                timerendcheck.Stop();
                this.Dispose();
                this.Close();
            }
            if (checktime == 1)
            {
                mycontrolAdruino?.Invoke("0");
               // timerendcheck.Stop();
            }
            checktime++;
            
        }

        private void CheckRecivedBarcode_Tick(object sender, EventArgs e)
        {
            if(ControlAdruino.DataCodeLen.Length >= 16 && txb_CodeLen.Text.Length <16)
            {
                txb_CodeLen.Text = ControlAdruino.DataCodeLen;
               // ControlAdruino.DataCodeLen = "";

            }
            if(ControlAdruino.DataCodeCheckSheet.Length >= 17 && Txb_CodeCheckSheet.Text.Length <17)
            {
                Txb_CodeCheckSheet.Text = ControlAdruino.DataCodeCheckSheet;
                //ControlAdruino.DataCodeCheckSheet = "";
            }
            if (txb_CodeLen.Text == lbl_CSV.Text && Txb_CodeCheckSheet.Text == lbl_CSV.Text + "p")
            {
                if (!Checkdata(lbl_CSV.Text))
                {
                    Insertdata(lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text, "OK", "");
                }
                else
                {
                    Updatedata("OK", lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text);
                }
                notifytoform1?.Invoke();
                OKNG = true;
                // timerendcheck.Stop();
                CheckRecivedBarcode.Stop();
                //this.Dispose();
                //this.Close();
            }
            else if (txb_CodeLen.Text.Length >= 16 && Txb_CodeCheckSheet.Text.Length >= 17 && txb_CodeLen.Text != lbl_CSV.Text && Txb_CodeCheckSheet.Text == lbl_CSV.Text + "p")
            {
                if (!Checkdata(lbl_CSV.Text))
                {
                    Insertdata(lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text, "NG", "");
                }
                else
                {
                    Updatedata("NG", lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text);
                }
                notifytoform1?.Invoke();
                MessageBox.Show("QR code Len sai", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                timerendcheck.Stop();
                mycontrolAdruino?.Invoke("0");
                CheckRecivedBarcode.Stop();
                this.Dispose();
                this.Close();
            }
            else if (txb_CodeLen.Text.Length >= 16 && Txb_CodeCheckSheet.Text.Length >= 17 && Txb_CodeCheckSheet.Text != lbl_CSV.Text + "p" && txb_CodeLen.Text == lbl_CSV.Text)
            {
                if (!Checkdata(lbl_CSV.Text))
                {
                    Insertdata(lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text, "NG", "");
                }
                else
                {
                    Updatedata("NG", lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text);
                }
                notifytoform1?.Invoke();
                mycontrolAdruino?.Invoke("0");
                MessageBox.Show("QR code CheckSheet sai", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                timerendcheck.Stop();
                CheckRecivedBarcode.Stop();
                this.Dispose();
                this.Close();
            }
           else if (txb_CodeLen.Text.Length >= 16 && Txb_CodeCheckSheet.Text.Length >= 17 && txb_CodeLen.Text != lbl_CSV.Text && Txb_CodeCheckSheet.Text != lbl_CSV.Text + "p")
            {
                if (!Checkdata(lbl_CSV.Text))
                {
                    Insertdata(lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text, "NG", "");
                }
                else
                {
                    Updatedata("NG", lbl_CSV.Text, txb_CodeLen.Text, Txb_CodeCheckSheet.Text);
                }
                notifytoform1?.Invoke();
                mycontrolAdruino?.Invoke("0");
                MessageBox.Show("QR code Len va CheckSheet sai", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                timerendcheck.Stop();
                CheckRecivedBarcode.Stop();
                this.Dispose();
                this.Close();
            }
        }

        private void CheckQR_FormClosed(object sender, FormClosedEventArgs e)
        {
            txb_CodeLen.Text = "";
            Txb_CodeCheckSheet.Text = "";
            ControlAdruino.DataCodeLen = "";
            ControlAdruino.DataCodeCheckSheet = "";
            fileSystemWatcher1.EnableRaisingEvents = false;
            timerendcheck.Stop();
            TimerCheckEnd.Stop();
            this.Dispose();
            GC.Collect();
        }

        private void TimerCheckEnd_Tick(object sender, EventArgs e)
        {
            if (OKNG)
            {
                mycontrolAdruino?.Invoke("0");
                this.Dispose();
                this.Close ();

            }
        }
    }
}
