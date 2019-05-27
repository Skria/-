using UnityEngine;
using System.Collections;
using LuaInterface;

namespace LuaFramework {
    public class LuaManager : Manager {
        private LuaState lua;
        private LuaLoader loader;
        private LuaLooper loop = null;

        // Use this for initialization

        public override void Init()
        {
            loader = new LuaLoader();
            lua = new LuaState();
            this.OpenLibs();
            lua.LuaSetTop(0);

            LuaBinder.Bind(lua);
            DelegateFactory.Init();
            LuaCoroutine.Register(lua, this);

            InitStart();
        }

        public void InitStart() {
            InitLuaPath();
            InitLuaBundle();
            this.lua.Start();    //启动LUAVM
            this.StartMain();
            this.StartLooper();
        }

        void StartLooper() {
            loop = gameObject.AddComponent<LuaLooper>();
            loop.luaState = lua;
        }

        //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
        protected void OpenCJson() {
            lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            lua.OpenLibs(LuaDLL.luaopen_cjson);
            lua.LuaSetField(-2, "cjson");

            lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
            lua.LuaSetField(-2, "cjson.safe");
        }

        void StartMain() {
            lua.DoFile("Main.lua");

            LuaFunction main = lua.GetFunction("Main");
            main.Call();
            main.Dispose();
            main = null;    
        }
        
        /// <summary>
        /// 初始化加载第三方库
        /// </summary>
        void OpenLibs() {
            lua.OpenLibs(LuaDLL.luaopen_pb);      
            lua.OpenLibs(LuaDLL.luaopen_sproto_core);
            lua.OpenLibs(LuaDLL.luaopen_protobuf_c);
            lua.OpenLibs(LuaDLL.luaopen_lpeg);
            lua.OpenLibs(LuaDLL.luaopen_bit);
            lua.OpenLibs(LuaDLL.luaopen_socket_core);

            this.OpenCJson();
        }

        /// <summary>
        /// 初始化Lua代码加载路径
        /// </summary>
        void InitLuaPath() {
            if (!AppConst.BundleMode)
            {
                Debug.Log("Path!!!!!!!!!");
                string rootPath = AppConst.FrameworkRoot;
                lua.AddSearchPath(rootPath + "/Lua");
                lua.AddSearchPath(rootPath + "/ToLua/Lua");
            }
            //if (AppConst.DebugMode) {
            //    string rootPath = AppConst.FrameworkRoot;
            //    lua.AddSearchPath(rootPath + "/Lua");
            //    lua.AddSearchPath(rootPath + "/ToLua/Lua");
            //} else {
            //    //string rootPath = AppConst.FrameworkRoot;
            //    //lua.AddSearchPath(rootPath + "Lua");
            //    //lua.AddSearchPath(rootPath + "/ToLua/Lua");
            //    //lua.AddSearchPath(Util.DataPath + "lua");
            //}
        }

        /// <summary>
        /// 初始化LuaBundle
        /// </summary>
        void InitLuaBundle() {
            if (loader.beZip) {
                //loader.AddBundle("lua/lua.unity3d");
                //loader.AddBundle("lua/lua_math.unity3d");
                //loader.AddBundle("lua/lua_system.unity3d");
                //loader.AddBundle("lua/lua_system_reflection.unity3d");
                //loader.AddBundle("lua/lua_unityengine.unity3d");
                //loader.AddBundle("lua/lua_common.unity3d");
                //loader.AddBundle("lua/lua_logic.unity3d");
                //loader.AddBundle("lua/lua_view.unity3d");
                //loader.AddBundle("lua/lua_controller.unity3d");
                //loader.AddBundle("lua/lua_misc.unity3d");

                //loader.AddBundle("lua/lua_protobuf.unity3d");
                //loader.AddBundle("lua/lua_3rd_cjson.unity3d");
                //loader.AddBundle("lua/lua_3rd_luabitop.unity3d");
                //loader.AddBundle("lua/lua_3rd_pbc.unity3d");
                //loader.AddBundle("lua/lua_3rd_pblua.unity3d");
                //loader.AddBundle("lua/lua_3rd_sproto.unity3d");
            }
        }

        public void DoFile(string filename) {
            lua.DoFile(filename);
        }

        public void LuaGC() {
            lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
        }

        public void Close() {
            loop.Destroy();
            loop = null;

            lua.Dispose();
            lua = null;
            loader = null;
        }


        public T CallFunction<T>(string funcName, params object[] args)
        {
            LuaFunction func = lua.GetFunction(funcName);
            if (func != null)
            {
                switch (args.Length)
                {
                    case 0:
                        return func.Invoke<T>();
                    case 1:
                        return func.Invoke<object, T>(args[0]);
                    case 2:
                        return func.Invoke<object, object, T>(args[0], args[1]);
                    case 3:
                        return func.Invoke<object, object, object, T>(args[0], args[1], args[2]);
                    case 4:
                        return func.Invoke<object, object, object, object, T>(args[0], args[1], args[2], args[3]);
                    case 5:
                        return func.Invoke<object, object, object, object, object, T>(args[0], args[1], args[2], args[3], args[4]);
                }
                //return func.LazyCall(args);
            }
            return default(T);
        }

        public T CallObjectFunction<T>(int handle, string funcName, params object[] args) where T : class
        {
            object[] argsN = new object[args.Length + 2];
            argsN[0] = handle;
            argsN[1] = funcName;
            for (int i = 0; i < args.Length; i++)
            {
                argsN[i + 2] = args[i];
            }
            return CallFunction<T>("CallHandleFunction", argsN);
        }

        public void CallFunction(string funcName, params object[] args)
        {
            CallFunction<object>(funcName, args);
        }

        public void CallObjectFunction(int handle, string funcName, params object[] args)
        {
            CallObjectFunction<object>(handle, funcName, args);
        }

        public int InitializeLuaObject(string luaClassName, params object[] args)
        {
            object[] argsN = new object[args.Length + 1];
            argsN[0] = luaClassName;
            for (int i = 0; i < args.Length; i++)
            {
                argsN[i + 1] = args[i];
            }
            return CallFunction<int>("InitializeClassHandle", argsN);
        }

    }
}