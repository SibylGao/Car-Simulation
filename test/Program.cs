using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using Simulate;
using RVO;
using SharpNav;
using Window;
using SharpNav.Geometry;
using SharpNav.Pathfinding;
using System.Diagnostics;
using System.IO;

namespace Window
{
    //初始化RVO
    public static class Program
    {
        //private static Thread stepThread;
        

        public static IList<AgentClass> _agents = new List<AgentClass>();
        public static SongNav nav;

        public static Stopwatch sw = new Stopwatch();//用来帮助计算程序耗时
        static long stepCounts = 0;//统计步数
        static MainWindow mainWindow = new MainWindow();
        [STAThread]
        static void Main()
        {
            
            mainWindow.Run(10);
        }

        private static readonly char[] lineSplitChars = { ' ' };
        //public static void Read()
        //{
        //    string path = "1";
        //    sw.Start();
        //    using (StreamReader reader = new StreamReader(path))
        //    {
        //        string file = reader.ReadToEnd();
        //        int Nov = 0;


        //        string[] framesong = file.Split('\n');
        //        try
        //        {
        //            foreach (string frame in file.Split('\n'))
        //            {
        //                sw.Restart();
        //                //trim any extras
        //                string tl = frame;
        //                string[] line = tl.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
        //                if (line == null || line.Length == 0)
        //                    continue;

        //                _agents.Clear();
        //                for (int i = 1; i < line.Length - 2; i++)
        //                {
        //                    i++;
        //                    try
        //                    {
        //                        _agents.Add(new AgentClass(_agents.Count, new Simulate.Vector2(float.Parse(line[i])/100, float.Parse(line[i + 1])/100), new Simulate.Vector2(0, 0), AgentStates.Enter));
        //                    }
        //                    catch
        //                    {
        //                        Console.WriteLine("超过：" + i);
        //                    }

        //                }
        //                Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒");
        //                Thread.Sleep(100);
        //                //mainWindow.DrawCrowd();
        //                //mainWindow.SwapBuffers();
        //            }
        //        }
        //        catch
        //        {

        //        }
                
        //    }
        //}

        public static void Read()
        {
            string path = "1";
            sw.Start();
            using (StreamReader reader = new StreamReader(path))
            {
                List<string> list = new List<string>();
                string l;
                while ((l = reader.ReadLine()) != null)
                {
                    list.Add(l);
                }
                reader.Close();

                try
                {
                    foreach (string frame in list)
                    {
                        sw.Restart();
                        //trim any extras
                        string tl = frame;
                        string[] line = tl.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
                        if (line == null || line.Length == 0)
                            continue;

                        _agents.Clear();
                        for (int i = 1; i < line.Length - 2; i++)
                        {
                            i++;
                            try
                            {
                                _agents.Add(new AgentClass(_agents.Count, new Simulate.Vector2(float.Parse(line[i]) / 100, float.Parse(line[i + 1]) / 100), new Simulate.Vector2(0, 0), AgentStates.Enter));
                            }
                            catch
                            {
                                Console.WriteLine("超过：" + i);
                            }

                        }
                        Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒");
                        Thread.Sleep(100);
                        //mainWindow.DrawCrowd();
                        //mainWindow.SwapBuffers();
                    }
                }
                catch
                {

                }

            }
        }



        public static void SongMain()
        {
            /*
            读取配置文件
                得到agent信息与状态
                得到步长等仿真所需参数
                得到事件时间表
            初始化地图
                读取地图，得到障碍物与导航图信息
            初始化RVO
                初始化步长等默认参数
                设置障碍物
            初始化Agent
                设置不同人物的性别年龄等占比，等信息
                将agent加入RVO中
                为每个出现的agent设置目标与导航点
            初始化输出
                设置输出路径等
            新建线程，重复执行"每步仿真"
            设置重复执行函数
            */

//#if outfile
//            //输出agent信息文件
//            FileHelper.SetOutPath("agents.txt");
//            //for (int i = 0; i < Program._agents.Count; i++)
//            //{
//            //    //FileHelper.Write(_agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " " + _agents[i].navPoints.Count + " ");
//            //    //FileHelper.Write(Simulator.Instance.getAgentPosition(i).x_ + " " + Simulator.Instance.getAgentPosition(i).y_ + " ");
//            //}
//#endif

#if showmesh
            InitMap();
#endif
#if read
            Thread read = new Thread(Read);
            read.Start();
            //Read();
#else
            InitSetting();

            sw.Start();
            InitMap();
            Console.WriteLine("读取生成耗时： " + sw.ElapsedMilliseconds/1000);

            sw.Restart();
            InitRVO();
            InitAgents();
            Console.WriteLine("初始化人物耗时： " + sw.ElapsedMilliseconds / 1000);

            InitOutput();

            sw.Restart();
            Intervals();
#endif




        }

