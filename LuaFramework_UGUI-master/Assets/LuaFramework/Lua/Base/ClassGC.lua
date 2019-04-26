-- Manage Lua Object Lifetime for C#.
local _ENV = require("3rd.castl.runtime")



ClassGC = {}
ClassGC.instanceMap = {}
ClassGC.instanceClassMap = {}
ClassGC.objectRefMap = {}
ClassGC.curHandleId = 1
ClassGC.autoRegister = false
ClassGC.autoRecordRef = false
--需要注册GC的
ClassGC.needRegister = {
    ["ViewComponent"] = true,
}


ClassGC.DumpId = 0

function ClassGC:RegisterInstance(instance)
    local handle = ClassGC.curHandleId + 1
    instance.__handle = handle
    self.instanceMap[handle] = instance
    local classMap = self:GetInstanceClassList(instance.__className)
    table.insert(classMap, handle)
    ClassGC.curHandleId = ClassGC.curHandleId + 1
    return handle
end

function ClassGC:GetInstanceClassList(className)
    if not self.instanceClassMap[className] then
        self.instanceClassMap[className] = {}
    end
    return self.instanceClassMap[className]
end

function ClassGC:GetInstancesByType(ctype)
    local instances = {}
    for k,v in pairs(self.instanceMap) do
        local ictype = v.__classType
        if ctype == ictype then
            table.insert(instances, v)
        end
    end
    return instances
end

function ClassGC:ReleaseInstances(instances)
    for _,v in ipairs(instances) do
        self:ReleaseInstance(v)
    end
end

function ClassGC:GetInstance(handle)
    if self.instanceMap[handle] then
        return self.instanceMap[handle]
    end
    error("Instance Handle Illegal, Please Check.")
    return nil
end

function ClassGC:ReleaseInstance(instance)
    self:ReleaseInstanceByHandle(instance.__handle)
end

function ClassGC:ReleaseInstanceByHandle(handle)
    if handle ~= nil then
        local instance = self.instanceMap[handle]
        if instance then
            local classMap = self:GetInstanceClassList(instance.__className)
            local name = tostring(instance)
            for i,v in ipairs(classMap) do
                if v == handle then
                    table.remove(classMap,i)
                    break
                end
            end
            print("ClassGC: Release Instance Handle ["..tostring(handle).."]"..name)
            self:CleanInstance(instance) --todo --temp
            self.instanceMap[handle] = nil
            return
        end
    end
    LogWarning("Release Instance Handle ["..tostring(handle).."] Illegal, Please Check.")
end

function ClassGC:GetObjectName(object)
    return tostring(object)
end

function ClassGC:CleanInstance(instance)
    print("ClassGC:Clean:"..self:GetObjectName(instance))
    local referenceMap = {}
    local function _clean(object,root)
        if not object then
            return
        end
        --castl Const
        if object == _ENV.null then
            return
        end
        --local name = self:GetObjectName(object)
        if referenceMap[object] then
            return
        end
        if object.__fromCastl then
            return
        end
        if object.__classType == CONST.ClassType.CORE or object.__classType == CONST.ClassType.SINGLETON then
            return
        end
        if object.__cls then
            return
        end
        if not root and object.__obj then
            return
        end
        referenceMap[object] = object
        if type(object) == "table" then
            local needCleanKeys = {}
            local obj = object
            if object.keys and type(object.keys) == "function" and object.__proto then
                needCleanKeys = object:keys()
                obj = object.__proto
            else
                for k,v in pairs(object) do
                    table.insert(needCleanKeys,k)
                end
            end
            for _,k in ipairs(needCleanKeys) do
                if obj == nil then
                    break
                end
                local v
                if k ~= nil and obj then
                    v = obj[k]
                end
                if type(k) == "table" then
                    _clean(k)
                end
                if type(v) == "table" then
                    _clean(v)
                end
                if type(v) == "userdata" then
                    obj[k] = nil
                end
                --[[if root and k~=nil then
                    obj[k] = nil
                end]]
            end
            if root then
                --触发LuaTable释放
                obj.cleaned = true
                obj.cleaned = nil
            end
        end
    end
    _clean(instance,true)
end

function ClassGC:GC()
    local maxHandle = 1
    for k,_ in pairs(self.instanceMap) do
        if k > maxHandle then
            maxHandle = k
        end
    end
    self.curHandleId = maxHandle
    local c = collectgarbage("count")
	print("ClassGC:Begin GC count = {0} kb", c)
	collectgarbage("collect")
	local c_ = collectgarbage("count")
	print("ClassGC:After GC count = {0} kb, Release {1} kb", c_, c - c_)
