using System;
using System.Collections.Generic;
using System.Drawing;

namespace Simulate
{
    //只适用于
    public class AgentClass
    {
        public int nov;//编号
        //public GameObject gObject;//绑定在场景中的物体
        public Vector2 positionNow;//当前位置
        public Vector2 positionTarget;//最终目标位置
        public AgentStates state;//跟结构体states绑定,代表agent状态
        public TimeSpan timeShow;//出现时间
        public bool isMan;//是否为男性
        public int age;//年龄状态
        public int area;//所在区域编号
        public IList<Vector2> navPoints = new List<Vector2>();//导航点数组
        public Color color;

        public AgentClass(int Nov, Vector2 PostionNow, AgentStates State, TimeSpan TimeShow, bool IsMan, int Age)
        {
            nov = Nov;
            //gObject = GObject;
            positionNow = PostionNow;
            state = State;
            timeShow = TimeShow;
            isMan = IsMan;
            age = Age;

            navPoints.Add(new Vector2(0, 0));
        }

        /// <summary>
        /// agent构造函数
        /// </summary>
        /// <param name="Nov">编号</param>
        /// <param name="GObject">绑定的场景中的物体</param>
        /// <param name="PositionNow">当前位置，初始化时当前位置也是目标</param>
        /// <param name="State">agent所处状态</param>
        public AgentClass(int Nov, Vector2 PositionNow,Vector2 target, AgentStates State)
        {
            nov = Nov;
            //gObject = GObject;
            positionNow = PositionNow;
            state = State;
            age = 2;
            positionTarget =target;

            int R = MathHelper.random.Next(255);
            int G = MathHelper.random.Next(255);
            int B = MathHelper.random.Next(255);
            B = (R + G > 400) ? R + G - 400 : B;//0 : 380 - R - G;
            B = (B > 255) ? 255 : B;
            color = Color.FromArgb(R, G, B);
            //color = Color.Blue;
        }

    }

}
