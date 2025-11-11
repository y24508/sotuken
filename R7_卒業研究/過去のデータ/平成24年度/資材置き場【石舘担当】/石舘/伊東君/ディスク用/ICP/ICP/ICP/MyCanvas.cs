using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using OpenNI;

namespace ICP
{
    class MyCanvas
    {
        private Form _form;
        private Bitmap _bmp;
        private Bitmap result;
        public Bitmap _palette;
        private MyPainter _painter;
        private String[] label = {"書ける","書けない","パレット表示"};
        private String[] mode = { "～", "ー", "●", "■" };
        private Color _pipette;

        private struct Cursor
        {
            public Point3D rightHandPoint;
            public Point3D leftHandPoint;
            public MyBrush mb;
            public MyBrush er;
            public Cursor(Point3D r, Point3D l,MyBrush mb,MyBrush er)
            {
                rightHandPoint = r;
                leftHandPoint = l;
                this.mb = mb;
                this.er = er;
            }

        }

        private List<Cursor> cursors;

        private struct Line
        {
            public Point3D toPoint;
            public Point3D fromPoint;
            public Line(Point3D p1, Point3D p2)
            {
                toPoint = p1;
                fromPoint = p2;
            }
        };

        private List<Line> lines;

        public MyCanvas(Form f)
        {
            _form = f;
            _bmp = new Bitmap(f.Size.Width, f.Size.Height);
            _palette = f.BackgroundImage as Bitmap;
            //_bmph = new Bitmap(f.Size.Width, f.Size.Height);
            result = new Bitmap(f.Size.Width, f.Size.Height);
            
            Graphics g = Graphics.FromImage(_bmp);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, _bmp.Size.Width, _bmp.Size.Height);

            

            lines = new List<Line>();
            cursors = new List<Cursor>();


        }

        public void JointPainter(MyPainter painter){
            _painter=painter;
        }

        public void DrawPoint(MyBrush ib, int x, int y)
        {
            lock(_bmp)
            {
                Graphics g = Graphics.FromImage(_bmp);
                g.FillEllipse(ib.Brush, x - ib.Radius, y - ib.Radius, ib.Diameter, ib.Diameter);
                
            }


        }

        public void DrawLine(Dictionary<SkeletonJoint, SkeletonJointPosition> dict, SkeletonJoint j1, SkeletonJoint j2)
        {
            Point3D pos1 = dict[j1].Position;
            Point3D pos2 = dict[j2].Position;

            if (dict[j1].Confidence == 0 || dict[j2].Confidence == 0)
                return;
            lines.Add(new Line(pos1, pos2));
        }
        public void DrawCursor(Dictionary<SkeletonJoint, SkeletonJointPosition> dict, SkeletonJoint r, SkeletonJoint l,MyBrush mb,MyBrush er)
        {
            Point3D pos1 = dict[r].Position;
            Point3D pos2 = dict[l].Position;

            if (dict[r].Confidence == 0 || dict[l].Confidence == 0)
                return;
            cursors.Add(new Cursor(pos1, pos2,mb,er));
        }

        //public void DrawDebug(String s)
        //{
        //    //Graphics g = Graphics.FromImage(_bmp);
        //    //g.DrawString(s, new Font("Arial", 12), new SolidBrush(Color.Green), 0, 0);
        //}

        //public void CleanSkeleton()
        //{
        //    //Graphics g = Graphics.FromImage(_bmph);
        //    //g.FillRectangle(new SolidBrush(Color.White), 0, 0, _bmp.Size.Width, _bmp.Size.Height);
        //    //_bmph.MakeTransparent(Color.White);
        //}

        public Color Pipette()
        {
            return _pipette;
        }
        public void AllClear()
        {
            lock (_bmp)
            {
                Graphics g = Graphics.FromImage(_bmp);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, _bmp.Width, _bmp.Height);
            }
        }
        public void Paint()

        {
            lock (_form)
            {

                Graphics rg = Graphics.FromImage(result);
                if (_painter._ccFlag)
                {
                    rg.DrawImage(_palette,0,0);
                }
                else
                {
                    rg.DrawImage(_bmp, 0, 0);
                }


                foreach(Line line in lines)
                {
                    rg.DrawLine(
                        new Pen(Color.Black),
                        new Point((int)line.fromPoint.X, (int)line.fromPoint.Y),
                        new Point((int)line.toPoint.X,(int)line.toPoint.Y)
                        );
                }
                lines.Clear();
                foreach (Cursor cursor in cursors)
                {
                    int mbsize = cursor.mb.Radius;
                    int ersize = cursor.er.Radius;
                    rg.DrawEllipse(new Pen(Color.Black), (int)cursor.rightHandPoint.X - mbsize, (int)cursor.rightHandPoint.Y - mbsize, mbsize * 2, mbsize * 2);
                    rg.DrawEllipse(new Pen(Color.Black), (int)cursor.leftHandPoint.X - ersize, (int)cursor.leftHandPoint.Y - ersize, ersize * 2, ersize * 2);
                    int x =(int)cursor.rightHandPoint.X;
                    int y =(int)(int)cursor.rightHandPoint.Y;
                    if (x < _palette.Width && x >= 0 && y < _palette.Height && y >= 0)
                    {
                        _pipette = _palette.GetPixel(x, y);
                    }
                }
           
                cursors.Clear();
                if (_painter._ccFlag)
                {

                    rg.DrawString("  　　　　　　" + label[2], new Font("MSゴシック", 18), new SolidBrush(Color.Black), 0, 0);
                }
                else if (_painter._Enabled)
                {
                    rg.DrawString("  　　　　　　" + label[0], new Font("MSゴシック", 18), new SolidBrush(Color.Black), 0, 0);
                }
                else
                {
                    rg.DrawString(" 　　　　　 　" + label[1], new Font("MSゴシック", 18), new SolidBrush(Color.Black), 0, 0);
                }
                rg.FillEllipse(new SolidBrush(_painter.GetBrushColor()), 5, 5, _painter.GetBrushSize() * 2, _painter.GetBrushSize() * 2);
                rg.Dispose();
                Graphics g = _form.CreateGraphics();
                g.DrawImage(result, 0, 0);
                g.Dispose();
            }
        }
    }
}
