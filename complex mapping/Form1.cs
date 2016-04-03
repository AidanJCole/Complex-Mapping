using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSML;

namespace complex_mapping
{
    public partial class Form1 : Form
    {

        static Double VERTEX_DISTANCE = 0.84248208146200745911;
        static Double MID_DISTANCE =    0.62686966290617781414;
        List<Matrix> transformations = new List<Matrix>();
        List<Complex> piece1 = new List<Complex>();
        List<List<Complex>> parts = new List<List<Complex>>();
        List<Complex> cameraPolygon = new List<Complex>();
        //List<Complex> firstShape = new List<Complex>();
        Complex location = new Complex(0, 0);
        Complex w = new Complex(1, 0);
        Boolean down1 = false;
        Boolean down2 = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int pointNumber = 256;
            Complex end = new Complex(Math.Cos(Math.PI / 4), Math.Sin(Math.PI / 4));
            for (int i = 0; i < pointNumber; i++)
            {
                cameraPolygon.Add(new Complex(i * end.Re / pointNumber, i * end.Im / pointNumber));
            }
            double thetaRange = Math.PI / 2;
            double thetaStart = Math.PI / 4;
            for (int i = 0; i < pointNumber; i++)
            {
                cameraPolygon.Add(new Complex(Math.Cos(thetaStart - i * thetaRange / pointNumber),
                    Math.Sin(thetaStart - i * thetaRange / pointNumber)));
            }
            for (int i = 0; i < pointNumber; i++)
            {
                cameraPolygon.Add(new Complex(
                    Math.Cos(thetaStart - thetaRange) - Math.Cos(thetaStart - thetaRange) * i / pointNumber,
                    Math.Sin(thetaStart - thetaRange) - Math.Sin(thetaStart - thetaRange) * i / pointNumber));
            }
            //for (int i = 0; i < 16; i++) {
            //    firstShape.Add(new Complex(.5, -.5 + 1.0 / 16 * i));
            //}


            // Let's start with our first quadrilateral.
            // start is 0,0
            piece1.Add(new Complex(0, 0));
            // Next is a new Complex, multiplied by pi/5
            Complex nextPiece = Complex.DistanceToCoordinate(MID_DISTANCE) * new Complex(Math.Cos(Math.PI / 5), Math.Sin(Math.PI / 5));
            piece1.Add(nextPiece);
            // Next is the vertex
            piece1.Add(Complex.DistanceToCoordinate(VERTEX_DISTANCE));
            // finally is the conjugate of nextPiece
            piece1.Add(Complex.Conj(nextPiece));

            Complex rotation = new Complex(Math.Cos(Math.PI / 10), Math.Sin(Math.PI  / 10));
            Matrix rotation2 = rotation * Matrix.Eye(2);
            transformations.Add(rotation2);

            for(int i = 0; i < 5; i++)
            {
                parts.Add(new List<Complex>());
                for(int j = 0; j < 4; j++)
                {
                    parts[i].Add(
                        piece1[j] * 
                        new Complex(
                            Math.Cos(Math.PI * 2 * i / 5 ), 
                            Math.Sin(Math.PI * 2 * i/ 5  )));
                }
            }

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawEllipse(Pens.Black, 0, 0, pictureBox1.Height - 1, pictureBox1.Height - 1);
            e.Graphics.DrawRectangle(Pens.Black,
                    .5f * pictureBox1.Height,
                    .5f * pictureBox1.Height,
                    1, 1);
            
           
            PointF[] points = new PointF[cameraPolygon.Count()];
            for (int i = 0; i < points.Length; i++)
            {
                

                Complex point = Complex.inversePhi(
                    cameraPolygon[i] * w, 
                    Complex.One, 
                    -location, 
                    -Complex.Conj(location), 
                    Complex.One);

                points[i] = new PointF(
                    (float)interpolate(point.Re, -1, 1, 0, pictureBox1.Height - 1),
                    (float)interpolate(point.Im, -1, 1, pictureBox1.Height - 1, 0));
            }
            e.Graphics.FillPolygon(Brushes.Blue, points);
            
