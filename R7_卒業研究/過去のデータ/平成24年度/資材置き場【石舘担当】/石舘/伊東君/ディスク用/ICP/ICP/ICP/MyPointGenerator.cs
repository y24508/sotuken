using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using OpenNI;
using NITE;

namespace ICP
{
    sealed class MyPointGenerator
    {
        public event EventHandler<MyPointEventArgs> SendPoints;
        public event EventHandler<MyPointEventArgs> SendComplete;
        public event EventHandler<MySkeletonEventArgs> SendSkeleton;
        public event EventHandler<MyFarEventArgs> SendFar;
        public event EventHandler<MyMiddleEventArgs> sendMiddle;
        public event EventHandler<MyNearEventArgs> sendNear;
        public event EventHandler<MyInvalidEventArgs> sendInvalid;
        public event EventHandler<MySwipeEventArgs> sendSwipe;
        public event EventHandler<MyPushEventArgs> sendPush;
        public event EventHandler<MyStatusEventArgs> sendStatus;
        public event EventHandler<MyAllClearEventArgs> sendAllClear;

        
        //ファイルを読み込み
        private readonly string SAMPLE_XML_FILE = @"ito2012.xml";
        private readonly string MY_CONFFIG_FILE = @"Demo.xml";
        //generatorやcapabilityの宣言など
        private Context context;
        private ScriptNode scriptNode;
        private DepthGenerator depth;
        private UserGenerator userGenerator;
        private ImageGenerator imageGenerator;
        private SkeletonCapability skeletonCapbility;
        private PoseDetectionCapability poseDetectionCapability;
        private string calibPose;
        private Thread readerThread;
        private int rlCount;

        //
        private MyConfig CONFIG;


   

        //ディクショナリィ
        private Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> joints;

        //flag
        
        private bool colorChangeflag = false;
        private bool shouldDrawSkeleton = true;

        //セッションおよびプッシュ検出
        private SessionManager sessionManager;
        enum SessionState
        {
            NotSession,
            StartSession,
            InSession
        }

        private SessionState sessionState = SessionState.NotSession;

        //ジェスチャ用
        SwipeDetector swipe;
        PushDetector push;
        WaveDetector wave;
        SteadyDetector steady;
        CircleDetector circle;

        //プロットイベントの有効／無効
        public bool Enabled
        {
            get;
            set;
        }
        public bool EraserEnabled
        {
            get;
            set;
        }

        public bool Dragged
        {
            get;
            set;
        }
        //停止用フラグ
        private volatile bool _shouldStop;

        //生成するプロットイベント群
        private List<VariousPoint> _points;

        public MyPointGenerator()
        {
            _points = new List<VariousPoint>();
            Enabled = false;
            EraserEnabled = false;
            Dragged = false;
            CONFIG = MyConfigReader.Read(MY_CONFFIG_FILE);
        }

