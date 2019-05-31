local Buff = Class("Buff", CONST.ClassType.CORE)

function Buff:ctor(inGamePanel,buffId,fromActor,toActor)
    self.buffId = buffId
    self.fromActor = fromActor
    self.toActor = toActor
    self.inGamePanel = inGamePanel
end


function Buff:DoBuff()
    self.toActor.curInfo.hp = self.toActor.curInfo.hp - self.fromActor.curInfo.attack
    print("角色" .. self.fromActor.actorHandle .. "对角色" .. self.toActor.actorHandle .. "造成了" .. self.fromActor.curInfo.attack)
    print("角色"..self.toActor.actorHandle .. "剩余生命" ..self.toActor.curInfo.hp )
end

return Buff