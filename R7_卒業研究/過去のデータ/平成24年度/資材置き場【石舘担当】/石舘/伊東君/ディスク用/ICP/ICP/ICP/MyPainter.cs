using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenNI;
using NITE;

namespace ICP
{
    class MyPainter
    {
        private bool _shouludRun;
        public bool _Enabled;
        public bool _ccFlag;

        private MyCanvas _myCanvas;    //この人のキャンバス
        private  MyBrush _myBrush;     //この人の筆
        private  MyBrush _myEraser;
        private MyPointGenerator _pg;     //座標データ生成器
        private MyColors _colors;


        public MyPainter(MyPointGenerator pg)
        {
            _pg = pg;
            _pg.SendPoints += new EventHandler<MyPointEventArgs>(ReceivePoints);
            _pg.SendComplete += new EventHandler<MyPointEventArgs>(ReceiveComplete);
            _pg.SendSkeleton += new EventHandler<MySkeletonEventArgs>(ReceiveSkeleton);
            _pg.SendFar += new EventHandler<MyFarEventArgs>(_pg_SendFar);
            _pg.sendMiddle += new EventHandler<MyMiddleEventArgs>(_pg_sendMiddle);
            _pg.sendNear += new EventHandler<MyNearEventArgs>(_pg_sendNear);
            _pg.sendInvalid += new EventHandler<MyInvalidEventArgs>(_pg_sendInvalid);
            //_pg.sendSwipe += new EventHandler<MySwipeEventArgs>(_pg_sendSwipe);
            _pg.sendPush += new EventHandler<MyPushEventArgs>(_pg_sendPush);
            _pg.sendStatus += new EventHandler<MyStatusEventArgs>(_pg_sendStatus);
            _pg.sendAllClear += new EventHandler<MyAllClearEventArgs>(_pg_sendAllClear);
           
            _colors = new MyColors();
            _Enabled = false;
        }

        void _pg_sendAllClear(object sender, MyAllClearEventArgs e)
        {
            _myCanvas.AllClear();
        }

        void _pg_sendStatus(object sender, MyStatusEventArgs e)
        {
            _Enabled = e.Enabled;
            _ccFlag = e.ccflag;
            
        }

        public Color GetBrushColor()
        {
            return _myBrush.GetColor();
        }
        public int GetBrushSize()
        {
            return _myBrush.Radius;
        }


        void _pg_sendPush(object sender, MyPushEventArgs e)
        {
            Color p = _myCanvas.Pipette();
            int r = p.R;
            int g = p.G;
            int b = p.B;

            if (r == 255 && g == 255 && b == 255) return;
            if (r == 0 && g == 0 && b == 0)
            {
                _myBrush.ChangeBrushSize(60);
                _myEraser.ChangeBrushSize(70);
            }
            else if (r == 1 && g == 1 && b == 1)
            {
                _myBrush.ChangeBrushSize(40);
                _myEraser.ChangeBrushSize(50);
            }
            else if (r == 2 && g == 2 && b == 2)
            {
                _myBrush.ChangeBrushSize(20);
                _myEraser.ChangeBrushSize(30);
            }
            else if (r == 3 && g == 3 && b == 3)
            {
                _myBrush.ChangeBrushSize(10);
                _myEraser.ChangeBrushSize(5);
            }
            else
            {
                _myBrush.ChangeBrushColor(_myCanvas.Pipette());
            }
            
        }

        //void _pg_sendSwipe(object sender, MySwipeEventArgs e)
        //{
        //    _myBrush.ChangeBrushColor(_colors.GetColor());
            
        //}


        void _pg_sendInvalid(object sender, MyInvalidEventArgs e)
        {
            _myBrush.ChangeBrushSize(0);
        }

        void _pg_sendNear(object sender, MyNearEventArgs e)
        {
            _myBrush.ChangeBrushSize(3);
        }

        void _pg_sendMiddle(object sender, MyMiddleEventArgs e)
        {
            _myBrush.ChangeBrushSize(10);
        }

        void _pg_SendFar(object sender, MyFarEventArgs e)
        {
            _myBrush.ChangeBrushSize(20);
        }

