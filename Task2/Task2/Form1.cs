using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Visualization;
namespace Task2
{
    public partial class Form1 : Form
    {
       public class Shape
       {
       public List<Point3> points = new List<Point3>();
       public Tuple<double, double, double> color;
       }
        List<double> storedContourValues = new List<double>();
        List<double> StoredMarchingValues = new List<double>();
        List<double> StoredIsoValues = new List<double>();
        List<Shape> ShapeListColor = new List<Shape>();
        List<Color> ContourColors;
        List<Brush> brushes = new List<Brush>()
         {
            Brushes.Blue,
            Brushes.Cyan,
            new Pen( Color.FromArgb(0,255,0)).Brush,
            Brushes.Yellow,
            Brushes.Orange,
            Brushes.Red
        };
        int[,] Table = new int[,] { {-1,-1,-1,-1},
                                    {1,4,-1,-1},
                                    {-1,-1,3,4},
                                    {1,3,-1,-1},
                                    {2,3,-1,-1},
                                    {4,3,1,2},
                                    {2,4,-1,-1},
                                    {1,2,-1,-1},
                                    {1,2,-1,-1},
                                    {2,4,-1,-1},
                                    {2,3,4,1},
                                    {2,3,-1,-1},
                                    {1,3,-1,-1},
                                    {3,4,-1,-1},
                                    {1,4,-1,-1},
                                    {-1,-1,-1,-1}
        };
        Mesh m1;
        int num=0,c=0;
        bool b = false, calc = false, scale = false, rotate1 = false, calc2 = false, lines = false, minORmax = false, rotateY = false;
        bool rotatenegY = false, rotate2 = false, rotateZ = false, rotatenegZ = false, translateonX = false, translateonY = false, translateonZ = false;
        bool ok1 = false, ok2 = false;
        double x = 0.0, y = 0.0, z = 0.0;
        double scaleX = 1, scaleY = 1, angle = 0.0;
        uint dataindex=0;
        public Form1()
        {
            InitializeComponent();

            comboBox2.SelectedItem = comboBox2.Items[0];
            comboBox3.SelectedItem = comboBox3.Items[3];
            textBox4.Text = "10";
        }
        void InitGraphics()
        {
            simpleOpenGlControl1.InitializeContexts();
            Gl.glClearColor(1, 1, 1, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);
            simpleOpenGlControl1.Paint += new PaintEventHandler(simpleOpenGlControl1_Paint);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            //Glu.gluOrtho2D(-100, 100, -100, 100);
            Glu.gluPerspective(60.0, (double)simpleOpenGlControl1.Width / (double)simpleOpenGlControl1.Height, 0.1, 1000.0);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }
        private void EdgeColoring()
        {
            userControl11 = new Task1.UserControl1();
            if (comboBox1.SelectedItem != null)
            {
                dataindex = (uint)m1.VarToIndex[comboBox1.SelectedItem];
            }
            double s_min = 0, s_max = 0;
            m1.GetMinMaxValues((uint)dataindex, out s_min, out s_max);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glMultMatrixd(m1.Transformation.Data);
            foreach (Zone z in m1.Zones)
            {
                foreach (Face f in z.Faces)
                {
                    Gl.glBegin(Gl.GL_LINES);
                    foreach (uint ee in f.Edges)
                    {
                        var dataA = z.Vertices[z.Edges[ee].Start].Data[dataindex];
                        var dataB = z.Vertices[z.Edges[ee].End].Data[dataindex];
                        double edge_value = (dataA + dataB) / 2.0;
                        if (comboBox2.SelectedItem == "continuous") 
                        {
                            Tuple<double, double, double> x = userControl11.ValueToColorContinuous(s_min, s_max, edge_value);
                            Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                            z.Vertices[z.Edges[ee].Start].Position.glTell();
                            z.Vertices[z.Edges[ee].End].Position.glTell();
                            textBox5.Text = x.Item1.ToString() + " " + x.Item2.ToString() + " " + x.Item3.ToString();
                        }
                        if (comboBox2.SelectedItem == "discrete")
                        {
                            Tuple<double, double, double> x = userControl11.ValueToColorDiscrete(s_min, s_max, edge_value);
                            Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                            z.Vertices[z.Edges[ee].Start].Position.glTell();
                            z.Vertices[z.Edges[ee].End].Position.glTell();
                            textBox5.Text = x.Item1.ToString() + " " + x.Item2.ToString() + " " + x.Item3.ToString();
                        }
                    }
                    Gl.glEnd();
                    Gl.glFlush();
                }
            }
            Gl.glPopMatrix();
        }
        private void FaceColoring()
        {
            if (comboBox1.SelectedItem != null)
            {
                dataindex = (uint)m1.VarToIndex[comboBox1.SelectedItem];
            }
            double s_min = 0, s_max = 0;
            m1.GetMinMaxValues((uint)dataindex, out s_min, out s_max);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glMultMatrixd(m1.Transformation.Data);
            foreach (Zone z in m1.Zones)
            {
                switch (z.ElementType) 
                {
                    case ElementType.Triangle:
                          Gl.glBegin(Gl.GL_TRIANGLES);
                          foreach (Face f in z.Faces)
                          {
                            double dataA = z.Vertices[f.Vertices[0]].Data[dataindex];
                            double dataB = z.Vertices[f.Vertices[1]].Data[dataindex];
                            double dataC = z.Vertices[f.Vertices[2]].Data[dataindex];
                            double FaceValue = (dataA + dataB + dataC) / 3;
                            if (comboBox2.SelectedItem == "continuous")
                            {
                              Tuple<double, double, double> x = userControl11.ValueToColorContinuous(s_min, s_max, FaceValue);
                              Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                              z.Vertices[f.Vertices[0]].Position.glTell();
                              z.Vertices[f.Vertices[1]].Position.glTell();
                              z.Vertices[f.Vertices[2]].Position.glTell();
                              textBox5.Text = x.Item1.ToString() + " " + x.Item2.ToString() + " " + x.Item3.ToString();
                            }
                            if (comboBox2.SelectedItem == "discrete")
                            {
                            Tuple<double, double, double> x = userControl11.ValueToColorDiscrete(s_min, s_max, FaceValue);
                            Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                            z.Vertices[f.Vertices[0]].Position.glTell();
                            z.Vertices[f.Vertices[1]].Position.glTell();
                            z.Vertices[f.Vertices[2]].Position.glTell();
                            textBox5.Text = x.Item1.ToString() + " " + x.Item2.ToString() + " " + x.Item3.ToString();
                            }
                            }
                          break;
                    case ElementType.IJKBrick:
                    case ElementType.IJKQuad:
                    case ElementType.FEBrick:
                    case ElementType.FEQuad:
                          Gl.glBegin(Gl.GL_QUADS);
                          foreach (Face f in z.Faces)
                          {
                              double dataA = z.Vertices[f.Vertices[0]].Data[dataindex];
                              double dataB = z.Vertices[f.Vertices[1]].Data[dataindex];
                              double dataC = z.Vertices[f.Vertices[2]].Data[dataindex];
                              double FaceValue = (dataA + dataB + dataC) / 3;
                              if (comboBox2.SelectedItem == "continuous")
                              {
                                  Tuple<double, double, double> x = userControl11.ValueToColorContinuous(s_min, s_max, FaceValue);
                                  Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                                  z.Vertices[f.Vertices[0]].Position.glTell();
                                  z.Vertices[f.Vertices[1]].Position.glTell();
                                  z.Vertices[f.Vertices[2]].Position.glTell();
                                  z.Vertices[f.Vertices[3]].Position.glTell();
                                  textBox5.Text = x.Item1.ToString() + " " + x.Item2.ToString() + " " + x.Item3.ToString();
                              }
                              if (comboBox2.SelectedItem == "discrete")
                              {
                                  Tuple<double, double, double> x = userControl11.ValueToColorDiscrete(s_min, s_max, FaceValue);
                                  Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                                  z.Vertices[f.Vertices[0]].Position.glTell();
                                  z.Vertices[f.Vertices[1]].Position.glTell();
                                  z.Vertices[f.Vertices[2]].Position.glTell();
                                  z.Vertices[f.Vertices[3]].Position.glTell();
                                  textBox5.Text = x.Item1.ToString() + " " + x.Item2.ToString() + " " + x.Item3.ToString();
                              }
                          }
                break;
                }
                Gl.glEnd();
            }
            Gl.glPopMatrix();
        }       
        private List<List<Point3>>LineContour()
        {
            double min = 0.0, max = 0.0;
            if (comboBox1.SelectedItem != null)
            {
                dataindex = (uint)m1.VarToIndex[comboBox1.SelectedItem];
            }
            m1.GetMinMaxValues((uint)dataindex, out min, out max);
            int NoOfLines = int.Parse(textBox4.Text);
            double steps = (max - min) / NoOfLines;
            double Contourvalue = min + (0.5 * steps);
            int i=0;
            List<List<Point3>> ContourLines = new List<List<Point3>>();
            while(Contourvalue<=max)
            {
                ContourLines.Add(new List<Point3>());
                foreach (Zone z in m1.Zones)
                {
                    foreach (Face f in z.Faces)
                    {
                        foreach (uint e in f.Edges)
                        {
                            var d1 = z.Vertices[z.Edges[e].Start].Data[dataindex];
                            var d2 = z.Vertices[z.Edges[e].End].Data[dataindex];
                            
                            if ((Contourvalue >= d1 && Contourvalue <= d2)||(Contourvalue>=d2&&Contourvalue<=d1))
                            {
                                 
                                double alpha = (Contourvalue - d1) / (d2 - d1);

                                double px1 = z.Vertices[z.Edges[e].Start].Position.x;
                                double py1 = z.Vertices[z.Edges[e].Start].Position.y;
                                double pz1 = z.Vertices[z.Edges[e].Start].Position.z;
                                double px2 = z.Vertices[z.Edges[e].End].Position.x;
                                double py2 = z.Vertices[z.Edges[e].End].Position.y;
                                double pz2 = z.Vertices[z.Edges[e].End].Position.z;
                                
                                double pointx = px1 + alpha * (px2 - px1);
                                double pointy = py1 + alpha * (py2 - py1);
                                double pointz = pz1 + alpha * (pz2 - pz1);
                                ContourLines[i].Add(new Point3(pointx,pointy,pointz));                            
                            }
                            
                        }
                     
                    }
                }
              
                storedContourValues.Add(Contourvalue);
                Contourvalue += steps;
                i++;
            }
                        
            return ContourLines;
        }
        private Point3 Interpolate(Point3 point1,Point3 point2,double data1,double data2,double alpha)
        {
            if(data1 < data2)
            {
                return Point3.Interpolate(alpha, point1, point2);
            }
            else
            {
                return Point3.Interpolate(alpha, point2, point1);
            }
        }
        private List<List<Point3>> MarchingSquare()
        {
            double min = 0.0, max = 0.0;
            if (comboBox1.SelectedItem != null)
            {
                dataindex = (uint)m1.VarToIndex[comboBox1.SelectedItem];
            }
            List<List<Point3>> MarchingList = new List<List<Point3>>();
            m1.GetMinMaxValues((uint)dataindex, out min, out max);
            int NoOfLines = int.Parse(textBox4.Text);
            double steps = (max - min) / NoOfLines;
            double Contourvalue = min + (0.5 * steps);
            int tableCase = 0;
            for (int i = 0; i < NoOfLines; i++)
            {
                MarchingList.Add(new List<Point3>());
                foreach (Zone z in m1.Zones)
                {
                    foreach (Face f in z.Faces)
                    {
                        double dataA = z.Vertices[f.Vertices[0]].Data[dataindex];
                        double dataB = z.Vertices[f.Vertices[1]].Data[dataindex];
                        double dataC = z.Vertices[f.Vertices[2]].Data[dataindex];
                        double dataD = z.Vertices[f.Vertices[3]].Data[dataindex];
                        tableCase = 0;
                        if (Contourvalue >= dataA)
                        {
                            tableCase += 1;
                        }
                        if (Contourvalue >= dataB)
                        {
                            tableCase += 8;
                        }
                        if (Contourvalue >= dataC)
                        {
                            tableCase += 4;
                        }
                        if (Contourvalue >= dataD)
                        {
                            tableCase += 2;
                        }
                        if (tableCase == 5 || tableCase == 10)
                        {
                        double check = (dataA + dataB + dataC + dataD) / 4;
                            if (check > Contourvalue)
                            {
                                if(tableCase==5)
                                {
                                    tableCase = 10;
                                }
                                else
                                {
                                    tableCase = 5;
                                }
                            }
                             
                        }
                        int edge1 = Table[tableCase,0];
                        int edge2 = Table[tableCase,1];
                        int edge3 = Table[tableCase,2];
                        int edge4 = Table[tableCase,3];
                        if (tableCase == 0 || tableCase == 15)
                            continue;
                        if (edge1 == 1 || edge2 == 1 || edge3 == 1 || edge4 == 1)
                        {
                            double alpha = (Contourvalue - Math.Min(dataA, dataB)) / (Math.Max(dataA, dataB) - Math.Min(dataA, dataB));
                            Point3 point1 = Interpolate(z.Vertices[f.Vertices[0]].Position, z.Vertices[f.Vertices[1]].Position, dataA, dataB, alpha);
                            MarchingList[i].Add(new Point3(point1.x, point1.y, point1.z));

                        }
                        if (edge1 == 2 || edge2 == 2 || edge3 == 2 || edge4 == 2)
                        {
                            double alpha = (Contourvalue - Math.Min(dataB, dataC)) / (Math.Max(dataB, dataC) - Math.Min(dataB, dataC));
                            Point3 point2 = Interpolate(z.Vertices[f.Vertices[1]].Position, z.Vertices[f.Vertices[2]].Position, dataB, dataC, alpha);
                            MarchingList[i].Add(new Point3(point2.x, point2.y, point2.z));
                        }
                        if (edge1 == 3 || edge2 == 3 || edge3 == 3 || edge4 == 3)
                        {
                            double alpha = (Contourvalue - Math.Min(dataC, dataD)) / (Math.Max(dataC, dataD) - Math.Min(dataC, dataD));
                            Point3 point3 = Interpolate(z.Vertices[f.Vertices[2]].Position, z.Vertices[f.Vertices[3]].Position, dataC, dataD, alpha);
                            MarchingList[i].Add(new Point3(point3.x, point3.y, point3.z));
                        }
                        if (edge1 == 4 || edge2 == 4 || edge3 == 4 || edge4 == 4)
                        {
                            double alpha = (Contourvalue - Math.Min(dataD, dataA)) / (Math.Max(dataD, dataA) - Math.Min(dataD, dataA));
                            Point3 point4 = Interpolate(z.Vertices[f.Vertices[3]].Position, z.Vertices[f.Vertices[0]].Position, dataD, dataA, alpha);
                            MarchingList[i].Add(new Point3(point4.x, point4.y, point4.z));
                        }
                    }
                }
                StoredMarchingValues.Add(Contourvalue);
                Contourvalue += steps;
            }
            return MarchingList;
        }
        private List<Shape>TwoFloadedContours()
        {
            double min = 0.0, max = 0.0;
            if (comboBox1.SelectedItem != null)
            {
                dataindex = (uint)m1.VarToIndex[comboBox1.SelectedItem];
            }
            m1.GetMinMaxValues((uint)dataindex, out min, out max);
            int NoOfLines = int.Parse(textBox4.Text);
            double steps = (max - min) / NoOfLines;
            double CurrentContourvalue = min + (0.5 * steps);
            double minContourvalue = min;
            double maxContourvalue = max;
            int idx = 0;
            for (int i = -1; i <= NoOfLines; i++)
            {
                foreach (Zone z in m1.Zones)
                {
                    foreach (Face f in z.Faces)
                    {
                        ShapeListColor.Add(new Shape());

                        double dataA = z.Vertices[f.Vertices[0]].Data[dataindex];
                        double dataB = z.Vertices[f.Vertices[1]].Data[dataindex];
                        double dataC = z.Vertices[f.Vertices[2]].Data[dataindex];
                        double dataD = z.Vertices[f.Vertices[3]].Data[dataindex];
                        if (i == NoOfLines)
                        {

                            minContourvalue = CurrentContourvalue;
                            CurrentContourvalue = maxContourvalue;
                        }
                        if ((dataA > minContourvalue && dataA < CurrentContourvalue))
                        {
                            Point3 point1 = z.Vertices[f.Vertices[0]].Position;
                            ShapeListColor[idx].points.Add(new Point3(point1.x, point1.y, point1.z));
                        }
                        if ((dataB > minContourvalue && dataB < CurrentContourvalue))
                        {
                            Point3 point2 = z.Vertices[f.Vertices[1]].Position;
                            ShapeListColor[idx].points.Add(new Point3(point2.x, point2.y, point2.z));
                        }
                        if ((dataC > minContourvalue && dataC < CurrentContourvalue))
                        {
                            Point3 point3 = z.Vertices[f.Vertices[2]].Position;
                            ShapeListColor[idx].points.Add(new Point3(point3.x, point3.y, point3.z));
                        }
                        if ((dataD > minContourvalue && dataD < CurrentContourvalue))
                        {
                            Point3 point4 = z.Vertices[f.Vertices[3]].Position;
                            ShapeListColor[idx].points.Add(new Point3(point4.x, point4.y, point4.z));
                        }
                        //-----------------------------------------------------
                        if ((dataA > CurrentContourvalue && dataB < CurrentContourvalue) || (dataB > CurrentContourvalue && dataA < CurrentContourvalue))
                        {
                            double alpha = (CurrentContourvalue - Math.Min(dataA, dataB)) / (Math.Max(dataA, dataB) - Math.Min(dataA, dataB));
                            Point3 point1 = Interpolate(z.Vertices[f.Vertices[0]].Position, z.Vertices[f.Vertices[1]].Position, dataA, dataB, alpha);
                            ShapeListColor[idx].points.Add(new Point3(point1.x, point1.y, point1.z));
                        }
                        if ((dataB > CurrentContourvalue && dataC < CurrentContourvalue) || (dataC > CurrentContourvalue && dataB < CurrentContourvalue))
                        {
                            double alpha = (CurrentContourvalue - Math.Min(dataB, dataC)) / (Math.Max(dataB, dataC) - Math.Min(dataB, dataC));
                            Point3 point2 = Interpolate(z.Vertices[f.Vertices[1]].Position, z.Vertices[f.Vertices[2]].Position, dataB, dataC, alpha);
                            ShapeListColor[idx].points.Add(new Point3(point2.x, point2.y, point2.z));
                        }
                        if ((dataC > CurrentContourvalue && dataD < CurrentContourvalue) || (dataD > CurrentContourvalue && dataC < CurrentContourvalue))
                        {
                            double alpha = (CurrentContourvalue - Math.Min(dataC, dataD)) / (Math.Max(dataC, dataD) - Math.Min(dataC, dataD));
                            Point3 point3 = Interpolate(z.Vertices[f.Vertices[2]].Position, z.Vertices[f.Vertices[3]].Position, dataC, dataD, alpha);
                            ShapeListColor[idx].points.Add(new Point3(point3.x, point3.y, point3.z));
                        }
                        if ((dataA > CurrentContourvalue && dataD < CurrentContourvalue) || (dataD > CurrentContourvalue && dataA < CurrentContourvalue))
                        {
                            double alpha = (CurrentContourvalue - Math.Min(dataD, dataA)) / (Math.Max(dataD, dataA) - Math.Min(dataD, dataA));
                            Point3 point4 = Interpolate(z.Vertices[f.Vertices[3]].Position, z.Vertices[f.Vertices[0]].Position, dataD, dataA, alpha);
                            ShapeListColor[idx].points.Add(new Point3(point4.x, point4.y, point4.z));
                        }
                        //----------------------------------------------
                        if ((dataA > minContourvalue && dataB < minContourvalue) || (dataB > minContourvalue && dataA < minContourvalue))
                        {
                            double alpha = (minContourvalue - Math.Min(dataA, dataB)) / (Math.Max(dataA, dataB) - Math.Min(dataA, dataB));
                            Point3 point1 = Interpolate(z.Vertices[f.Vertices[0]].Position, z.Vertices[f.Vertices[1]].Position, dataA, dataB, alpha);
                            ShapeListColor[idx].points.Add(new Point3(point1.x, point1.y, point1.z));
                        }
                        if ((dataB > minContourvalue && dataC < minContourvalue) || (dataC > minContourvalue && dataB < minContourvalue))
                        {
                            double alpha = (minContourvalue - Math.Min(dataB, dataC)) / (Math.Max(dataB, dataC) - Math.Min(dataB, dataC));
                            Point3 point2 = Interpolate(z.Vertices[f.Vertices[1]].Position, z.Vertices[f.Vertices[2]].Position, dataB, dataC, alpha);
                            ShapeListColor[idx].points.Add(new Point3(point2.x, point2.y, point2.z));
                        }
                        if ((dataC > minContourvalue && dataD < minContourvalue) || (dataD > minContourvalue && dataC < minContourvalue))
                        {
                            double alpha = (minContourvalue - Math.Min(dataC, dataD)) / (Math.Max(dataC, dataD) - Math.Min(dataC, dataD));
                            Point3 point3 = Interpolate(z.Vertices[f.Vertices[2]].Position, z.Vertices[f.Vertices[3]].Position, dataC, dataD, alpha);
                            ShapeListColor[idx].points.Add(new Point3(point3.x, point3.y, point3.z));
                        }
                        if ((dataA > minContourvalue && dataD < minContourvalue) || (dataD > minContourvalue && dataA < minContourvalue))
                        {
                            double alpha = (minContourvalue - Math.Min(dataD, dataA)) / (Math.Max(dataD, dataA) - Math.Min(dataD, dataA));
                            Point3 point4 = Interpolate(z.Vertices[f.Vertices[3]].Position, z.Vertices[f.Vertices[0]].Position, dataD, dataA, alpha);
                            ShapeListColor[idx].points.Add(new Point3(point4.x, point4.y, point4.z));
                        }
                        //--------------------------------------

                        if (comboBox2.SelectedItem == "continuous")
                        {
                            //var avg = (minContourvalue + CurrentContourvalue) / 2;
                            var c1 = userControl11.ValueToColorContinuous(min, max, minContourvalue);
                            var c2 = userControl11.ValueToColorContinuous(min, max, CurrentContourvalue);
                            double avgR = (c1.Item1 + c2.Item1) / 2;
                            double avgG = (c1.Item2 + c2.Item2) / 2;
                            double avgB = (c1.Item3 + c2.Item3) / 2;
                            ShapeListColor[idx].color = Tuple.Create(avgR, avgG, avgB);
                        }
                        else if (comboBox2.SelectedItem == "discrete")
                        {
                            //var avg = (minContourvalue + CurrentContourvalue) / 2;
                            var c1 = userControl11.ValueToColorDiscrete(min, max, minContourvalue);
                            var c2 = userControl11.ValueToColorDiscrete(min, max, CurrentContourvalue);
                            double avgR = (c1.Item1 + c2.Item1) / 2;
                            double avgG = (c1.Item2 + c2.Item2) / 2;
                            double avgB = (c1.Item3 + c2.Item3) / 2;
                            ShapeListColor[idx].color = Tuple.Create(avgR, avgG, avgB);
                        }
                        idx++;
                    }
                }
                minContourvalue = CurrentContourvalue;
                CurrentContourvalue += steps;
            }
            ShapeListColor.RemoveAll((s => s.points.Count == 0));
            /*foreach (var s in ShapeListColor)
            {
                s.points = sortPoints(s.points);

            }*/
           return ShapeListColor;
        }
        private List<List<Point3>> isoSurfaces()
        {
            double min = 0.0, max = 0.0;
            if (comboBox1.SelectedItem != null)
            {
                dataindex = (uint)m1.VarToIndex[comboBox1.SelectedItem];
            }
            m1.GetMinMaxValues((uint)dataindex, out min, out max);
            int IsoValues = int.Parse(textBox4.Text);
            double steps = (max - min) / IsoValues;
            double myIsoValues = min + (0.5 * steps);
            uint varIndex = 0;
            List<List<Point3>> surfacePointList = new List<List<Point3>>();
            for (int k = 0; k < IsoValues; k++)
            {
                surfacePointList.Add(new List<Point3>());
                foreach (Zone z in m1.Zones)
                {
                    foreach (Element element in z.Elements)
                    {
                        double[] arrayData = new double[8];
                        for (int i = 0; i < 8; i++)
                        {
                            varIndex = element.vertInOrder[i];
                            arrayData[i] = z.Vertices[varIndex].Data[dataindex];
                        }
                        Byte caseNumber = ISOSurface.GetElementCase(arrayData, myIsoValues);
                        //MessageBox.Show("CaseNumber= "+ caseNumber.ToString() + " IsoValue= " + Isovalue.ToString());
                        int[] edges = new int[16];
                        int edgeIndex = 0;
                        double Alpha = 0.0;
                        Point3 MynewPosition = new Point3();
                        double[] startAndEndData = new double[2];
                        Point3[] startAndendPoints = new Point3[2];
                        Edge edgePoints;
                        edges = ISOSurface.GetCaseEdges(Convert.ToInt32(caseNumber));
                        //MessageBox.Show(edges.Count().ToString());
                        for (int i = 0; i < edges.Length; i += 3)
                        {
                            // MessageBox.Show(edges[i].ToString());
                            if (edges[i] != -1 && edges[i + 1] != -1 && edges[i + 2] != -1)
                            {
                                //MessageBox.Show("ISO");
                                for (int j = i; j < i + 3; j++)
                                {
                                    edgeIndex = edges[j];
                                    edgePoints = ISOSurface.GetEdgePoints(edgeIndex);
                                    varIndex = element.vertInOrder[edgePoints.Start];
                                    startAndEndData[0] = z.Vertices[varIndex].Data[dataindex];
                                    startAndendPoints[0] = z.Vertices[varIndex].Position;
                                    varIndex = element.vertInOrder[edgePoints.End];
                                    startAndEndData[1] = z.Vertices[varIndex].Data[dataindex];
                                    startAndendPoints[1] = z.Vertices[varIndex].Position;
                                    Alpha = (myIsoValues - startAndEndData[0]) / (startAndEndData[1] - startAndEndData[0]);
                                    MynewPosition = startAndendPoints[0] + (Alpha * (startAndendPoints[1] - startAndendPoints[0]));
                                    //  MessageBox.Show(MynewPosition.ToString());
                                    surfacePointList[k].Add(MynewPosition);
                                }
                            }
                        }
                    }
                }
                StoredIsoValues.Add(myIsoValues);
                myIsoValues += steps;
            }
            return surfacePointList;
        }
        private List<Point3>isoSurface()
        {
            double min = 0.0, max = 0.0;
            if (comboBox1.SelectedItem != null)
            {
                dataindex = (uint)m1.VarToIndex[comboBox1.SelectedItem];
            }
            m1.GetMinMaxValues((uint)dataindex, out min, out max);
            //MessageBox.Show(min.ToString() + " " + max.ToString());
            double Isovalue = double.Parse(textBox4.Text);
            uint varIndex;
            List<Point3> surfacePointList = new List<Point3>();
            foreach(Zone z in m1.Zones)
            {
                foreach(Element element in z.Elements)
                {
                    double[] arrayData = new double[8];
                    for(int i=0;i<8;i++)
                    {
                        varIndex = element.vertInOrder[i];
                        arrayData[i] = z.Vertices[varIndex].Data[dataindex];
                    }
                    Byte caseNumber = ISOSurface.GetElementCase(arrayData, Isovalue);
                    //MessageBox.Show("CaseNumber= "+ caseNumber.ToString() + " IsoValue= " + Isovalue.ToString());
                    int[] edges = new int[16];
                    int edgeIndex = 0;
                    double Alpha = 0.0;
                    Point3 MynewPosition=new Point3();
                    double[] startAndEndData = new double[2];
                    Point3[] startAndendPoints = new Point3[2];
                    Edge edgePoints;
                    edges = ISOSurface.GetCaseEdges(Convert.ToInt32(caseNumber));
                    //MessageBox.Show(edges.Count().ToString());
                    for(int i=0;i<edges.Length;i+=3)
                    {
                       // MessageBox.Show(edges[i].ToString());
                        if (edges[i] != -1 && edges[i + 1] != -1 && edges[i + 2] != -1)
                        {
                            //MessageBox.Show("ISO");
                            for (int j = i; j < i + 3; j++)
                            {
                                edgeIndex = edges[j];
                                edgePoints = ISOSurface.GetEdgePoints(edgeIndex);
                                varIndex = element.vertInOrder[edgePoints.Start];
                                startAndEndData[0] = z.Vertices[varIndex].Data[dataindex];
                                startAndendPoints[0] = z.Vertices[varIndex].Position;
                                varIndex = element.vertInOrder[edgePoints.End];
                                startAndEndData[1] = z.Vertices[varIndex].Data[dataindex];
                                startAndendPoints[1] = z.Vertices[varIndex].Position;
                                Alpha = (Isovalue - startAndEndData[0]) / (startAndEndData[1] - startAndEndData[0]);
                                MynewPosition = startAndendPoints[0] + (Alpha * (startAndendPoints[1] - startAndendPoints[0]));
                              //  MessageBox.Show(MynewPosition.ToString());
                                surfacePointList.Add(MynewPosition);
                            }
                        }
                    }
                }
            }
            StoredIsoValues.Add(Isovalue);
            return surfacePointList;
        }
        private void ColorContour()
        {
            double min = 0.0, max = 0.0;
            if (comboBox1.SelectedItem != null)
            {
                dataindex = (uint)m1.VarToIndex[comboBox1.SelectedItem];
            }
            double noLines = double.Parse(textBox4.Text);
            m1.GetMinMaxValues((uint)dataindex, out min, out max);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glMultMatrixd(m1.Transformation.Data);
            int k = 0;
            if (comboBox3.SelectedItem == "MoreThan")
            {
                List<List<Point3>> isoSurfaceList = isoSurfaces();
                for(int i=0;i<isoSurfaceList.Count;i++)
                {
                    if (comboBox2.SelectedItem == "continuous")
                    {
                        Tuple<double, double, double> x = userControl11.ValueToColorContinuous(min, max, StoredIsoValues[k]);
                        k++;
                        Gl.glColor3d(x.Item1, x.Item2, x.Item3);

                    }
                    if (comboBox2.SelectedItem == "discrete")
                    {
                        Tuple<double, double, double> x = userControl11.ValueToColorDiscrete(min, max, StoredIsoValues[k]);
                        k++;
                        Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                    }
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    for(int j=0;j<isoSurfaceList[i].Count;j+=3)
                    {
                        isoSurfaceList[i][j].glTell();
                        isoSurfaceList[i][j + 1].glTell();
                        isoSurfaceList[i][j + 2].glTell();
                    }
                    Gl.glEnd();
                }
            }
            if(comboBox3.SelectedItem =="IsoSurface")
            {
                List<Point3> isoSurfaceList = isoSurface();
                if (comboBox2.SelectedItem == "continuous")
                {
                    Tuple<double, double, double> x = userControl11.ValueToColorContinuous(min, max, StoredIsoValues[0]);
                    Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                }
                if(comboBox2.SelectedItem=="discrete")
                {
                    Tuple<double, double, double> x = userControl11.ValueToColorDiscrete(min, max, StoredIsoValues[0]);
                    Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                }
                MessageBox.Show("The Length of IsoValue= "+StoredIsoValues.Count.ToString());  
                MessageBox.Show("The Length of IsoList= "+isoSurfaceList.Count().ToString());
                MessageBox.Show("Min= " + min.ToString() + " Max= " + max.ToString());
                
                Gl.glBegin(Gl.GL_TRIANGLES);
                for (int i = 0; i < isoSurfaceList.Count; i += 3)
                {
                   isoSurfaceList[i].glTell();
                   isoSurfaceList[i + 1].glTell();
                   isoSurfaceList[i + 2].glTell();
                }
                Gl.glEnd();
             }
            
            if (comboBox3.SelectedItem == "LineContour")
            {
                List<List<Point3>> ContourLines = LineContour();
                for (int i = 0; i < ContourLines.Count; i++)
                {
                    if (comboBox2.SelectedItem == "continuous")
                    {
                       // MessageBox.Show("LOL1");
                        Tuple<double, double, double> x = userControl11.ValueToColorContinuous(min, max, storedContourValues[k]);
                        k++;
                        Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                    }
                    if (comboBox2.SelectedItem == "discrete")
                    {
                        //MessageBox.Show("LOL2");
                        Tuple<double, double, double> x = userControl11.ValueToColorDiscrete(min, max, storedContourValues[k]);
                        k++;
                        Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                    }
                    Gl.glBegin(Gl.GL_LINES);
                    for (int j = 0; j < ContourLines[i].Count; j++)
                    {
                        Gl.glVertex3d(ContourLines[i][j].x, ContourLines[i][j].y, ContourLines[i][j].z);
                    }
                    Gl.glEnd();
                }
            }
            else if (comboBox3.SelectedItem == "MarchingSquare")
            {
                List<List<Point3>> marchingList = MarchingSquare();
                k = 0;
                //MessageBox.Show(marchingList.Count.ToString());
                for (int i = 0; i < marchingList.Count; i++)
                {
                    if (comboBox2.SelectedItem == "continuous")
                    {
                        //MessageBox.Show("LOL3");
                        Tuple<double, double, double> x = userControl11.ValueToColorContinuous(min, max, StoredMarchingValues[k]);
                        k++;
                        Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                    }
                    if (comboBox2.SelectedItem == "discrete")
                    {
                        //MessageBox.Show("LOL4");
                        Tuple<double, double, double> x = userControl11.ValueToColorDiscrete(min, max, StoredMarchingValues[k]);
                        k++;
                        Gl.glColor3d(x.Item1, x.Item2, x.Item3);
                    }
                    Gl.glBegin(Gl.GL_LINES);
                    for (int j = 0; j < marchingList[i].Count; j++)
                    {
                        Gl.glVertex3d(marchingList[i][j].x, marchingList[i][j].y, marchingList[i][j].z);
                    }
                    Gl.glEnd();
                }
                Gl.glPopMatrix();
            }
            else if (comboBox3.SelectedItem == "TwoFloadedContours")
            {
                List<Shape> ShapeList = TwoFloadedContours();
                foreach(var s in ShapeList)
                {
                    Gl.glBegin(Gl.GL_POLYGON);
                    Gl.glColor3d(s.color.Item1, s.color.Item2, s.color.Item3);
                    foreach(Point3 p in s.points)
                    {   
                        p.glTell();
                    }
                    Gl.glEnd();
                }
            }
           
        }
        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClearColor(1, 1, 1, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            
            if(b==true)
            {   
                m1 = new Mesh(textBox1.Text.ToString());
                m1.Transformation.Translate(0, 0, -180);
                m1.glDraw();
                
                if (c == 1)
                {
                    num = m1.VarToIndex.Count;
                    foreach (string str in m1.VarToIndex.Keys)
                    {
                        comboBox1.Items.Add(str);
                    }
                   
                }
                c = 0;
            }
            if(m1!=null)
            {
                //m1.Transformation.Translate(x, y, 0);
                m1.glDraw();
               
            }
           
            if (scale==true)
            {
                //Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);
                m1.Transformation.Scale(scaleX,scaleY,1);
                m1.glDraw();
            }
            if (rotate1 == true)
            {
                //Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);
                m1.Transformation *= Visualization.Matrix.RotationX(angle);
               m1.glDraw();
            }
            if(rotate2==true)
            {
                //Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);
                m1.Transformation *= Visualization.Matrix.RotationX(angle);
                
                m1.glDraw();
            }
            if(rotateY==true)
            {
                //Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);
                m1.Transformation *= Visualization.Matrix.RotationY(angle);
               
                m1.glDraw();
            }
            if(rotatenegY==true)
            {
                //Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);
                m1.Transformation *= Visualization.Matrix.RotationY(angle);
                
                m1.glDraw();
            }
            if(rotateZ==true)
            {
                //Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);
                m1.Transformation *= Visualization.Matrix.RotationZ(angle);
                m1.glDraw();
            }
            if (rotatenegZ == true)
            {
                //Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT);
                m1.Transformation *= Visualization.Matrix.RotationZ(angle);
                m1.glDraw();
            }
            if(translateonX==true)
            {
                m1.Transformation.Translate(x, y, z);
                //m1.Transformation *= Visualization.Matrix.Translation(x, y, z);
                m1.glDraw();
            }

            if(calc==true)
            {
                EdgeColoring();
            }
            if(calc2==true)
            {
                FaceColoring();
            }
            if(lines==true)
            {
                ColorContour();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
           if( ofd.ShowDialog()==DialogResult.OK)
           {  
               textBox1.Text = ofd.FileName;
               comboBox1.Items.Clear();
               dataindex = 0;
           }
           c = 1;
           b = true;
           simpleOpenGlControl1.Invalidate();
           Gl.glClearColor(1, 1, 1, 1);
           Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_STENCIL_BUFFER_BIT); 
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitGraphics();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
           
        }
        private void simpleOpenGlControl1_Load(object sender, EventArgs e)
        {
           
        }
        private void button4_Click(object sender, EventArgs e)
        {
            simpleOpenGlControl1.Invalidate();
            translateonX = true;
           y+= 10;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            simpleOpenGlControl1.Invalidate();
            y -= 10;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            simpleOpenGlControl1.Invalidate();
            x -= 10;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            simpleOpenGlControl1.Invalidate();
            translateonX = true;
            x += 2;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            scale = true;
            simpleOpenGlControl1.Invalidate();
            scaleX += 0.1;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            scale = true;
            simpleOpenGlControl1.Invalidate();
            scaleY += 0.1;
        }
        private void button11_Click(object sender, EventArgs e)
        {
            rotate1 = true;
            simpleOpenGlControl1.Invalidate();
            angle += 0.2;
        }
        private void button12_Click(object sender, EventArgs e)
        {
            rotate2= true;
            simpleOpenGlControl1.Invalidate();
            angle -= 0.2;
        }
        private void button13_Click(object sender, EventArgs e)
        {
            /*if ((comboBox2.SelectedItem == "continuous" && checkBox2.Checked == true) || 
                (comboBox2.SelectedItem == "continuous" && checkBox1.Checked == true) || 
                (comboBox2.SelectedItem == "discrete" && checkBox1.Checked == true) || 
                (comboBox2.SelectedItem == "discrete" && checkBox2.Checked == true))
            {
                var dataindex=m1.VarToIndex[comboBox1.SelectedItem];
                double s_min=0,s_max=0;
                m1.GetMinMaxValues((uint)dataindex,out s_min,out s_max);
                textBox2.Text = s_min.ToString();
                textBox3.Text = s_max.ToString();
            }*/
        }
        private void label4_Click(object sender, EventArgs e)
        {

        }
        private void button14_Click(object sender, EventArgs e)
        {
           
                calc = true; 
                simpleOpenGlControl1.Invalidate();   
        }
        private void button15_Click(object sender, EventArgs e)
        {
            calc2 = true;
            simpleOpenGlControl1.Invalidate();   
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        private void button2_Click(object sender, EventArgs e)
        {
            lines = true;
            simpleOpenGlControl1.Invalidate();
            storedContourValues.Clear();
            StoredMarchingValues.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rotateY = true;
            simpleOpenGlControl1.Invalidate();
            angle += 0.2;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            rotatenegY = true;
            simpleOpenGlControl1.Invalidate();
            angle -= 0.2;
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            rotateZ = true;
            simpleOpenGlControl1.Invalidate();
            angle += 0.2;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            rotatenegZ = true;
            simpleOpenGlControl1.Invalidate();
            angle -= 0.2;

        }
    }
}
