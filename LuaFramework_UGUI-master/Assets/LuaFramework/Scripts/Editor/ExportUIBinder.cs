using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class ExportUIBinder
{
    [MenuItem("Assets/Project X/Export UI Binder")]
    private static void ExportBinder()
    {
        var selected = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(selected);
        Debug.Log(path);
        if (path.Substring(path.Length - 7, 7) == ".prefab")
        {
            StartExportBinder(path, (GameObject)selected);
        }
    }

    private static void StartExportBinder(string path, GameObject prefab)
    {
        int p = path.LastIndexOf("/");
        int p2 = path.LastIndexOf("/", p - 1);
        string moduleName = path.Substring(p2 + 1, p - (p2 + 1));
        List<GameObjectInfo> all = new List<GameObjectInfo>();
        GameObjectInfo start = new GameObjectInfo("", "", prefab);

        FindGameObject(start, all);
        List<GameObjectInfo> needOutput = new List<GameObjectInfo>();

        Dictionary<string, string> nameMap = new Dictionary<string, string>();

        for (int i = 0; i < all.Count; i++)
        {
            GameObjectInfo info = all[i];
            if (info.name.StartsWith("m_"))
            {
                if (!nameMap.ContainsKey(info.name))
                {
                    needOutput.Add(info);
                    nameMap.Add(info.name, info.path);
                }
                else
                {
                    Debug.LogError("Please Rename Exsit Same Key:" + info.name + "\n" + nameMap[info.name] + "\n" + info.path);
                    return;
                }

            }
        }
        string binderName = prefab.name + "Binder";
        string dir = Application.dataPath + "/LuaFramework/Lua/UIBinder/" + moduleName;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string binderPath = dir + "/" + binderName + ".lua";
        StreamWriter sw;
        if (File.Exists(binderPath))
        {
            File.Delete(binderPath);
        }
        sw = File.CreateText(binderPath);

        sw.WriteLine("local " + binderName + " = Class(\"" + binderName + "\", CONST.ClassType.CSHARP)");
        sw.WriteLine("function " + binderName + ":ctor(gameObject)");
        sw.WriteLine("    local transform = gameObject.transform");
        for (int i = 0; i < needOutput.Count; i++)
        {
            GameObjectInfo info = needOutput[i];
            string componentName = "";
            if (info.gameObject.GetComponent<InputField>() != null)
            {
                componentName = "InputField";
            }
            else if (info.gameObject.GetComponent<Dropdown>() != null)
            {
                componentName = "Dropdown";
            }
            else if (info.gameObject.GetComponent<Slider>() != null)
            {
                componentName = "Slider";
            }
            else if (info.gameObject.GetComponent<ToggleGroup>() != null)
            {
                componentName = "ToggleGroup";
            }
            else if (info.gameObject.GetComponent<Toggle>() != null)
            {
                componentName = "Toggle";
            }
            else if (info.gameObject.GetComponent<Button>() != null)
            {
                componentName = "Button";
            }
            else if (info.gameObject.GetComponent<Image>() != null)
            {
                componentName = "Image";
            }
            else if (info.gameObject.GetComponent<Text>() != null)
            {
                componentName = "Text";
            }
            else if (info.gameObject.GetComponent<RectTransform>() != null)
            {
                componentName = "RectTransform";
            }

            sw.WriteLine("    self." + info.name + " = transform:Find(\"" + info.path + "\").gameObject");
            if (componentName != "")
            {
                sw.WriteLine("    self." + info.name + componentName + " = self." + info.name + ":GetComponent(\"" + componentName + "\")");
            }
        }
        sw.WriteLine("end\nreturn " + binderName);
        sw.Close();
        sw.Dispose();
        Debug.Log("Success Build Binder:" + binderPath);
    }

    static void FindGameObject(GameObjectInfo start, List<GameObjectInfo> all)
    {
        if (start == null || start.gameObject == null || start.gameObject.transform.childCount == 0) return;
        for (int j = 0; j < start.gameObject.transform.childCount; j++)
        {
            Transform child = start.gameObject.transform.GetChild(j);
            GameObjectInfo info = new GameObjectInfo(start.path + ((start.path != "") ? "/" : "") + child.name, child.name, child.gameObject);
            all.Add(info);
            FindGameObject(info, all);
        }
    }

    class GameObjectInfo
    {
        public string path;
        public string name;
        public GameObject gameObject;

        public GameObjectInfo(string path, string name, GameObject go)
        {
            this.path = path;
            this.gameObject = go;
            this.name = name;
        }
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

}
