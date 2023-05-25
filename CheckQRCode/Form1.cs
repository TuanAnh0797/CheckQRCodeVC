using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using KAutoHelper;

namespace CheckQRCode
{
    public partial class Form1 : Form
    {
        public string PathImageCaptureScreen;
        public string PathImageButtom;
        public string PathInfor;
        public Image<Bgr,byte> ImageScreen;
        public Image<Bgr,byte> ImageButton;
        public Image<Bgr, byte> ImageInfor;
        public string myconnectionstring;
        public string[] config;
        public SerialPort PortBarcode;
        public SerialPort PortAdruino;
        public Form1()
        {
            InitializeComponent();
            config = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Config.txt");
            PathImageCaptureScreen = Directory.GetCurrentDirectory() + "\\SetupImage\\Screenshooting.png";
            PathImageButtom = Directory.GetCurrentDirectory() + "\\SetupImage\\Button.png";
            PathInfor = Directory.GetCurrentDirectory() + "\\SetupImage\\INFOR.png";
            myconnectionstring = Directory.GetCurrentDirectory() + "\\Database\\DataCheckQR.db";
            Locationscreen();
           

            ControlAdruino.Comport = config[1].Split('"')[1].Trim('"');
            ControlAdruino.DelayStart = config[2].Split('"')[1].Trim('"');
            ControlAdruino.ComportBarcode = config[3].Split('"')[1].Trim('"');
            fileSystemWatcherCheckQR.Path = config[0].Split('"')[1].Trim('"');
            fileSystemWatcherCheckQR.Filter = "*.log";
            fileSystemWatcherCheckQR.IncludeSubdirectories = true;
            fileSystemWatcherCheckQR.EnableRaisingEvents = true;
            try
            {
                PortAdruino = new SerialPort();
                PortAdruino.PortName = ControlAdruino.Comport;
                PortAdruino.BaudRate = 9600;
                PortAdruino.Parity = Parity.None;
                PortAdruino.StopBits = StopBits.One;
                PortAdruino.DataBits = 8;
                PortAdruino.Open();
            }
            catch (Exception)
            {

                MessageBox.Show("Chưa kết nối với Adruino", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

            }
            ControlAdruino.DataCodeCheckSheet = "";
            ControlAdruino.DataCodeLen = "";
            Updatedata();
            Openportbarcode();
            if (!PortBarcode.IsOpen)
            {
                MessageBox.Show("Không thể kết nối với Barcode", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
            PortBarcode.DataReceived += PortBarcode_DataReceived;

        }
        public void Openportbarcode()
        {
            try
            {
                PortBarcode = new SerialPort();
                PortBarcode.PortName = ControlAdruino.ComportBarcode;
                PortBarcode.BaudRate = 9600;
                PortBarcode.DataBits = 8;
                PortBarcode.StopBits = StopBits.One;
                PortBarcode.Parity = Parity.None;
                PortBarcode.Open();
            }
            catch (Exception)
            {

                MessageBox.Show("Không thể kết nối với Barcode", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }

        }

        private void PortBarcode_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort p = (SerialPort)sender;
            string datareceied = p.ReadExisting().Trim('\r');
            if (datareceied == "MEASURE" && Application.OpenForms["CheckQR"] == null)
            {
                StartCheck();
                datareceied = "";
            }
            if (datareceied.Length >= 17 && datareceied.EndsWith("p"))
            {
                ControlAdruino.DataCodeCheckSheet = datareceied;
                datareceied = "";
            }
            if (datareceied.Length >= 16 && !datareceied.EndsWith("p"))
            {
                ControlAdruino.DataCodeLen = datareceied;
                datareceied = "";
            }
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

        private void Btn_measure_Click(object sender, EventArgs e)
        {
            btn_measure.BackColor = Color.Yellow;
            btn_measure.Enabled = false;
            StartCheck();
            
        }
        public void StartCheck()
        {
            CaptureandMoveMouse.Capture(PathImageCaptureScreen);
            //  ImageButton = new Image<Bgr, byte>(PathImageButtom);
            ImageScreen = new Image<Bgr, byte>(PathImageCaptureScreen);
            ImageButton = new Image<Bgr, byte>(PathImageButtom);
            Mat imgout = new Mat();
            CvInvoke.MatchTemplate(ImageScreen, ImageButton, imgout, Emgu.CV.CvEnum.TemplateMatchingType.CcorrNormed);
            // getpersenmatching
            Image<Gray, float> result = ImageScreen.MatchTemplate(ImageButton, TemplateMatchingType.CcoeffNormed);
            double[] minValues, maxValues;
            Point[] minLocations, maxLocations;
            result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
            double similarityScore = maxValues[0];
            //
            double minVal = 0.8;
            double maxVal = 0.0;
            Point minLoc = new Point();
            Point maxLoc = new Point();
            CvInvoke.MinMaxLoc(imgout, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
            if (similarityScore > 0.7)
            {

                OnorOffRelay("1");
                Thread.Sleep(Int32.Parse(ControlAdruino.DelayStart) * 1000);
                CaptureandMoveMouse.Movemouseandclick(maxLoc, ImageButton);
                

                btn_measure.Enabled = true;
                btn_measure.BackColor = Color.Green;
            }
            else
            {
                MessageBox.Show("Không tìm thấy nút Start", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                btn_measure.Enabled = true;
                btn_measure.BackColor = Color.Green;
            }
        }
        private void OnorOffRelay(string a)
        {
            try
            {
                byte[] b = Encoding.UTF8.GetBytes(a);
                PortAdruino.Write(b, 0, b.Length);
            }
            catch (Exception)
            {
                btn_measure.Enabled = true;
                btn_measure.BackColor = Color.Green;
                MessageBox.Show("Không gửi được dữ liệu đến Adruino", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
          
        }
        private void FileSystemWatcherCheckQR_Created(object sender, FileSystemEventArgs e)
        {
            CheckQR c = new CheckQR(e.Name.Split('_')[1].Trim('\r'), e.FullPath);
            c.notifytoform1 = new CheckQR.notifytoform(Updatedata);
            c.mycontrolAdruino = new CheckQR.ControlAdruinoOnof(OnorOffRelay);
            this.Hide();
            ControlAdruino.DataCodeLen = "";
            ControlAdruino.DataCodeCheckSheet = "";
            c.ShowDialog();
           
            this.Show();
        }
        public void Updatedata()
        {
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={myconnectionstring};Version=3;"))
            {
                conn.Open();
                DataTable dt = new DataTable();
                SQLiteDataAdapter adap = new SQLiteDataAdapter("SELECT * from DataCheckQR Where FirstCheck ='NG' or ReCheck ='NG' ORDER By TimeUpdate ASC ", conn);
                adap.Fill(dt);
                dtg_listerror.DataSource = dt;
                conn.Close();
            }
            
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            PortBarcode.Close();
            PortBarcode = null;
            timeroffinfor.Stop();
            OnorOffRelay("0");
            PortAdruino.Close();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.H)
            {
                Checkdata ef = new Checkdata();
                ef.Show();
            }

        }

        private void btn_infor_Click(object sender, EventArgs e)
        {
            CaptureandMoveMouse.Capture(PathImageCaptureScreen);
            ImageScreen = new Image<Bgr, byte>(PathImageCaptureScreen);
            ImageInfor = new Image<Bgr, byte>(PathInfor);
            Mat imgout = new Mat();
            CvInvoke.MatchTemplate(ImageScreen, ImageInfor, imgout, Emgu.CV.CvEnum.TemplateMatchingType.CcorrNormed);
            // getpersenmatching
            Image<Gray, float> result = ImageScreen.MatchTemplate(ImageInfor, TemplateMatchingType.CcoeffNormed);
            double[] minValues, maxValues;
            Point[] minLocations, maxLocations;
            result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
            double similarityScore = maxValues[0];
            //
            double minVal = 0.8;
            double maxVal = 0.0;
            Point minLoc = new Point();
            Point maxLoc = new Point();
            CvInvoke.MinMaxLoc(imgout, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
            if (similarityScore > 0.7)
            {

                OnorOffRelay("1");
                Thread.Sleep(Int32.Parse(ControlAdruino.DelayStart) * 1000);
                CaptureandMoveMouse.Movemouseandclick(maxLoc, ImageInfor);
            }
            else
            {
                MessageBox.Show("Không tìm thấy nút Infor", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
            //timeroffinfor.Start();
        }

        private void timeroffinfor_Tick(object sender, EventArgs e)
        {
            timeroffinfor.Stop();
        }

        private void dtg_listerror_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                // Kiểm tra nếu cột đầu tiên của hàng này bằng "OK"
                var cell1 = dtg_listerror.Rows[e.RowIndex].Cells[5];
                if (cell1.Value != null && cell1.Value.ToString() == "NG")
                {
                    // Kiểm tra nếu cột thứ hai của hàng này bằng "NG"
                    var cell2 = dtg_listerror.Rows[e.RowIndex].Cells[6];
                    if ((cell2.Value != null && cell2.Value.ToString() == "NG") || cell2.Value == "")
                    {
                        // Đặt màu nền cho hàng này thành màu xanh
                        dtg_listerror.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else if (cell2.Value != null && cell2.Value.ToString() == "OK")
                    {
                        dtg_listerror.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                    }
                }
                else if (cell1.Value != null && cell1.Value.ToString() == "OK")
                {
                    var cell2 = dtg_listerror.Rows[e.RowIndex].Cells[6];
                    if ((cell2.Value != null && cell2.Value.ToString() == "NG"))
                    {
                        dtg_listerror.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
        }
    }
}
