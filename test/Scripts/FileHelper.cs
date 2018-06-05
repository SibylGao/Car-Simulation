using System;
using System.IO;
using System.Text;

public static class FileHelper
{
    static string outPath;//路径名
    static BufferedStream bs;
    static byte[] _bytes;
    public static int writeflame=0;//帧数

    /// <summary>
    /// 输出初始化
    /// </summary>
    /// <param name="txtpath">未指定文件名则默认当前时间为文件名</param>
    public static void SetOutPath(string outpath="")
    {
        if (outpath == "") outPath = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
        else outPath = outpath;
        if (File.Exists(outPath))
        {
            try
            {
                File.Delete(outPath);
            }
            catch
            {

            }
        }
        bs = new BufferedStream(File.Create(outPath));
    }
    /// <summary>
    /// 构造函数初试化
    /// </summary>
    /// <param name="pathName">默认名称为当前时间,如果文件名重名将会删除掉原来的文件</param>
    //OutputHelper(string pathName = "")
    //{
    //    if(File.Exists(pathName))
    //    {
    //        File.Delete(pathName);
    //    }
    //    if (pathName == "") txtPath = System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString();
    //    bs = new BufferedStream(File.Create(txtPath));
    //}


    public static bool EndOutput(float t,int agentsNum)
    {
        if (outPath == null) return false;

        FileHelper.Write(writeflame*t + " " + t + " " + writeflame + " " + agentsNum);

        bs.Flush();
        bs.Close();
        return true;
    }

    /// <summary>
    /// 向输出文件中写入新数据，newLine设置为true则写后换行
    /// </summary>
    /// <param name="str"></param>
    /// <param name="newLine"></param>
    /// <returns></returns>
    public static bool Write(string str, bool newLine=false)
    {
        if (outPath == null) return false;
        _bytes = new UTF8Encoding().GetBytes(str);
        bs.Write(_bytes, 0, _bytes.Length);
        if (newLine == true)
        {
            _bytes = new UTF8Encoding().GetBytes("\r\n");
            bs.Write(_bytes, 0, _bytes.Length);
        }
        return true;
    }

