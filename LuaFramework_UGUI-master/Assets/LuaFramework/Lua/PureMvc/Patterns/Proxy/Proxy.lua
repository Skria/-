IProxy = require("PureMVC/Interfaces/IProxy")
Notifier = require("PureMVC/Patterns/Observer/Notifier")
local Proxy = Class("Proxy", CONST.ClassType.CORE, Notifier, IProxy)

Proxy._proxyName = nil
Proxy._data = nil

function Proxy:ctor(proxyName, data)
    self._proxyName = proxyName or "Proxy"
    self._data = data
end

function Proxy:OnRegister()
end

function Proxy:OnRemove()
end

function Proxy:GetProxyName()
    return self._proxyName
end

function Proxy:GetData()
    return self._data
end

return Proxy