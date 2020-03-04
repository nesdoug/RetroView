

using System;
using System.Drawing;

namespace RetroView
{
    public partial class Form1
    {
        public void convert_image()
        {
            double h_size, s_size, l_size;
            double h_level, s_level, l_level;
            double hue = 0, sat = 0, light = 0;
            int h_window;
            double s_window, l_window;
            int orig_val, diff;
            //float temp_fl;


            Color temp_color = Color.Black;
            // copy left to right
            Rectangle cloneRect = new Rectangle(0, 0, orig_width, orig_height);
            System.Drawing.Imaging.PixelFormat format = right_pic.PixelFormat;
            right_pic = left_pic.Clone(cloneRect, format);

            //apply brightness and contrast
            float contrastF = (float)(contrast + 50) / 50;

            for (int yy = 0; yy < orig_height; yy++)
            {
                for (int xx = 0; xx < orig_width; xx++)
                {
                    temp_color = right_pic.GetPixel(xx, yy);

                    float rr = (float)temp_color.R;
                    float gg = (float)temp_color.G;
                    float bb = (float)temp_color.B;

                    //apply brightness and contrast to each
                    rr -= 128;
                    rr *= contrastF;
                    rr += 128;
                    rr += brightness;
                    if (rr > 255) rr = 255;
                    if (rr < 0) rr = 0;

                    gg -= 128;
                    gg *= contrastF;
                    gg += 128;
                    gg += brightness;
                    if (gg > 255) gg = 255;
                    if (gg < 0) gg = 0;

                    bb -= 128;
                    bb *= contrastF;
                    bb += 128;
                    bb += brightness;
                    if (bb > 255) bb = 255;
                    if (bb < 0) bb = 0;

                    right_pic.SetPixel(xx, yy, Color.FromArgb((int)rr,(int)gg,(int)bb));
                }
            }


                //window = max_hue - min_hue;
                //multF = (float)min_hue + (colorF * (float)window / 255)
                h_level = hue_lev - 1;
            h_window = max_hue - min_hue;
            if (h_level == 0)
            {
                h_size = 0;
                
            }
            else
            {
                h_size = 360.0 / (float)hue_lev;
                // need 1 more, thus hue_lev not h_window
            }
            

            //actual_sat_min = min_sat / 100f;
            //actual_sat_max = max_sat / 100f;
            s_level = sat_lev - 1;
            s_window = max_sat - min_sat;
            if (s_level > 0)
            {
                s_size = 100.0 / s_level;

            }
            else
            {
                s_size = 0.0;
            }

            //actual_light_min = min_light / 100.0;
            //actual_light_max = max_light / 100.0;
            l_level = light_lev - 1;
            l_window = max_light - min_light;
            if (l_level > 0)
            {
                l_size = 100.0 / l_level;

            }
            else
            {
                l_size = 0.0;
            }

            erase_arrays();

            if ((hue_lev != 256) || (light_lev != 256) || (sat_lev != 256) ||
                (min_hue != 0) || (min_light != 0) || (min_sat != 0) ||
                (max_hue != 360) || (max_light != 100) || (max_sat != 100))
            {
                // process hue, sat, light / bright hsb hsl
                for (int yy = 0; yy < orig_height; yy++)
                {
                    for (int xx = 0; xx < orig_width; xx++)
                    {
                        temp_color = right_pic.GetPixel(xx, yy);
                        
                        float rr = (float)(temp_color.R / 255.0);
                        float gg = (float)(temp_color.G / 255.0);
                        float bb = (float)(temp_color.B / 255.0);
                        
                        float min = Math.Min(Math.Min(rr, gg), bb);
                        float max = Math.Max(Math.Max(rr, gg), bb);
                        float delta = max - min;
                        hue = 0;
                        sat = 0;
                        light = (double)((max + min) / 2.0);
                        if(delta != 0)
                        {
                            if(light < 0.5)
                            {
                                sat = (double)(delta / (max + min));
                            }
                            else
                            {
                                sat = (double)(delta / (max + min));
                                //sat = (double)(delta / (2.0 - max - min));
                            }

                            if(rr == max)
                            {
                                hue = (double)(gg - bb) / delta;
                            }
                            else if(gg == max)
                            {
                                hue = 2.0 + ((double)(bb - rr) / delta);
                            }
                            else
                            {
                                hue = 4.0 + ((double)(rr - gg) / delta);
                            }
                        }
                        hue *= 60;
                        if (hue < 0) hue += 360;
                        sat = sat * 100.0;
                        light = light * 100.0;

                        // process hue
                        if (h_level <= 128) // too error prone for 129+
                        {
                            if (h_size != 0)
                            {
                                //add hue to the dither array diff
                                hue = add_diff((int)Math.Round(hue), xx, yy, 0);
                                //get in range
                                if (hue > 359) hue = 359;
                                if (hue < 0) hue = 0;

                                orig_val = (int)hue;
                                hue = Math.Round((Math.Round(hue / h_size)) * h_size);
                                diff = orig_val - (int)hue;
                                process_dither(diff, xx, yy, 0);
                            }
                            else
                            { // 1 level
                                hue = min_hue;
                            }
                        }
                        //else hue = orig hue
                        if(h_size != 0)
                        {
                            hue = min_hue + (hue * h_window / 360.0);
                        }
                        

                        // error check
                        while (hue >= 360) hue -= 360;
                        while (hue < 0) hue += 360;



                        // process sat
                        if (s_level <= 128) // too error prone for 129+
                        {
                            if (s_size != 0.0)
                            {
                                //add sat to the dither array diff
                                sat = add_diff((int)Math.Round(sat), xx, yy, 1);
                                //get in range
                                if (sat > 100) sat = 100;
                                if (sat < 0) sat = 0;

                                orig_val = (int)sat;
                                sat = Math.Round((Math.Round(sat / s_size)) * s_size);
                                diff = orig_val - (int)sat;
                                process_dither(diff, xx, yy, 1);
                            }
                            else
                            { // 1 level
                                sat = min_sat;
                            }
                        }
                        //else sat = orig sat

                        if (s_size != 0)
                        {
                            sat = min_sat + (sat * s_window / 100.0);
                        }
                        

                        // error check
                        if (sat > 100.0) sat = 100.0;
                        if (sat < 0.0) sat = 0.0;
                        // back to standard
                        sat = sat / 100.0;


                        

                        // process light
                        if (l_level <= 128) // too error prone for 129+
                        {
                            if (l_size != 0.0)
                            {
                                //add light to the dither array diff
                                light = add_diff((int)Math.Round(light), xx, yy, 2);
                                //get in range
                                if (light > 100) light = 100;
                                if (light < 0) light = 0;

                                orig_val = (int)light;
                                light = Math.Round((Math.Round(light / l_size)) * l_size);
                                diff = orig_val - (int)light;
                                process_dither(diff, xx, yy, 2);
                            }
                            else
                            { // 1 level
                                light = min_light;
                            }
                        }
                        //else sat = orig sat
                        if (l_size != 0)
                        {
                            light = min_light + (light * l_window / 100.0);
                        }
                        

                        // error check
                        if (light > 100.0) light = 100.0;
                        if (light < 0.0) light = 0.0;
                        // back to standard
                        light = light / 100.0;


                        temp_color = GetHSB(hue, sat, light);

                        right_pic.SetPixel(xx, yy, temp_color);
                    }
                }
            }

            

            // process red, green, blue

            double red1, green1,  blue1;
            double r_window, g_window, b_window;
            double r_level, g_level, b_level;
            double r_size, g_size, b_size;
            int red2, green2, blue2;

            r_level = red_lev - 1;
            r_window = max_r - min_r;
            if (r_level > 0)
            {
                r_size = 255.0 / r_level;
            }
            else
            {
                r_size = 0;
            }

            g_level = green_lev - 1;
            g_window = max_g - min_g;
            if (g_level > 0)
            {
                g_size = 255.0 / g_level;
            }
            else
            {
                g_size = 0;
            }

            b_level = blue_lev - 1;
            b_window = max_b - min_b;
            if (b_level > 0)
            {
                b_size = 255.0 / b_level;
            }
            else
            {
                b_size = 0;
            }

            erase_arrays();

            

            for (int yy = 0; yy < orig_height; yy++)
            {
                for (int xx = 0; xx < orig_width; xx++)
                {
                    temp_color = right_pic.GetPixel(xx, yy);
                    red1 = temp_color.R;
                    green1 = temp_color.G;
                    blue1 = temp_color.B;

                    //cut bright contrast

                    // process red
                    if (r_level < 255)
                    {
                        if (r_size != 0)
                        {
                            //add red to the dither array diff
                            red1 = add_diff((int)Math.Round(red1), xx, yy, 0);
                            //get in range
                            if (red1 > 255) red1 = 255;
                            if (red1 < 0) red1 = 0;

                            orig_val = (int)red1;
                            red1 = Math.Round((Math.Round(red1 / r_size)) * r_size);
                            diff = orig_val - (int)red1;
                            process_dither(diff, xx, yy, 0);
                        }
                        else
                        { // 1 level
                            red1 = min_r;
                        }
                    }
                    //else hue = orig hue
                    if (r_size != 0)
                    {
                        red1 = min_r + (red1 * r_window / 255.0);
                    }
                    
                    red2 = (int)red1;
                    // error check
                    if (red2 > 255) red2 = 255;
                    if (red2 < 0) red2 = 0;

                    // process green
                    if (g_level < 255)
                    {
                        if (g_size != 0)
                        {
                            //add green to the dither array diff
                            green1 = add_diff((int)Math.Round(green1), xx, yy, 1);
                            //get in range
                            if (green1 > 255) green1 = 255;
                            if (green1 < 0) green1 = 0;

                            orig_val = (int)green1;
                            green1 = Math.Round((Math.Round(green1 / g_size)) * g_size);
                            diff = orig_val - (int)green1;
                            process_dither(diff, xx, yy, 1);
                        }
                        else
                        { // 1 level
                            green1 = min_g;
                        }
                    }
                    //else hue = orig hue
                    if (g_size != 0)
                    {
                        green1 = min_g + (green1 * g_window / 255.0);
                    }

                    green2 = (int)green1;
                    // error check
                    if (green2 > 255) green2 = 255;
                    if (green2 < 0) green2 = 0;

                    // process blue
                    if (b_level < 255)
                    {
                        if (b_size != 0)
                        {
                            //add blue to the dither array diff
                            blue1 = add_diff((int)Math.Round(blue1), xx, yy, 2);
                            //get in range
                            if (blue1 > 255) blue1 = 255;
                            if (blue1 < 0) blue1 = 0;

                            orig_val = (int)blue1;
                            blue1 = Math.Round((Math.Round(blue1 / b_size)) * b_size);
                            diff = orig_val - (int)blue1;
                            process_dither(diff, xx, yy, 2);
                        }
                        else
                        { // 1 level
                            blue1 = min_b;
                        }
                    }
                    //else hue = orig hue
                    if (b_size != 0)
                    {
                        blue1 = min_b + (blue1 * b_window / 255.0);
                    }
                    
                    blue2 = (int)blue1;
                    // error check
                    if (blue2 > 255) blue2 = 255;
                    if (blue2 < 0) blue2 = 0;


                    temp_color = Color.FromArgb(red2, green2, blue2);

                    right_pic.SetPixel(xx, yy, temp_color);
                }
            }

        }


