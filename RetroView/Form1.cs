using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RetroView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // image size constants
        public const int MAX_WIDTH = 256;
        public const int MAX_HEIGHT = 240;

        // globals
        public static Bitmap left_pic = new Bitmap(MAX_WIDTH, MAX_HEIGHT);
        public static Bitmap right_pic = new Bitmap(MAX_WIDTH, MAX_HEIGHT);
        public static int[,] dither_array = new int[MAX_WIDTH, MAX_HEIGHT];
        public static int[,] dither_array2 = new int[MAX_WIDTH, MAX_HEIGHT];
        public static int[,] dither_array3 = new int[MAX_WIDTH, MAX_HEIGHT];
        public static int has_loaded, has_processed;
        public static int hue_lev = 256, light_lev = 256, sat_lev = 256;
        public static int red_lev = 256, green_lev = 256, blue_lev = 256;
        public static int min_hue, min_light, min_sat, min_r, min_g, min_b;
        public static int max_hue = 360, max_light = 100, max_sat = 100;
        public static int max_r = 255, max_g = 255, max_b = 255;
        public static int orig_width, orig_height;
        public static int brightness = 0, contrast = 0, dither = 6;


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        { // hue levels
            if (e.KeyChar == (char)Keys.Return)
            {
                tb1_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            tb1_set();
        }

        private void tb1_set()
        {
            string str = textBox1.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 256;
                else if (outvar > 64) outvar = 64;
                if (outvar < 1) outvar = 1;
                hue_lev = outvar;
                textBox1.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox1.Text = hue_lev.ToString();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        { // light levels
            if (e.KeyChar == (char)Keys.Return)
            {
                tb2_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            //textBox2.Text = sat_lev.ToString();
            tb2_set();
        }

        private void tb2_set()
        {
            string str = textBox2.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 256;
                else if (outvar > 64) outvar = 64;
                if (outvar < 1) outvar = 1;
                sat_lev = outvar;
                textBox2.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox2.Text = sat_lev.ToString();
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        { // sat levels
            if (e.KeyChar == (char)Keys.Return)
            {
                tb3_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            //textBox3.Text = light_lev.ToString();
            tb3_set();
        }

        private void tb3_set()
        {
            string str = textBox3.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 256;
                else if (outvar > 64) outvar = 64;
                if (outvar < 1) outvar = 1;
                light_lev = outvar;
                textBox3.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox3.Text = light_lev.ToString();
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        { // red levels
            if (e.KeyChar == (char)Keys.Return)
            {
                tb4_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            //textBox4.Text = red_lev.ToString();
            tb4_set();
        }

        private void tb4_set()
        {
            string str = textBox4.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 256;
                else if (outvar > 64) outvar = 64;
                if (outvar < 1) outvar = 1;
                red_lev = outvar;
                textBox4.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox4.Text = red_lev.ToString();
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        { // green levels
            if (e.KeyChar == (char)Keys.Return)
            {
                tb5_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            //textBox5.Text = green_lev.ToString();
            tb5_set();
        }

        private void tb5_set()
        {
            string str = textBox5.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 256;
                else if (outvar > 64) outvar = 64;
                if (outvar < 1) outvar = 1;
                green_lev = outvar;
                textBox5.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox5.Text = green_lev.ToString();
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        { // blue levels
            if (e.KeyChar == (char)Keys.Return)
            {
                tb6_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox6_Leave(object sender, EventArgs e)
        {
            //textBox6.Text = blue_lev.ToString();
            tb6_set();
        }

        private void tb6_set()
        {
            string str = textBox6.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 256;
                else if (outvar > 64) outvar = 64;
                if (outvar < 1) outvar = 1;
                blue_lev = outvar;
                textBox6.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox6.Text = blue_lev.ToString();
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        { // hue min
            if (e.KeyChar == (char)Keys.Return)
            {
                tb7_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox7_Leave(object sender, EventArgs e)
        {
            //textBox7.Text = min_hue.ToString();
            tb7_set();
        }

        private void tb7_set()
        {
            string str = textBox7.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 360) outvar = 360;
                if (outvar < 0) outvar = 0;
                min_hue = outvar;
                textBox7.Text = outvar.ToString();

                if (min_hue > max_hue)
                {
                    max_hue = min_hue;
                    textBox13.Text = max_hue.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox7.Text = min_hue.ToString();
            }
        }
        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        { // sat min
            if (e.KeyChar == (char)Keys.Return)
            {
                tb8_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox8_Leave(object sender, EventArgs e)
        {
            //textBox8.Text = min_sat.ToString();
            tb8_set();
        }


        private void tb8_set()
        {
            string str = textBox8.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 100) outvar = 100;
                if (outvar < 0) outvar = 0;
                min_sat = outvar;
                textBox8.Text = outvar.ToString();

                if (min_sat > max_sat)
                {
                    max_sat = min_sat;
                    textBox14.Text = max_sat.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox8.Text = min_sat.ToString();
            }
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        { // light min
            if (e.KeyChar == (char)Keys.Return)
            {
                tb9_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox9_Leave(object sender, EventArgs e)
        {
            //textBox9.Text = min_light.ToString();
            tb9_set();
        }

        private void tb9_set()
        {
            string str = textBox9.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 100) outvar = 100;
                if (outvar < 0) outvar = 0;
                min_light = outvar;
                textBox9.Text = outvar.ToString();

                if (min_light > max_light)
                {
                    max_light = min_light;
                    textBox15.Text = max_light.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox9.Text = min_light.ToString();
            }
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        { // red min
            if (e.KeyChar == (char)Keys.Return)
            {
                tb10_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox10_Leave(object sender, EventArgs e)
        {
            //textBox10.Text = min_r.ToString();
            tb10_set();
        }


        private void tb10_set()
        {
            string str = textBox10.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 255;
                if (outvar < 0) outvar = 0;
                min_r = outvar;
                textBox10.Text = outvar.ToString();

                if (min_r > max_r)
                {
                    max_r = min_r;
                    textBox16.Text = max_r.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox10.Text = min_r.ToString();
            }
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        { // green min
            if (e.KeyChar == (char)Keys.Return)
            {
                tb11_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox11_Leave(object sender, EventArgs e)
        {
            //textBox11.Text = min_g.ToString();
            tb11_set();
        }

        private void tb11_set()
        {
            string str = textBox11.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 255;
                if (outvar < 0) outvar = 0;
                min_g = outvar;
                textBox11.Text = outvar.ToString();

                if (min_g > max_g)
                {
                    max_g = min_g;
                    textBox17.Text = max_g.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox11.Text = min_g.ToString();
            }
        }

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        { // blue min
            if (e.KeyChar == (char)Keys.Return)
            {
                tb12_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox12_Leave(object sender, EventArgs e)
        {
            //textBox12.Text = min_b.ToString();
            tb12_set();
        }

        private void tb12_set()
        {
            string str = textBox12.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 255;
                if (outvar < 0) outvar = 0;
                min_b = outvar;
                textBox12.Text = outvar.ToString();

                if (min_b > max_b)
                {
                    max_b = min_b;
                    textBox18.Text = max_b.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox12.Text = min_b.ToString();
            }
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        { // hue max
            if (e.KeyChar == (char)Keys.Return)
            {
                tb13_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox13_Leave(object sender, EventArgs e)
        {
            //textBox13.Text = max_hue.ToString();
            tb13_set();
        }


        private void tb13_set()
        {
            string str = textBox13.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 360) outvar = 360;
                if (outvar < 0) outvar = 0;
                max_hue = outvar;
                textBox13.Text = outvar.ToString();

                if (max_hue < min_hue)
                {
                    min_hue = max_hue;
                    textBox7.Text = min_hue.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox13.Text = max_hue.ToString();
            }
        }

        private void textBox14_KeyPress(object sender, KeyPressEventArgs e)
        { // sat max
            if (e.KeyChar == (char)Keys.Return)
            {
                tb14_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox14_Leave(object sender, EventArgs e)
        {
            //textBox14.Text = max_sat.ToString();
            tb14_set();
        }


        private void tb14_set()
        {
            string str = textBox14.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 100) outvar = 100;
                if (outvar < 0) outvar = 0;
                max_sat = outvar;
                textBox14.Text = outvar.ToString();

                if (max_sat < min_sat)
                {
                    min_sat = max_sat;
                    textBox8.Text = min_sat.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox14.Text = max_sat.ToString();
            }
        }

        private void textBox15_KeyPress(object sender, KeyPressEventArgs e)
        { // light max
            if (e.KeyChar == (char)Keys.Return)
            {
                tb15_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox15_Leave(object sender, EventArgs e)
        {
            //textBox15.Text = max_light.ToString();
            tb15_set();
        }


        private void tb15_set()
        {
            string str = textBox15.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 100) outvar = 100;
                if (outvar < 0) outvar = 0;
                max_light = outvar;
                textBox15.Text = outvar.ToString();

                if (max_light < min_light)
                {
                    min_light = max_light;
                    textBox9.Text = min_light.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox15.Text = max_light.ToString();
            }
        }

        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
        { // red max
            if (e.KeyChar == (char)Keys.Return)
            {
                tb16_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        
        private void textBox16_Leave(object sender, EventArgs e)
        {
            //textBox16.Text = max_r.ToString();
            tb16_set();
        }


        private void tb16_set()
        {
            string str = textBox16.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 255;
                if (outvar < 0) outvar = 0;
                max_r = outvar;
                textBox16.Text = outvar.ToString();

                if (max_r < min_r)
                {
                    min_r = max_r;
                    textBox10.Text = min_r.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox16.Text = max_r.ToString();
            }
        }

        private void textBox17_KeyPress(object sender, KeyPressEventArgs e)
        { // green max
            if (e.KeyChar == (char)Keys.Return)
            {
                tb17_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox17_Leave(object sender, EventArgs e)
        {
            //textBox17.Text = max_g.ToString();
            tb17_set();
        }


        private void tb17_set()
        {
            string str = textBox17.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 255;
                if (outvar < 0) outvar = 0;
                max_g = outvar;
                textBox17.Text = outvar.ToString();

                if (max_g < min_g)
                {
                    min_g = max_g;
                    textBox11.Text = min_g.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox17.Text = max_g.ToString();
            }
        }

        private void textBox18_KeyPress(object sender, KeyPressEventArgs e)
        { // blue max
            if (e.KeyChar == (char)Keys.Return)
            {
                tb18_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox18_Leave(object sender, EventArgs e)
        {
            //textBox18.Text = max_b.ToString();
            tb18_set();
        }

        

        private void tb18_set()
        {
            string str = textBox18.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 255) outvar = 255;
                if (outvar < 0) outvar = 0;
                max_b = outvar;
                textBox18.Text = outvar.ToString();

                if (max_b < min_b)
                {
                    min_b = max_b;
                    textBox12.Text = min_b.ToString();
                }
            }
            else
            {
                // revert back to previous
                textBox18.Text = max_b.ToString();
            }
        }



        // ************************************************



        private void button1_Click(object sender, EventArgs e)
        { // convert the left pic to right pic
            if(has_loaded == 1)
            {
                has_processed = 1;

                //convert image left_pic --> right_pic
                convert_image();

                //right_pic = left_pic;
                pictureBox2.Image = right_pic;
                pictureBox2.Refresh();

            }
        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        { // load a new image to left box
            has_loaded = 1;

            // open dialogue, load image file

            using(OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files .png .jpg .bmp .gif)|*.png;*.jpg;*.bmp;*.gif|"+"All Files (*.*)|*.*";


                //"png files (*.png)|*.png|bmp files (*.bmp)|*.bmp|jpg files (*.jpg)|*.jpg";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // blank the left
                    for(int xx = 0; xx < MAX_WIDTH; xx++)
                    {
                        for (int yy = 0; yy < MAX_HEIGHT; yy++)
                        {
                            left_pic.SetPixel(xx, yy, Color.Gray);
                        }
                    }
                    
                    Bitmap temp_bmp = new Bitmap(dlg.FileName);


                    int resize_width = MAX_WIDTH;
                    int resize_height = MAX_HEIGHT;
                    if(temp_bmp.Width < MAX_WIDTH)
                    {
                        resize_width = temp_bmp.Width;
                    }
                    if (temp_bmp.Height < MAX_HEIGHT)
                    {
                        resize_height = temp_bmp.Height;
                    }
                    // keep size ratio of original file
                    float wid = (float)temp_bmp.Width;
                    float hit = (float)temp_bmp.Height;
                    float ratio = (float)((MAX_HEIGHT*wid) / (MAX_WIDTH * hit));

                    if (ratio >= 1.0)
                    { //wider than tall, shrink height
                        hit = (float)resize_height / ratio;
                        resize_height = (int)hit;
                    }
                    else
                    { //taller than wide, shrink width
                        wid = (float)resize_width * ratio;
                        resize_width = (int)wid;
                    }
                    //double check max and min, error proof it.
                    if (resize_width < 1) resize_width = 1;
                    if (resize_width > MAX_WIDTH) resize_width = MAX_WIDTH;
                    if (resize_height < 1) resize_height = 1;
                    if (resize_height > MAX_HEIGHT) resize_height = MAX_HEIGHT;

                    orig_width = resize_width; // save these for later
                    orig_height = resize_height;

                    // resize to fit
                    using (Graphics g2 = Graphics.FromImage(left_pic))
                    {
                        g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g2.DrawImage(temp_bmp, 0, 0, resize_width, resize_height);
                    }

                    // save to picture box

                    pictureBox1.Image = left_pic;
                    pictureBox1.Refresh();
                    
                }

            }

        }

        

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        { // save the right box
            if((has_loaded == 1) && (has_processed == 1))
            {

                Rectangle cloneRect = new Rectangle(0, 0, orig_width, orig_height);
                System.Drawing.Imaging.PixelFormat format = right_pic.PixelFormat;
                Bitmap cloneBMP = right_pic.Clone(cloneRect, format);
                

                // open dialogue
                // save file
                // export image pic of the current view
                SaveFileDialog sfd = new SaveFileDialog();
                //sfd.Filter = "Images|*.png;*.bmp;*.jpg;*.gif";
                sfd.Filter = "PNG|*.png|BMP|*.bmp|JPG|*.jpg|GIF|*.gif";
                //ImageFormatConverter format = ImageFormatConverter.StandardValuesCollection;
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string ext = System.IO.Path.GetExtension(sfd.FileName);
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                            cloneBMP.Save(sfd.FileName, ImageFormat.Jpeg);
                            break;
                        case ".bmp":
                            cloneBMP.Save(sfd.FileName, ImageFormat.Bmp);
                            break;
                        case ".gif":
                            cloneBMP.Save(sfd.FileName, ImageFormat.Gif);
                            break;
                        default:
                            cloneBMP.Save(sfd.FileName, ImageFormat.Png);
                            break;

                    }
                }
            }
            else
            {
                MessageBox.Show("Error. Picture either hasn't been loaded, or hasn't been processed.");
            }
        }

        private void textBox19_KeyPress(object sender, KeyPressEventArgs e)
        { // brightness
            if (e.KeyChar == (char)Keys.Return)
            {
                brightness_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox19_Leave(object sender, EventArgs e)
        {
            brightness_set();
        }

        private void brightness_set()
        {
            string str = textBox19.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 250) outvar = 250;
                if (outvar < -250) outvar = -250;
                brightness = outvar;
                textBox19.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox19.Text = brightness.ToString();
            }
        }

        private void textBox20_KeyPress(object sender, KeyPressEventArgs e)
        { // contrast
            if (e.KeyChar == (char)Keys.Return)
            {
                contrast_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox20_Leave(object sender, EventArgs e)
        {
            contrast_set();
        }

        private void contrast_set()
        {
            string str = textBox20.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 100) outvar = 100;
                if (outvar < 0) outvar = 0;
                contrast = outvar;
                textBox20.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox20.Text = contrast.ToString();
            }
        }

        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)
        { // dither
            if (e.KeyChar == (char)Keys.Return)
            {
                dither_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox21_Leave(object sender, EventArgs e)
        {
            dither_set();
        }

        private void dither_set()
        {
            string str = textBox21.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > 12) outvar = 12;
                if (outvar < 0) outvar = 0;
                dither = outvar;
                textBox21.Text = outvar.ToString();
            }
            else
            {
                // revert back to previous
                textBox21.Text = dither.ToString();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        { // close the app
            // close the program
            Application.Exit();
        }


        private void reset_values()
        {
            hue_lev = 256;
            textBox1.Text = "256";
            sat_lev = 256;
            textBox2.Text = "256";
            light_lev = 256;
            textBox3.Text = "256";
            red_lev = 256;
            textBox4.Text = "256";
            green_lev = 256;
            textBox5.Text = "256";
            blue_lev = 256;
            textBox6.Text = "256";

            min_hue = 0;
            textBox7.Text = "0";
            min_sat = 0;
            textBox8.Text = "0";
            min_light = 0;
            textBox9.Text = "0";
            min_r = 0;
            textBox10.Text = "0";
            min_g = 0;
            textBox11.Text = "0";
            min_b = 0;
            textBox12.Text = "0";

            max_hue = 360;
            textBox13.Text = "360";
            max_sat = 100;
            textBox14.Text = "100";
            max_light = 100;
            textBox15.Text = "100";
            max_r = 255;
            textBox16.Text = "255";
            max_g = 255;
            textBox17.Text = "255";
            max_b = 255;
            textBox18.Text = "255";
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        { // reset
            reset_values();
        }

        private void bitBWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();
            
            sat_lev = 1;
            textBox2.Text = "1";
            light_lev = 2;
            textBox3.Text = "2";
            max_sat = 0;
            textBox14.Text = "0";
        }


        private void bitGreenMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            green_lev = 2;
            textBox5.Text = "2";
            max_r = 0;
            textBox16.Text = "0";
            max_b = 0;
            textBox18.Text = "0";
        }

        private void bitRGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            red_lev = 2;
            textBox4.Text = "2";
            green_lev = 2;
            textBox5.Text = "2";
            blue_lev = 2;
            textBox6.Text = "2";
        }

        
        private void zXSpectrumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 6;
            textBox1.Text = "6";
            sat_lev = 3;
            textBox2.Text = "3";
            light_lev = 2;
            textBox3.Text = "2";
        }


        private void gameboyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 1;
            textBox1.Text = "1";
            sat_lev = 1;
            textBox2.Text = "1";
            light_lev = 4;
            textBox3.Text = "4";

            min_hue = 90;
            textBox7.Text = "90";
            min_sat = 90;
            textBox8.Text = "90";
            min_light = 20;
            textBox9.Text = "20";
        }

        private void segaMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            red_lev = 4;
            textBox4.Text = "4";
            green_lev = 4;
            textBox5.Text = "4";
            blue_lev = 4;
            textBox6.Text = "4";
        }

        private void nESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 12;
            textBox1.Text = "12";
            sat_lev = 3;
            textBox2.Text = "3";
            light_lev = 6;
            textBox3.Text = "6";

            min_sat = 10;
            textBox8.Text = "10";
        }

        /*private void atari7800ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 15;
            textBox1.Text = "15";
            sat_lev = 3;
            textBox2.Text = "3";
            light_lev = 16;
            textBox3.Text = "16";
        }*/

        private void segaGenesisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            red_lev = 8;
            textBox4.Text = "8";
            green_lev = 8;
            textBox5.Text = "8";
            blue_lev = 8;
            textBox6.Text = "8";
        }

        private void gameGearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            red_lev = 16;
            textBox4.Text = "16";
            green_lev = 16;
            textBox5.Text = "16";
            blue_lev = 16;
            textBox6.Text = "16";
        }

        private void sNESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            red_lev = 32;
            textBox4.Text = "32";
            green_lev = 32;
            textBox5.Text = "32";
            blue_lev = 32;
            textBox6.Text = "32";
        }

        private void sNESDirectColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            red_lev = 16;
            textBox4.Text = "16";
            green_lev = 16;
            textBox5.Text = "16";
            blue_lev = 8;
            textBox6.Text = "8";

            max_r = 247;
            textBox16.Text = "247";
            max_g = 247;
            textBox17.Text = "247";
            max_b = 231;
            textBox18.Text = "231";
        }








        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("RetroView\nby Doug Fraker, 2020.\nPosterize Color to reduce bit depth. \n\nVersion 1.0");

        }


        
    }
}
