Notifier = require("PureMVC/Patterns/Observer/Notifier")
IMediator = require("PureMVC/Interfaces/IMediator")
local Mediator = Class("Mediator", CONST.ClassType.CORE, Notifier, IMediator)

Mediator._mediatorName = nil
Mediator._viewComponent = nil

function Mediator:ctor(mediatorName, viewComponent)
    self._mediatorName = mediatorName
    self._viewComponent = viewComponent
    self._observers = {}
end

function Mediator:ListNotificationInterests()
    return {}
end

function Mediator:_HandleNotification(notification)
	if self._viewComponent == nil then
		self:HandleNotification(notification)
	elseif self._viewComponent:IsActive() then
		self:HandleNotification(notification)
	end
end

function Mediator:HandleNotification(notification)
end

function Mediator:RegisterObserver(notifyName, notifyMethod)
	local view = View.GetInstance()
	local observer = Observer.New(notifyMethod, self)
	view:RegisterObserver(notifyName, observer)
	self._observers[notifyName] = true
end

function Mediator:RemoveObserver(notifyName)
	if self._observers == nil then
		return 
	end
	local view = View.GetInstance()
	view:RemoveObserver(notifyName, self)
	self._observers[notifyName] = nil
end

function Mediator:OnRegister()
end

function Mediator:OnRemove()
end

function Mediator:GetMediatorName()
    return self._mediatorName
end

function Mediator:GetViewComponent()
    return self._viewComponent
end

return Mediator