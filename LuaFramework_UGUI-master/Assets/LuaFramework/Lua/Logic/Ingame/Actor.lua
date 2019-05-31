local Actor = Class("Actor", CONST.ClassType.CORE)
local Skill = require("Logic/Ingame/Skill")
function Actor:ctor(actorHandle,baseInfo,playerTeam,inGamePanel)
    self.actorHandle = actorHandle
    self.playerTeam = playerTeam
    self.baseInfo = baseInfo
    self.inGamePanel = inGamePanel
    self.actorStatus = CONST.ActorStatus.live
    self.buffQuene = {} --人物实际挂载的buff
    self._coroutineMap = {}
    self.curInfo = {
        hp = baseInfo.hp,
        speed = baseInfo.speed,
        attack = baseInfo.attack
    }
    self.ASkillId = 1
end

function Actor:Attack(aimActor)
    local tempAskill = Skill.New(self.inGamePanel,self.ASkillId,self,aimActor)
    tempAskill:DoSkill()
end

function Actor:BeAttack()
   
end

function Actor:HpChange()
   
end

function Actor:DoBuffQuene()
    --Buff效果计算

    for i=1,#self.buffQuene do
        self.buffQuene[i]:DoBuff() 
    end

end

function Actor:AddBuff(buff)
    table.insert(self.buffQuene,buff)
    if buff.isUseNow then
        self:DoBuffQuene()
        self:ClearZeroBuff()
    end
end

function Actor:ReduceLastRound()
    if #self.buffQuene == 0 then
        return 
    end

    for i=#self.buffQuene,1 do
        self.buffQuene[i].buffLastRound = self.buffQuene[i].buffLastRound - 1
    end
end

function Actor:ClearZeroBuff()
    if #self.buffQuene == 0 then
        return 
    end
    for i=#self.buffQuene,1 do
        if self.buffQuene[i].buffLastRound <= 0 then
            table.remove(self.buffQuene,i)
        end
    end
end

function Actor:TurnAction(table,aimActor,callback)
    self:StartCoroutine(function()
        WaitForSeconds(2)
        print("我行动了 handle:" .. self.actorHandle .. "目标:" .. aimActor.actorHandle)
        --计算自身buff
        self:DoBuffQuene()
        --进攻
        self:Attack(aimActor)
        --自身buff回合-1  
        self:ReduceLastRound()
        --清除持续回合为0的buff
        self:ClearZeroBuff()
        callback()
    end)
 
end

function Actor:StartCoroutine(func, ...)
    local cid = StartCoroutine(func, self, ...)
    table.insert(self._coroutineMap, cid)
    return cid
end

return Actor