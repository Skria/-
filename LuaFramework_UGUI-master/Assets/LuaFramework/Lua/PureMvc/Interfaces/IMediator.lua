local IMediator = Class("IMediator", CONST.ClassType.CORE)

IMediator._mediatorName = nil
IMediator._viewComponent = nil

function IMediator:HandleNotification(notification)
end

function IMediator:_HandleNotification(notification)
end

function IMediator:OnRegister()
end

function IMediator:OnRemove()
end

return IMediator