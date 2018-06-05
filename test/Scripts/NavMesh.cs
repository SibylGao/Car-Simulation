// * 文件名:NavMesh.cs
// * 描述:从unity导出的mesh数据中提取障边缘线(碍物线)
// * 创建人:宋锡源
// * 创建日期：20180409
// * ************************************************/
using System;
using System.Collections.Generic;
using System.IO;
///************************************************
namespace Simulate
{
    public class Line
    {
        public Vector2 point1;
        public Vector2 point2;
        public bool valid;//发生重合则变无效，相当于去掉重复边

        public Line(Vector2 p1,Vector2 p2)
        {
            point1 = p1;
            point2 = p2;
            valid = true;
        }

        public override string ToString()
        {
            return point1.ToString()+"------"+point2.ToString();
        }

        //重载等于运算符
        public static bool operator ==(Line Line1, Line Line2)
        {
            return ((Line1.point1 == Line2.point1 && Line1.point2 == Line2.point2) || (Line1.point1 == Line2.point2 && Line1.point2 == Line2.point1));
        }

        public static bool operator !=(Line Line1, Line Line2)
        {
            return !((Line1.point1 == Line2.point1 && Line1.point2 == Line2.point2) || (Line1.point1 == Line2.point2 && Line1.point2 == Line2.point1));
        }
    }

    public class NavMesh
    {
        private List<String> _meshStr;
        public List<Vector2> _points;
        public List<Vector2> _navPoints;
        public List<float> _navPointName;
        public List<Line> _lines;
        public List<Vector2> _navPointLine;

        string meshPath;//路径名
        

        public void InitMesh(string pathname="mesh.obj")
        {
            _meshStr = new List<string>();
            _points = new List<Vector2>();
            _navPoints = new List<Vector2>();
            _lines = new List<Line>();


            _navPointName= new List<float>();
            _navPointLine = new List<Vector2>();

            meshPath = pathname;
            ReadFileInformation(meshPath,ref _meshStr);
            for (int i = 0; i < _meshStr.Count; i++)
            {
                string[] lineStr = _meshStr[i].Split(' ');
                if(lineStr[0]=="v")
                {
                    _points.Add(new Vector2(float.Parse(lineStr[1]), float.Parse(lineStr[3])));
                }
                else if(lineStr[0] == "f")
                {
                    LinesFromTriangle(_points[int.Parse(lineStr[1])-1], _points[int.Parse(lineStr[2]) - 1], _points[int.Parse(lineStr[3]) - 1]);
                }
                else if(lineStr[0] == "np")
                {
                    _navPoints.Add(new Vector2(float.Parse(lineStr[2]),float.Parse(lineStr[3])));
                    _navPointName.Add(int.Parse(lineStr[1]));
                }
                else if (lineStr[0] == "lnp")//自定义导航点连接线
                {
                    _navPointLine.Add(new Vector2(int.Parse(lineStr[1]),int.Parse(lineStr[2])));
                }
            }

            float[,] w = new float[_navPointName.Count, _navPointName.Count];
            for(int i=0;i< _navPointLine.Count;i++)
            {
                int x = _navPointName.IndexOf(_navPointLine[i].x_);
                int y = _navPointName.IndexOf(_navPointLine[i].y_);
                w[x,y] = (int)MathHelper.abs(_navPoints[x]-_navPoints[y]);
                w[y,x] = (int)MathHelper.abs(_navPoints[x] - _navPoints[y]);
            }

            for (int i = 0; i < w.GetLength(0); i++)
            {
                for (int j = 0; j < w.GetLength(1); j++)
                {
                    Console.Write(w[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        private bool ReadFileInformation(String path, ref List<String> list)
        {
            try
            {
                StreamReader m_FileReader;
                String line;
                m_FileReader = new StreamReader(path);
                while ((line = m_FileReader.ReadLine()) != null)
                {
                    list.Add(line);
                }
                m_FileReader.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }


        public void LinesFromTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            _lines.Add(new Line(p1, p2));
            _lines.Add(new Line(p2, p3));
            _lines.Add(new Line(p3, p1));
        }

        public void Get()
        {
            for (int i = 0; i < _lines.Count; i++)
            {
                //if(_lines[i].valid == true)
                {
                    for (int ii = i + 1; ii < _lines.Count; ii++)
                    {
                        if (_lines[i] == _lines[ii])// && _lines[ii].valid == true)
                        {
                            _lines[i].valid = false;
                            _lines[ii].valid = false;
                        }
                    }
                }
            }

            for(int i = 0; i < _lines.Count; i++)
            {
                if(_lines[i].valid==true)
                {
                    Console.WriteLine(i.ToString());
                    Console.WriteLine(_lines[i].ToString());
                }
            }
        }

    }
}
