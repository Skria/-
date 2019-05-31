local UIInGamePanel = Class("UIInGamePanel", CONST.ClassType.CSHARP, ViewComponent)
local UIInGamePanelMediator = require("Mediator/InGame/UIInGamePanelMediator")
local Actor = require("Logic/Ingame/Actor")
function UIInGamePanel:InitView(gameObject)
    UIInGamePanel.__super.InitView(self, gameObject)
    self.mediator = UIInGamePanelMediator.New("UIInGamePanelMediator", self)
    Facade:RegisterMediator(self.mediator)
    self.binder = LoadUIBinder("InGame", "UIInGamePanel", gameObject)
    self.iconTweenGroup = {
        self.binder.m_1:GetComponent("UITweenPosition"),
        self.binder.m_2:GetComponent("UITweenPosition"),
        self.binder.m_3:GetComponent("UITweenPosition"),
        self.binder.m_4:GetComponent("UITweenPosition"),
    }

    self.iconRectGroup = {
        self.binder.m_1:GetComponent("RectTransform"),
        self.binder.m_2:GetComponent("RectTransform"),
        self.binder.m_3:GetComponent("RectTransform"),
        self.binder.m_4:GetComponent("RectTransform"),
    }

    self.startPos = self.binder.m_start:GetComponent("RectTransform").anchoredPosition
    self.endPos = self.binder.m_end:GetComponent("RectTransform").anchoredPosition
    self.myActorTeam = {}
    self.enemyActorTeam = {}
    self.curActionMove = {}
    self.gameTurnC = nil

   
end


function UIInGamePanel:OpenView(intent)
    self:InitActor()
    self:GameStart()
end

function UIInGamePanel:GameStart()
    self:CalculateAction()
end

function UIInGamePanel:CalculateAction()
    local time = {

    }
    local minTime = 1000
    local minDuration = 1000
    for i=1,2 do
        local myActor = self.myActorTeam[i]
        local mySpeed = myActor.curInfo.speed
        local myHandle = myActor.actorHandle
        local myTime = math.ceil((CONST.MaxCurAction - self.curActionMove[myHandle]) / mySpeed) 
        if myTime < minTime then
            minTime = myTime
            minDuration = minTime / mySpeed
        end
        local temp = {
            time = myTime,
            speed = mySpeed,
            actorHandle = myHandle,
            actor = myActor,
        }
        table.insert(time,temp)
        local enemyActor = self.enemyActorTeam[i]
        local enemySpeed = enemyActor.curInfo.speed
        local enemyHandle = enemyActor.actorHandle
        local enemyTime = math.ceil((CONST.MaxCurAction - self.curActionMove[enemyHandle]) / enemySpeed) 
        if enemyTime < minTime then
            minTime = enemyTime
            minDuration = minTime / enemySpeed
        end
        temp = {
            time = enemyTime,
            speed = enemySpeed,
            actorHandle = enemyHandle,
            actor = enemyActor,
        }
        table.insert(time,temp)
    end

    table.sort(time, function(a,b)
         if a.time == b.time then
            return a.speed <= b.speed
         else
            return a.time < b.time
         end
    end)
    local sumCount = 0
    local finishCount = 0
    for i=1,#time do
        local temp = time[i]
        self.iconTweenGroup[temp.actorHandle].from = self.iconRectGroup[temp.actorHandle].anchoredPosition
        self.iconTweenGroup[temp.actorHandle].duration = minDuration/50
        if temp.time == minTime and temp.actor.actorStatus ~= CONST.ActorStatus.Dead then
            sumCount = sumCount + 1
            local aimActor = nil
            if temp.actor.playerTeam == CONST.PlayerTeam.Enemy then
                aimActor = self.myActorTeam[1]
            else
                aimActor = self.enemyActorTeam[1]
            end
            temp.actor:TurnAction(self,aimActor,function()
                finishCount = finishCount + 1
                self.iconRectGroup[temp.actorHandle].anchoredPosition = self.startPos
            end)
            self.iconTweenGroup[temp.actorHandle].to = self.endPos
            self.curActionMove[temp.actorHandle] = 0
        else
            self.curActionMove[temp.actorHandle] = self.curActionMove[temp.actorHandle] + temp.speed * minTime
            local temppos = (self.endPos + self.startPos) * (self.curActionMove[temp.actorHandle] / CONST.MaxCurAction)
            self.iconTweenGroup[temp.actorHandle].to = (self.endPos - self.startPos) * (self.curActionMove[temp.actorHandle] / CONST.MaxCurAction) + self.startPos
        end
        self.iconTweenGroup[temp.actorHandle]:Restart()
    end
    self:StartCoroutine(function()
        while true do
            WaitForSeconds(1)
            if finishCount == sumCount then
                break
            end
        end
        self:CalculateAction()
    end)
end



function UIInGamePanel:InitActor()
    local temp = {
        {
            hp = 100,
            speed = 1,
            attack = 7,
        },
        {
            hp = 100,
            speed = 3,
            attack = 5,
        },
        {
            hp = 100,
            speed = 5,
            attack = 3,
        },
        {
            hp = 100,
            speed = 7,
            attack = 1,
        }
    }
    for i=1,2 do
        local actor = Actor.New(i,temp[i],CONST.PlayerTeam.Myself,self)
        self.curActionMove[i] = 0 
        table.insert(self.myActorTeam,actor)
    end

    for i=3,4 do
        local actor = Actor.New(i,temp[i],CONST.PlayerTeam.Enemy,self)
        self.curActionMove[i] = 0 
        table.insert(self.enemyActorTeam,actor)
    end
end


function UIInGamePanel:GetActorByActorHandle(actorHandle)
    local actor = nil    
    for i=1,2 do
        if self.myActorTeam[i].actorHandle  == actorHandle then
            actor = self.myActorTeam[i].actorHandle
            return actor
        end

        if self.enemyActorTeam[i].actorHandle  == actorHandle then
            actor = self.enemyActorTeam[i].actorHandle
            return actor
        end
    end
    print("角色不存在")
    return nil
end

function UIInGamePanel:CloseView()

end

function UIInGamePanel:DestroyView()
    
end