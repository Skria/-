using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LuaFramework
{
    public enum UILayer
    {
        Normal = 1,
        Over,
        Top,
        FixTop,
    }
    public class UIManager : Manager
    {
        Dictionary<string, PanelData> panelDataDic = new Dictionary<string, PanelData>();

        Dictionary<UILayer, List<PanelData>> openSortDic = new Dictionary<UILayer, List<PanelData>>(); //每一层UI的打开顺序

        List<PanelData> openPanelList = new List<PanelData>();  //全部UI打开的顺序

        public int normalLayerStartCount = 100;
        public int topLayerStartCount = 3500;
        public int fixLayerStartCount = 10000;
        public int layerDis = 50;

        public override void Init()
        {
            openSortDic[UILayer.Normal] = new List<PanelData>();
            openSortDic[UILayer.Top] = new List<PanelData>();
            openSortDic[UILayer.FixTop] = new List<PanelData>();
        }

        class PanelData
        {
            public string uiName = string.Empty;
            public UILayer uiLayer = UILayer.Normal;
            public GameObject gameObject = null;
            public string path = string.Empty;
            public Panel panel = null;
            public bool isInit = false;
            public bool isOpen = false;
            public PanelData overPanel = null;

            public PanelData(string sceneName, string uiName, int uiLayer)
            {
                this.uiName = uiName;
                this.uiLayer = (UILayer)uiLayer;
                path = "" + uiName;
                gameObject = Instantiate(Resources.Load<GameObject>(this.path));
                gameObject.SetActive(false);
                panel = gameObject.GetComponent<Panel>();
                panel.canvas = gameObject.GetComponent<Canvas>();
                panel.canvas.worldCamera = Camera.main;
                panel.canvas.planeDistance = 1;
            }
        }

        public void RegisteredPanel(string sceneName, string uiName, int uiLayer)
        {
            if (panelDataDic.ContainsKey(uiName))
            {
                return;
            }
            PanelData tempData = new PanelData(sceneName, uiName, uiLayer);
            panelDataDic.Add(uiName, tempData);
        }

        public void OpenPanel(string name, LuaTable intent)
        {
            if (!panelDataDic.ContainsKey(name))
            {
                return;
            }
            PanelData tempData = panelDataDic[name];
            if(tempData.uiLayer != UILayer.Over)
            {
                openPanelList.Add(tempData);
                openSortDic[tempData.uiLayer].Add(tempData);
            }
            else
            {
                int count = openPanelList.Count;
                if(count == 0)
                {
                    Debug.LogError("未打开任何一个UI而开启了挂载UI");
                    return;
                }
                openPanelList[count - 1].overPanel = tempData;
            }
            if (tempData.gameObject == null)
            {
                tempData.gameObject = Instantiate(Resources.Load<GameObject>(tempData.path));
            }
            SortCanvas();
            if(tempData.isInit == false)
            {
                tempData.isInit = true;
                tempData.panel.InitView();
            }

            if(tempData.isOpen == false)
            {
                tempData.isOpen = true;
                tempData.panel.OpenView(intent);
            }
        }

        public void BackPanel()
        {
            PanelData tempData = openPanelList[openPanelList.Count - 1];
            tempData.gameObject.SetActive(false);
            openPanelList.RemoveAt(openPanelList.Count - 1);
            openSortDic[tempData.uiLayer].RemoveAt(openSortDic[tempData.uiLayer].Count - 1);
            tempData.isOpen = false;
            if(tempData.overPanel != null)
            {
                tempData.overPanel.isOpen = false;
                tempData.overPanel.gameObject.SetActive(false);
                tempData.overPanel = null;
            }
            SortCanvas();
            tempData = openPanelList[openPanelList.Count - 1];
            tempData.panel.ReopenView();
        }

        public void SortCanvas()
        {
            for(int i = 0; i < openSortDic[UILayer.Normal].Count; i++)
            {
                PanelData tempData = openSortDic[UILayer.Normal][i];
                tempData.panel.canvas.sortingOrder = normalLayerStartCount + i * layerDis;
                if (i == openSortDic[UILayer.Normal].Count - 1)
                {
                    tempData.panel.gameObject.SetActive(true);
                }
                else
                {
                    tempData.panel.gameObject.SetActive(false);
                    continue;
                }
                if(tempData.overPanel != null)
                {
                    tempData.overPanel.panel.canvas.sortingOrder = normalLayerStartCount + i * layerDis + 1;
                    tempData.overPanel.gameObject.SetActive(true);
                }
            }

            for (int i = 0; i < openSortDic[UILayer.Top].Count; i++)
            {
                PanelData tempData = openSortDic[UILayer.Top][i];
                tempData.panel.canvas.sortingOrder = topLayerStartCount + i * layerDis;
                tempData.panel.gameObject.SetActive(true);
                if (tempData.overPanel != null)
                {
                    tempData.overPanel.panel.canvas.sortingOrder = normalLayerStartCount + i * layerDis + 1;
                    tempData.overPanel.gameObject.SetActive(true);
                }
            }

            for (int i = 0; i < openSortDic[UILayer.FixTop].Count; i++)
            {
                PanelData tempData = openSortDic[UILayer.FixTop][i];
                openSortDic[UILayer.FixTop][i].panel.canvas.sortingOrder = fixLayerStartCount + i * layerDis;
                openSortDic[UILayer.FixTop][i].panel.gameObject.SetActive(true);
                if (tempData.overPanel != null)
                {
                    tempData.overPanel.panel.canvas.sortingOrder = normalLayerStartCount + i * layerDis + 1;
                    tempData.overPanel.gameObject.SetActive(true);
                }
            }
        }
    }
}