        public void Start()
        {

            this.context = Context.CreateFromXmlFile(SAMPLE_XML_FILE, out scriptNode);
            this.rlCount = 0;
            this.context.GlobalMirror = true;
            //depthに取得した値を入れている。depth=深度
            this.depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            //depthがNullだった場合、エラー。
            if (this.depth == null)
            {
                throw new Exception("Viewer must have a depth node!");
            }

            imageGenerator = context.FindExistingNode(NodeType.Image) as ImageGenerator;
            if (imageGenerator == null)
            {
                throw new Exception(context.GlobalErrorState);
            }
            this.userGenerator = new UserGenerator(this.context);
            this.skeletonCapbility = this.userGenerator.SkeletonCapability;
            this.poseDetectionCapability = this.userGenerator.PoseDetectionCapability;
            this.calibPose = this.skeletonCapbility.CalibrationPose;

            //イベント
            this.userGenerator.NewUser += userGenerator_NewUser;
            this.userGenerator.LostUser += userGenerator_LostUser;
            this.poseDetectionCapability.PoseDetected += poseDetectionCapability_PoseDetected;
            this.skeletonCapbility.CalibrationComplete += skeletonCapbility_CalibrationComplete;

            this.skeletonCapbility.SetSkeletonProfile(SkeletonProfile.All);
            //ディクショナリィ
            this.joints = new Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>();
            this.userGenerator.StartGenerating();

            //セッション関係
            sessionManager = new SessionManager(context, "Click,Wave,RaiseHand", "Click");
            sessionManager.SessionStart += new EventHandler<PositionEventArgs>(sessionManager_SessionStart);
            sessionManager.SessionEnd += new EventHandler(sessionManager_SessionEnd);
            
            swipe = new SwipeDetector();
            //swipe.GeneralSwipe += new EventHandler<DirectionVelocityAngleEventArgs>(swipe_GeneralSwipe);
            swipe.SwipeUp += new EventHandler<VelocityAngleEventArgs>(swipe_SwipeUp);
            swipe.SwipeDown += new EventHandler<VelocityAngleEventArgs>(swipe_SwipeDown);
            swipe.SwipeLeft += new EventHandler<VelocityAngleEventArgs>(swipe_SwipeLeft);
            swipe.SwipeRight += new EventHandler<VelocityAngleEventArgs>(swipe_SwipeRight);

            push = new PushDetector();
            push.Stable += new EventHandler<VelocityEventArgs>(push_Stable);

            wave = new WaveDetector();
            wave.PointDestroy += new EventHandler<IdEventArgs>(wave_PointDestroy);

            steady = new SteadyDetector();
            steady.Steady += new EventHandler<SteadyEventArgs>(steady_Steady);

            circle = new CircleDetector();
            circle.OnCircle += new EventHandler<CircleEventArgs>(circle_OnCircle);


            sessionManager.AddListener(swipe);
            sessionManager.AddListener(push);
            sessionManager.AddListener(wave);
            sessionManager.AddListener(steady);
            sessionManager.AddListener(circle);
           
            //sessionManager.
            //スレッド開始
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
            Thread th = new Thread(GeneratePoints);
            _shouldStop = false;
            th.Start();
        }

        void circle_OnCircle(object sender, CircleEventArgs e)
        {
            if (colorChangeflag) return;
            if (Enabled == true)
            {
                Dragged = true;
            }
            rlCount = 0;
            Console.WriteLine("円運動");
            //throw new NotImplementedException();
        }

        void steady_Steady(object sender, SteadyEventArgs e)
        {
            if (rlCount > 10)
            {
                rlCount = 0;
            }
            Console.WriteLine("ステディ");
            if (colorChangeflag)
            {
                OnSendPush();
                Thread.Sleep(CONFIG.SteadyInterval);
            }
            else
            {
                if (Dragged == true)
                {
                    this.Enabled = false;
                    this.Dragged = false;
                    Console.WriteLine("書かない！！");
                    Thread.Sleep(CONFIG.SteadyInterval);
                }
                else
                {
                    Dragged = true;
                }
            }
        }

        void wave_PointDestroy(object sender, IdEventArgs e)
        {
            rlCount = 0;
            Console.WriteLine("ポイントなし");
            if (colorChangeflag) return;
            
            this.Enabled = false;
            this.Dragged = false;
            Console.WriteLine("書かない！！");
        }

        void push_Stable(object sender, VelocityEventArgs e)
        {
            rlCount = 0;
            Console.WriteLine("プッシュ安定");

            if (colorChangeflag)
            {
                    OnSendPush();
                    Thread.Sleep(CONFIG.PushStableInterval);
            }
            else
            {
                this.Enabled = true;
                this.Dragged = false;
                Console.WriteLine("書ける");
                Thread.Sleep(CONFIG.PushStableInterval);
            }
        }

        void swipe_SwipeRight(object sender, VelocityAngleEventArgs e)
        {
            if (colorChangeflag) return;
            if (Enabled == true){
                Dragged = true;
            }
            if (!this.Enabled)
            {
                rlCount++;
                if (rlCount >10)
                {
                    OnSendAllClear();
                }
            }
            
            Console.WriteLine("右のスワイプ");
            Thread.Sleep(CONFIG.SwipeInterval);
        }

        

