Notifier = require("PureMVC/Patterns/Observer/Notifier")
ICommand = require("PureMVC/Interfaces/ICommand")
local MacroCommand = Class("MacroCommand", CONST.ClassType.CORE, Notifier, ICommand)

MacroCommand.subCommands = nil

function MacroCommand:ctor()
    self.subCommands = {}
    self:InitializeMacroCommand()
end

function MacroCommand:InitializeMacroCommand()
end

function MacroCommand:AddSubCommand(commandCls)
    table.insert(self.subCommands, commandCls)
end

function MacroCommand:Excute(notification)
    while #self.subCommands > 0 do
        local commandCls = self.subCommands[1]
        local inst = commandCls.New()
        inst:Excute(notification)
        table.remove(self.subCommands, 1)
    end
end

function MacroCommand:Release()
    
end

return MacroCommand