        private static void InitSetting()
        {
            SettingHelper.InitRead("set.xml");
            //SettingHelper.CreatXml();//创建一个示例XML

        }

        private static void InitOutput()
        {
            FileHelper.SetOutPath("output.txt");

#if outfile1
            for (int i = 0; i < _agents.Count; i++)
            {
                FileHelper.Write(_agents[i].age.ToString() + " ");
            }
            FileHelper.NewLine();
#endif
        }

        private static void InitMap()
        {
            //NavMesh navMesh1 = new NavMesh();
            //navMesh1.InitMesh("mesh.obj");
            
            nav = new SongNav("../../Meshes/mesh.obj");
            

        }

        /// <summary>
        /// RVO初始化函数
        /// 设置步长等等参数，设置障碍物
        /// </summary>
        private static void InitRVO()
        {
            ///* Specify the global time step of the simulation. */
            Simulator.Instance.setTimeStep(Settings.RVODefault.deltaT);
            Simulator.Instance.setAgentDefaults(Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, Settings.RVODefault.maxSpeed, new RVO.Vector2(1f, 1f));

            //FileHelper.SetOutPath("obs.txt");
            List<Simulate.Line> obs = nav.GetObstacle();
            for (int i = 0; i < obs.Count; i++)
            {
                if (obs[i].valid == true)
                {
                    IList<RVO.Vector2> ObsVector = new List<RVO.Vector2>();
                    ObsVector.Add(new RVO.Vector2(obs[i].point1.x_, obs[i].point1.y_));
                    ObsVector.Add(new RVO.Vector2(obs[i].point2.x_, obs[i].point2.y_));
                    Simulator.Instance.addObstacle(ObsVector);

                    //FileHelper.Write(obs[i].point1.x_+" "+ obs[i].point1.y_ +" "+ obs[i].point2.x_ + " " + obs[i].point2.y_,true);
                }
            }
            Simulator.Instance.processObstacles();
        }

