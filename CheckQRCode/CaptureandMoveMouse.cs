using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CheckQRCode
{
    public static class CaptureandMoveMouse
    {
        // mousemove and click
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //mouse andclick
        // gétcale
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }
        public static float getScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }
        // endgetscale
        private static class Win32Native
        {
            public const int DESKTOPVERTRES = 0x75;
            public const int DESKTOPHORZRES = 0x76;

            [DllImport("gdi32.dll")]
            public static extern int GetDeviceCaps(IntPtr hDC, int index);
        }
        public static void Capture(string pathsave)
        {
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;
            IntPtr hwnd = IntPtr.Zero;
            int width, height;
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                var hDC = g.GetHdc();
                width = Win32Native.GetDeviceCaps(hDC, Win32Native.DESKTOPHORZRES);
                height = Win32Native.GetDeviceCaps(hDC, Win32Native.DESKTOPVERTRES);
                g.ReleaseHdc(hDC);
            }

            using (var img = new Bitmap(width, height))
            {
                using (var g = Graphics.FromImage(img))
                {
                    g.CopyFromScreen(0, 0, 0, 0, img.Size);
                }
                img.Save(pathsave);
            }
        }
        public static void Movemouseandclick(Point pointmaxlot, Image<Bgr, byte> Imagecheck)
        {
            int Xmouse = (int)(pointmaxlot.X * 1 / getScalingFactor() + Imagecheck.Width/getScalingFactor() / 2);
            int Ymouse =(int)(pointmaxlot.Y * 1 / getScalingFactor() + Imagecheck.Height/getScalingFactor() / 2);
            SetCursorPos(Xmouse, Ymouse);
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)Xmouse, (uint)Ymouse, 0, 0); // Thực hiện nhấn chuột trái tại vị trí đã chọn
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)Xmouse, (uint)Ymouse, 0, 0); // Thực hiện giải phóng chuột tại vị trí đã chọn

        }
    }
}
