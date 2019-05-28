local UIInGamePanelBinder = Class("UIInGamePanelBinder", CONST.ClassType.CSHARP)
function UIInGamePanelBinder:ctor(gameObject)
    local transform = gameObject.transform
    self.m_1 = transform:Find("Image/m_1").gameObject
    self.m_1Image = self.m_1:GetComponent("Image")
    self.m_2 = transform:Find("Image/m_2").gameObject
    self.m_2Image = self.m_2:GetComponent("Image")
    self.m_3 = transform:Find("Image/m_3").gameObject
    self.m_3Image = self.m_3:GetComponent("Image")
    self.m_4 = transform:Find("Image/m_4").gameObject
    self.m_4Image = self.m_4:GetComponent("Image")
    self.m_start = transform:Find("Image/m_start").gameObject
    self.m_startImage = self.m_start:GetComponent("Image")
    self.m_end = transform:Find("Image/m_end").gameObject
    self.m_endImage = self.m_end:GetComponent("Image")
end
return UIInGamePanelBinder
