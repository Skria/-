IModel = require("PureMVC/Interfaces/IModel")

Model = Class("Model", CONST.ClassType.CORE, {}, IModel)

Model._instance = nil
Model._proxyMap = nil

function Model:ctor()
    self._proxyMap = {}
    self:InitializeModel()
end

function Model:InitializeModel()
end

function Model:RegisterProxy(proxy)
    self._proxyMap[proxy:GetProxyName()] = proxy
    proxy:OnRegister()
end

function Model:RetrieveProxy(proxyName)
    local proxy = self._proxyMap[proxyName]
    return proxy
end

function Model:RemoveProxy(proxyName)
    local proxy = self._proxyMap[proxyName]
    proxy:OnRemove()
    self._proxyMap[proxyName] = nil
    return proxy
end

function Model:HasProxy(proxyName)
    return (self._proxyMap[proxyName] ~= nil)
end

function Model.GetInstance()
    return _G["Model"]._instance
end

function Model:Clean()

end

Model._instance = Model.New()

return Model