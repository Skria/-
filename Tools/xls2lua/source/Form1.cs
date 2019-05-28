using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Spire.Xls;

namespace xls2lua
{
    public partial class Form1 : Form
    {
        NRegister m_stNkm;
        static string s_strXlsPath = string.Empty;
        static string s_strLuaPath = string.Empty;
        static string s_strCheckPath = string.Empty;
        static string s_strStatusText = string.Empty;

        System.Windows.Forms.Timer m_Timer;

        static void SetStatus(string str)
        {
            lock (s_strStatusText)
            {
                s_strStatusText = str;
            }
        }
        static string GetStatus()
        {
            string str = string.Empty;

            lock (s_strStatusText)
            {
                str = s_strStatusText;
            }
            return str;
        }

        public Form1()
        {
            InitializeComponent();
            m_stNkm = new NRegister("nkm_tool");
            try
            {
                m_inputPath.Text = m_stNkm.GetRegistData("xls_path");
                m_inputOutPath.Text = m_stNkm.GetRegistData("lua_outpath");
                m_inputCheckOutPath.Text = m_stNkm.GetRegistData("check_outpath");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void m_btnBrowse1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    m_inputPath.Text = fbd.SelectedPath;
                }
                catch (System.Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
            
        }

        private void m_btnBrowse2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    m_inputOutPath.Text = fbd.SelectedPath;
                }
                catch (System.Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }

        static void WorkThreadFunc()
        {
            do
            {
                string strXlsPath = s_strXlsPath;
                string strTemPath = strXlsPath;
                int nLastIndex = strTemPath.LastIndexOf('\\');
                if (nLastIndex != strTemPath.Length - 1 && strTemPath.LastIndexOf('/') != strTemPath.Length - 1)
                    strTemPath += "/csvtemp/";
                if (!Directory.Exists(strTemPath))
                    Directory.CreateDirectory(strTemPath);

                ClearDir(strTemPath);
                SetStatus("Convert xls 2 csv...");
                //1.将xls转成csv
				string[] xlsxFile = Directory.GetFiles(strXlsPath, "*.xlsx");
				string[] xlsmFile = Directory.GetFiles(strXlsPath, "*.xlsm");
				List<string> aryXlsFiles = new List<string>();
				for (int i = 0; i < xlsxFile.Length; i++)
				{
					aryXlsFiles.Add(xlsxFile[i]);
				}
				for (int i = 0; i < xlsmFile.Length; i++)
				{
					aryXlsFiles.Add(xlsmFile[i]);
				}
				if (aryXlsFiles.Count < 1)
					break;
                foreach (var v in aryXlsFiles)
                {
                    ConvertXls2Csv(v, strTemPath);
                }
                //清空lua文件夹
                ClearDir(s_strLuaPath);
                ClearDir(s_strCheckPath);
                //2.将csv 转成 lua
                SetStatus("Convert csv 2 lua...");
                string[] aryCsvFiles = Directory.GetFiles(strTemPath, "*.csv");
                foreach (var v in aryCsvFiles)
                {
                    GenCsv2LuaNew(v, s_strLuaPath, s_strCheckPath);
                }
                SetStatus("Completed");
            }
            while (false);
        }

        private void m_btnCovert_Click(object sender, EventArgs e)
        {
            string strXlsPath = m_inputPath.Text;
            string strLuaPath = m_inputOutPath.Text;
            string strCheckPath = m_inputCheckOutPath.Text;
            if (strXlsPath.Length < 1)
            {
                MessageBox.Show("选择xls路径");
                return;
            }
            if (strLuaPath.Length < 1)
            {
                MessageBox.Show("选择lua路径");
                return;
            }
            if (!Directory.Exists(strXlsPath))
            {
                MessageBox.Show("xls路径不存在");
                return;
            }
            s_strXlsPath = strXlsPath;
            s_strLuaPath = strLuaPath;
            s_strCheckPath = strCheckPath;
            if (s_strLuaPath.LastIndexOf('/') != s_strLuaPath.Length - 1 &&
                s_strLuaPath.LastIndexOf('\\') != s_strLuaPath.Length - 1)
            {
                s_strLuaPath += "/";
            }
            if (s_strCheckPath.LastIndexOf('/') != s_strCheckPath.Length - 1 &&
                s_strCheckPath.LastIndexOf('\\') != s_strCheckPath.Length - 1)
            {
                s_strCheckPath += "/";
            }

            m_txStatus.Text = "Start...";
            Thread t = new Thread(WorkThreadFunc);
            t.Start();

            //等待进程结束
            if (null == m_Timer)
            {
                m_Timer = new System.Windows.Forms.Timer();
                m_Timer.Tick += new EventHandler(timer_Tick);
                m_Timer.Interval = 20;
            }
            m_Timer.Stop();
            m_Timer.Start();

        }

