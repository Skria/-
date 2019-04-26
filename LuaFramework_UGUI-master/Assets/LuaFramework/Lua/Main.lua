--主入口函数。从这里开始lua逻辑
require("Base/ClassLoader")
require("Base/Class")
require("Function")
require("Const")
require("PureMVC/Patterns/Facade/Facade")
require("Base/ClassGC")

function Main()					
	print("logic start")	

end

-- --场景切换通知
-- function OnLevelWasLoaded(level)
-- 	collectgarbage("collect")
-- 	Time.timeSinceLevelLoad = 0
-- end

-- function OnApplicationQuit()
-- end