        private void ReceivePoints(object sender, MyPointEventArgs e)
        {
            if (_shouludRun)
            {
                foreach (VariousPoint vp in e.Points)
                {
                    if (vp.Type == VariousPoint.PointType.DrawPoint)
                    {
                        this.DrawPoint(vp.Point.X, vp.Point.Y);
                    }
                    else
                    {
                        this.ErasePoint(vp.Point.X, vp.Point.Y);
                    }
                }
            }
        }

        private void ReceiveComplete(object sender, MyPointEventArgs e)
        {
            if (_shouludRun)
            {
                lock (_myCanvas)
                {
                    _myCanvas.Paint();
                }
            }
        }

        private void ReceiveSkeleton(object sender, MySkeletonEventArgs e)
        {
            if (_shouludRun)
            {
                this.DrawSkeleton(e.Dict, _myBrush,_myEraser);
            }
        }
        public void RePaint()
        {
            if (_shouludRun)
            {
                _myCanvas.Paint();
            }
        }

        //キャンバスを持たせる
        public void HaveCanvas(MyCanvas ic)
        {
            _myCanvas = ic;
            _myCanvas.JointPainter(this);
        }

        //筆を持たせる
        public void HaveBrush(MyBrush ib)
        {
            _myBrush = ib;
        }

        public void HaveEraser(MyBrush ib)
        {
            _myEraser = ib;
        }
        //点を描く
        private void DrawPoint(int x, int y)
        {
            lock (_myCanvas)
            {
                _myCanvas.DrawPoint(_myBrush, x, y);
            }
        }
        //点を消す
        private void ErasePoint(int x, int y)
        {
            lock (_myCanvas)
            {
                _myCanvas.DrawPoint(_myEraser, x, y);
            }
        }

        private void DrawSkeleton(Dictionary<SkeletonJoint, SkeletonJointPosition> dict,MyBrush mb,MyBrush er)
        {
            lock (_myCanvas)
            {
                //_myCanvas.CleanSkeleton();
                _myCanvas.DrawLine(dict, SkeletonJoint.Head, SkeletonJoint.Neck);

                _myCanvas.DrawLine(dict, SkeletonJoint.LeftShoulder, SkeletonJoint.Torso);
                _myCanvas.DrawLine(dict, SkeletonJoint.RightShoulder, SkeletonJoint.Torso);

                _myCanvas.DrawLine(dict, SkeletonJoint.Neck, SkeletonJoint.LeftShoulder);
                _myCanvas.DrawLine(dict, SkeletonJoint.LeftShoulder, SkeletonJoint.LeftElbow);
                _myCanvas.DrawLine(dict, SkeletonJoint.LeftElbow, SkeletonJoint.LeftHand);

                _myCanvas.DrawLine(dict, SkeletonJoint.Neck, SkeletonJoint.RightShoulder);
                _myCanvas.DrawLine(dict, SkeletonJoint.RightShoulder, SkeletonJoint.RightElbow);
                _myCanvas.DrawLine(dict, SkeletonJoint.RightElbow, SkeletonJoint.RightHand);

                _myCanvas.DrawLine(dict, SkeletonJoint.LeftHip, SkeletonJoint.Torso);
                _myCanvas.DrawLine(dict, SkeletonJoint.RightHip, SkeletonJoint.Torso);
                _myCanvas.DrawLine(dict, SkeletonJoint.LeftHip, SkeletonJoint.RightHip);

                _myCanvas.DrawLine(dict, SkeletonJoint.LeftHip, SkeletonJoint.LeftKnee);
                _myCanvas.DrawLine(dict, SkeletonJoint.LeftKnee, SkeletonJoint.LeftFoot);

                _myCanvas.DrawLine(dict, SkeletonJoint.RightHip, SkeletonJoint.RightKnee);
                _myCanvas.DrawLine(dict, SkeletonJoint.RightKnee, SkeletonJoint.RightFoot);
                _myCanvas.DrawCursor(dict, SkeletonJoint.RightHand, SkeletonJoint.LeftHand,mb,er);
                //_myCanvas.Paint();
            }
        }

        private void DrawDebug(String message, int x, int y, int z, int r, int g, int b)
        {
            
           // s += message;
        }
        public void Start()
        {
            _shouludRun = true;
        }

        public void Stop()
        {
            _shouludRun = false;
        }

    }
}
