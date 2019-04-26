Notifier = require("PureMVC/Patterns/Observer/Notifier")
ICommand = require("PureMVC/Interfaces/ICommand")
local SimpleCommand = Class("SimpleCommand", CONST.ClassType.CORE, Notifier, ICommand)

function SimpleCommand:Excute(notification)
end

function SimpleCommand:Release()
    
end

return SimpleCommand