    /// <summary>
    /// 新的一帧函数，帧头格式自定义
    /// </summary>
    /// <param name="str"></param>
    /// <param name="newLine"></param>
    /// <returns></returns>
    public static bool NewFrame(string str)
    {
        if (outPath == null) return false;
        //_bytes = new UTF8Encoding().GetBytes(writeflame + " " + str);
        _bytes = new UTF8Encoding().GetBytes(str);
        bs.Write(_bytes, 0, _bytes.Length);
        writeflame++;
        return true;
    }
    public static bool NewLine()
    {
        if (outPath == null) return false;
        _bytes = new UTF8Encoding().GetBytes("\r\n");
        bs.Write(_bytes, 0, _bytes.Length);
        return true;
    }
}
 //XmlDocument doc = new XmlDocument();//创建一个XML文档对象
 //   string xmlPath = "输出文件/";
 //   int nameCount = 0;
 //   int xmlCount = 0;
 //   void initXml()
 //   {
 //       //XmlDeclaration dec;//声明XML头部信息
 //       //dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
 //       ////添加进doc对象子节点
 //       //doc.AppendChild(dec);
 //       ////创建根节点
 //       //XmlElement root = doc.CreateElement("root");
 //       //doc.AppendChild(root);

 //       //// doc.Save("my.xml");
 //   }

 //   void addXml()
 //   {
 //       //if (!File.Exists(xmlPath + nameCount.ToString()))
 //       //{
 //       //    doc.RemoveAll();
 //       //    //创建根节点
 //       //    XmlElement root0 = doc.CreateElement("root");
 //       //    doc.AppendChild(root0);
 //       //    doc.Save(xmlPath + nameCount.ToString());
 //       //}
 //       //xmlCount++;
 //       //doc.Load(xmlPath + nameCount.ToString());
 //       //XmlElement root = doc.DocumentElement;
 //       //XmlElement frame = doc.CreateElement("Step");
 //       //frame.SetAttribute("n", xmlCount.ToString());
 //       //frame.SetAttribute("t", timeNow.ToString());
 //       //frame.SetAttribute("total", totalAgents.ToString());
 //       //for (int i = 0; i < _agents.Count; i++)
 //       //{
 //       //    if (_agents[i].state >= 0)
 //       //    {

 //       //        if (_agents[i].lastPos.x().ToString("#0.00") != Simulator.Instance.getAgentPosition(_agents[i].nov).x().ToString("#0.00") || _agents[i].lastPos.y().ToString("#0.00") != Simulator.Instance.getAgentPosition(_agents[i].nov).y().ToString("#0.00"))
 //       //        {
 //       //            _agents[i].lastPos = Simulator.Instance.getAgentPosition(_agents[i].nov);

 //       //            //再创建根节点下的子节点
 //       //            XmlElement agent = doc.CreateElement("a");

 //       //            //设置子节点属性
 //       //            agent.SetAttribute("i", _agents[i].nov.ToString());

 //       //            //子节点下再创建标记
 //       //            XmlElement x = doc.CreateElement("x");
 //       //            XmlText xText = doc.CreateTextNode(Simulator.Instance.getAgentPosition(_agents[i].nov).x().ToString("#0.00"));
 //       //            x.AppendChild(xText);
 //       //            agent.AppendChild(x);

 //       //            //子节点下再创建标记
 //       //            XmlElement y = doc.CreateElement("y");
 //       //            XmlText yText = doc.CreateTextNode(Simulator.Instance.getAgentPosition(_agents[i].nov).y().ToString("#0.00"));
 //       //            y.AppendChild(yText);
 //       //            agent.AppendChild(y);
 //       //            frame.AppendChild(agent);
 //       //        }

 //       //    }
 //       //}
 //       //root.AppendChild(frame);
 //       //doc.Save(xmlPath + nameCount.ToString());
 //       //if (xmlCount == 10)
 //       //{
 //       //    xmlCount = 0;
 //       //    nameCount++;
 //       //}
 //   }

    //void addTxt()
    //{
    //    int agentNum = 0;
    //    for (int i = 0; i < _agents.Count; i++)
    //    {
    //        if (Simulator.Instance.getAgentPosition(_agents[i].nov).x() < 20 && Simulator.Instance.getAgentPosition(_agents[i].nov).x() > -20 && Simulator.Instance.getAgentPosition(_agents[i].nov).y() > -43 && Simulator.Instance.getAgentPosition(_agents[i].nov).y() < 43)
    //        {
    //            agentNum++;
    //        }
    //    }
    //    totalAgents = agentNum;
    //    if (agentNum != 0)
    //    {
    //        myWriteLine(agentNum.ToString(), false);
    //        for (int i = 0; i < _agents.Count; i++)
    //        {
    //            if (_agents[i].state >= 0)
    //            {
    //                //if (Double.IsNaN(Simulator.Instance.getAgentPosition(_agents[i].nov).x()))
    //                //{
    //                //    Simulator.Instance.setAgentPosition(_agents[i].nov, new RVO.Vector2(0, -340));
    //                //}
    //                if (Simulator.Instance.getAgentPosition(_agents[i].nov).x() < 20 && Simulator.Instance.getAgentPosition(_agents[i].nov).x() > -20 && Simulator.Instance.getAgentPosition(_agents[i].nov).y() > -43 && Simulator.Instance.getAgentPosition(_agents[i].nov).y() < 43)
    //                {
    //                    myWriteLine("/" + _agents[i].nov + " " + (Simulator.Instance.getAgentPosition(_agents[i].nov).x() * 100).ToString("#0") + "," + (Simulator.Instance.getAgentPosition(_agents[i].nov).y() * 100).ToString("#0") + " " + 0, false);
    //                }
    //            }
    //        }
    //        _bytes = new UTF8Encoding().GetBytes("\r\n");
    //        bs.Write(_bytes, 0, _bytes.Length);
    //    }
    //}

    //void initXml()
    //{


    //    XmlDeclaration dec;//声明XML头部信息
    //    dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
    //    //添加进doc对象子节点
    //    doc.AppendChild(dec);
    //    //创建根节点
    //    XmlElement root = doc.CreateElement("root");
    //    doc.AppendChild(root);

    //   // doc.Save("my.xml");
    //}

    //void addXml(int frameNov)
    //{
    //    //doc.Load("my.xml");
    //    XmlElement root = doc.DocumentElement;

    //    XmlElement frame = doc.CreateElement("Step");
    //    frame.SetAttribute("n", frameNov.ToString());
    //    frame.SetAttribute("t", timeNow.ToString());
    //    for (int i = 0; i < _agents.Count; i++)
    //    {
    //        if (_agents[i].state >= 0)
    //        {
    //            //再创建根节点下的子节点
    //            XmlElement agent = doc.CreateElement("a");

    //            //设置子节点属性
    //            agent.SetAttribute("i", _agents[i].nov.ToString());

    //            //子节点下再创建标记
    //            XmlElement x = doc.CreateElement("x");
    //            XmlText xText = doc.CreateTextNode(Simulator.Instance.getAgentPosition(_agents[i].nov).x().ToString("#0.00"));
    //            x.AppendChild(xText);
    //            agent.AppendChild(x);

    //            //子节点下再创建标记
    //            XmlElement y = doc.CreateElement("y");
    //            XmlText yText = doc.CreateTextNode(Simulator.Instance.getAgentPosition(_agents[i].nov).y().ToString("#0.00"));
    //            y.AppendChild(yText);
    //            agent.AppendChild(y);
    //            frame.AppendChild(agent);
    //        }
    //    }
    //    root.AppendChild(frame);

    //}
