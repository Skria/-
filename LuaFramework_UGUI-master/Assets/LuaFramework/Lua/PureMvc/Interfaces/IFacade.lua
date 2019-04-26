local IFacade = Class("IFacade", CONST.ClassType.CORE)

function IFacade:RegisterProxy(proxy)
end

function IFacade:RetrieveProxy(proxyName)
end

function IFacade:RemoveProxy(proxyName)
end

function IFacade:HasProxy(proxyName)
end

function IFacade:RegisterCommand(notificationName, commandClassRef)
end

function IFacade:RemoveCommand(notificationName)
end

function IFacade:HasCommand(notificationName)
end

function IFacade:RegisterMediator(mediator)
end

function IFacade:RetrieveMediator(mediatorName)
end

function IFacade:RemoveMediator(mediatorName)
end

function IFacade:HasMediator(mediatorName)
end

function IFacade:NotifyObservers(notification)
end

return IFacade