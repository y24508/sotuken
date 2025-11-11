using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ICP
{
    class MyColors
    {
        private List<Color> colors;
        private int index;
        public MyColors()
        {
            colors = new List<Color>();
            colors.Add(Color.Black);
            colors.Add(Color.Red);
            colors.Add(Color.Green);
            colors.Add(Color.Blue);
            index = 0;
        }
        public Color GetColor()
        {
            index ++;
            if(index >= colors.Count)
            {
                index = 0;
            }
            return colors[index];
        }
    }
}
