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
    
    for i=1,#self.buffQuene do
        self.buffQuene[i]:DoBuff() 
    end
end

function Actor:AddBuff(buff)
    table.insert(self.buffQuene,buff)
    self:DoBuffQuene()
end

function Actor:TurnAction(table,aimActor,callback)
    self:StartCoroutine(function()
        WaitForSeconds(2)
        print("我行动了 handle:" .. self.actorHandle .. "目标:" .. aimActor.actorHandle)
        self:Attack(aimActor)
        callback()
    end)
 
end

function Actor:StartCoroutine(func, ...)
    local cid = StartCoroutine(func, self, ...)
    table.insert(self._coroutineMap, cid)
    return cid
end

return Actor