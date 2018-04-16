using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;

namespace Task1
{
    public partial class UserControl1: UserControl
    {
        List<Brush> brushes = new List<Brush>()
                 {
                Brushes.Blue,
                Brushes.Orange,
                new Pen( Color.FromArgb(0,255,0)).Brush,
                Brushes.Yellow,
                Brushes.Cyan,
                Brushes.Red
                };
        public UserControl1()
        {
            InitializeComponent();
        }
        
        private void main_canvas_Paint(object sender, PaintEventArgs e)
        {
           
            var g = main_canvas.CreateGraphics();
            
            g.DrawRectangle(Pens.Blue, new Rectangle(0, 10, 100, 90));
            g.FillRectangle(Brushes.Blue, new Rectangle(0, 10, 100, 90));

            g.DrawRectangle(Pens.Orange, new Rectangle(100, 10, 100, 90));
            g.FillRectangle(Brushes.Orange, new Rectangle(100, 10, 100, 90));

            var b = new LinearGradientBrush(new Rectangle(200, 10, 100, 90), Color.FromArgb(0, 255, 0),
                Color.FromArgb(0, 255, 0),
                LinearGradientMode.Horizontal);
            g.FillRectangle(b, new Rectangle(200, 10, 100, 90));

            g.DrawRectangle(Pens.Yellow, new Rectangle(300, 10, 100, 90));
            g.FillRectangle(Brushes.Yellow, new Rectangle(300, 10, 100, 90));

            g.DrawRectangle(Pens.Cyan, new Rectangle(400, 10, 100, 90));
            g.FillRectangle(Brushes.Cyan, new Rectangle(400, 10, 100, 90));

            g.DrawRectangle(Pens.Red, new Rectangle(500, 10, 100, 90));
            g.FillRectangle(Brushes.Red, new Rectangle(500, 10, 100, 90));

            var a1 = new LinearGradientBrush(new Rectangle(0, 110, 100, 90),
                Color.Blue,
                Color.Orange,
                LinearGradientMode.Horizontal);
            g.FillRectangle(a1, new Rectangle(0, 110, 100, 90));

            var a2 = new LinearGradientBrush(new Rectangle(100, 110, 100, 90),
               Color.Orange,
               Color.FromArgb(0, 255, 0),
               LinearGradientMode.Horizontal);
            g.FillRectangle(a2, new Rectangle(100, 110, 100, 90));

            var a3 = new LinearGradientBrush(new Rectangle(200, 110, 100, 90),
              Color.FromArgb(0, 255, 0),
              Color.Yellow,
              LinearGradientMode.Horizontal);
            g.FillRectangle(a3, new Rectangle(200, 110, 100, 90));
            var a4 = new LinearGradientBrush(new Rectangle(300, 110, 100, 90),
              Color.Yellow,
              Color.Cyan,
              LinearGradientMode.Horizontal);
            g.FillRectangle(a4, new Rectangle(300, 110, 100, 90));

            var a5 = new LinearGradientBrush(new Rectangle(400, 110, 100, 90),
              Color.Cyan,
              Color.Red,
              LinearGradientMode.Horizontal);
            g.FillRectangle(a5, new Rectangle(400, 110, 100, 90));
           


        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem=="Discrete" && comboBox2.SelectedItem=="Value To Color")
            {
                int var1 = int.Parse(textBox1.Text);
                int var2 = int.Parse(textBox2.Text);
                int var3 = int.Parse(textBox3.Text);
                double var4 = var1;
                double var5 = var2;
                double var6 = var3;
                double i =Math.Floor( 6 * ((var6 - var4) / (var5 - var4)));
                for(int j=0;j<brushes.Count;j++)
                {
                    if((int)i==j)
                    {
                        var g = panel1.CreateGraphics();
                        var col = new Pen(brushes[j]).Color;
                        g.FillRectangle(brushes[j], new Rectangle(0, 0, panel1.Width, panel1.Height));
                        textBox4.Text = col.R.ToString() + " " + col.G.ToString() + " " + col.B.ToString();
                    }
                }
            }
            else if(comboBox1.SelectedItem=="Discrete" && comboBox2.SelectedItem=="Color To Value")
            {
                int R = int.Parse(textBox3.Text);
                int G = int.Parse(textBox5.Text);
                int B = int.Parse(textBox6.Text);
                var col2 = new Pen(brushes[4]).Color;
                //MessageBox.Show(col2.R.ToString() + " " + col2.G.ToString() + " " + col2.B.ToString());
               for(int i=0;i<brushes.Count;i++)
               {
                   var col = new Pen(brushes[i]).Color;
                   if(R==col.R && G==col.G && B==col.B)
                   {
                       int s_max=int.Parse(textBox2.Text);
                       
                       double result1 = (double)i / 6;
                       
                       double res =Math.Ceiling(result1 * s_max);
                       MessageBox.Show("The Result= "+ res.ToString());
                       textBox4.Text = res.ToString();
                       var g = panel1.CreateGraphics();
                       g.FillRectangle(brushes[i], new Rectangle(0, 0, panel1.Width, panel1.Height));
                   }
               }
            }
            else if(comboBox1.SelectedItem=="Continuous" && comboBox2.SelectedItem=="Value To Color")
            {
                int s_min = int.Parse(textBox1.Text);
                int s_max = int.Parse((textBox2.Text));
                int s = int.Parse(textBox3.Text);
               // double alpha = (double)(s - s_min) /(s_max - s_min);
                double index = 5 * ((double)(s - s_min) / (s_max - s_min));
                double alpha = index - (int)index;
                var c0 = new Pen(brushes[(int)index]).Color;
                var c1 = new Pen(brushes[(int)index + 1]).Color;
                double red = (int)c0.R + alpha * ((int)c1.R - (int)c0.R);
                double green = (int)c0.G + alpha * ((int)c1.G - (int)c0.G);
                double blue = (int)c0.B + alpha * ((int)c1.B - (int)c0.B);
                panel1.CreateGraphics().FillRectangle(new Pen(Color.FromArgb((int)red, (int)(green), (int)(blue))).Brush, new Rectangle(0, 0, panel1.Width, panel1.Height));
                textBox4.Text = red.ToString() + " " + green.ToString() + " " + blue.ToString();
            }
             else if(comboBox1.SelectedItem=="Continuous" && comboBox2.SelectedItem=="Color To Value")
            {
                int s_min = int.Parse(textBox1.Text);
                int s_max = int.Parse((textBox2.Text));
                int Red = int.Parse(textBox3.Text);
                int Green = int.Parse(textBox5.Text);
                int Blue = int.Parse(textBox6.Text);
                double s=0;
                double deltaS = (s_max - s_min) / 5;
                double Ds = 0;
                int res = 0;
                double alphaRed = -1;
                double alphaGreen = -1;
                double alphaBlue = -1;
                 for(int i=0;i<brushes.Count-1;i++)
                 {
                     var color1=new Pen(brushes[i]).Color;
                     var color2=new Pen(brushes[i+1]).Color;
                     int minR = Math.Min(color1.R, color2.R);
                     int maxR = Math.Max(color1.R, color2.R);
                     int minG = Math.Min(color1.G, color2.G);
                     int maxG = Math.Max(color1.G, color2.G);
                     int minB = Math.Min(color1.B, color2.B);
                     int maxB = Math.Max(color1.B, color2.B);
                     if ((Red <= maxR && Red >= minR) && (Green <= maxG && Green >= minG) && (Blue <= maxB && Blue >= minB)) 
                     {
                         if (color1.R != color2.R)
                         {
                             alphaRed = (double)(Red - color1.R) / (color2.R - color1.R);
                             res = i;
                             break;
                         }
                         if (color1.G != color2.G)
                         {
                             alphaGreen = (double)(Green - color1.G) / (color2.G - color1.G);
                             res = i;
                             break;
                         }
                         if (color1.B != color2.B)
                         {
                             alphaBlue = (double)(Blue - color1.B) / (color2.B - color1.B);
                             res = i;
                             break;
                         }
                         
                     }
                 }
                 if(alphaRed==-1)
                 {
                     if(alphaGreen!=-1)
                     {
                         alphaRed=alphaGreen;
                     }
                     else
                     {
                         alphaRed=alphaBlue;
                     }
                 }
                  if(alphaGreen==-1)
                 {
                     if(alphaRed!=-1)
                     {
                         alphaGreen = alphaRed;
                     }
                     else
                     {
                         alphaGreen=alphaBlue;
                     }
                 }
                 if(alphaBlue==-1)
                 {
                     if(alphaRed!=-1)
                     {
                         alphaBlue = alphaRed;
                     }
                     else
                     {
                         alphaBlue = alphaGreen;
                     }
                 }
                 Ds = alphaRed + res;
                 s = (Ds * deltaS) + s_min;
                 panel1.CreateGraphics().FillRectangle(new Pen(Color.FromArgb((int)Red, (int)(Green), (int)(Blue))).Brush, new Rectangle(0, 0, panel1.Width, panel1.Height));
                 textBox4.Text = s.ToString();
                 
            }
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
           
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if ((comboBox1.SelectedItem == "Discrete" || comboBox1.SelectedItem == "Continuous") && comboBox2.SelectedItem == "Color To Value")
            {
                label5.Text = "Enter the color value:";
            }
            else
            {
                label5.Text = "Enter the number of S";
            }*/
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
