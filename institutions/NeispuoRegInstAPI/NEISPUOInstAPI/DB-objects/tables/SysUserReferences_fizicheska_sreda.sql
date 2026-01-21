ALTER TABLE [inst_basic].[Building] ADD FOREIGN KEY ([SysUserID]) REFERENCES [core].[SysUser] ([SysUserID])
GO

ALTER TABLE [inst_basic].[BuildingAreaSpeciality] ADD FOREIGN KEY ([SysUserID]) REFERENCES [core].[SysUser] ([SysUserID])
GO

ALTER TABLE [inst_basic].[BuildingModernizationDegree] ADD FOREIGN KEY ([SysUserID]) REFERENCES [core].[SysUser] ([SysUserID])
GO

ALTER TABLE [inst_basic].[BuildingRoom] ADD FOREIGN KEY ([SysUserID]) REFERENCES [core].[SysUser] ([SysUserID])
GO

ALTER TABLE [inst_basic].[BuildingRoomEquipment] ADD FOREIGN KEY ([SysUserID]) REFERENCES [core].[SysUser] ([SysUserID])
GO

ALTER TABLE [inst_basic].[DistanceLearningCondition] ADD FOREIGN KEY ([SysUserID]) REFERENCES [core].[SysUser] ([SysUserID])
GO