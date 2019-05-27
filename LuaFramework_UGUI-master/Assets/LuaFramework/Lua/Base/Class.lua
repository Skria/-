-- Just for Lua Object Oriented Programming.
local _ENV = require("3rd.castl.runtime")

__ClassTracerTop = ""
 __ClassNeedTrace =  false--AppConst.LuaTrace

__ClassBaseMeta = {
    __tostring = function (tab)
        return tab.__name
    end,
}

__ObjectBaseMetaKeys = function(tab)
    local keys = {}
    if tab.__proto and type(tab.__proto) == "table" then
        for k,v in pairs(tab.__proto) do
            table.insert(keys,k)
        end
    end
    
    return keys
end

__ObjectBaseMeta = {
    __tostring = function (tab)
        return tab.__name
    end,
    __index = function(tab,key)
        if __ClassNeedTrace then
            __ClassTracerTop = "LastRecord=> tab:"..tostring(tab)..",key:"..tostring(key).."\n"..tostring(debug.traceback())
        end
        if key == nil then
            return nil
        end
        if tab.__proto == nil then
            return nil
        end
        local value = tab.__proto[key]
        return value
    end,
    __newindex = function (tab, key, value)
        if __ClassNeedTrace then
            __ClassTracerTop = "LastRecord=> tab:"..tostring(tab)..",key:"..tostring(key).."\n"..tostring(debug.traceback())
        end
        if key == nil then
            return nil
        end
        if tab.__proto == nil then
            return nil
        end
        tab.__proto[key] = value
    end
}

CloneRef = {}
-- local socket = require("socket")

function Clone(target)
    local sw
    --[[if AppConst.IsEditor and AppConst.LuaTrace then
        local top = getTopTraceback(2)
        if top then
            if not CloneRef[top] then
                CloneRef[top] = {
                    all = 0,
                    max = 0,
                    count = 0
                }
            end
            CloneRef[top].count = CloneRef[top].count + 1
        end
        sw = socket.gettime()
    end]]
    if type(target) ~= "table" then
        return target
    end
    local referenceMap = {}
    local function _copy(object, root)
        if object == nil then
            return object
        end
        --castl Const
        if object == _ENV.null then
            referenceMap[object] = object
            return object
        end

        if referenceMap[object] then
            return referenceMap[object]
        elseif type(object) ~= "table" then
            return object
        end
        
        if not root and object.__cls then --类不复制
            referenceMap[object] = object
            return object
        end

        local newOne = {}
        referenceMap[object] = newOne
        for k,v in pairs(object) do
            local newK = _copy(k)
            local newV = _copy(v)
            newOne[newK] =  newV
        end
        ------------------------
        if object.__fromCastl then
            return newOne
        end
        local meta = getmetatable(object)
        if meta then
            if meta == __ClassBaseMeta or meta == __ObjectBaseMeta then --固定meta不复制
                newOne = setmetatable(newOne,meta)
            else
                local newMetaOne = {}
                referenceMap[meta] = newMetaOne
                for k,v in pairs(meta) do
                    local newK = _copy(k)
                    local newV = _copy(v)
                    newMetaOne[newK] =  newV
                end
                newOne = setmetatable(newOne,newMetaOne)
            end
        end
        return newOne
    end
    local newObj = _copy(target,true)
    --[[if AppConst.IsEditor and AppConst.LuaTrace then
        local time = socket.gettime() - sw
        if top then
            CloneRef[top].all = CloneRef[top].all + time
            if time > CloneRef[top].max then
                CloneRef[top].max = time
            end
        end
    end]]
    return newObj
end

function ClassInterface(class,...)
    local interfaces = {...}
    if #interfaces > 0 then
        for _,interface in ipairs(interfaces) do
            for k,v in pairs(interface) do
                if not class[k] and type(v)=="function" then
                    class[k] = v
                end
            end
        end
    end
end

function Class(className, ctype, super, ...)
    local classSuper = super
    if classSuper == nil then
        classSuper = {}
    end
    if type(classSuper) ~= "table" then
        error("Only Extend Lua Class, Target Class Type is "..type(classSuper))
        return nil
    end
    local class = Clone(classSuper)
    setmetatable(class,nil)
    ClassInterface(class,...)
    class.__addr = tostring(class)
    class.__className = className
    class.__name = "[Cls:"..class.__className..", "..class.__addr.."]"
    class.__super = classSuper
    class.__classType = ctype
    class.__index = class
    class.__cls = true
    if class.ctor == nil then
        class.ctor = function(...) end
    end
    class = setmetatable(class,__ClassBaseMeta)
    class.__needReg = ClassGC:NeedRegisterObj(class)
    class.New = function(...)
        local object = {}
        local proto = {}
        proto.keys = __ObjectBaseMetaKeys
        proto.class = class
        proto.__prt = true
        proto.__cls = false
        proto = setmetatable(proto,class)
        object.__proto = proto
        object.__className = class.__className
        object.__obj = true
        object.__cls = false
        object.__prt = false
        object.__addr = tostring(object)
        object.__name = "[Obj:"..tostring(object.__className)..", "..tostring(object.__addr).."]"
        object = setmetatable(object,__ObjectBaseMeta)
        if object.__needReg then
            ClassGC:RegisterInstance(object)
        end
        object:ctor(...)
        return object
    end
    ClassLoader:RegisterClass(class, ctype)
    return class
end

function GetClassTracerTop()
    return __ClassTracerTop
end
