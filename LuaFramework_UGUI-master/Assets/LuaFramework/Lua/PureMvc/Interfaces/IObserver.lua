local IObserver = Class("IObserver", CONST.ClassType.CORE)

IObserver._notifyMethod = nil
IObserver._notifyContext = nil

function IObserver:NotifyObserver(notifaction)
end

function IObserver:CompareNotifyContext(obj)
end

return IObserver