using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ICP
{
    class MyPointEventArgs : EventArgs
    {

        public VariousPoint[] Points
        {
            get;
            set;
        }

        public int PointsCount
        {
            get;
            private set;
        }

        public MyPointEventArgs()
        {
        }

        public MyPointEventArgs(List<VariousPoint> points)
        {
            PointsCount = points.Count;
            Points = new VariousPoint[points.Count];
            points.CopyTo(Points);
        }

    }
}
