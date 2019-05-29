local _ENV = require("3rd.castl.runtime")
UIManager = App.UIManager
AtlasManager = App.AtlasManager
function OpenPanel(uiName,intent)
	UIManager:OpenPanel(uiName,intent)
end

function OpenPanel(uiName)
	UIManager:OpenPanel(uiName,{})
end

function RegisteredPanel(sceneName,uiName,uiLayer)
	UIManager:RegisteredPanel(sceneName,uiName,uiLayer)
end

-- 加载生成的UIBinder
function LoadUIBinder(module, panelName, gameObject)
    local status,binder = pcall(require,"UIBinder/"..module.."/"..panelName.."Binder")
    if not status then
        LogError("Load UIBinder Failed:"..module.." > "..panelName)
    end
    local binderIns = binder.New(gameObject)
    return binderIns
end


function GetSprite(atlasName,spriteName)
	return AtlasManager:GetSprite(atlasName,spriteName)
end