//}























//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Runtime.InteropServices;
//using RVO;
//using System.Xml;
//using System.IO;
//using System.Text;

//public partial class SimControlle : MonoBehaviour
//{
//    XmlDocument doc = new XmlDocument();//创建一个XML文档对象
//    string xmlPath = "输出文件/";
//    int nameCount = 0;
//    int xmlCount = 0;





//    void initXml()
//    {


//        //XmlDeclaration dec;//声明XML头部信息
//        //dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
//        ////添加进doc对象子节点
//        //doc.AppendChild(dec);
//        ////创建根节点
//        //XmlElement root = doc.CreateElement("root");
//        //doc.AppendChild(root);

//        //// doc.Save("my.xml");
//    }

//    void addXml()
//    {
//        if (!File.Exists(xmlPath + nameCount.ToString()))
//        {
//            doc.RemoveAll();
//            //创建根节点
//            XmlElement root0 = doc.CreateElement("root");
//            doc.AppendChild(root0);
//            doc.Save(xmlPath + nameCount.ToString());
//        }
//        xmlCount++;
//        doc.Load(xmlPath + nameCount.ToString());
//        XmlElement root = doc.DocumentElement;
//        XmlElement frame = doc.CreateElement("Step");
//        frame.SetAttribute("n", xmlCount.ToString());
//        frame.SetAttribute("t", timeNow.ToString());
//        frame.SetAttribute("total", totalAgents.ToString());
//        for (int i = 0; i < _agents.Count; i++)
//        {
//            if (_agents[i].state >= 0)
//            {

//                if (_agents[i].lastPos.x().ToString("#0.00") != Simulator.Instance.getAgentPosition(_agents[i].nov).x().ToString("#0.00") || _agents[i].lastPos.y().ToString("#0.00") != Simulator.Instance.getAgentPosition(_agents[i].nov).y().ToString("#0.00"))
//                {
//                    _agents[i].lastPos = Simulator.Instance.getAgentPosition(_agents[i].nov);

//                    //再创建根节点下的子节点
//                    XmlElement agent = doc.CreateElement("a");

//                    //设置子节点属性
//                    agent.SetAttribute("i", _agents[i].nov.ToString());

//                    //子节点下再创建标记
//                    XmlElement x = doc.CreateElement("x");
//                    XmlText xText = doc.CreateTextNode(Simulator.Instance.getAgentPosition(_agents[i].nov).x().ToString("#0.00"));
//                    x.AppendChild(xText);
//                    agent.AppendChild(x);

//                    //子节点下再创建标记
//                    XmlElement y = doc.CreateElement("y");
//                    XmlText yText = doc.CreateTextNode(Simulator.Instance.getAgentPosition(_agents[i].nov).y().ToString("#0.00"));
//                    y.AppendChild(yText);
//                    agent.AppendChild(y);
//                    frame.AppendChild(agent);
//                }

//            }
//        }
//        root.AppendChild(frame);
//        doc.Save(xmlPath + nameCount.ToString());
//        if (xmlCount == 10)
//        {
//            xmlCount = 0;
//            nameCount++;
//        }
//    }




//    public string txtPath;
//    //public string txtPath = "2222.text";
//    int stepCount = 0;
//    BufferedStream bs;

//    byte[] _bytes;
//    void initTxt()
//    {
//        if (!Directory.Exists("输出文件"))
//        {
//            Directory.CreateDirectory("输出文件");
//        }
//        txtPath = "输出文件/" + System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString();
//        bs = new BufferedStream(File.Create(txtPath));

//        for (int i = 0; i < _agents.Count; i++)
//        {
//            _bytes = new UTF8Encoding().GetBytes(_agents[i].agentType + " ");
//            bs.Write(_bytes, 0, _bytes.Length);//sw.Write(_agents[i].type + " ");
//        }

//        _bytes = new UTF8Encoding().GetBytes("\r\n");
//        bs.Write(_bytes, 0, _bytes.Length);
//    }

