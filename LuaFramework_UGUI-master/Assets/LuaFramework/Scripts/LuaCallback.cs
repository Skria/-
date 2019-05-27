using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;

namespace Framework
{
    public class LuaCallback
    {
        public LuaTable super;
        public LuaFunction call;
        public LuaCallback(LuaTable super, LuaFunction call)
        {
            this.super = super;
            this.call = call;
        }

        public void Call()
        {
            if (call != null)
            {
                if (super != null)
                {
                    call.Call(super);
                }
                else
                {
                    call.Call();
                }
            }
        }

        public void Call(object arg1)
        {
            if (call != null)
            {
                if (super != null)
                {
                    call.Call(super, arg1);
                }
                else
                {
                    call.Call(arg1);
                }
            }
        }

        public virtual void Release()
        {
            if (call != null)
            {
                call.Dispose();
            }
            call = null;
            super = null;
        }
    }

    public class TweenLuaCallback : LuaCallback
    {
        public TweenLuaCallback(LuaTable super, LuaFunction call) : base(super, call)
        {
            this.super = super;
            this.call = call;
        }

        public override void Release()
        {
            base.Release();
        }
    }
}