        private static void InitObstacle()
        {
            IList<RVO.Vector2> ObsVector = new List<RVO.Vector2>();
            float x = 10;
            float z = 0;
            float sx = 10;
            float sz = 10;

            //Debug.Log(Obs.name);
            //Debug.Log((x + (sx * 0.5f))+ "  " +  (z + (sz * 0.5f)));
            //Debug.Log((x - (sx * 0.5f)) + "  " + (z + (sz * 0.5f)));
            //Debug.Log((x + (sx * 0.5f)) + "  " + (z - (sz * 0.5f)));
            //Debug.Log((x - (sx * 0.5f)) + "  " + (z - (sz * 0.5f)));

            ObsVector.Add(new RVO.Vector2(x + (sx * 0.5f), z + (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x - (sx * 0.5f), z + (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x - (sx * 0.5f), z - (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x + (sx * 0.5f), z - (sz * 0.5f)));

            Simulator.Instance.addObstacle(ObsVector);

            Simulator.Instance.processObstacles();
        }

        /// <summary>
        /// 添加所有agent,场景中agent与避障算法里的同步
        /// </summary>
        private static void InitAgents()
        {
            Simulate.Vector2 posStart;
            Simulate.Vector2 posTarget;
            //从配置文件中获取人数数据
            int numsBoys = int.Parse(SettingHelper.ReadAttribute("People", "Boys", "num"));
            //为每个agent配置随机起始与目标点
            for (int i = 0; i < numsBoys; i++)
            {
                //positionStart = new Simulate.Vector2(21.4f, -121f);new Simulate.Vector2(130.7f, -144f)
                var pointStart = nav.navMeshQuery.FindRandomPoint();
                var pointEnd = nav.navMeshQuery.FindRandomPoint();
                posStart = new Simulate.Vector2(pointStart.Position.X, pointStart.Position.Z);
                posTarget = new Simulate.Vector2(pointEnd.Position.X, pointEnd.Position.Z);
                //endPt = new NavPoint(new NavPolyId(20), new SVector3(-10, -2, 10));
                float randomSpeed = Simulate.MathHelper.RandomNormal(20,1f,0.6f,2f);
                int nov = Simulator.Instance.addAgent(new RVO.Vector2(posStart.x_, posStart.y_), Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, randomSpeed, new RVO.Vector2(1, 1));
                _agents.Add(new AgentClass(nov, posStart, posTarget, AgentStates.Enter));//初始化agent编号、位置、目标、状态
            }

            ////重置一次最终目标
            //Simulate.Vector2 e1 = new Simulate.Vector2(20, -60);
            //Simulate.Vector2 e2 = new Simulate.Vector2(-240, -96);
            //Simulate.Vector2 e3 = new Simulate.Vector2(-128, 320);
            //Simulate.Vector2 e = new Simulate.Vector2();
            //for (int i = 0; i < numsBoys; i++)
            //{
            //    float d1 = Simulate.MathHelper.abs(_agents[i].positionNow - e1);
            //    float d2 = Simulate.MathHelper.abs(_agents[i].positionNow - e2);
            //    float d3 = Simulate.MathHelper.abs(_agents[i].positionNow - e3);
            //    if (d1 <= d2 && d1 <= d3) e = e1;
            //    else if (d2 <= d1 && d2 <= d3) e = e2;
            //    else if (d3 <= d1 && d3 <= d2) e = e3;
            //    //NavPoint navP = nav.navMeshQuery.FindRandomPointAroundCircle(nav.navMeshQuery.FindNearestPoly(new Vector3(e.x_, 0, e.y_), new Vector3(10, 10, 10)), 5);
            //    Vector3 navP = nav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(nav.navMeshQuery.FindNearestPoly(new Vector3(e.x_, 0, e.y_), new Vector3(10, 10, 10)).Polygon.Id));
            //    _agents[i].positionTarget = new Simulate.Vector2(navP.X, navP.Z);
            //}


            //重置一次最终目标
            for (int i = 0; i < numsBoys; i++)
            {
                int min = 0;
                float minVector2 = Simulate.MathHelper.abs( _agents[i].positionNow - nav.level._out[0]);
                float delta = Simulate.MathHelper.abs( _agents[i].positionNow-nav.level._out[0]);
                for (int n=0;n<nav.level._out.Count;n++)
                {
                    if(delta > Simulate.MathHelper.abs(nav.level._out[n] - _agents[i].positionNow))
                    {
                        min = n;
                        delta = Simulate.MathHelper.abs(nav.level._out[n] - _agents[i].positionNow);
                    }
                }
                //NavPoint navP = nav.navMeshQuery.FindRandomPointAroundCircle(nav.navMeshQuery.FindNearestPoly(new Vector3(e.x_, 0, e.y_), new Vector3(10, 10, 10)), 5);
                Vector3 navP = nav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(nav.navMeshQuery.FindNearestPoly(new Vector3(nav.level._out[min].x_, 0, nav.level._out[min].y_), new Vector3(20, 20, 20)).Polygon.Id));
                _agents[i].positionTarget = new Simulate.Vector2(navP.X, navP.Z);
                //_agents[i].positionTarget = new Simulate.Vector2(nav.level._out[min].x_, nav.level._out[min].y_);
            }



            //第一次赋初始位置和目标位置时计算一次
            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].navPoints = nav.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
                _agents[i].navPoints.Add(_agents[i].positionTarget);
            }

            ////输出agent信息文件
            //FileHelper.SetOutPath("agents.txt");
            //FileHelper.Write(_agents.Count.ToString(), true);
            //for (int i = 0; i < _agents.Count; i++)
            //{
            //    if (_agents[i].navPoints.Count > 100)
            //    {
            //        for (int j = 0; j < _agents[i].navPoints.Count; j++)
            //        {
            //            if (_agents[i].navPoints[j].x_ == 0 && _agents[i].navPoints[j].y_ == 0)
            //            {
            //                _agents[i].navPoints.RemoveAt(j--);
            //            }
            //        }
            //    }
            //    //FileHelper.Write(_agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " " + _agents[i].navPoints.Count + " ");
            //    FileHelper.Write(Simulator.Instance.getAgentPosition(i).x_ + " " + Simulator.Instance.getAgentPosition(i).y_ + " " + _agents[i].navPoints.Count + " ");
            //    for (int j = 0; j < _agents[i].navPoints.Count; j++) FileHelper.Write(_agents[i].navPoints[j].x_.ToString("0.00") + " " + _agents[i].navPoints[j].y_.ToString("0.00") + " ");
            //    FileHelper.NewLine();
            //}


        }


