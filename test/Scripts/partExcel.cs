using System.IO;
using Excel;
using System.Data;
using UnityEngine;
using System;
using UnityEngine.UI;
using OfficeOpenXml;
using Common;
using System.Xml;

public partial  class SimControlle : MonoBehaviour
{
    string xmlSetPath="set.xml";
    public void addSetXml()
    {
        XmlDocument doc = new XmlDocument();//创建一个XML文档对象
        if (!File.Exists(xmlSetPath))
        {
            doc.RemoveAll();
            //创建根节点
            XmlElement root0 = doc.CreateElement("root");
            doc.AppendChild(root0);
            doc.Save(xmlSetPath);
        }

        doc.Load(xmlSetPath);
        XmlElement root = doc.DocumentElement;
        root.RemoveAll();
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            XmlElement Session = doc.CreateElement("Session");
            Transform objTransform = Content.transform.GetChild(i);
            Text text = objTransform.transform.GetChild(0).GetComponent<Text>();
            Session.SetAttribute("s", text.text);
            InputField IF = objTransform.GetChild(1).GetComponent<InputField>();
            Session.SetAttribute("num", IF.text);
            InputField IFname = objTransform.GetChild(2).GetComponent<InputField>();
            Session.SetAttribute("name", IFname.text);
            Dropdown DDH = objTransform.transform.GetChild(3).GetComponent<Dropdown>();
            Session.SetAttribute("hour",DDH.value.ToString());
            Dropdown DDM = objTransform.GetChild(4).GetComponent<Dropdown>();
            Session.SetAttribute("minute",DDM.value.ToString());
            root.AppendChild(Session);
        }
        