            Brush[] colors = {
                new SolidBrush(Color.FromArgb(204,255,0)),
                new SolidBrush(Color.FromArgb(0,255,102)),
                new SolidBrush(Color.FromArgb(0,102,255)),
                new SolidBrush(Color.FromArgb(204,0,255)),
                new SolidBrush(Color.FromArgb(255,0,0))};
            for (int k = 0; k < transformations.Count; k++)
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    PointF[] shape = new PointF[4];
                    for (int j = 0; j < 4; j++)
                    {
                        
                        Complex temp = Complex.phi(parts[i][j], transformations[k][1, 1], transformations[k][1, 2],
                                                                transformations[k][2, 1], transformations[k][2, 2]);
                        shape[j] = new PointF(
                        (float)interpolate(temp.Re, -1, 1, 0, pictureBox1.Height - 1),
                        (float)interpolate(temp.Im, -1, 1, pictureBox1.Height - 1, 0));
                    }
                    e.Graphics.FillPolygon(colors[i], shape);
                }
            }

            e.Graphics.DrawRectangle(Pens.Green,
                    ((float)interpolate(location.Re,-1,1,0,pictureBox1.Height)),
                    ((float)interpolate(location.Im, -1,1,pictureBox1.Height,0)),
                    1, 1);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawEllipse(Pens.Black, 0, 0, pictureBox2.Height - 1, pictureBox2.Height - 1);
            e.Graphics.DrawRectangle(Pens.Black,
                    .5f * pictureBox2.Height,
                    .5f * pictureBox2.Height,
                    1, 1);
            
            PointF[] points = new PointF[cameraPolygon.Count()];
            for (int i = 0; i < points.Length; i++)
            {
                Complex point = cameraPolygon[i];
                
                points[i] = new PointF(
                    (float)interpolate(point.Re, -1, 1, 0, pictureBox2.Height - 1),
                    (float)interpolate(point.Im, -1, 1, 0, pictureBox2.Height - 1));
            }
            e.Graphics.FillPolygon(Brushes.Blue, points);


            Brush[] colors = {
                new SolidBrush(Color.FromArgb(204,255,0)),
                new SolidBrush(Color.FromArgb(0,255,102)),
                new SolidBrush(Color.FromArgb(0,102,255)),
                new SolidBrush(Color.FromArgb(204,0,255)),
                new SolidBrush(Color.FromArgb(255,0,0))};
            for (int i = 0; i < parts.Count; i++)
            {
                PointF[] shape = new PointF[4];
                for (int j = 0; j < 4; j++)
                {
                    Complex first = Complex.phi(parts[i][j], Complex.One, -location, -Complex.Conj(location), Complex.One) / w;
                    shape[j] = new PointF(
                    (float)interpolate(first.Re, -1, 1, 0, pictureBox2.Height - 1),
                    (float)interpolate(first.Im, -1, 1, pictureBox2.Height - 1, 0));
                }
                e.Graphics.FillPolygon(colors[i], shape);
            }
            
        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawEllipse(Pens.Black, 0, 0, pictureBox3.Height - 1, pictureBox3.Height - 1);
            e.Graphics.DrawRectangle(Pens.Black,
                    .5f * pictureBox3.Height,
                    .5f * pictureBox3.Height,
                    1, 1);
            PointF end = new PointF((float)interpolate(w.Re, -1, 1, 0, pictureBox3.Height - 1),
                (float)interpolate(w.Im, -1, 1, pictureBox3.Height - 1, 0));
            e.Graphics.DrawLine(Pens.Red, end, new PointF((float)interpolate(0, -1, 1, 0, pictureBox3.Height - 1),
                (float)interpolate(0, -1, 1, pictureBox3.Height - 1, 0)));
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            down1 = true;
            Point coordinates = e.Location;
            int max = pictureBox1.Height - 1;
            double x = ((double)coordinates.X) / max * 2 - 1;
            double y = ((double)coordinates.Y - max) / -max * 2 - 1;
            location = new Complex(x, y);
            Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (down1)
            {
                Point coordinates = e.Location;
                int max = pictureBox1.Height - 1;
                double x = ((double)coordinates.X) / max * 2 - 1;
                double y = ((double)coordinates.Y - max) / -max * 2 - 1;
                location = new Complex(x, y);
                Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            down1 = false;
            Point coordinates = e.Location;
            int max = pictureBox1.Height - 1;
            double x = ((double)coordinates.X) / max * 2 - 1;
            double y = ((double)coordinates.Y - max) / -max * 2 - 1;
            location = new Complex(x, y);
            Refresh();
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            down2 = true;
            double Rex = interpolate(e.X, 0, pictureBox3.Width, -1, 1);
            double Rey = interpolate(e.Y, pictureBox3.Height, 0, -1, 1);
            w = new Complex(Math.Cos(Math.Atan2(Rey, Rex)), Math.Sin(Math.Atan2(Rey, Rex)));
            Refresh();
        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (down2)
            {
                double Rex = interpolate(e.X, 0, pictureBox3.Width, -1, 1);
                double Rey = interpolate(e.Y, pictureBox3.Height, 0, -1, 1);
                w = new Complex(Math.Cos(Math.Atan2(Rey, Rex)), Math.Sin(Math.Atan2(Rey, Rex)));
                Refresh();
            }
        }

        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            down2 = false;
            double Rex = interpolate(e.X, 0, pictureBox3.Width, -1, 1);
            double Rey = interpolate(e.Y, pictureBox3.Height, 0, -1, 1);
            w = new Complex(Math.Cos(Math.Atan2(Rey, Rex)), Math.Sin(Math.Atan2(Rey, Rex)));
            Refresh();
        }
        
        private double interpolate(double input, double inputMin, double inputMax, 
            double outputMin, double outputMax)
        {
            return ((input - inputMin) / (inputMax - inputMin)) * (outputMax - outputMin) + outputMin;
        }
    }
}