        /// <summary>
        /// 根据agent当前位置与目的地计算设置最佳速度矢量，每次更新位置需要调用
        /// </summary>
        static void SetPreferredVelocities()
        {
            /*
                * Set the preferred velocity to be a vector of unit magnitude
                * (speed) in the direction of the goal.
                */
            for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
            {
                RVO.Vector2 goalVector;

                //给agent的属性赋值，方便计算path
                RVO.Vector2 pos = Simulator.Instance.getAgentPosition(i);
                _agents[i].positionNow = new Simulate.Vector2(pos.x_, pos.y_);

                if (_agents[i].navPoints.Count > 0) goalVector = new RVO.Vector2(_agents[i].navPoints[0].x_, _agents[i].navPoints[0].y_) - Simulator.Instance.getAgentPosition(i);
                else goalVector = new RVO.Vector2(0, 0);
                if (RVOMath.absSq(goalVector) > 1.0f)
                {
                    goalVector = RVOMath.normalize(goalVector);
                }

                //乘以一个系数让其接近人行走速度
                goalVector *= 1.6f;//人快走的速度

                Simulator.Instance.setAgentPrefVelocity(i, goalVector);

                /* Perturb a little to avoid deadlocks due to perfect symmetry. */
                float angle = (float)Simulate.MathHelper.random.NextDouble() * 2.0f * (float)Math.PI;
                float dist = (float)Simulate.MathHelper.random.NextDouble() * 0.0001f;

                Simulator.Instance.setAgentPrefVelocity(i, Simulator.Instance.getAgentPrefVelocity(i) +
                    dist * new RVO.Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
            }
        }


        /// <summary>
        /// 用来计算agent下一步位置并在场景中更新显示
        /// </summary>
        static int maxNavSteps = 0;//仿真多少步后重新计算导航点
        static long steptimebefore = 0;
        public static void UpdatePosition()
        {
           
            SetPreferredVelocities();
            Simulator.Instance.doStep();

#if outfile1
            //写入一帧帧头  song
            FileHelper.NewFrame(_agents.Count.ToString());
            long timebefore = sw.ElapsedMilliseconds;
            //写入一帧所有数据
            for (int i = 0; i < _agents.Count; i++)
            {
                if (_agents[i].state != AgentStates.Hide)
                {
                    RVO.Vector2 pos = Simulator.Instance.getAgentPosition(i);//
                    _agents[i].positionNow = new Simulate.Vector2(pos.x_, pos.y_);
                    if (Double.IsNaN(pos.x())) Console.WriteLine("不是数！" + "nov:" + _agents[i].nov + "; agent[i]:" + i);
                    else
                    {
                        //位置输出到文件
                        //FileHelper.Write("/" + _agents[i].nov + " " + _agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " 0");//最后的0是预留的
                        //FileHelper.Write("/" + i + " " + (Simulator.Instance.getAgentPosition(i).x_ * 100).ToString("f1") + " " + (Simulator.Instance.getAgentPosition(i).y_ * 100).ToString("f1") + " 0");//最后的0是预留的

                        FileHelper.Write("/" + i + " " + (Simulator.Instance.getAgentPosition(i).x_).ToString("f1") + "," + (Simulator.Instance.getAgentPosition(i).y_).ToString("f1") + " 0");//最后的0是预留的
                        //Console.WriteLine("/" + _agents[i].nov + " " + Simulator.Instance.getAgentPosition(i).x_.ToString("f1") + " " + Simulator.Instance.getAgentPosition(i).y_.ToString("f1") + " 0");
                    }
                }
            }
            Console.WriteLine("储存一帧总时间： " + (sw.ElapsedMilliseconds - timebefore).ToString());
            FileHelper.NewLine();//换行

#elif outfile2

            if (maxNavSteps == 20)
            {
                //输出agent信息文件1
                FileHelper.SetOutPath("agents.txt");
                for (int i = 0; i < _agents.Count; i++)
                {
                    if (_agents[i].navPoints.Count > 100)
                    {
                        for (int j = 0; j < _agents[i].navPoints.Count; j++)
                        {
                            if (_agents[i].navPoints[j].x_ == 0 && _agents[i].navPoints[j].y_ == 0)
                            {
                                _agents[i].navPoints.RemoveAt(j--);
                            }
                        }
                    }
                    //FileHelper.Write(_agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " " + _agents[i].navPoints.Count + " ");
                    FileHelper.Write(Simulator.Instance.getAgentPosition(i).x_ + " " + Simulator.Instance.getAgentPosition(i).y_ + " " + _agents[i].navPoints.Count + " ");
                    for (int j = 0; j < _agents[i].navPoints.Count; j++) FileHelper.Write(_agents[i].navPoints[j].x_.ToString("0.00") + " " + _agents[i].navPoints[j].y_.ToString("0.00") + " ");
                    FileHelper.NewLine();
                }
            }

            ////输出agent信息文件2
            //FileHelper.SetOutPath("agents.txt");
            //for (int i = 0; i < _agents.Count; i++)
            //{
            //    //FileHelper.Write(_agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " " + _agents[i].navPoints.Count + " ");
            //    FileHelper.Write(Simulator.Instance.getAgentPosition(i).x_ + " " + Simulator.Instance.getAgentPosition(i).y_ + " ");
            //}


            ////输出agent信息文件
            //for (int i = 0; i < Program._agents.Count; i++)
            //{
            //    //FileHelper.Write(_agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " " + _agents[i].navPoints.Count + " ");
            //    FileHelper.Write(Program._agents[i].positionNow.x_ + " " + Program._agents[i].positionNow.y_ + " ");
            //}
            //FileHelper.NewLine();
#endif


            if (FileHelper.writeflame > 1000) Exit();

            //如果到达导航点，就到下一个目标
            for (int i = 0; i < _agents.Count; i++)
            {
                if (_agents[i].navPoints.Count > 0 && Simulate.MathHelper.abs(_agents[i].positionNow - _agents[i].navPoints[0]) < 6f)
                {
                    _agents[i].navPoints.RemoveAt(0);
                }
            }

            //暂时不进行重新规划
            if (maxNavSteps++ > 3000)
            {
                maxNavSteps = 0;
                Thread thread = new Thread(ReplanNav);
                thread.Start();
            }
            

            CheckDelete();


            //统计
            stepCounts++;
            if(stepCounts%10==0)
            {
                Console.WriteLine("总人数： " + _agents.Count);
                Console.WriteLine("10步耗时： " + ((float)(sw.ElapsedMilliseconds-steptimebefore)) / 1000);
                steptimebefore = sw.ElapsedMilliseconds;
            }
            if (_agents.Count < 100)
            {
                Console.WriteLine("总步数： " + stepCounts);
                Console.WriteLine("总耗时： " + sw.ElapsedMilliseconds / 1000); 
                sw.Stop();
            }

            

        }
        public static void CheckDelete()
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                for (int n = 0; n < nav.level._out.Count; n++)
                {
                    try
                    {
                        if (Simulate.MathHelper.abs(nav.level._out[n] - _agents[i].positionNow) < 30)
                        {
                            Simulator.Instance.delAgentAt(i);
                            _agents.RemoveAt(i);
                        }
                    }
                    catch
                    {

                    }
                }
                //if (_agents[i].navPoints.Count < 7)
                //{
                //    Simulator.Instance.delAgentAt(i);
                //    _agents.RemoveAt(i);
                //    //Console.WriteLine("del " + i);
                //}
            }
        }

        public static void Repeat(this Action action, int interval)
        {
            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (interval != 0) Thread.Sleep(interval);
                    action();
                }
            })).Start();
        }

        static void Exit()
        {
            FileHelper.EndOutput(0.25f, _agents.Count);
            Environment.Exit(0);
        }

        private static void Intervals()
        {
            Repeat(UpdatePosition, 0);//每步仿真

            //Repeat(CheckDelete, 1000);//每步仿真
            //Decisions.DecideFromAgent(_agents);

            

            //Console.WriteLine("make decisions");
        }

        //重新规划
        public static void ReplanNav()
        {
            long replaytimebefore = sw.ElapsedMilliseconds;
            for (int i = 0; i < _agents.Count; i++)
            {
                try
                {
                    _agents[i].navPoints = nav.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
                    _agents[i].navPoints.Add(_agents[i].positionTarget);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("人数："+_agents.Count + "  "+i);
                    Console.WriteLine("多线程对_agents.count访问出错-replan");
                    //throw e;
                }
                
            }
            Console.WriteLine("新线程重新规划耗时： "+ (sw.ElapsedMilliseconds-replaytimebefore)/1000);
        }
    }




}
