local Skill = Class("Skill", CONST.ClassType.CORE)
local Buff = require("Logic/Ingame/Buff")
function Skill:ctor(inGamePanel,skillId,fromActor,aimActor)
    self.skillId = skillId
    self.inGamePanel = inGamePanel
    self.fromActor = fromActor
    self.aimActor = aimActor
    self:SkillInit()
end

function Skill:DoSkill()
    local buff = Buff.New(self.inGamePanel,self.buffId,self.fromActor,self.aimActor)
    self.fromActor:AddBuff(buff)
end

function Skill:SkillInit()
    self.buffId = 1
end

return Skill