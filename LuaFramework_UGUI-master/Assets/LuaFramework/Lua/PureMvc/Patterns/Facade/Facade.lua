IFacade = require("PureMVC/Interfaces/IFacade")
Controller = require("PureMVC/Core/Controller")
Model = require("PureMVC/Core/Model")
View = require("PureMVC/Core/View")
Notification = require("PureMVC/Patterns/Observer/Notification")
Mediator = require("PureMVC/Patterns/Mediator/Mediator")
SimpleCommand = require("PureMVC/Patterns/Command/SimpleCommand")
MacroCommand = require("PureMVC/Patterns/Command/MacroCommand")
Proxy = require("PureMVC/Patterns/Proxy/Proxy")
ViewComponent = require("PureMVC/Patterns/ViewComponent/ViewComponent")

Facade = Class("Facade", CONST.ClassType.CORE, IFacade)

Facade._instance = nil
Facade._controller = nil
Facade._model = nil
Facade._view = nil

function Facade:ctor()
    self:InitializeFacade()
end

function Facade:InitializeFacade()
    self:InitializeModel()
    self:InitializeController()
    self:InitializeView()
end

function Facade:InitializeController()
    self._controller = Controller.GetInstance()
end

function Facade:InitializeModel()
    self._model = Model.GetInstance()
end

function Facade:InitializeView()
    self._view = View.GetInstance()
end

function Facade:RegisterCommand(notificationName, commandCls)
    self._controller:RegisterCommand(notificationName, commandCls)
end

function Facade:RemoveCommand(notificationName)
    self._controller:RemoveCommand(notificationName)
end

function Facade:HasCommand(notificationName)
    return self._controller:HasCommand(notificationName)
end

function Facade:RegisterProxy(proxy)
    self._model:RegisterProxy(proxy)
end

function Facade:RetrieveProxy(proxyName)
    return self._model:RetrieveProxy(proxyName)
end

function Facade:RemoveProxy(proxyName)
    return self._model:RemoveProxy(proxyName)
end

function Facade:HasProxy(proxyName)
    return self._model:HasProxy(proxyName)
end

function Facade:RegisterMediator(mediator)
    self._view:RegisterMediator(mediator)
end

function Facade:RetrieveMediator(mediatorName)
    return self._view:RetrieveMediator(mediatorName)
end

function Facade:RemoveMediator(mediatorName)
    return self._view:RemoveMediator(mediatorName)
end

function Facade:HasMediator(mediatorName)
    return self._view:HasMediator(mediatorName)
end

function Facade:SendNotification(notificationName, body, type)
    self:NotifyObservers(Notification.New(notificationName, body, type))
end

function Facade:SendNotificationAsync(notificationName, body, type)
    self:NotifyObserversAsync(Notification.New(notificationName, body, type))
end

function Facade:NotifyObservers(notification)
    self._view:NotifyObservers(notification)
end

function Facade:NotifyObserversAsync(notification)
    self._view:NotifyObserversAsync(notification)
end

function Facade:Clean()
    self._view:Clean()
    self._controller:Clean()
    self._model:Clean()
    local instances = ClassGC:GetInstancesByType(CONST.ClassType.CSHARP)
    for _,v in ipairs(instances) do
        if v.StopAllCoroutine and type(v.StopAllCoroutine) == "function" then
            v:StopAllCoroutine()
        end
    end

    ClassGC:ReleaseInstances(instances)
    ClassGC:GC()
end

function Facade.GetInstance()
    return _G["Facade"]._instance
end

Facade._instance = Facade.New()

Facade = Facade.GetInstance()

return Facade