end


function ClassGC:GetRecordKey(obj)
    local key
    if obj.__handle then
        key = obj.__handle
    else
        if not obj.__className and obj.__tostring then
            return
        end
        key = self:GetObjectName(obj)
    end
    return key
end

function ClassGC:NeedRegisterObj(class)
    local super = class
    while super do
        if self.needRegister[super.__className] then
            return true
        end
        super = super.__super
    end
    return false
end


function ClassGC:DumpMemRef()
    local path = UnityEngine.Application.dataPath.."/../Temp/LuaMemRef"
    local mri = require("Base/MemoryReferenceInfo")
    -- Set config.
    mri.m_cConfig.m_bAllMemoryRefFileAddTime = false
    --mri.m_cConfig.m_bSingleMemoryRefFileAddTime = false
    --mri.m_cConfig.m_bComparedMemoryRefFileAddTime = false
    local oldId = self.DumpId
    self.DumpId = self.DumpId + 1
    mri.m_cMethods.DumpMemorySnapshot(path, tostring(self.DumpId), -1)
    
--[[
    -- 打印当前 Lua 虚拟机中某一个对象的所有相关引用。
    -- strSavePath - 快照保存路径，不包括文件名。
    -- strExtraFileName - 添加额外的信息到文件名，可以为 "" 或者 nil。
    -- nMaxRescords - 最多打印多少条记录，-1 打印所有记录。
    -- strObjectName - 对象显示名称。
    -- cObject - 对象实例。
    -- MemoryReferenceInfo.m_cMethods.DumpMemorySnapshotSingleObject(strSavePath, strExtraFileName, nMaxRescords, strObjectName, cObject)
    collectgarbage("collect")
    mri.m_cMethods.DumpMemorySnapshotSingleObject("./", "SingleObjRef-Object", -1, "Author", _G.Author)

    -- We can also find string references.
    collectgarbage("collect")
    mri.m_cMethods.DumpMemorySnapshotSingleObject("./", "SingleObjRef-String", -1, "Author Name", "yaukeywang")
]]
    -- 比较两份内存快照结果文件，打印文件 strResultFilePathAfter 相对于 strResultFilePathBefore 中新增的内容。
    -- strSavePath - 快照保存路径，不包括文件名。
    -- strExtraFileName - 添加额外的信息到文件名，可以为 "" 或者 nil。
    -- nMaxRescords - 最多打印多少条记录，-1 打印所有记录。
    -- strResultFilePathBefore - 第一个内存快照文件。
    -- strResultFilePathAfter - 第二个用于比较的内存快照文件。
    -- MemoryReferenceInfo.m_cMethods.DumpMemorySnapshotComparedFile(strSavePath, strExtraFileName, nMaxRescords, strResultFilePathBefore, strResultFilePathAfter)
    if oldId > 0 then
        mri.m_cMethods.DumpMemorySnapshotComparedFile(path, "Compared", -1, path.."/LuaMemRefInfo-All-["..oldId.."].txt", path.."/LuaMemRefInfo-All-["..self.DumpId.."].txt")
    end
--[[
    -- 按照关键字过滤一个内存快照文件然后输出到另一个文件.
    -- strFilePath - 需要被过滤输出的内存快照文件。
    -- strFilter - 过滤关键字
    -- bIncludeFilter - 包含关键字(true)还是排除关键字(false)来输出内容。
    -- bOutputFile - 输出到文件(true)还是 console 控制台(false)。
    -- MemoryReferenceInfo.m_cBases.OutputFilteredResult(strFilePath, strFilter, bIncludeFilter, bOutputFile)
    -- Filter all result include keywords: "Author".
    mri.m_cBases.OutputFilteredResult("./LuaMemRefInfo-All-[2-After].txt", "Author", true, true)

    -- Filter all result exclude keywords: "Author".
    mri.m_cBases.OutputFilteredResult("./LuaMemRefInfo-All-[2-After].txt", "Author", false, true)
]]
    -- All dump finished!
    print("Dump memory snapshot information finished!")
end

function ClassGCStatus()
    --ClassGC:DumpMemRef()
    print("ClassGCStatus////////////////////////////")
    printTable(View._instance.record)
    printTable(CloneRef)
    print("ClassGCStatus-----------------------------")
end