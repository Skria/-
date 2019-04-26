INotification = require("PureMVC/Interfaces/INotification")
local Notification = Class("Notification", CONST.ClassType.CORE, {}, INotification)

Notification._name = nil
Notification._body = nil
Notification._type = nil

function Notification:ctor(name, body, type)
    self._name = name
    self._body = body
    self._type = type
end

function Notification:ToString()
    local msg = "NotificationName:"..self._name
    msg = msg.."\nBody:"..tostring(self._body)
    msg = msg.."\nType:"..tostring(self._type)
    return msg
end

function Notification:GetName()
    return self._name
end

function Notification:GetBody()
    return self._body
end

function Notification:GetType()
    return self._type
end

function Notification:Release()
    if self._body then
        --[[for k,_ in pairs(self._body) do
            self._body[k] = nil
        end]]
    end
    self._body = nil
end

return Notification