        void swipe_SwipeLeft(object sender, VelocityAngleEventArgs e)
        {
            if (colorChangeflag) return;
            if (Enabled == true)
            {
                Dragged = true;
            }
            if (!this.Enabled)
            {
                rlCount++;
                if (rlCount > 10)
                {
                    OnSendAllClear();
                }
            }
            Console.WriteLine("左のスワイプ");
            Thread.Sleep(CONFIG.SwipeInterval);
        }

        void swipe_SwipeDown(object sender, VelocityAngleEventArgs e)
        {
            if (colorChangeflag) return;
            if (Enabled == true)
            {
                Dragged = true;
            }
            rlCount = 0;
            Console.WriteLine("下のスワイプ");
            Thread.Sleep(CONFIG.SwipeInterval);

        }

        void swipe_SwipeUp(object sender, VelocityAngleEventArgs e)
        {
            if (colorChangeflag) return;
            if (Enabled == true)
            {
                Dragged = true;
            }
            rlCount = 0;
            Console.WriteLine("上のスワイプ");
            Thread.Sleep(CONFIG.SwipeInterval);
        }

        void sessionManager_SessionEnd(object sender, EventArgs e)
        {
            sessionState = SessionState.NotSession;
            Console.WriteLine(sessionState.ToString());
        }

        void sessionManager_SessionStart(object sender, PositionEventArgs e)
        {
            sessionState = SessionState.StartSession;
            Console.WriteLine(sessionState.ToString());
        }

        public void Stop()
        {
            _shouldStop = true;
        }

        public void Add(VariousPoint item)
        {
            lock (_points)
            {
                     _points.Add(item);
             }
        }
        public void GeneratePoints()
        {

            while (!_shouldStop)
            {
                Thread.Sleep(CONFIG.GeneratePointInterval);
                lock (_points)
                {
                    if (_points.Count > 0)
                    {
                        OnSendPoint(_points);
                    }
                    _points.Clear();
                    OnSendComplete();
                }

            }
        }


        public void OnSendPoint(List<VariousPoint> points)
        {
            if (points != null)
            {
                SendPoints(this, new MyPointEventArgs(points));
            }
        }
        public void OnSendComplete()
        {
            SendComplete(this, new MyPointEventArgs());
        }

        public void OnSendSkeleton(Dictionary<SkeletonJoint, SkeletonJointPosition> dict)
        {
            SendSkeleton(this, new MySkeletonEventArgs(dict));
        }

        public void OnSendFar()
        {
            SendFar(this, new MyFarEventArgs());
        }

        public void OnSendMiddle()
        {
            sendMiddle(this,new MyMiddleEventArgs());
        }

        public void OnSendNear()
        {
            sendNear(this, new MyNearEventArgs());
        }

        public void OnSendInvalid()
        {
            sendInvalid(this, new MyInvalidEventArgs());
        }

        public void OnSendSwipe()
        {
            sendSwipe(this, new MySwipeEventArgs());
        }
        public void OnSendPush()
        {
            sendPush(this, new MyPushEventArgs());
        }
        private void OnSendStatus(bool enabled,bool ccflag)
        {
            sendStatus(this, new MyStatusEventArgs(enabled,ccflag));
        }
        private void OnSendAllClear()
        {
            sendAllClear(this, new MyAllClearEventArgs());
        }
        //追加したもの
        private unsafe void ReaderThread()
        {
            //深度の実データを入れている。
            DepthMetaData depthMD = new DepthMetaData();

            //フラグがＯＦＦになるまで無限ループ
            while (!_shouldStop)
            {
                try
                {
                    this.context.WaitAndUpdateAll();
                    sessionManager.Update(context);
                    Thread.Sleep(CONFIG.ContextUpdateInterval);
                    
                }
                catch (Exception)
                {
                }
                ImageMetaData imageMD = imageGenerator.GetMetaData();
                this.depth.GetMetaData(depthMD);

                lock (this)
                {
                    int[] users = this.userGenerator.GetUsers();
                    //便利なfor分ｆ（userにusersを入れている。)
                    foreach (int user in users)
                    {
                        if (true)
                        {

                            //入った人数分以下の処理を行う。
                            if (this.shouldDrawSkeleton && this.skeletonCapbility.IsTracking(user))
                            {

                                DrawSkeleton(user);
                                //DrawFlag(user, SkeletonJoint.RightHand, SkeletonJoint.RightShoulder);
                                DrawCircle(user, SkeletonJoint.RightHand);
                                CleanFlag(user, SkeletonJoint.LeftHand, SkeletonJoint.LeftShoulder);
                                CleanCircle(user, SkeletonJoint.LeftHand);
                                ChangeBrushSize(user, SkeletonJoint.Neck);
                                DrawStatus(user, Enabled, colorChangeflag);
                                //Console.WriteLine(user);
                            }
                        }

                    }
                }
            }
        }
        