        // note, there is no easy way to conver hsb to rgb
        // note H 0-360, S = 0-1.0, B = 0-1.0
        // got from fog creek software
        private Color GetHSB(double hue, double sat, double light)
        {
            if((sat < 0)||(sat > 1) || (light < 0) || (light > 1)){
                return Color.Black;
            }
            //Color temp_color2 = Color.Black;

            int i;
            double f, w, q, t;
            double r, g, b;
            double other_light;

            if(sat == 0.0)
            {
                i = (int)(light * 255.0);
                return Color.FromArgb(i, i, i);
            }
            if (hue >= 360.0)
            {
                hue = 0;
            }

            // this formula is wrong
            // it returns if sat 1.0 other colors zero, but they usually aren't
            hue = (hue / 60.0);
            i = (int)Math.Floor(hue);
            f = hue - (double)i;
            w = light * (1.0 - sat);
            q = light * (1.0 - (sat * f));
            t = light * (1.0 - (sat * (1.0 - f)));
            //bug fix
            //other_light = 0;
            
            switch (i)
            {
                case 0:
                    r = light;
                    g = t;
                    b = w;
                    break;
                case 1:
                    r = q;
                    g = light;
                    b = w;
                    break;
                case 2:
                    r = w;
                    g = light;
                    b = t;
                    break;
                case 3:
                    r = w;
                    g = q;
                    b = light;
                    break;
                case 4:
                    r = t;
                    g = w;
                    b = light;
                    break;
                case 5:
                default:
                    r = light;
                    g = w;
                    b = q;
                    break;

            }
            return Color.FromArgb((int)(r * 255f), (int)(g * 255f), (int)(b * 255f));
            
        }



