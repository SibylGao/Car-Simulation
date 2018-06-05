using System.IO;
using System.Data;
using System;
using System.Xml;
using System.Collections.Generic;


public static class SettingHelper
{
    static string xmlPath= "set.xml";
    static XmlDocument doc = new XmlDocument();//创建一个XML文档对象;

    public static void CreatXml()
    {
        if(!File.Exists(xmlPath)) File.Create(xmlPath);

        XmlDocument newDoc = new XmlDocument();//创建一个XML文档对象;
        {
            XmlElement settings = newDoc.CreateElement("Settings");
            {
                XmlElement people = newDoc.CreateElement("People");
                {
                    XmlElement boys = newDoc.CreateElement("Boys");
                    boys.SetAttribute("num", "4");
                    XmlText textBoy = newDoc.CreateTextNode("测试");
                    boys.AppendChild(textBoy);
                    people.AppendChild(boys);

                    XmlElement girls = newDoc.CreateElement("Girls");
                    girls.SetAttribute("num", "16");
                    people.AppendChild(girls);
                }
                settings.AppendChild(people);
            }
            newDoc.AppendChild(settings);
        }
        newDoc.Save(xmlPath);
         
    }

    public static void InitRead(string xmlName)
    {
        xmlPath = xmlName;
        
    }

    //public static void InitWrite(string xmlName = "set.xml")
    //{
    //    xmlPath = xmlName;
    //    if (!File.Exists(xmlPath))
    //    {
    //        doc.RemoveAll();
    //        //创建根节点
    //        XmlElement root = doc.CreateElement("Settings");
    //        doc.AppendChild(root);
    //        doc.Save(xmlPath);
    //    }
    //}
    

    public static string ReadAttribute(string elementname,string nodename,string attributename)
    {
        XmlDocument docRead = new XmlDocument();
        if (File.Exists(xmlPath))
        {
            docRead.Load(xmlPath);
            //获取根节点
            XmlElement settings = docRead.DocumentElement;
            XmlNodeList elements = settings.GetElementsByTagName(elementname);
            XmlNodeList node = ((XmlElement)elements[0]).GetElementsByTagName(nodename);
            return ((XmlElement)node[0]).GetAttribute(attributename);
            //XmlNodeList timeNodes = settings.GetElementsByTagName(element);
            //foreach (XmlNode stepNode in timeNodes)
            //{
            //    if (stepNode.Name == node) return ((XmlElement)stepNode).GetAttribute(attributename);
            //}
        }
        return null;
    }

    //public static string ReadAttribute(string node, string attribute)
    //{
    //    XmlDocument docRead = new XmlDocument();
    //    if (File.Exists(xmlPath))
    //    {
    //        docRead.Load(xmlPath);
    //        //获取根节点
    //        XmlElement root = docRead.DocumentElement;
    //        XmlNodeList timeNodes = root.GetElementsByTagName(node);
    //        foreach (XmlNode stepNode in timeNodes)
    //        {
    //            string s = ((XmlElement)stepNode).GetAttribute("s");
    //            string num = ((XmlElement)stepNode).GetAttribute("num");
    //            string name = ((XmlElement)stepNode).GetAttribute("name");
    //            string hour = ((XmlElement)stepNode).GetAttribute("hour");
    //            string minute = ((XmlElement)stepNode).GetAttribute("minute");
    //        }
    //        return null;
    //    }
    //    return null;
    //}

}
