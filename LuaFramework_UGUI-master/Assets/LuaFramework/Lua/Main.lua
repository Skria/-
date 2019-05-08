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
	print("logic start1")
	UIManager:RegisteredPanel("SMain","TestUI",CONST.UILayer.Normal)
	UIManager:RegisteredPanel("SMain","Normal1",CONST.UILayer.Normal)
	UIManager:RegisteredPanel("SMain","Over1",CONST.UILayer.Over)
	UIManager:RegisteredPanel("SMain","Top1",CONST.UILayer.Top)
	UIManager:RegisteredPanel("SMain","FixTop1",CONST.UILayer.FixTop)

	print("logic start2")	
	OpenPanel("TestUI")
end
