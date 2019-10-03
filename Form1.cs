using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translatesub
{
    public partial class Form1 : Form
    {
        string nVDO = "";
        Image img;
        Bitmap bitmap,bmp;
        Color color;
        int value_color = 50;
        int check = 0 ;
        float sizeimg_H = 0 , sizeimg_W =0 ;
        List<Bitmap> bitmaps = new List<Bitmap>();
        string startupPath = Environment.CurrentDirectory;
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            var size = pictureBox1.Size;
            trackBar1.Maximum = size.Height;
            panelcut.Visible = false;
        }

        private void Browse_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                nVDO = openFileDialog1.SafeFileName;
                txtInputFile.Text = openFileDialog1.SafeFileName;
                txtInputFile.Visible = true;
                Debug.WriteLine(openFileDialog1.FileName);

                axVLCPlugin21.playlist.add(openFileDialog1.FileName, openFileDialog1.SafeFileName, null);
                axVLCPlugin21.playlist.play();

            }

            if (!Directory.Exists(startupPath + "/" + nVDO))
            {
                Directory.CreateDirectory(startupPath + "/" + nVDO);
            }
            LaunchCommandLineApp(openFileDialog1.FileName, startupPath, nVDO);

            

            string[] files = Directory.GetFiles(startupPath + "/" + nVDO, "*.jpg");
            DataTable table = new DataTable();
            table.Columns.Add("File Name");

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = new FileInfo(files[i]);
                table.Rows.Add(file.Name);
                
            }

            picDataGrid.DataSource = table;
            
            
        }
        static void LaunchCommandLineApp(String nameVDO, String startupPath, String nVDO)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "ffmpeg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = " -i \"" + nameVDO + "\" -vf fps=1 \"" + startupPath + "/" + nVDO + "\"/%04d.jpg";

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                MessageBox.Show("error");
            }

        }
        private void PicDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string imgName = picDataGrid.CurrentRow.Cells[0].Value.ToString();
            //รูปปกติ
            img = Image.FromFile(startupPath + "/" + nVDO + "/" + imgName);
            bitmap = (Bitmap)img;
            var size = pictureBox1.Size;
            //รูปที่รี
            bitmap = ResizeImage(img, size.Width, size.Height);
            pictureBox1.Image = bitmap;
            

        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = trackBar1.Value.ToString();
            panelcut.Visible = true;
            panelcut.Location = new Point(63, 399- trackBar1.Value);
        }

        private void Button1_Click(object sender, EventArgs e)
        {


            float reimg_h = (trackBar1.Value* 100) / bitmap.Height;
            //ไซต์ image crob 
            sizeimg_H = (img.Height * reimg_h) / 100;

            Console.WriteLine("sum = "+ reimg_h + "     "+img.Height  + "   "+ sizeimg_H );

            DirectoryInfo di = new DirectoryInfo(startupPath + "/" + nVDO);
            FileInfo[] Images = di.GetFiles("*.jpg");
            for (int i = 0; i < Images.Length; i++)
            {

                bmp = new Bitmap(Images[i].FullName);
                Bitmap bmpCrop = bmp.Clone(new Rectangle(0, bmp.Height - (int)sizeimg_H, bmp.Width, (int)sizeimg_H), bmp.PixelFormat);
                //list
                bitmaps.Add(bmpCrop);
                
            }
           
        }      

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null && check == 0)
            {
                color = bitmap.GetPixel(e.X, e.Y);
                panelcolor.BackColor = color;
            }

        }
        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                color = bitmap.GetPixel(e.X, e.Y);
                panelcolor.BackColor = color;
                check = 1;
            }

        }

        private void crop(Bitmap bmpcrop )
        {

            int a = color.A + value_color;
            int r = color.R + value_color;
            int g = color.G + value_color;
            int b = color.B + value_color;

            int a2 = color.A - value_color;
            int r2 = color.R - value_color;
            int g2 = color.G - value_color;
            int b2 = color.B - value_color;
            
            
            


            /* for (int y = 0; y < bmpCrop.Height; y++)
             {
                 for (int x = 0; x < bmpCrop.Width; x++)
                 {

                     bmp_color = bmpCrop.GetPixel(x, y);
                     int aa = bmp_color.A;
                     int rr = bmp_color.R;
                     int gg = bmp_color.G;
                     int bb = bmp_color.B;
                     if (rr >= r2 && rr <= r && gg >= g2 && gg <= g && bb >= b2 && bb <= b)
                         bmpCrop.SetPixel(x, y, sub_color);
                     else
                         bmpCrop.SetPixel(x, y, back_color);

                 }

             }*/
           // bmpCrop.Save(@"C:\Users\Anongrat\Desktop\output\" + imgName);

           // pictureBox2.Image = bmpCrop;
            
        }
    }
}