//    void endTxt()
//    {
//        _bytes = new UTF8Encoding().GetBytes(Simulator.Instance.getGlobalTime() + " " + Simulator.Instance.getTimeStep() * 2 + " " + stepCount + " " + _agents.Count);
//        bs.Write(_bytes, 0, _bytes.Length);
//        bs.Flush();
//        bs.Close();
//    }

//    void myWriteLine(string str, bool newLine)
//    {
//        _bytes = new UTF8Encoding().GetBytes(str);
//        bs.Write(_bytes, 0, _bytes.Length);
//        if (newLine == true)
//        {
//            _bytes = new UTF8Encoding().GetBytes("\r\n");
//            bs.Write(_bytes, 0, _bytes.Length);
//        }

//    }

//    void addTxt()
//    {

//        int agentNum = 0;
//        for (int i = 0; i < _agents.Count; i++)
//        {
//            if (Simulator.Instance.getAgentPosition(_agents[i].nov).x() < 20 && Simulator.Instance.getAgentPosition(_agents[i].nov).x() > -20 && Simulator.Instance.getAgentPosition(_agents[i].nov).y() > -43 && Simulator.Instance.getAgentPosition(_agents[i].nov).y() < 43)
//            {
//                agentNum++;
//            }
//        }
//        totalAgents = agentNum;
//        if (agentNum != 0)
//        {
//            myWriteLine(agentNum.ToString(), false);
//            for (int i = 0; i < _agents.Count; i++)
//            {
//                if (_agents[i].state >= 0)
//                {
//                    //if (Double.IsNaN(Simulator.Instance.getAgentPosition(_agents[i].nov).x()))
//                    //{
//                    //    Simulator.Instance.setAgentPosition(_agents[i].nov, new RVO.Vector2(0, -340));
//                    //}
//                    if (Simulator.Instance.getAgentPosition(_agents[i].nov).x() < 20 && Simulator.Instance.getAgentPosition(_agents[i].nov).x() > -20 && Simulator.Instance.getAgentPosition(_agents[i].nov).y() > -43 && Simulator.Instance.getAgentPosition(_agents[i].nov).y() < 43)
//                    {
//                        myWriteLine("/" + _agents[i].nov + " " + (Simulator.Instance.getAgentPosition(_agents[i].nov).x() * 100).ToString("#0") + "," + (Simulator.Instance.getAgentPosition(_agents[i].nov).y() * 100).ToString("#0") + " " + 0, false);
//                    }
//                }
//            }
//            _bytes = new UTF8Encoding().GetBytes("\r\n");
//            bs.Write(_bytes, 0, _bytes.Length);
//        }
//    }


















//    //void initXml()
//    //{


//    //    XmlDeclaration dec;//声明XML头部信息
//    //    dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
//    //    //添加进doc对象子节点
//    //    doc.AppendChild(dec);
//    //    //创建根节点
//    //    XmlElement root = doc.CreateElement("root");
//    //    doc.AppendChild(root);

//    //   // doc.Save("my.xml");
//    //}

//    //void addXml(int frameNov)
//    //{
//    //    //doc.Load("my.xml");
//    //    XmlElement root = doc.DocumentElement;

//    //    XmlElement frame = doc.CreateElement("Step");
//    //    frame.SetAttribute("n", frameNov.ToString());
//    //    frame.SetAttribute("t", timeNow.ToString());
//    //    for (int i = 0; i < _agents.Count; i++)
//    //    {
//    //        if (_agents[i].state >= 0)
//    //        {
//    //            //再创建根节点下的子节点
//    //            XmlElement agent = doc.CreateElement("a");

//    //            //设置子节点属性
//    //            agent.SetAttribute("i", _agents[i].nov.ToString());

//    //            //子节点下再创建标记
//    //            XmlElement x = doc.CreateElement("x");
//    //            XmlText xText = doc.CreateTextNode(Simulator.Instance.getAgentPosition(_agents[i].nov).x().ToString("#0.00"));
//    //            x.AppendChild(xText);
//    //            agent.AppendChild(x);

//    //            //子节点下再创建标记
//    //            XmlElement y = doc.CreateElement("y");
//    //            XmlText yText = doc.CreateTextNode(Simulator.Instance.getAgentPosition(_agents[i].nov).y().ToString("#0.00"));
//    //            y.AppendChild(yText);
//    //            agent.AppendChild(y);
//    //            frame.AppendChild(agent);
//    //        }
//    //    }
//    //    root.AppendChild(frame);

//    //}
//}





