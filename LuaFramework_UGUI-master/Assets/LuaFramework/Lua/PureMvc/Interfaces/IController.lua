local IController = Class("IController", CONST.ClassType.CORE)

function IController:RegisterCommand(notificationName, commandClassRef)
end

function IController:ExcuteCommand(notification)
end

function IController:RemoveCommand(notificationName)
end

function IController:HasCommand(notificationName)
end

return IController