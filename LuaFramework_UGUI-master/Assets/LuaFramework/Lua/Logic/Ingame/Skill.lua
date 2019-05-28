local Skill = Class("Skill", CONST.ClassType.CORE)

function Actor:ctor(fromActorHandle,toActorHandle,inGamePanel,skillId)
    self.skillId = skillId
    self.fromActorHandle = fromActorHandle
    self.toActorHandle = toActorHandle
    self.inGamePanel = inGamePanel
    self.buffQuene = {}


    self:SkillInit()
end

function Actor:SkillInit()
   
end