using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ICP
{
    class MyBrush
    {

        private SolidBrush sb;
        //private SolidBrush currentBrush;
        private int radius = 10;

        public void ChangeBrushColor(Color c)
        {
            sb = new SolidBrush(c);
        }

        public void ChangeBrushSize(int rad)
        {
            radius = rad;
        }
        public MyBrush()
        {
            sb = new SolidBrush(Color.Black);
        }

        public MyBrush(int r)
        {
            sb = new SolidBrush(Color.Aqua);
            radius = r;

        }
        
        public MyBrush(Color c ,int r)
        {
            sb = new SolidBrush(c);
            radius = r;
        }
        //ブラシの取得
        public SolidBrush Brush
        {
            get
            {
                return sb;
            }
        }

        //直径の取得
        public int Diameter
        {
            get
            {
                return radius * 2;
            }
        }
        //半径の取得、
        public int Radius
        {
            get
            {
                return radius;
            }
        }

        //色の取得
        public Color GetColor()
        {
            return sb.Color;
        }
    }
}