         void skeletonCapbility_CalibrationComplete(object sender, CalibrationProgressEventArgs e)
        {
            if (e.Status == CalibrationStatus.OK)
            {
                this.skeletonCapbility.StartTracking(e.ID);
                this.joints.Add(e.ID, new Dictionary<SkeletonJoint, SkeletonJointPosition>());
            }
            else if (e.Status != CalibrationStatus.ManualAbort)
            {
                if (this.skeletonCapbility.DoesNeedPoseForCalibration)
                {
                    this.poseDetectionCapability.StartPoseDetection(calibPose, e.ID);
                }
                else
                {
                    this.skeletonCapbility.RequestCalibration(e.ID, true);
                }
            }
        }

        void poseDetectionCapability_PoseDetected(object sender, PoseDetectedEventArgs e)
        {
            this.poseDetectionCapability.StopPoseDetection(e.ID);
            this.skeletonCapbility.RequestCalibration(e.ID, true);
        }
        //ユーザの入りの処理
        void userGenerator_NewUser(object sender, NewUserEventArgs e)
        {
            if (this.skeletonCapbility.DoesNeedPoseForCalibration)
            {
                this.poseDetectionCapability.StartPoseDetection(this.calibPose, e.ID);
            }
            else
            {
                this.skeletonCapbility.RequestCalibration(e.ID, true);
            }
        }
        //ユーザの出の処理
		void userGenerator_LostUser(object sender, UserLostEventArgs e)
        {
            this.joints.Remove(e.ID);
        }

        private void GetJoint(int user, SkeletonJoint joint)
        {
            SkeletonJointPosition pos = this.skeletonCapbility.GetSkeletonJointPosition(user, joint);
            if (pos.Position.Z == 0)
            {
                pos.Confidence = 0;
            }
            else
            {
                pos.Position = this.depth.ConvertRealWorldToProjective(pos.Position);
            }
            this.joints[user][joint] = pos;
        }

        // *2
        private void GetJoints(int user)
        {
            //*3
            GetJoint(user, SkeletonJoint.Head);
            GetJoint(user, SkeletonJoint.Neck);

            GetJoint(user, SkeletonJoint.LeftShoulder);
            GetJoint(user, SkeletonJoint.LeftElbow);
            GetJoint(user, SkeletonJoint.LeftHand);

            GetJoint(user, SkeletonJoint.RightShoulder);
            GetJoint(user, SkeletonJoint.RightElbow);
            GetJoint(user, SkeletonJoint.RightHand);

            GetJoint(user, SkeletonJoint.Torso);

            GetJoint(user, SkeletonJoint.LeftHip);
            GetJoint(user, SkeletonJoint.LeftKnee);
            GetJoint(user, SkeletonJoint.LeftFoot);

            GetJoint(user, SkeletonJoint.RightHip);
            GetJoint(user, SkeletonJoint.RightKnee);
            GetJoint(user, SkeletonJoint.RightFoot);
        }
       
        //ユーザに色を与えている。*1
        //private void DrawSkeleton(Graphics g, Color color, int user)
        private void DrawSkeleton(int user)    
        {
            //*2
            GetJoints(user);
            Dictionary<SkeletonJoint, SkeletonJointPosition> dict = this.joints[user];
            //線を引く*4
            OnSendSkeleton(dict);
            
        }

        private void DrawStatus(int user, bool enabled,bool ccflag)
        {
            OnSendStatus(enabled,ccflag);
        }

       

