using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNI;
namespace ICP
{
    class MySkeletonEventArgs : EventArgs
    {

        public Dictionary<SkeletonJoint, SkeletonJointPosition> Dict 
        {
            get;
            set;
        }

        public MySkeletonEventArgs(Dictionary<SkeletonJoint, SkeletonJointPosition> dict)
        {
            Dict = dict;
        }
    }
}
