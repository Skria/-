IObserver = require("PureMVC/Interfaces/IObserver")
local Observer = Class("Observer", CONST.ClassType.CORE, {}, IObserver)

Observer._notifyMethod = nil
Observer._notifyContext = nil

function Observer:ctor(notifyMethod, notifyContext)
    self._notifyMethod = notifyMethod
    self._notifyContext = notifyContext
end

function Observer:NotifyObserver(notification)
    self._notifyMethod(self._notifyContext, notification)
end

function Observer:CompareNotifyContext(obj)
    return self._notifyContext == obj
end

return Observer