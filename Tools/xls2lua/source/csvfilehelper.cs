using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
public class CSVFileHelper
{
    /// <summary>
    /// 将DataTable中数据写入到CSV文件中
    /// </summary>
    /// <param name="dt">提供保存数据的DataTable</param>
    /// <param name="fileName">CSV的文件路径</param>
    public static void SaveCSV(DataTable dt, string fullPath, Encoding encodtype)
    {
        FileInfo fi = new FileInfo(fullPath);
        if (!fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
        StreamWriter sw = new StreamWriter(fs, encodtype);//System.Text.Encoding.UTF8
        string data = "";
        //写出列名称
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            data += dt.Columns[i].ColumnName.ToString();
            if (i < dt.Columns.Count - 1)
            {
                data += ",";
            }
        }
        sw.WriteLine(data);
        //写出各行数据
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            data = "";
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                string str = dt.Rows[i][j].ToString();
                str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                if (str.Contains(",") || str.Contains("''") 
                    || str.Contains("\r") || str.Contains("\n")) //含逗号 冒号 换行符的需要放到引号中
                {
                    str = string.Format("\"{0}\"", str);
                }

                data += str;
                if (j < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
        }
        sw.Close();
        fs.Close();
    }

    public static void SaveCSV2(List<string[]> arydt, string fullPath, Encoding encodtype)
    {
        FileInfo fi = new FileInfo(fullPath);
        if (!fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
        StreamWriter sw = new StreamWriter(fs, encodtype);//System.Text.Encoding.UTF8
        int linec = arydt.Count;
        int columc = arydt[1].Length;
        string strsave = "";
        for (int i = 0; i < linec; ++i)
        {
            string[] line = arydt[i];
            string strline = "";
            if (line != null && line.Length > 0)
            {
                strline = line[0];
                for (int l = 1; l < line.Length; ++l)
                {
                    strline += ",";
                    strline += line[l];
                }
                strline += "\r\n";
                strsave += strline;
            }
        }
        sw.Write(strsave);
        sw.Close();
        fs.Close();
    }
    //是否奇数个\"
    static bool ifOddQuota(string line)
    {
        int qcont = 0;
        int strlen = line.Length;
        for (int i = 0; i < strlen; i++)
        {
            if (line[i] == '\"')
                qcont++;
        }
        return qcont % 2 == 1;
    }

    public static List<string[]> OpenCSV2(string filePath, out Encoding encodetype)
    {
        encodetype = Encoding.ASCII;
        if (!File.Exists(filePath))
            return null;
        Encoding encoding = GetFileEncodeType(filePath);//Common.GetType(filePath); //Encoding.ASCII;//
        encodetype = encoding;
        DataTable dt = new DataTable();
        FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

        //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
        StreamReader sr = new StreamReader(fs, encoding);
        string alltext = sr.ReadToEnd();

        string[] lineArray = alltext.Split('\r');
        List<string[]> Arrays = new List<string[]>();
        int linen = 0;
        for (int i = 0; i < lineArray.Length; i++)
        {
            if (string.IsNullOrEmpty(lineArray[i]) ||
                string.IsNullOrWhiteSpace(lineArray[i]))
                continue;
            string[] aryline = lineArray[i].Split(',');
            if (aryline.Length > 0)
                aryline[0] = aryline[0].Replace("\n", "");
            Arrays.Add(aryline);
            linen++;
        }

        sr.Close();
        fs.Close();
        return Arrays;
    }
    /// <summary>
    /// 将CSV文件的数据读取到DataTable中
    /// </summary>
    /// <param name="fileName">CSV文件路径</param>
    /// <returns>返回读取了CSV数据的DataTable</returns>
    public static DataTable OpenCSV(string filePath, out Encoding encodetype)
    {
        encodetype = Encoding.ASCII;
        if (!File.Exists(filePath))
            return null;
        Encoding encoding = GetFileEncodeType(filePath);//Common.GetType(filePath); //Encoding.ASCII;//
        encodetype = encoding;
        DataTable dt = new DataTable();
        FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        
        //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
        StreamReader sr = new StreamReader(fs, encoding);
        
        //string fileContent = sr.ReadToEnd();
        //encoding = sr.CurrentEncoding;
        //记录每次读取的一行记录
        string strLine = "";
        //记录每行记录中的各字段内容
        string[] aryLine = null;
        string[] tableHead = null;
        //标示列数
        int columnCount = 0;
        //标示是否是读取的第一行
        bool IsFirst = true;
        //逐行读取CSV中的数据
        int readedc = 0;
        string dataline = "";
        while ((strLine = sr.ReadLine()) != null)
        {
            //strLine = Common.ConvertStringUTF8(strLine, encoding);
            //strLine = Common.ConvertStringUTF8(strLine);
            if (string.IsNullOrEmpty(dataline))
                dataline = strLine;
            else
            {
                //dataline += "\r\n";
                dataline += strLine;
            }
            if (ifOddQuota(dataline))
                continue;
            readedc++;
            if (IsFirst == true)
            {
                tableHead = dataline.Split(',');
                IsFirst = false;
                columnCount = tableHead.Length;
                //创建列
                for (int i = 0; i < columnCount; i++)
                {
                    string coname = tableHead[i];
                    if (dt.Columns.Contains(coname))
                        coname += i;
                    DataColumn dc = new DataColumn(coname);
                    
                    dt.Columns.Add(dc);
                }
            }
            else
            {
                aryLine = dataline.Split(',');
                DataRow dr = dt.NewRow();
                for (int j = 0; j < columnCount; j++)
                {
                    if(aryLine.Length > j)
                        dr[j] = aryLine[j];
                }
                dt.Rows.Add(dr);
            }
            dataline = "";
        }
        if (aryLine != null && aryLine.Length > 0)
        {
            //dt.DefaultView.Sort = tableHead[0] + " " + "asc";
        }
        
        sr.Close();
        fs.Close();
        return dt;
    }

    public static System.Text.Encoding GetFileEncodeType(string filen)
    {
        try
        {
            System.IO.FileStream fs = new FileStream(filen, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            if (null != fs)
            {
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                byte[] bbuf = br.ReadBytes(2);
                br.Close();
                fs.Close();
                if (bbuf[0] >= 0xef)
                {
                    if (bbuf[0] == 0xef && bbuf[1] == 0xbb)
                        return System.Text.Encoding.UTF8;
                    else if (bbuf[0] == 0xfe && bbuf[1] == 0xff)
                        return System.Text.Encoding.BigEndianUnicode;
                    else if (bbuf[0] == 0xef && bbuf[1] == 0xfe)
                        return System.Text.Encoding.Unicode;
                    else
                        return System.Text.Encoding.Default;
                }
            }

        }
        catch (System.Exception e)
        {
            System.Windows.Forms.MessageBox.Show(e.Message);
        }
        return System.Text.Encoding.Default;
    }
}