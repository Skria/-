local UIInGamePanelMediator = Class("UIInGamePanelMediator", CONST.ClassType.LUA, Mediator)

function UIInGamePanelMediator:OnRegister()
  
end

function UIInGamePanelMediator:OnRemove()

end

function UIInGamePanelMediator:ListNotificationInterests()
    return { 
      
    }
end

function UIInGamePanelMediator:HandleNotification(notification)
    
end

return UIInGamePanelMediator