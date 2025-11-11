using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ICP
{
    public partial class MainForm : Form
    {
        private MyPointGenerator pg;
        private MyPainter painter;
        private MyCanvas canvas;
        private MyBrush brush;
        private MyBrush eraser;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
            pg = new MyPointGenerator();
            canvas = new MyCanvas(this);
            brush = new MyBrush(10);
            eraser = new MyBrush(Color.White, 20);
            painter = new MyPainter(pg);
            painter.HaveBrush(brush);
            painter.HaveEraser(eraser);
            painter.HaveCanvas(canvas);
            painter.Start();
            pg.Start();
            this.Invalidate();
        }
        

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            pg.Stop();
            painter.Stop();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            painter.RePaint();
        }

        //private void MainForm_MouseDown(object sender, MouseEventArgs e)
        //{
        //    pg.Enabled = true;
        //    pg.Add(new Point(e.X, e.Y));
        //}

        //private void MainForm_MouseMove(object sender, MouseEventArgs e)
        //{
        //    pg.Add(new Point(e.X, e.Y));
        //}

        //private void MainForm_MouseUp(object sender, MouseEventArgs e)
        //{
        //    pg.Enabled = false;
        //}   


    }
}
