using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace ICP
{
    public struct VariousPoint
    {
       
        public enum PointType
        {
            DrawPoint,
            EraserPoint
        }
        public Point Point;

        public PointType Type;
        public VariousPoint(PointType type, Point point)
        {
            this.Type = type;
            this.Point = point;
        }


    }
}
