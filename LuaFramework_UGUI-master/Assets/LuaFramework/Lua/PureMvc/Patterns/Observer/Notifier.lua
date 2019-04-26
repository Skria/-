INotifier = require("PureMVC/Interfaces/INotifier")
local Notifier = Class("Notifier", CONST.ClassType.CORE, {}, INotifier)


function Notifier:SendNotification(notificationName, body, type)
    Facade.GetInstance():SendNotification(notificationName, body, type)
end

function Notifier:SendNotificationAsync(notificationName, body, type)
    Facade.GetInstance():SendNotificationAsync(notificationName, body, type)
end


return Notifier