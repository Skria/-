-- Create and Manage Lua Object for Lua and C#.
ClassLoader = {}
ClassLoader.classMap = {}

function ClassLoader:RegisterClass(class, type)
    local classData = {
        class = class,
        type = type
    }
    if self.classMap[class.__className] then
        error(class.__className.." Already Registed in ClassLoader, Will be Covered.")
    end
    self.classMap[class.__className] = classData
end

function ClassLoader:LoadClass(className)
    if self.classMap[className] then
        return self.classMap[className]
    end
    error(className.." not Registed in ClassLoader.")
    return nil
end

-- For Lua (Do not Registe in ClassGC)
function ClassLoader:Instantiate(className, ...)
    local classData = self:LoadClass(className)
    if classData then
        return classData.class.New(...)
    end
    return nil
end

-- For C#
function ClassLoader:InstantiateHandle(className, ...)
    local object = self:Instantiate(className, ...)
    object.__fromUnity = true
    if object then
        return object.__handle
    end
    return 0
end

function ClassLoader:GetInstanceByHandle(handle)
    return ClassGC:GetInstance(handle)
end

function InitializeClassHandle(className, ...)
    return ClassLoader:InstantiateHandle(className, ...)
end

function CallHandleFunction(handle, func, ...)
    local inst = ClassGC:GetInstance(handle)
    local result = nil
    if inst[func] then
        result = inst[func](inst, ...)
    else
        error(tostring(inst).." Do not Has Func:"..func..".")
    end
    return result
end

function ReleaseInstanceHandle(handle)
    return ClassGC:ReleaseInstanceByHandle(handle)
end