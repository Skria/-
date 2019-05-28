using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : Manager
{
    //图集名字
    static string[] m_AtlasName = {
            "HeroIcon",
        };
    static Dictionary<string, SpriteAtlas> m_AtlasMap;
    static Dictionary<string, Sprite> m_SpritesMap;
    public override void Init()
    {
        m_AtlasMap = new Dictionary<string, SpriteAtlas>();
        m_SpritesMap = new Dictionary<string, Sprite>();

        string rootPath = AppConst.AtlasRoot + "\\";
        for(int i = 0; i < m_AtlasName.Length; i++)
        {
            string tempPath = rootPath + m_AtlasName[i];
            SpriteAtlas tempAtlas = App.ResourceManager.LoadAsset<SpriteAtlas>(tempPath) as SpriteAtlas;
            m_AtlasMap.Add(m_AtlasName[i],tempAtlas);
        }
    }

    public Sprite GetSprite(string atlasName,string spriteName)
    {
        Sprite sprite = null;
        if (m_SpritesMap.TryGetValue(spriteName, out sprite))
        {
            return sprite;
        }

        if(sprite == null)
        {
            SpriteAtlas atlas = m_AtlasMap[atlasName];
            sprite = atlas.GetSprite(spriteName);
            m_SpritesMap.Add(spriteName, sprite);
        }
        return sprite;
    }
}
