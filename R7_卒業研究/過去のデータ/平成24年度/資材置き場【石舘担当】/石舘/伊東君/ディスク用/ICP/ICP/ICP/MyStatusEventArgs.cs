using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICP
{
    class MyStatusEventArgs:EventArgs
    {
        public bool Enabled
        {
            set;
            get;
        }
        public bool ccflag
        {
            set;
            get;
        }
        public MyStatusEventArgs(bool e,bool c)
        {
            Enabled = e;
            ccflag = c;
        }

    }
}