        private void ChangeBrushSize(int user, SkeletonJoint neck)
        {
            SkeletonJointPosition posNK = this.skeletonCapbility.GetSkeletonJointPosition(user, neck);
            if (posNK.Position.Z == 0)
            {
                posNK.Confidence = 0;
            }
            else
            {
                posNK.Position = this.depth.ConvertRealWorldToProjective(posNK.Position);
            }

            Point3D pos3DNK = posNK.Position;

            if (CONFIG.MiddleDistance <= pos3DNK.Z)
            {
                colorChangeflag = false;
               // OnSendFar();
            }
            else if (CONFIG.PaletteDistance <= pos3DNK.Z && CONFIG.MiddleDistance >pos3DNK.Z)
            {
                colorChangeflag = false;
                //OnSendMiddle();
            }
            else if (CONFIG.PaletteDistance > pos3DNK.Z)
            {
                Enabled = false;
                EraserEnabled = false;
                colorChangeflag = true;
                //OnSendInvalid();
            }


        }

        //書くFlag
        private void DrawFlag(int user, SkeletonJoint hand, SkeletonJoint neck)
        {
            SkeletonJointPosition posRH = this.skeletonCapbility.GetSkeletonJointPosition(user, hand);
            if (posRH.Position.Z == 0)
            {
                posRH.Confidence = 0;
            }
            else
            {
                posRH.Position = this.depth.ConvertRealWorldToProjective(posRH.Position);
            }
            Point3D pos3DRH = posRH.Position;
            SkeletonJointPosition posNK = this.skeletonCapbility.GetSkeletonJointPosition(user, neck);
            if (posNK.Position.Z == 0)
            {
                posNK.Confidence = 0;
            }
            else
            {
                posNK.Position = this.depth.ConvertRealWorldToProjective(posNK.Position);
            }
            Point3D pos3DNK = posNK.Position;
            if (pos3DNK.Z - 50 > pos3DRH.Z)
            {
                this.Enabled = false;
            }
            else
            {
                //this.Enabled = false;
            }

        }


        //消すFlag
        private void CleanFlag(int user, SkeletonJoint hand, SkeletonJoint neck)
        {
            SkeletonJointPosition posLH = this.skeletonCapbility.GetSkeletonJointPosition(user, hand);
            if (posLH.Position.Z == 0)
            {
                posLH.Confidence = 0;
            }
            else
            {
                posLH.Position = this.depth.ConvertRealWorldToProjective(posLH.Position);
            }
            Point3D pos3DLH = posLH.Position;
            SkeletonJointPosition posNK = this.skeletonCapbility.GetSkeletonJointPosition(user, neck);
            if (posNK.Position.Z == 0)
            {
                posNK.Confidence = 0;
            }
            else
            {
                posNK.Position = this.depth.ConvertRealWorldToProjective(posNK.Position);
            }
            Point3D pos3DNK = posNK.Position;
            if (pos3DNK.Z - CONFIG.LeftArmDistance > pos3DLH.Z)
            {
                if (!colorChangeflag)
                {
                    this.EraserEnabled = true;
                }
            }
            else
            {
                this.EraserEnabled = false;
            }
        }

        private void CleanCircle(int user, SkeletonJoint joint)
        {
            if (this.EraserEnabled == true)
            {
                SkeletonJointPosition pos = this.skeletonCapbility.GetSkeletonJointPosition(user, joint);
                if (pos.Position.Z == 0)
                {
                    pos.Confidence = 0;
                }
                else
                {
                    pos.Position = this.depth.ConvertRealWorldToProjective(pos.Position);
                }
                Point p = new Point((int)pos.Position.X, (int)pos.Position.Y);
                this.Add(new VariousPoint(VariousPoint.PointType.EraserPoint,p));
            }

        }

       

        //○表示
        private void DrawCircle(int user, SkeletonJoint joint)
        {
            if (this.Enabled == true)
            {
                SkeletonJointPosition pos = this.skeletonCapbility.GetSkeletonJointPosition(user, joint);
                if (pos.Position.Z == 0)
                {
                    pos.Confidence = 0;
                }
                else
                {
                    pos.Position = this.depth.ConvertRealWorldToProjective(pos.Position);
                }
                Point p = new Point((int)pos.Position.X, (int)pos.Position.Y);
                this.Add(new VariousPoint(VariousPoint.PointType.DrawPoint, p));
            }

        }
       
    }
}
