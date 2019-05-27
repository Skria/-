--主入口函数。从这里开始lua逻辑
require("Base/ClassLoader")
require("Base/Class")
require("Function")
require("Const")
require("Base/ClassGC")
require("PureMVC/Patterns/Facade/Facade")
App = LuaFramework.App

require("Common")

UIManager = App.UIManager
function Main()	
    RegisteredPanel("InGame","UIInGamePanel",CONST.UILayer.Normal)
    OpenPanel("UIInGamePanel")
end
