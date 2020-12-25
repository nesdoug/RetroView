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
            comboBox1.SelectedIndex = 0;
        }

        // image size constants
        public const int MAX_WIDTH = 256;
        public const int MAX_HEIGHT = 240;
        public const int FLOYD_STEIN = 0;
        public const int BAYER8 = 1;
        public readonly int[,] BAYER_MATRIX = 
        {
            { 0,48,12,60,3,51,15,63 },
            { 32,16,44,28,35,19,47,31 },
            { 8,56,4,52,11,59,7,55 },
            { 40,24,36,20,43,27,39,23 },
            { 2,50,14,62,1,49,13,61 },
            { 34,18,46,30,33,17,45,29 },
            { 10,58,6,54,9,57,5,53 },
            { 42,26,38,22,41,25,37,21 }
        }; // 1/64 times this
        public const int BAYER_MULT = 64;

        // globals
        public static int dither_style = 0;
        public static Bitmap left_pic = new Bitmap(MAX_WIDTH, MAX_HEIGHT);
        public static Bitmap hidden_pic = new Bitmap(MAX_WIDTH, MAX_HEIGHT); // left pic resized
        public static Bitmap right_pic = new Bitmap(MAX_WIDTH, MAX_HEIGHT);
        public static Bitmap zoom_left_pic = new Bitmap(MAX_WIDTH, MAX_HEIGHT); // zoomed in
        public static Bitmap zoom_right_pic = new Bitmap(MAX_WIDTH, MAX_HEIGHT); // zoomed in
        public static Bitmap dont_crash_pic = new Bitmap(MAX_WIDTH, MAX_HEIGHT); // fix crash
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
        public static int out_size_x = 256, out_size_y = 240;
        public static int zoom_factor = 1;


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
        { // Process Button = convert the left pic to right pic
            if(has_loaded == 1)
            {
                has_processed = 1;

                //resize left --> hidden pic
                resize_image();

                //convert image hidden pic --> right_pic
                convert_image();


                //zoom it and draw to right side
                do_zoom();
            }
        }


        public void resize_image()
        {
            // blank the hidden
            for (int xx = 0; xx < MAX_WIDTH; xx++)
            {
                for (int yy = 0; yy < MAX_HEIGHT; yy++)
                {
                    hidden_pic.SetPixel(xx, yy, Color.Gray);
                }
            }

            Rectangle cloneRect = new Rectangle(0, 0, orig_width, orig_height);
            System.Drawing.Imaging.PixelFormat format = left_pic.PixelFormat;
            Bitmap cloneBMP = left_pic.Clone(cloneRect, format);

            int resize_width = orig_width;
            int resize_height = orig_height;
            if (out_size_x < orig_width)
            {
                resize_width = out_size_x;
            }
            if (out_size_y < orig_height)
            {
                resize_height = out_size_y;
            }
            
            //double check max and min, error proof it.
            if (resize_width < 1) resize_width = 1;
            if (resize_width > MAX_WIDTH) resize_width = MAX_WIDTH;
            if (resize_height < 1) resize_height = 1;
            if (resize_height > MAX_HEIGHT) resize_height = MAX_HEIGHT;

            // resize to fit output size
            using (Graphics g2 = Graphics.FromImage(hidden_pic))
            {
                g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g2.DrawImage(cloneBMP, 0, 0, resize_width, resize_height);
            }

            // hidden pic now has the resized image
        }


        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        { // load a new image to left box
            has_loaded = 1;

            // open dialogue, load image file

            using(OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files .png .jpg .bmp .gif)|*.png;*.jpg;*.bmp;*.gif|"+"All Files (*.*)|*.*";


                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    zoom_factor = 1;
                    comboBox1.SelectedIndex = 0;

                    // blank the left and right
                    for (int xx = 0; xx < MAX_WIDTH; xx++)
                    {
                        for (int yy = 0; yy < MAX_HEIGHT; yy++)
                        {
                            left_pic.SetPixel(xx, yy, Color.Gray);
                            dont_crash_pic.SetPixel(xx, yy, Color.Gray);
                        }
                    }
                    pictureBox2.Image = dont_crash_pic;
                    
                    Bitmap temp_bmp = new Bitmap(dlg.FileName);


                    int resize_width = MAX_WIDTH;
                    int resize_height = MAX_HEIGHT;
                    

                    float ratio1 = 1.0F;
                    float ratio2 = 1.0F;

                    if(temp_bmp.Width > MAX_WIDTH)
                    {
                        ratio1 = temp_bmp.Width / (float)MAX_WIDTH;
                    }
                    if(temp_bmp.Height > MAX_HEIGHT)
                    {
                        ratio2 = temp_bmp.Height / (float)MAX_HEIGHT;
                    }
                    // which is bigger? divide by that
                    if(ratio1 > ratio2)
                    {
                        resize_width = (int)Math.Round(temp_bmp.Width / ratio1);
                        resize_height = (int)Math.Round(temp_bmp.Height / ratio1);
                    }
                    else
                    {
                        resize_width = (int)Math.Round(temp_bmp.Width / ratio2);
                        resize_height = (int)Math.Round(temp_bmp.Height / ratio2);
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

                int max_out_width = out_size_x;
                if (orig_width < out_size_x) max_out_width = orig_width;

                int max_out_height = out_size_y;
                if (orig_height < out_size_y) max_out_height = orig_height;

                Rectangle cloneRect = new Rectangle(0, 0, max_out_width, max_out_height);
                System.Drawing.Imaging.PixelFormat format = right_pic.PixelFormat;
                Bitmap cloneBMP = right_pic.Clone(cloneRect, format);
                

                // open dialogue
                // save file
                // export image pic of the current view
                SaveFileDialog sfd = new SaveFileDialog();
                
                sfd.Filter = "PNG|*.png|BMP|*.bmp|JPG|*.jpg|GIF|*.gif";
                
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
                if (outvar < -100) outvar = -100;
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

        private void floydSteinbergToolStripMenuItem_Click(object sender, EventArgs e)
        {
            floydSteinbergToolStripMenuItem.Checked = true;
            orderedBayer8x8ToolStripMenuItem.Checked = false;
            dither_style = FLOYD_STEIN;
        }

        private void orderedBayer8x8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            floydSteinbergToolStripMenuItem.Checked = false;
            orderedBayer8x8ToolStripMenuItem.Checked = true;
            dither_style = BAYER8;
        }

        

        private void textBox21_Leave(object sender, EventArgs e)
        {
            dither_set();
        }

        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            // out x size
            if (e.KeyChar == (char)Keys.Return)
            {
                tb22_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox22_Leave(object sender, EventArgs e)
        {
            // out x size
            tb22_set();
        }

        private void tb22_set()
        {
            //out_size_x out_size_y
            string str = textBox22.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > MAX_WIDTH) outvar = MAX_WIDTH;
                if (outvar < 1) outvar = 1;
                
                textBox22.Text = outvar.ToString();

                out_size_x = outvar;
            }
            else
            {
                // revert back to previous
                textBox22.Text = out_size_x.ToString();
            }

        }

        private void textBox23_KeyPress(object sender, KeyPressEventArgs e)
        {
            // out y size
            if (e.KeyChar == (char)Keys.Return)
            {
                tb23_set();

                e.Handled = true; // prevent ding on return press
            }
        }

        private void textBox23_Leave(object sender, EventArgs e)
        {
            // out y size
            tb23_set();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //zoom functions
            if (comboBox1.SelectedIndex == 0) zoom_factor = 1;
            else if (comboBox1.SelectedIndex == 1) zoom_factor = 2;
            else if (comboBox1.SelectedIndex == 2) zoom_factor = 4;
            else if (comboBox1.SelectedIndex == 3) zoom_factor = 8;
            else if (comboBox1.SelectedIndex == 4) zoom_factor = 16;
            else zoom_factor = 1; // default

            // todo, redraw screens
            do_zoom();
        }


        public void do_zoom()
        {
            int clone_x, clone_y;

            clone_x = 256 / zoom_factor;
            clone_y = 240 / zoom_factor;
            if (clone_x < 1) clone_x = 1;
            if (clone_y < 1) clone_y = 1;

            Rectangle cloneRect = new Rectangle(0, 0, clone_x, clone_y);
            System.Drawing.Imaging.PixelFormat format = right_pic.PixelFormat;
            Bitmap cloneBMP = left_pic.Clone(cloneRect, format);

            using (Graphics g2 = Graphics.FromImage(zoom_left_pic))
            {
                g2.InterpolationMode = InterpolationMode.NearestNeighbor;
                g2.DrawImage(cloneBMP, 0, 0, 256, 240);
            }

            pictureBox1.Image = zoom_left_pic;
            pictureBox1.Refresh();


            // it was crashing here, cloning changed the size of
            // the right_pic, so I made yet another
            // bitmap and copied very slowly a full 256x240 pixels

            Color TempColor = Color.Black;

            for(int y = 0; y < 240; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    if((y< right_pic.Height) && (x < right_pic.Width))
                    {
                        TempColor = right_pic.GetPixel(x, y);
                        dont_crash_pic.SetPixel(x, y, TempColor);
                    }
                    else
                    {
                        dont_crash_pic.SetPixel(x, y, Color.Gray);
                    }
                }
            }


            cloneBMP = dont_crash_pic.Clone(cloneRect, format); 


            using (Graphics g2 = Graphics.FromImage(zoom_right_pic))
            {
                g2.InterpolationMode = InterpolationMode.NearestNeighbor;
                g2.DrawImage(cloneBMP, 0, 0, 256, 240);
            }

            pictureBox2.Image = zoom_right_pic;
            pictureBox2.Refresh();
        }

        

        private void tb23_set()
        {
            string str = textBox23.Text;
            int outvar = 0;
            if (int.TryParse(str, out outvar))
            {
                if (outvar > MAX_HEIGHT) outvar = MAX_HEIGHT;
                if (outvar < 1) outvar = 1;
                
                textBox23.Text = outvar.ToString();

                out_size_y = outvar;
            }
            else
            {
                // revert back to previous
                textBox23.Text = out_size_y.ToString();
            }

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

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 240;
            textBox23.Text = "240";
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
            
            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 240;
            textBox23.Text = "240";
            
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

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 240;
            textBox23.Text = "240";
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

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 240;
            textBox23.Text = "240";
        }



        private void zXSpectrumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 6;
            textBox1.Text = "6";
            sat_lev = 2;
            textBox2.Text = "2";
            light_lev = 2;
            textBox3.Text = "2";

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 192;
            textBox23.Text = "192";
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
            min_light = 10;
            textBox9.Text = "10";

            out_size_x = 160;
            textBox22.Text = "160";
            out_size_y = 144;
            textBox23.Text = "144";
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

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 192;
            textBox23.Text = "192";
        }

        private void nESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 12;
            textBox1.Text = "12";
            sat_lev = 2;
            textBox2.Text = "2";
            light_lev = 6;
            textBox3.Text = "6";

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 240;
            textBox23.Text = "240";
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

            out_size_x = 256; // NOTE should be 320, but I'd have to change a lot
            textBox22.Text = "256";
            out_size_y = 224;
            textBox23.Text = "224";
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

            out_size_x = 160;
            textBox22.Text = "160";
            out_size_y = 144;
            textBox23.Text = "144";
        }


        private void gameboyColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            red_lev = 32;
            textBox4.Text = "32";
            green_lev = 32;
            textBox5.Text = "32";
            blue_lev = 32;
            textBox6.Text = "32";

            out_size_x = 160;
            textBox22.Text = "160";
            out_size_y = 144;
            textBox23.Text = "144";
            //same as SNES but smaller screen
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

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 224;
            textBox23.Text = "224";
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

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 224;
            textBox23.Text = "224";
        }


        private void sepiaToneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 1;
            textBox1.Text = "1";
            min_hue = 25;
            textBox7.Text = "25";
            min_sat = 10;
            textBox8.Text = "10";

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 240;
            textBox23.Text = "240";
        }

        private void artSketchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 4;
            textBox1.Text = "4";
            sat_lev = 5;
            textBox2.Text = "5";
            max_sat = 50;
            textBox14.Text = "50";
            light_lev = 4;
            textBox3.Text = "4";
            min_light = 10;
            textBox9.Text = "10";
            max_light = 90;
            textBox15.Text = "90";

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 240;
            textBox23.Text = "240";
        }

        private void prettyInPinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_values();

            hue_lev = 1;
            textBox1.Text = "1";
            min_hue = 330;
            textBox7.Text = "330";
            sat_lev = 3;
            textBox2.Text = "3";
            min_sat = 10;
            textBox8.Text = "10";
            max_sat = 80;
            textBox14.Text = "80";

            out_size_x = 256;
            textBox22.Text = "256";
            out_size_y = 240;
            textBox23.Text = "240";
        }


        private void levelsChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Levels Chart\n1=always min\n2=1 bit\n4=2 bit\n8=3 bit\n16=4 bit\n32=5 bit\n64=6 bit\n128=7 bit\n256=8 bit");

        }


        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("RetroView\nby Doug Fraker, 2020.\nPosterize Color to reduce bit depth. \n\nVersion 1.1");

        }


        
    }
}