        void timer_Tick(object sender, EventArgs e)
        {
            m_txStatus.Text = GetStatus();
        }

        static void ClearDir(string dir)
        {
            if (!Directory.Exists(dir))
                return;
            try
            {
                string[] aryFiles = Directory.GetFiles(dir);
                foreach (var v in aryFiles)
                {
                    if (v.EndsWith(".csv") || v.EndsWith(".lua"))
                        File.Delete(v);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("ClearDir:{0} err:{1}", dir, e.Message));
            }
        }

        static void ConvertXls2Csv(string strXlsPath, string strCsvPath)
        {
            Workbook wb = new Workbook();
            try
            {
                wb.LoadFromFile(strXlsPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            if (null == wb.Worksheets)
                return;
            System.Text.Encoding encd = System.Text.Encoding.UTF8;
            SetStatus(string.Format("convert {0} to csv", strXlsPath));
            Thread.Sleep(0);
            try
            {
                for (byte i = 0; i < wb.Worksheets.Count; ++i)
                    wb.Worksheets[i].SaveToFile(strCsvPath + "/" + wb.Worksheets[i].Name + ".csv", ",", encd);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            wb.Dispose();
        }

        class COneCsvCell
        {
            string _strVarName; //变量名
            string _strVarType; //变量类型
            string _strCsType;
            string _strTip;    //
            int _nColIndex; //所在列
            int _nRepeatCount; // 重复次数 >1表示数组
            bool _bIsNumber = false;
            string _strCombKey = string.Empty;
            List<COneCsvCell> _lstRepeat;

            static string CheckType(string sType)
            {
                if (sType.CompareTo("\"int32\"") == 0)
                    return "int";
                return sType.Replace("\"","");
            }
            public COneCsvCell(string strVName, string strVType, int nIndex, string tip, string strRepeat)
            {
                _strVarName = strVName.Replace("\"","");
                _strVarType = strVType;
                _strCsType = CheckType(strVType);
                _nColIndex = nIndex;
                _bIsNumber = strVType.IndexOf("string")==-1;
                _strCombKey = strVName.Replace("\"", "");
                _strCombKey += _bIsNumber ? "_n" : "_s";
                _strTip = tip==null?string.Empty:tip;
                //去掉tip换行
                _strTip = _strTip.Replace("\n", " ");
                strRepeat = strRepeat.Trim();
                bool bRepeated = strRepeat.CompareTo(sAryValidateTitle0[1]) == 0;
                if (bRepeated)
                {
                    int.TryParse(strVType, out _nRepeatCount);
                    if(_nRepeatCount > 1)
                        _lstRepeat = new List<COneCsvCell>();
                }
            }
            
            public bool IsNumber { get { return _bIsNumber; } }
            public string CombKey { 
                get 
                {
                    if (_nRepeatCount > 1)
                        return string.Format("{0}{1}", _lstRepeat[0].CombKey, _nRepeatCount);
                    return _strCombKey; 
                } 
            }

            public string VarName { get { return _strVarName; } }
            public string CsType
            {
                get
                {
                    return _strCsType;
                }
            }
            public int CellIndex { get { return _nColIndex; } }
            public int RepeateCount { get { return _nRepeatCount; } }
            public List<COneCsvCell> RepeatedList { get { return _lstRepeat; } }
            public string Tip 
            { 
                get 
                {
                    if (_nRepeatCount > 1 && _lstRepeat.Count > 0)
                        return _lstRepeat[0].Tip;
                    return _strTip; 
                } 
            }
        };

        static string[] sAryValidateTitle0 = new string[] { "\"required\"", "\"repeated\"", "\"optional\"" };
        static bool IsValidateCell(string strTitleName)
        {
            if (string.IsNullOrEmpty(strTitleName))
                return false;
            strTitleName=strTitleName.Trim();
            int nLen = sAryValidateTitle0.Length;
            for (int i = 0; i < nLen; ++i)
            {
                if (sAryValidateTitle0[i].CompareTo(strTitleName) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        static void GenCsv2Lua(string strCsvPath, string strLuaPath)
        {
            SetStatus(string.Format("convert {0} to lua", strCsvPath));
            if (!Directory.Exists(strLuaPath))
                Directory.CreateDirectory(strLuaPath);
            Thread.Sleep(0);
            System.Text.Encoding encd;
            List<string[]> csvContent = CSVFileHelper.OpenCSV2(strCsvPath, out encd);
            //第三行 变量名 第二行 类型
            if (csvContent.Count < 4)
                return;
            List<COneCsvCell> lstTitle = new List<COneCsvCell>();
            //解析类型&变量
            string[] aryTitle0 = csvContent[0];
            string[] aryType = csvContent[1];
            string[] aryVName = csvContent[2];
            string[] aryTips = csvContent[3];

            int nColN = Math.Min(aryTitle0.Length, aryVName.Length);
            int nRepeatCount = 0;
            List<COneCsvCell> lstRepeat = null;
            for (int i = 0; i < nColN; ++i)
            {
                if (IsValidateCell(aryTitle0[i]) && !string.IsNullOrEmpty(aryType[i]))
                {
                    COneCsvCell cell = new COneCsvCell(aryVName[i], aryType[i], i, aryTips[i], aryTitle0[i]);
                    if (nRepeatCount > 0 && lstRepeat != null)
                    {
                        lstRepeat.Add(cell);
                        --nRepeatCount;
                    }
                    else
                    {
                        lstTitle.Add(cell);
                        nRepeatCount = cell.RepeateCount;
                        if (nRepeatCount > 1)
                            lstRepeat = cell.RepeatedList;
                    }
                }
            }
            if (lstTitle.Count < 1)
                return;

            string strLuaFileTitle = Path.GetFileNameWithoutExtension(strCsvPath);
            string strLuaFullPath = s_strLuaPath + strLuaFileTitle +".lua";

            string strLuaWrite = string.Empty;
            try
            {
                FileStream fs = new FileStream(strLuaFullPath, FileMode.Create);
                var utf8WithoutBom = new System.Text.UTF8Encoding(false);
                StreamWriter sw = new StreamWriter(fs, utf8WithoutBom);
                sw.WriteLine("--this source code was auto-generated by xls2lua.exe, do not modify it\n");
                //1 索引Enum
                strLuaWrite = "\tEVar = \n\t{\n";
                int setn = 0;
                int nTitleCount = lstTitle.Count;
                foreach(var v in lstTitle)
                {
                    ++setn;
                    if (setn == 1)
                        continue;
                    strLuaWrite += "\t\t";
                    strLuaWrite += v.CombKey;
                    strLuaWrite += "=";
                    strLuaWrite += (setn-1).ToString();
                    strLuaWrite += ",";
                    //加上注释
                    strLuaWrite += "--" + v.Tip;
                    strLuaWrite += "\n";
                }
                strLuaWrite += "\t},\n";
                

                
                //定义类
                sw.Write("\n");
                sw.WriteLine("local "+strLuaFileTitle + "_DATA=");
                sw.WriteLine("{");

                //写入key索引
                sw.Write(strLuaWrite);
                string strUIID=string.Empty, strValue=string.Empty;
                //2写入数据表
                for (int i = 4; i < csvContent.Count; ++i)
                {
                    strLuaWrite = string.Empty;
                    string[] data = csvContent[i];
                    setn = 0;
                    //单元类
                    strUIID = data[lstTitle[0].CellIndex];
                    if (string.IsNullOrEmpty(strUIID))
                        continue;
                    strLuaWrite = "\t[";
                    strLuaWrite += data[lstTitle[0].CellIndex];
                    strLuaWrite += "]={";
                    foreach (var v in lstTitle)
                    {
                        ++setn;
                        if (setn == 1)
                            continue;
                        if (v.RepeateCount > 1)//数组
                        {
                            strValue = "{";
                            List<COneCsvCell> lst = v.RepeatedList;
                            if (null != lst)
                            {
                                string strValue1 = string.Empty;
                                foreach (var vv in lst)
                                {
                                    strValue1 = data[vv.CellIndex];
                                    if (string.IsNullOrEmpty(strValue1))
                                    {
                                        if (vv.IsNumber)
                                            strValue1 = "0";
                                        else
                                            strValue1 = "\'\'";
                                        continue;//数组的的值如果为0或者 '',直接压缩掉
                                    }
                                    strValue += strValue1;
                                    strValue += ",";
                                }
                            }
                            strValue += "}";
                        }
                        else
                        {
                            strValue = data[v.CellIndex];
                            if (string.IsNullOrEmpty(strValue))
                            {
                                if (v.IsNumber)
                                    strValue = "0";
                                else
                                    strValue = "\'\'";
                            }
                        }
                        strLuaWrite += strValue;
                        strLuaWrite += ",";
                    }
                    strLuaWrite += "},\n";
                    sw.Write(strLuaWrite);
                }
                sw.WriteLine("}");
                sw.WriteLine("return "+strLuaFileTitle + "_DATA");
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {

            }
        }

        static int GetStructLength(int curIndex,int indexCount, string[] aryTitle0, string[] aryType)
        {
            int length = 0;
            int fieldCount = int.Parse(aryType[curIndex]);
            int fieldIndex = curIndex + 1;
            for (int i = 0;i< fieldCount;i++)
            {
                int fieldLength = 1;
                bool isFieldRepeated = (aryTitle0[fieldIndex] == "\"repeated\"");
                bool isFieldStruct = (aryTitle0[fieldIndex] == "\"optional_struct\"");
                if (isFieldStruct || isFieldRepeated)
                {
                    fieldLength += GetStructLength(fieldIndex, indexCount, aryTitle0, aryType);
                }
                length += fieldLength;
                fieldIndex += fieldLength;
            }
            return length;
        }

        static void GenCsv2LuaNew(string strCsvPath, string strLuaPath, string strCheckPath)
        {
            try
            {
                Thread.Sleep(0);
                System.Text.Encoding encd;
                List<string[]> csvContent = CSVFileHelper.OpenCSV2(strCsvPath, out encd);
                //第三行 变量名 第二行 类型
                if (csvContent.Count < 4)
                    return;
                //解析类型&变量
                string[] aryTitle0 = csvContent[0];
                string[] aryType = csvContent[1];
                string[] aryVName = csvContent[2];
                string[] aryTips = csvContent[3];
                int lstTitleCount = 0;
                int startCol = -1;
                for (int i = 0; i < aryTitle0.Length; i++)
                {
                    if (IsValidateCell(aryTitle0[i]) && !string.IsNullOrEmpty(aryType[i]))
                    {
                        if (startCol < 0) startCol = i;
                        lstTitleCount++;
                    }
                }
                if (lstTitleCount < 1)
                    return;
                
                string strLuaFileTitle = Path.GetFileNameWithoutExtension(strCsvPath);
                string strLuaFullPath = s_strLuaPath + strLuaFileTitle + ".lua";
                string strCheckFullPath = string.Empty;
                if (strCheckPath != string.Empty)
                {
                    if (!Directory.Exists(strCheckPath))
                        Directory.CreateDirectory(strCheckPath);
                    strCheckFullPath = strCheckPath + strLuaFileTitle + ".lua";
                }
                SetStatus(string.Format("convert {0} to lua", strCsvPath));
                if (!Directory.Exists(strLuaPath))
                    Directory.CreateDirectory(strLuaPath);
                FileStream fs = new FileStream(strLuaFullPath, FileMode.Create);
                var utf8WithoutBom = new System.Text.UTF8Encoding(false);
                StreamWriter sw = new StreamWriter(fs, utf8WithoutBom);
                string strLuaWrite = string.Empty;
                sw.WriteLine("--this source code was auto-generated by xls2lua.exe, do not modify it\n");
                sw.WriteLine("local " + strLuaFileTitle + "_DATA=");
                sw.WriteLine("{");
                //1 索引Enum
                strLuaWrite = "\tEVar = \n\t{\n";
                int setn = 1;
				
                for (int i = 4; i < csvContent.Count; i++)
                {
                    string lineWrite = "";
                    string[] data = csvContent[i];
                    List<int> structEnd = new List<int>();
                    List<int> repeatStructEnd = new List<int>();
                    List<int> readRepeatCountLabel = new List<int>();
                    List<int> usedRepeatCount = new List<int>();
                    List<int> readedRepeatCount = new List<int>();
                    if (!string.IsNullOrEmpty(data[startCol]))
                    {
                        lineWrite = "\t["+ ToValue(aryTitle0[startCol], data[startCol]) + "]={";
                        for (int j = startCol+1; j < data.Length && j< aryTitle0.Length; j++)
                        {
                            bool isEmpty = string.IsNullOrEmpty(aryTitle0[j]);
                            if (isEmpty) continue;
                            bool isRepeated = (aryTitle0[j] == "\"repeated\"");
                            bool isStruct = (aryTitle0[j] == "\"optional_struct\"");
                            bool ignore = false;
                            int length = 1;
                            if (isStruct || isRepeated)
                            {
                                length = GetStructLength(j, data.Length, aryTitle0, aryType);
                            }
                            int repeatIndex = readRepeatCountLabel.Count - 1;
                            if (repeatIndex > -1)
                            {
                                if(readedRepeatCount[repeatIndex] >= usedRepeatCount[repeatIndex])
                                {
                                    ignore = true;
                                }
                            }

                            if (i == 4 && structEnd.Count == 0 && repeatStructEnd.Count == 0)
                            {
                                strLuaWrite += "\t\t";
                                strLuaWrite += aryVName[j].Trim('"') + "_" + GetType(aryType[j], aryTitle0[j]);
                                strLuaWrite += "=";
                                strLuaWrite += (setn).ToString();
                                strLuaWrite += ",";
                                //加上注释
                                strLuaWrite += "--" + aryTips[j].Replace('\n', ';');
                                strLuaWrite += "\n";
                                setn++;
                            }

                            if (!ignore)
                            {
                                if (isRepeated)
                                {
                                    lineWrite += "{";
                                    repeatStructEnd.Add(j + length);
                                    readRepeatCountLabel.Add(int.Parse(aryType[j]));
                                    readedRepeatCount.Add(0);
                                    usedRepeatCount.Add(int.Parse(data[j]));
                                }

                                if (isStruct)
                                {
                                    lineWrite += "{";
                                    structEnd.Add(j + length);
                                }

                                if (!isRepeated && !isStruct)
                                {
                                    lineWrite += ToValue(aryType[j], data[j]);
                                    if (repeatIndex > -1)
                                    {
                                        if (repeatStructEnd.Count > 0 && structEnd.Count > 0)
                                        {
                                            if (j == structEnd[structEnd.Count - 1] && repeatStructEnd[repeatStructEnd.Count - 1] <= structEnd[structEnd.Count - 1])
                                            {
                                                readedRepeatCount[repeatIndex]++;
                                            }
                                        }
                                        else
                                        {
                                            readedRepeatCount[repeatIndex]++;
                                        }
                                    }
                                    
                                }
                            }

                            for (int k = structEnd.Count - 1; k >= 0; k--)
                            {
                                if (structEnd[k] == j)
                                {
                                    string lastStr = lineWrite.Substring(lineWrite.Length - 1, 1);
                                    if (lastStr == "," || lastStr == "{") lineWrite = lineWrite.Substring(0, lineWrite.Length - 1);
                                    if (lastStr != "{")
                                    {
                                        lineWrite += "},";
                                    }
                                    if (lineWrite.Substring(lineWrite.Length - 3, 3) == "{},")
                                    {
                                        lineWrite = lineWrite.Substring(0, lineWrite.Length - 3);
                                    }
                                    structEnd.RemoveAt(k);
                                    if (repeatIndex > -1)
                                    {
                                        readedRepeatCount[repeatIndex]++;
                                    }
                                }
                            }

                            for (int k = repeatStructEnd.Count - 1; k >= 0; k--)
                            {
                                if (repeatStructEnd[k] == j)
                                {
                                    string lastStr = lineWrite.Substring(lineWrite.Length - 1, 1);
                                    if (lastStr == "," || lastStr == "{") lineWrite = lineWrite.Substring(0, lineWrite.Length - 1);
                                    if (lastStr != "{")
                                    {
                                        lineWrite += "},";
                                    }
                                    repeatStructEnd.RemoveAt(k);
                                    readRepeatCountLabel.RemoveAt(repeatIndex);
                                    readedRepeatCount.RemoveAt(repeatIndex);
                                    usedRepeatCount.RemoveAt(repeatIndex);
                                    repeatIndex = readRepeatCountLabel.Count - 1;
                                }
                            }
                            string lastStr_ = lineWrite.Substring(lineWrite.Length - 1, 1);
                            if (!isRepeated && !isStruct && lastStr_ != ",") lineWrite += ",";

                        }
                        if (lineWrite.Substring(lineWrite.Length - 1,1)==",")lineWrite = lineWrite.Substring(0, lineWrite.Length - 1);
                        lineWrite += "},\n";
                        if (i == 4)
                        {
                            sw.Write(strLuaWrite);
                            sw.Write("\t},\n");
                        }
                        sw.Write(lineWrite);
                    }
                    
                }
                sw.WriteLine("}");
                sw.WriteLine("return " + strLuaFileTitle + "_DATA");
                sw.Flush();
                sw.Close();
                if (strCheckFullPath != string.Empty)
                {
                    File.Copy(strLuaFullPath, strCheckFullPath, true);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {

            }
        }

        static string GetType(string type_,string title0)
        {
            string t = "";
            string rep = "1";
            string type = type_;
            if (type_.IndexOf("\"") == -1)
            {
                rep = type_;
                type = title0;
            }
            int r = int.Parse(rep);
            if (type == "\"string\"")
            {
                t = "s"+((r>1)? rep : "");
            }
            else if(type == "\"int32\"" || type == "\"int64\"")
            {
                t = "n" + ((r > 1) ? rep : "");
            }
            else if (type == "\"float32\"")
            {
                t = "f" + ((r > 1) ? rep : "");
            }
            else if (type == "\"double\"")
            {
                t = "d" + ((r > 1) ? rep : "");
            }
            else if(type == "\"repeated\"")
            {
                t = "repeated" + ((r > 1) ? rep : "");
            }
            else if (type == "\"optional_struct\"")
            {
                t = "struct" + ((r > 1) ? rep : "");
            }
            return t;
        }

        static string ToValue(string type,string value)
        {
            string v = value;
            string data = "";
            if(type == "\"string\"")
            {
                if (string.IsNullOrEmpty(v)) v = "\"\"";
				if (!v.StartsWith("\"")) v = "\"" + v;
				if (!v.EndsWith("\"")) v += "\"";
                data = v;
            }
            else
            {
                if (string.IsNullOrEmpty(v)) v = "0";
                data = v;
            }
            return data;
        }
        

        //转c#静态类
        static void GenCsv2Csharp(string strCsvPath, string strLuaPath)
        {
            SetStatus(string.Format("convert {0} to cs", strCsvPath));
            if (!Directory.Exists(strLuaPath))
                Directory.CreateDirectory(strLuaPath);
            Thread.Sleep(0);
            System.Text.Encoding encd;
            List<string[]> csvContent = CSVFileHelper.OpenCSV2(strCsvPath, out encd);
            //第三行 变量名 第二行 类型
            if (csvContent.Count < 4)
                return;
            List<COneCsvCell> lstTitle = new List<COneCsvCell>();
            //解析类型&变量
            string[] aryTitle0 = csvContent[0];
            string[] aryType = csvContent[1];
            string[] aryVName = csvContent[2];
            string[] aryTips = csvContent[3];

            int nColN = Math.Min(aryTitle0.Length, aryVName.Length);
            int nRepeatCount = 0;
            List<COneCsvCell> lstRepeat = null;
            for (int i = 0; i < nColN; ++i)
            {
                if (IsValidateCell(aryTitle0[i]) && !string.IsNullOrEmpty(aryType[i]))
                {
                    COneCsvCell cell = new COneCsvCell(aryVName[i], aryType[i], i, aryTips[i], aryTitle0[i]);
                    if (nRepeatCount > 0 && lstRepeat != null)
                    {
                        lstRepeat.Add(cell);
                        --nRepeatCount;
                    }
                    else
                    {
                        lstTitle.Add(cell);
                        nRepeatCount = cell.RepeateCount;
                        if (nRepeatCount > 1)
                            lstRepeat = cell.RepeatedList;
                    }
                }
            }
            if (lstTitle.Count < 1)
                return;

            string strLuaFileTitle = Path.GetFileNameWithoutExtension(strCsvPath);
            string strLuaFullPath = s_strLuaPath + strLuaFileTitle + ".cs";

            string strLuaWrite = string.Empty;
            try
            {
                FileStream fs = new FileStream(strLuaFullPath, FileMode.Create);
                var utf8WithoutBom = new System.Text.UTF8Encoding(false);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine("//this source code was auto-generated by xls2lua.exe, do not modify it\n");
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic;");

                //1 索引Enum
                strLuaWrite = "\tpublic class DataItem \n\t{\n";
                int setn = 0;
                int nTitleCount = lstTitle.Count;
                foreach (var v in lstTitle)
                {
                    ++setn;
                    if (setn == 1)
                        continue;
                    strLuaWrite += "\t\t";
                    strLuaWrite += v.CsType;
                    strLuaWrite += " ";
                    strLuaWrite += v.VarName;
                    strLuaWrite += ";";
                    //加上注释
                    strLuaWrite += "//" + v.Tip;
                    strLuaWrite += "\n";
                }

                //构造函数
                strLuaWrite += "\t\tpublic DataItem(";
                setn = 0;
                int nTotal = lstTitle.Count;
                foreach (var v in lstTitle)
                {
                    ++setn;
                    if (setn == 1)
                        continue;
                    strLuaWrite += v.CsType;
                    strLuaWrite += " ";
                    strLuaWrite += v.VarName;
                    if(setn != nTotal)
                        strLuaWrite += ",";
                }
                strLuaWrite += ")\n";
                strLuaWrite += "\t\t{\n";
                setn = 0;
                foreach (var v in lstTitle)
                {
                    ++setn;
                    if (setn == 1)
                        continue;
                    strLuaWrite += "\t\t\tthis.";
                    strLuaWrite += v.VarName;
                    strLuaWrite += "=";
                    strLuaWrite += v.VarName;
                    strLuaWrite += ";\n";
                }
                strLuaWrite += "\t\t}\n";
                strLuaWrite += "\t}\n";

                


                //定义类
                sw.Write("\n");
                sw.WriteLine("public class " + strLuaFileTitle + "_DATA");
                sw.WriteLine("{");

                
                
                //写入类定义
                sw.Write(strLuaWrite);

                strLuaWrite = string.Empty;
                strLuaWrite = "\tstatic Dictionary<int, DataItem> Datas = new Dictionary<int,DataItem>();\n";
                sw.WriteLine(strLuaWrite);

                //静态初始化函数
                sw.WriteLine("\tstatic void InitData()");
                sw.WriteLine("\t{");
                sw.WriteLine("\t\tDataItem data=null;");
                string strUIID = string.Empty, strValue = string.Empty;
                string strCsWrite = string.Empty;
                //2写入数据表
                for (int i = 4; i < csvContent.Count; ++i)
                {
                    strLuaWrite = string.Empty;
                    string[] data = csvContent[i];
                    setn = 0;
                    //单元类
                    strUIID = data[lstTitle[0].CellIndex];
                    if (string.IsNullOrEmpty(strUIID))
                        continue;
                    strCsWrite = string.Empty;
                    strCsWrite = "\t\tdata = new DataItem(";
                    setn = 0;
                    foreach (var v in lstTitle)
                    {
                        ++setn;
                        if (setn == 1)
                            continue;
                        strValue = data[v.CellIndex];
                        if (string.IsNullOrEmpty(strValue))
                        {
                            if (v.IsNumber)
                                strValue = "0";
                            else
                                strValue = "string.Empty";
                        }
                        strCsWrite += strValue;
                        if(setn < nTotal)
                            strCsWrite += ",";
                    }
                    strCsWrite += ");\n";
                    strCsWrite += "\t\tDatas.Add(";
                    strCsWrite += strUIID;
                    strCsWrite += ",data);\n";
                    
                    sw.Write(strCsWrite);
                }
                sw.WriteLine("\t}");//end Init

                sw.WriteLine("}");
                //sw.WriteLine("return " + strLuaFileTitle + "_DATA");
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {

            }
        }
        static void WorkThread2CSFunc()
        {
            do
            {
                string strXlsPath = s_strXlsPath;
                string strTemPath = strXlsPath;
                int nLastIndex = strTemPath.LastIndexOf('\\');
                if (nLastIndex != strTemPath.Length - 1 && strTemPath.LastIndexOf('/') != strTemPath.Length - 1)
                    strTemPath += "/csvtemp/";
                if (!Directory.Exists(strTemPath))
                    Directory.CreateDirectory(strTemPath);

                ClearDir(strTemPath);
                SetStatus("Convert xls 2 csv...");
                //1.将xls转成csv
                string[] aryXlsFiles = Directory.GetFiles(strXlsPath, "*.xlsx");
                if (aryXlsFiles.Length < 1)
                    break;
                foreach (var v in aryXlsFiles)
                {
                    ConvertXls2Csv(v, strTemPath);
                }
                //清空lua文件夹
                //ClearDir(s_strLuaPath);
                //2.将csv 转成 lua
                SetStatus("Convert csv 2 lua...");
                string[] aryCsvFiles = Directory.GetFiles(strTemPath, "*.csv");
                foreach (var v in aryCsvFiles)
                {
                    GenCsv2Csharp(v, s_strLuaPath);
                }
                SetStatus("Completed");
            }
            while (false);
        }
        /*private void m_btnToCs_Click(object sender, EventArgs e)
        {
            string strXlsPath = m_inputPath.Text;
            string strLuaPath = m_inputOutPath.Text;
            string strCheckPath = m_inputCheckOutPath.Text;
            if (strXlsPath.Length < 1)
            {
                MessageBox.Show("选择xls路径");
                return;
            }
            if (strLuaPath.Length < 1)
            {
                MessageBox.Show("选择lua路径");
                return;
            }
            if (!Directory.Exists(strXlsPath))
            {
                MessageBox.Show("xls路径不存在");
                return;
            }
            s_strXlsPath = strXlsPath;
            s_strLuaPath = strLuaPath;
            if (s_strLuaPath.LastIndexOf('/') != s_strLuaPath.Length - 1 &&
                s_strLuaPath.LastIndexOf('\\') != s_strLuaPath.Length - 1)
            {
                s_strLuaPath += "/";
            }

            m_txStatus.Text = "Start...";
            Thread t = new Thread(WorkThread2CSFunc);
            t.Start();

            //等待进程结束
            if (null == m_Timer)
            {
                m_Timer = new System.Windows.Forms.Timer();
                m_Timer.Tick += new EventHandler(timer_Tick);
                m_Timer.Interval = 20;
            }
            m_Timer.Stop();
            m_Timer.Start();
        }
        */
        private void m_inputOutPath_TextChanged(object sender, EventArgs e)
        {
            m_stNkm.SetRegistData("lua_outpath", m_inputOutPath.Text);
        }

        private void m_inputPath_TextChanged(object sender, EventArgs e)
        {
            m_stNkm.SetRegistData("xls_path", m_inputPath.Text);
        }

        private void m_btnBrowse3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    m_inputCheckOutPath.Text = fbd.SelectedPath;
                }
                catch (System.Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }

        private void m_inputCheckOutPath_TextChanged(object sender, EventArgs e)
        {
            m_stNkm.SetRegistData("check_outpath", m_inputCheckOutPath.Text);
        }
    }
}
