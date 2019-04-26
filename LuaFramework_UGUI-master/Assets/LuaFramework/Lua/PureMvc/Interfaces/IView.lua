local IView = Class("IView", CONST.ClassType.CORE)

function IView:RegisterObserver(notificationName, observer)
end

function IView:RemoveObserver(notificationName, notifyContext)
end

function IView:NotifyObservers(notification)
end

function IView:RegisterMediator(mediator)
end

function IView:RetrieveMediator(mediatorName)
end

function IView:RemoveMediator(mediatorName)
end

function IView:HasMediator(mediatorName)
end

return IView