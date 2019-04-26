IController = require("PureMVC/Interfaces/IController")
View = require("PureMVC/Core/View")
Observer = require("PureMVC/Patterns/Observer/Observer")

Controller = Class("Controller", CONST.ClassType.CORE, {}, IController)

Controller._instance = nil
Controller._commandMap = nil
Controller._view = nil

function Controller:ctor()
    self._commandMap = {}
    self:InitializeController()
end

function Controller:InitializeController()
    self._view = View.GetInstance()
end

function Controller:ExcuteCommand(notification)
    local commandCls = self._commandMap[notification:GetName()]
    if commandCls then
        local command = commandCls.New()
        command:Excute(notification)
        command:Release()
    end
end

function Controller:RegisterCommand(notificationName, commandCls)
    if not self._commandMap[notificationName] then
        self._view:RegisterObserver(notificationName, Observer.New(self.ExcuteCommand, self))
    end
    self._commandMap[notificationName] = commandCls
end

function Controller:RemoveCommand(notificationName)
    if self._commandMap[notificationName] then
        self._view:RemoveObserver(notificationName, self)
    end
    self._commandMap[notificationName] = nil
end

function Controller:HasCommand(notificationName)
    return (self._commandMap[notificationName] ~= nil)
end

function Controller.GetInstance()
    return _G["Controller"]._instance
end

function Controller:Clean()

end

Controller._instance = Controller.New()

return Controller