        private void erase_arrays()
        {
            for(int i = 0; i < MAX_WIDTH; i++)
            {
                for (int j = 0; j < MAX_HEIGHT; j++)
                {
                    dither_array[i, j] = 0;
                    dither_array2[i, j] = 0;
                    dither_array3[i, j] = 0;
                }
            }
        }


        private int add_diff(int invalue, int xx, int yy, int which_arr)
        {

            if (which_arr == 0)
            {
                invalue = invalue + dither_array[xx, yy];
            }
            else if (which_arr == 1)
            {
                invalue = invalue + dither_array2[xx, yy];
            }
            else
            {
                invalue = invalue + dither_array3[xx, yy];
            }

            return invalue;
        }

        private void process_dither(int invalue, int xx, int yy, int which_arr)
        { // invalue is diff
            if (dither == 0) return;

            int outvalue, change;
            float temp_fl;
            temp_fl = (float)invalue;
            float dither_fl = (float)dither / 10;
            
            if(xx < 255)
            {
                change = (int)(temp_fl * 0.4375 * dither_fl); // 7/16 0.4375
                if (which_arr == 0)
                {
                    dither_array[xx + 1, yy] = dither_array[xx + 1, yy] + change;
                }
                else if (which_arr == 1)
                {
                    dither_array2[xx + 1, yy] = dither_array2[xx + 1, yy] + change;
                }
                else
                {
                    dither_array3[xx + 1, yy] = dither_array3[xx + 1, yy] + change;
                }
            }

            if (yy < 239)
            {
                if (xx > 0)
                {
                    change = (int)(temp_fl * 0.1875 * dither_fl); // 3/16 0.1875
                    if (which_arr == 0)
                    {
                        dither_array[xx - 1, yy + 1] = dither_array[xx - 1, yy + 1] + change;
                    }
                    else if (which_arr == 1)
                    {
                        dither_array2[xx - 1, yy + 1] = dither_array2[xx - 1, yy + 1] + change;
                    }
                    else
                    {
                        dither_array3[xx - 1, yy + 1] = dither_array3[xx - 1, yy + 1] + change;
                    }
                }

                //all x's
                change = (int)(temp_fl * 0.3125 * dither_fl); // 5/16 0.3125
                if (which_arr == 0)
                {
                    dither_array[xx, yy + 1] = dither_array[xx, yy + 1] + change;
                }
                else if (which_arr == 1)
                {
                    dither_array2[xx, yy + 1] = dither_array2[xx, yy + 1] + change;
                }
                else
                {
                    dither_array3[xx, yy + 1] = dither_array3[xx, yy + 1] + change;
                }

                if (xx < 255)
                {
                    change = (int)(temp_fl * 0.0625 * dither_fl); // 1/16 0.0625
                    if (which_arr == 0)
                    {
                        dither_array[xx + 1, yy + 1] = dither_array[xx + 1, yy + 1] + change;
                    }
                    else if (which_arr == 1)
                    {
                        dither_array2[xx + 1, yy + 1] = dither_array2[xx + 1, yy + 1] + change;
                    }
                    else
                    {
                        dither_array3[xx + 1, yy + 1] = dither_array3[xx + 1, yy + 1] + change;
                    }
                }

            }

            
        }
    }
}