        doc.Save(xmlSetPath);
    }

    void readSetXml()
    {
        XmlDocument docRead = new XmlDocument();
        if (File.Exists(xmlSetPath))
        {
            {
                docRead.Load(xmlSetPath);
                //获取根节点
                XmlElement root = docRead.DocumentElement;
                XmlNodeList timeNodes = root.GetElementsByTagName("Session");
                foreach (XmlNode stepNode in timeNodes)
                {
                    string s = ((XmlElement)stepNode).GetAttribute("s");
                    string num = ((XmlElement)stepNode).GetAttribute("num");
                    string name= ((XmlElement)stepNode).GetAttribute("name");
                    string hour = ((XmlElement)stepNode).GetAttribute("hour");
                    string minute = ((XmlElement)stepNode).GetAttribute("minute");
                    addList(s, num,name, int.Parse(hour),int.Parse(minute));
                }
            }
        }
    }


    //void excelSet()
    //{
    //    ////FileStream stream = File.Open(Application.dataPath + "/设置.xlsx", FileMode.Open, FileAccess.Read);
    //    FileStream stream = File.Open("s.xls", FileMode.Open, FileAccess.Read);

    //    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);//CreateOpenXmlReader(stream);

    //    DataSet result = excelReader.AsDataSet();

    //    int columns = result.Tables[0].Columns.Count;
    //    int rows = result.Tables[0].Rows.Count;

    //    for (int i = 1; i < rows; i++)
    //    {
    //        int s = int.Parse(result.Tables[0].Rows[i][0].ToString());
    //        int n = int.Parse(result.Tables[0].Rows[i][1].ToString());
    //        int h = int.Parse(result.Tables[0].Rows[i][2].ToString());
    //        int m = int.Parse(result.Tables[0].Rows[i][3].ToString());

    //        //DateTime t = System.DateTime.Parse(result.Tables[0].Rows[i][2].ToString());
    //        //Debug.Log(s +" "+n+"   "+t.TimeOfDay.Hours+":"+ t.TimeOfDay.Minutes);
    //        //_agentSet.Add(new AgentsSet(s, n, t.Hour, t.Minute));
    //        addList(s.ToString(), n.ToString(),"G1", h, m);
    //    }

    //}

    public void addList(string s,string n,string name,int h,int m)
    {
        GameObject obj = Instantiate(MyList, Content.transform) as GameObject;
        obj.name = s.ToString();
        Text tex = obj.transform.GetChild(0).GetComponent<Text>();
        tex.text = s.ToString();
        InputField IF = obj.transform.GetChild(1).GetComponent<InputField>();
        IF.text = n.ToString();
        InputField IFName = obj.transform.GetChild(2).GetComponent<InputField>();
        IFName.text = name.ToString();

        Dropdown DDH = obj.transform.GetChild(3).GetComponent<Dropdown>();
        setH(DDH);
        DDH.value = h;// t.TimeOfDay.Hours;
        Dropdown DDM = obj.transform.GetChild(4).GetComponent<Dropdown>();
        setM(DDM);
        DDM.value = m;// t.TimeOfDay.Minutes;
    }

    public void myAdd()
    {
        //    addList(Content.transform.childCount.ToString(),"1", int.Parse(Content.transform.GetChild(Content.transform.childCount-1).GetChild(1).GetComponent<InputField>().text), Content.transform.GetChild(Content.transform.childCount-1).GetChild(1).GetComponent<Dropdown>().value);
        //
        addList(Content.transform.childCount.ToString(), "1","G1231", 1, 1);

    }

    void setH(Dropdown DDH)
    {
        for (int i = 0; i <= 24; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = i.ToString();
            DDH.options.Add(data);
        }
   
    }
    void setM(Dropdown DDM)
    {
        for (int i = 0; i <= 60; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = i.ToString();
            DDM.options.Add(data);
        }
    }

    //public void ExcelWrite()
    //{
    //    string path ="s.xls";
    //    FileStream fs = new FileStream(path, FileMode.Create);

    //    using (var package = new ExcelPackage(fs))
    //    {
    //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
    //        worksheet.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//水平居中  
    //        worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//水平居中  
    //        worksheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//水平居中 
    //        worksheet.Column(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//水平居中 
    //        worksheet.Cells[1, 1].Value = "场次";
    //        worksheet.Cells[1, 2].Value = "人数";
    //        worksheet.Cells[1, 3].Value = "发车小时";
    //        worksheet.Cells[1, 4].Value = "发车分钟";
    //        for (int i = 0; i < Content.transform.childCount; i++)
    //        {
    //            Transform objTransform = Content.transform.GetChild(i);
    //            Text text = objTransform.transform.GetChild(0).GetComponent<Text>();
    //            worksheet.Cells[i + 2, 1].Value = text.text;
    //            InputField IF = objTransform.GetChild(1).GetComponent<InputField>();
    //            worksheet.Cells[i + 2, 2].Value = IF.text;

    //            Dropdown DDH = objTransform.transform.GetChild(2).GetComponent<Dropdown>();
    //            worksheet.Cells[i + 2, 3].Value = DDH.value;
    //            Dropdown DDM = objTransform.GetChild(3).GetComponent<Dropdown>();
    //            worksheet.Cells[i + 2, 4].Value = DDM.value;
    //        }
    //        package.Save();
    //    }
    //    fs.Close();
    //}




    public void OpenProject()
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
        pth.filter = "txt (*.txt)";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath;  // default path  
        pth.title = "打开项目";
        pth.defExt = "txt";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (OpenFileDialog.GetOpenFileName(pth))
        {
            string filepath = pth.file;//选择的文件路径;  
            Debug.Log(filepath);
        }
    }
    public void SaveProject()
    {
        SaveFileDlg pth = new SaveFileDlg();
        pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
        pth.filter = "txt (*.txt)";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath;  // default path  
        pth.title = "保存项目";
        pth.defExt = "txt";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (SaveFileDialog.GetSaveFileName(pth))
        {
            string filepath = pth.file;//选择的文件路径;  
            Debug.Log(filepath);
        }
    }
}
