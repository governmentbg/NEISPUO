ALTER TABLE [inst_basic].[Building] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[Building] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[BuildingModernizationDegree] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[BuildingModernizationDegree] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[BuildingRoom] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[BuildingRoom] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[BuildingRoomEquipment] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[BuildingRoomEquipment] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[changeYearLogTemp] ADD  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [inst_basic].[DistanceLearningCondition] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[DistanceLearningCondition] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionDepartment] ADD  CONSTRAINT [DF_InstitutionDepartment_IsMain]  DEFAULT ((0)) FOR [IsMain]
GO
ALTER TABLE [inst_basic].[InstitutionDepartment] ADD  CONSTRAINT [DF__Instituti__Valid__2057CCD0]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionDepartment] ADD  CONSTRAINT [DF__Instituti__Valid__214BF109]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  CONSTRAINT [DF_InstitutionDetail_IsODZ]  DEFAULT ((0)) FOR [IsODZ]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  CONSTRAINT [DF_InstitutionDetail_IsProfSchool]  DEFAULT ((0)) FOR [IsProfSchool]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  CONSTRAINT [DF_InstitutionDetail_IsNational]  DEFAULT ((0)) FOR [IsNational]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  CONSTRAINT [DF_InstitutionDetail_IsProvideEduServ]  DEFAULT ((0)) FOR [IsProvideEduServ]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  CONSTRAINT [DF_InstitutionDetail_IsDelegateBudget]  DEFAULT ((0)) FOR [IsDelegateBudget]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  CONSTRAINT [DF_InstitutionDetail_IsNonIndDormitory]  DEFAULT ((0)) FOR [IsNonIndDormitory]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  CONSTRAINT [DF_InstitutionDetail_IsInternContract]  DEFAULT ((0)) FOR [IsInternContract]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  CONSTRAINT [DF_InstitutionDetail_IsAppInnovSystem]  DEFAULT ((0)) FOR [IsAppInnovSystem]
GO
ALTER TABLE [inst_basic].[InstitutionDetail] ADD  DEFAULT ((0)) FOR [IsProfSchoolWithSpecialties]
GO
ALTER TABLE [inst_basic].[InstitutionInnovation] ADD  CONSTRAINT [DF__Instituti__Valid__3DE82FB7]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionInnovation] ADD  CONSTRAINT [DF__Instituti__Valid__3EDC53F0]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionPhone] ADD  CONSTRAINT [DF_InstitutionPhone_IsInstitution]  DEFAULT ((0)) FOR [IsInstitution]
GO
ALTER TABLE [inst_basic].[InstitutionPhone] ADD  CONSTRAINT [DF_InstitutionPhone_IsMain]  DEFAULT ((0)) FOR [IsMain]
GO
ALTER TABLE [inst_basic].[InstitutionPhone] ADD  CONSTRAINT [DF__Instituti__Valid__22401542]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionPhone] ADD  CONSTRAINT [DF__Instituti__Valid__2334397B]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionProject] ADD  CONSTRAINT [DF_InstitutionProject_IsArchive]  DEFAULT ((0)) FOR [IsArchive]
GO
ALTER TABLE [inst_basic].[InstitutionProject] ADD  CONSTRAINT [DF__Instituti__Valid__24285DB4]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionProject] ADD  CONSTRAINT [DF__Instituti__Valid__251C81ED]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner] ADD  CONSTRAINT [DF_InstitutionProjectPartner_isCoordinator]  DEFAULT ((0)) FOR [IsCoordinator]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner] ADD  CONSTRAINT [DF__Instituti__Valid__2DB1C7EE]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner] ADD  CONSTRAINT [DF__Instituti__Valid__2EA5EC27]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPriorityArea] ADD  CONSTRAINT [DF__Instituti__Valid__2610A626]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPriorityArea] ADD  CONSTRAINT [DF__Instituti__Valid__2704CA5F]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionPublicCouncil] ADD  CONSTRAINT [DF__Instituti__Valid__2F9A1060]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionPublicCouncil] ADD  CONSTRAINT [DF__Instituti__Valid__308E3499]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionSchoolBoard] ADD  CONSTRAINT [DF_InstitutionSchoolBoard_ValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionSchoolBoard] ADD  CONSTRAINT [DF_InstitutionSchoolBoard_ValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[InstitutionVehicle] ADD  CONSTRAINT [DF__Instituti__Valid__318258D2]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[InstitutionVehicle] ADD  CONSTRAINT [DF__Instituti__Valid__32767D0B]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[PersonCompSkill] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[PersonCompSkill] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[PersonDetail] ADD  CONSTRAINT [DF_PersonDetail_IsExtendStudent]  DEFAULT ((0)) FOR [IsExtendStudent]
GO
ALTER TABLE [inst_basic].[PersonDetail] ADD  CONSTRAINT [DF_PersonDetail_IsPensioneer]  DEFAULT ((0)) FOR [IsPensioneer]
GO
ALTER TABLE [inst_basic].[PersonDetail] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[PersonDetail] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[PersonFLSkill] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[PersonFLSkill] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[PersonOKS] ADD  CONSTRAINT [DF_PersonOKS_UniversityID]  DEFAULT ((499)) FOR [UniversityID]
GO
ALTER TABLE [inst_basic].[PersonOKS] ADD  CONSTRAINT [DF_PersonOKS_IsPKTeacher]  DEFAULT ((0)) FOR [IsPKTeacher]
GO
ALTER TABLE [inst_basic].[PersonOKS] ADD  CONSTRAINT [DF__PersonOKS__Valid__766D4B53]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[PersonOKS] ADD  CONSTRAINT [DF__PersonOKS__Valid__77616F8C]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[PersonOKSSubjectGroup] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[PersonOKSSubjectGroup] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[PersonPKS] ADD  CONSTRAINT [DF_PersonPKS_UniversityID]  DEFAULT ((499)) FOR [UniversityID]
GO
ALTER TABLE [inst_basic].[PersonPKS] ADD  CONSTRAINT [DF__PersonPKS__Valid__7E0E6D1B]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[PersonPKS] ADD  CONSTRAINT [DF__PersonPKS__Valid__7F029154]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[PersonQCourse] ADD  CONSTRAINT [DF_PersonQCourse_UniversityID]  DEFAULT ((499)) FOR [UniversityID]
GO
ALTER TABLE [inst_basic].[PersonQCourse] ADD  CONSTRAINT [DF__PersonQCo__Valid__088B3037]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[PersonQCourse] ADD  CONSTRAINT [DF__PersonQCo__Valid__097F5470]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_IsTravel]  DEFAULT ((0)) FOR [IsTravel]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_ContractTypeID]  DEFAULT ((16)) FOR [ContractTypeID]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_ContractYear]  DEFAULT ((9999)) FOR [ContractYear]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_IsAccoutablePerson]  DEFAULT ((0)) FOR [IsAccountablePerson]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_IsHospital]  DEFAULT ((0)) FOR [IsHospital]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_IsMentor]  DEFAULT ((0)) FOR [IsMentor]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_IsTrainee]  DEFAULT ((0)) FOR [IsTrainee]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_IsNotMeetReq]  DEFAULT ((0)) FOR [IsNotMeetReq]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_CurrentlyValid]  DEFAULT ((1)) FOR [CurrentlyValid]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF__Staff__ValidFrom__57E7F8DC]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF__Staff__ValidTo__58DC1D15]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_HasTELK]  DEFAULT ((0)) FOR [HasTELK]
GO
ALTER TABLE [inst_basic].[StaffPosition] ADD  CONSTRAINT [DF_StaffPosition_IsValid]  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_basic].[Building]  WITH CHECK ADD  CONSTRAINT [FK_BuildingBuildingType] FOREIGN KEY([BuildingTypeID])
REFERENCES [inst_nom].[BuildingType] ([BuildingTypeID])
GO
ALTER TABLE [inst_basic].[Building] CHECK CONSTRAINT [FK_BuildingBuildingType]
GO
ALTER TABLE [inst_basic].[Building]  WITH CHECK ADD  CONSTRAINT [FK_BuildingInstitution] FOREIGN KEY([InstitutionID], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [inst_basic].[Building] CHECK CONSTRAINT [FK_BuildingInstitution]
GO
ALTER TABLE [inst_basic].[Building]  WITH CHECK ADD  CONSTRAINT [FK_BuildingInstitutionDepartment] FOREIGN KEY([InstitutionDepartmentID])
REFERENCES [inst_basic].[InstitutionDepartment] ([InstitutionDepartmentID])
GO
ALTER TABLE [inst_basic].[Building] CHECK CONSTRAINT [FK_BuildingInstitutionDepartment]
GO
ALTER TABLE [inst_basic].[Building]  WITH CHECK ADD  CONSTRAINT [FK_BuildingSchoolShiftType] FOREIGN KEY([SchoolShiftTypeID])
REFERENCES [inst_nom].[SchoolShiftType] ([SchoolShiftTypeID])
GO
ALTER TABLE [inst_basic].[Building] CHECK CONSTRAINT [FK_BuildingSchoolShiftType]
GO
ALTER TABLE [inst_basic].[Building]  WITH CHECK ADD  CONSTRAINT [FK_BuildingSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[Building] CHECK CONSTRAINT [FK_BuildingSysUser]
GO
ALTER TABLE [inst_basic].[BuildingModernizationDegree]  WITH CHECK ADD  CONSTRAINT [FK_BuildingModernizationDegreeBuilding] FOREIGN KEY([BuildingID])
REFERENCES [inst_basic].[Building] ([BuildingID])
GO
ALTER TABLE [inst_basic].[BuildingModernizationDegree] CHECK CONSTRAINT [FK_BuildingModernizationDegreeBuilding]
GO
ALTER TABLE [inst_basic].[BuildingModernizationDegree]  WITH CHECK ADD  CONSTRAINT [FK_BuildingModernizationDegreeModernizationDegree] FOREIGN KEY([ModernizationDegreeID])
REFERENCES [inst_nom].[ModernizationDegree] ([ModernizationDegreeID])
GO
ALTER TABLE [inst_basic].[BuildingModernizationDegree] CHECK CONSTRAINT [FK_BuildingModernizationDegreeModernizationDegree]
GO
ALTER TABLE [inst_basic].[BuildingModernizationDegree]  WITH CHECK ADD  CONSTRAINT [FK_BuildingModernizationDegreeSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[BuildingModernizationDegree] CHECK CONSTRAINT [FK_BuildingModernizationDegreeSysUser]
GO
ALTER TABLE [inst_basic].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingAreaType] FOREIGN KEY([BuildingAreaTypeID])
REFERENCES [inst_nom].[BuildingAreaType] ([BuildingAreaTypeID])
GO
ALTER TABLE [inst_basic].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingAreaType]
GO
ALTER TABLE [inst_basic].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoom_BuildingRoomAreaType] FOREIGN KEY([BuildingRoomID])
REFERENCES [inst_basic].[BuildingRoom] ([BuildingRoomID])
GO
ALTER TABLE [inst_basic].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoom_BuildingRoomAreaType]
GO
ALTER TABLE [inst_basic].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoomBuilding] FOREIGN KEY([BuildingID])
REFERENCES [inst_basic].[Building] ([BuildingID])
GO
ALTER TABLE [inst_basic].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoomBuilding]
GO
ALTER TABLE [inst_basic].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoomBuildingRoomType] FOREIGN KEY([BuildingRoomTypeID])
REFERENCES [inst_nom].[BuildingRoomType] ([BuildingRoomTypeID])
GO
ALTER TABLE [inst_basic].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoomBuildingRoomType]
GO
ALTER TABLE [inst_basic].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoomInstitution] FOREIGN KEY([InstitutionID], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [inst_basic].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoomInstitution]
GO
ALTER TABLE [inst_basic].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoomSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoomSysUser]
GO
ALTER TABLE [inst_basic].[BuildingRoomEquipment]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoomEquipmentBuildingRoom] FOREIGN KEY([BuildingRoomID])
REFERENCES [inst_basic].[BuildingRoom] ([BuildingRoomID])
GO
ALTER TABLE [inst_basic].[BuildingRoomEquipment] CHECK CONSTRAINT [FK_BuildingRoomEquipmentBuildingRoom]
GO
ALTER TABLE [inst_basic].[BuildingRoomEquipment]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoomEquipmentEquipmentType] FOREIGN KEY([EquipmentTypeID])
REFERENCES [inst_nom].[EquipmentType] ([EquipmentTypeId])
GO
ALTER TABLE [inst_basic].[BuildingRoomEquipment] CHECK CONSTRAINT [FK_BuildingRoomEquipmentEquipmentType]
GO
ALTER TABLE [inst_basic].[BuildingRoomEquipment]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoomEquipmentSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[BuildingRoomEquipment] CHECK CONSTRAINT [FK_BuildingRoomEquipmentSysUser]
GO
ALTER TABLE [inst_basic].[DistanceLearningCondition]  WITH CHECK ADD  CONSTRAINT [FK_DistanceLearningConditionInstitution] FOREIGN KEY([InstitutionID], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [inst_basic].[DistanceLearningCondition] CHECK CONSTRAINT [FK_DistanceLearningConditionInstitution]
GO
ALTER TABLE [inst_basic].[DistanceLearningCondition]  WITH CHECK ADD  CONSTRAINT [FK_DistanceLearningConditionSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[DistanceLearningCondition] CHECK CONSTRAINT [FK_DistanceLearningConditionSysUser]
GO
ALTER TABLE [inst_basic].[InstitutionDepartment]  WITH CHECK ADD  CONSTRAINT [FK_InstDep_Country] FOREIGN KEY([CountryID])
REFERENCES [location].[Country] ([CountryID])
GO
ALTER TABLE [inst_basic].[InstitutionDepartment] CHECK CONSTRAINT [FK_InstDep_Country]
GO
ALTER TABLE [inst_basic].[InstitutionDepartment]  WITH CHECK ADD  CONSTRAINT [FK_InstDep_LocalArea] FOREIGN KEY([LocalAreaID])
REFERENCES [location].[LocalArea] ([LocalAreaID])
GO
ALTER TABLE [inst_basic].[InstitutionDepartment] CHECK CONSTRAINT [FK_InstDep_LocalArea]
GO
ALTER TABLE [inst_basic].[InstitutionDepartment]  WITH CHECK ADD  CONSTRAINT [FK_InstDep_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionDepartment] CHECK CONSTRAINT [FK_InstDep_SysUser]
GO
ALTER TABLE [inst_basic].[InstitutionDepartment]  WITH CHECK ADD  CONSTRAINT [FK_InstDep_Town] FOREIGN KEY([TownID])
REFERENCES [location].[Town] ([TownID])
GO
ALTER TABLE [inst_basic].[InstitutionDepartment] CHECK CONSTRAINT [FK_InstDep_Town]
GO
ALTER TABLE [inst_basic].[InstitutionDetail]  WITH CHECK ADD  CONSTRAINT [FK_InstDetails_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionDetail] CHECK CONSTRAINT [FK_InstDetails_SysUser]
GO
ALTER TABLE [inst_basic].[InstitutionDetail]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionDetailInstitution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [inst_basic].[InstitutionDetail] CHECK CONSTRAINT [FK_InstitutionDetailInstitution]
GO
ALTER TABLE [inst_basic].[InstitutionInnovation]  WITH CHECK ADD  CONSTRAINT [FK_InstInnovationSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionInnovation] CHECK CONSTRAINT [FK_InstInnovationSysUser]
GO
ALTER TABLE [inst_basic].[InstitutionInnovation]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionInnovation_Institution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [inst_basic].[InstitutionInnovation] CHECK CONSTRAINT [FK_InstitutionInnovation_Institution]
GO
ALTER TABLE [inst_basic].[InstitutionInnovation]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionInnovationInovationType] FOREIGN KEY([InnovationTypeID])
REFERENCES [inst_nom].[InnovationType] ([InnovationTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionInnovation] CHECK CONSTRAINT [FK_InstitutionInnovationInovationType]
GO
ALTER TABLE [inst_basic].[InstitutionPhone]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionPhone_Institution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [inst_basic].[InstitutionPhone] CHECK CONSTRAINT [FK_InstitutionPhone_Institution]
GO
ALTER TABLE [inst_basic].[InstitutionPhone]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionPhone_InstitutionDepartment] FOREIGN KEY([DepartmentID])
REFERENCES [inst_basic].[InstitutionDepartment] ([InstitutionDepartmentID])
GO
ALTER TABLE [inst_basic].[InstitutionPhone] CHECK CONSTRAINT [FK_InstitutionPhone_InstitutionDepartment]
GO
ALTER TABLE [inst_basic].[InstitutionPhone]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionPhone_PhoneType] FOREIGN KEY([PhoneTypeID])
REFERENCES [inst_nom].[PhoneType] ([PhoneTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionPhone] CHECK CONSTRAINT [FK_InstitutionPhone_PhoneType]
GO
ALTER TABLE [inst_basic].[InstitutionPhone]  WITH CHECK ADD  CONSTRAINT [FK_InstPhoneSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionPhone] CHECK CONSTRAINT [FK_InstPhoneSysUser]
GO
ALTER TABLE [inst_basic].[InstitutionProject]  WITH NOCHECK ADD  CONSTRAINT [FK_InstitutionProjectInstitution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [inst_basic].[InstitutionProject] NOCHECK CONSTRAINT [FK_InstitutionProjectInstitution]
GO
ALTER TABLE [inst_basic].[InstitutionProject]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionProjectProjectType] FOREIGN KEY([ProjectTypeID])
REFERENCES [inst_nom].[ProjectType] ([ProjectTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionProject] CHECK CONSTRAINT [FK_InstitutionProjectProjectType]
GO
ALTER TABLE [inst_basic].[InstitutionProject]  WITH CHECK ADD  CONSTRAINT [FK_InstProjectSysUsers] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionProject] CHECK CONSTRAINT [FK_InstProjectSysUsers]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner]  WITH CHECK ADD  CONSTRAINT [FK_InstProjectPartnerInstProject] FOREIGN KEY([InstitutionProjectID])
REFERENCES [inst_basic].[InstitutionProject] ([InstitutionProjectID])
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner] CHECK CONSTRAINT [FK_InstProjectPartnerInstProject]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner]  WITH CHECK ADD  CONSTRAINT [FK_InstProjectPartnerPartnerType] FOREIGN KEY([ProjectPartnerTypeID])
REFERENCES [inst_nom].[ProjectPartnerType] ([ProjectPartnerTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner] CHECK CONSTRAINT [FK_InstProjectPartnerPartnerType]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner]  WITH CHECK ADD  CONSTRAINT [FK_InstProjPartner_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionProjectPartner] CHECK CONSTRAINT [FK_InstProjPartner_SysUser]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPriorityArea]  WITH CHECK ADD  CONSTRAINT [FK_InsProjPriorityInstitutionProject] FOREIGN KEY([ProjectID])
REFERENCES [inst_basic].[InstitutionProject] ([InstitutionProjectID])
GO
ALTER TABLE [inst_basic].[InstitutionProjectPriorityArea] CHECK CONSTRAINT [FK_InsProjPriorityInstitutionProject]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPriorityArea]  WITH CHECK ADD  CONSTRAINT [FK_InstProjPriority_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionProjectPriorityArea] CHECK CONSTRAINT [FK_InstProjPriority_SysUser]
GO
ALTER TABLE [inst_basic].[InstitutionProjectPriorityArea]  WITH CHECK ADD  CONSTRAINT [FK_InstProjPriorityProjectPriorityAreaType] FOREIGN KEY([ProjectPriorityAreaTypeID])
REFERENCES [inst_nom].[ProjectPriorityAreaType] ([ProjectPriorityAreaTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionProjectPriorityArea] CHECK CONSTRAINT [FK_InstProjPriorityProjectPriorityAreaType]
GO
ALTER TABLE [inst_basic].[InstitutionPublicCouncil]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionPublicCouncilInstitution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [inst_basic].[InstitutionPublicCouncil] CHECK CONSTRAINT [FK_InstitutionPublicCouncilInstitution]
GO
ALTER TABLE [inst_basic].[InstitutionPublicCouncil]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionPublicCouncilPublicCouncilRole] FOREIGN KEY([PublicCouncilRoleID])
REFERENCES [inst_nom].[PublicCouncilRole] ([PublicCouncilRoleID])
GO
ALTER TABLE [inst_basic].[InstitutionPublicCouncil] CHECK CONSTRAINT [FK_InstitutionPublicCouncilPublicCouncilRole]
GO
ALTER TABLE [inst_basic].[InstitutionPublicCouncil]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionPublicCouncilPublicCouncilType] FOREIGN KEY([PublicCouncilTypeID])
REFERENCES [inst_nom].[PublicCouncilType] ([PublicCouncilTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionPublicCouncil] CHECK CONSTRAINT [FK_InstitutionPublicCouncilPublicCouncilType]
GO
ALTER TABLE [inst_basic].[InstitutionSchoolBoard]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolBoardSchoolBoardRole] FOREIGN KEY([SchoolBoardRoleID])
REFERENCES [inst_nom].[SchoolBoardRole] ([SchoolBoardRoleID])
GO
ALTER TABLE [inst_basic].[InstitutionSchoolBoard] CHECK CONSTRAINT [FK_InstitutionSchoolBoardSchoolBoardRole]
GO
ALTER TABLE [inst_basic].[InstitutionSchoolBoard]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolBoardSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionSchoolBoard] CHECK CONSTRAINT [FK_InstitutionSchoolBoardSysUser]
GO
ALTER TABLE [inst_basic].[InstitutionVehicle]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionVehicleAcqType] FOREIGN KEY([AcquisitionWayTypeID])
REFERENCES [inst_nom].[AcquisitionWayType] ([AcquisitionWayTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionVehicle] CHECK CONSTRAINT [FK_InstitutionVehicleAcqType]
GO
ALTER TABLE [inst_basic].[InstitutionVehicle]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionVehicleDocumentType] FOREIGN KEY([DocumentTypeID])
REFERENCES [inst_nom].[VehicleDocumentType] ([VehicleDocumentTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionVehicle] CHECK CONSTRAINT [FK_InstitutionVehicleDocumentType]
GO
ALTER TABLE [inst_basic].[InstitutionVehicle]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionVehicleInstitution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [inst_basic].[InstitutionVehicle] CHECK CONSTRAINT [FK_InstitutionVehicleInstitution]
GO
ALTER TABLE [inst_basic].[InstitutionVehicle]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionVehiclePlaceCountType] FOREIGN KEY([PlaceCountTypeID])
REFERENCES [inst_nom].[PlaceCountType] ([PlaceCountTypeID])
GO
ALTER TABLE [inst_basic].[InstitutionVehicle] CHECK CONSTRAINT [FK_InstitutionVehiclePlaceCountType]
GO
ALTER TABLE [inst_basic].[InstitutionVehicle]  WITH CHECK ADD  CONSTRAINT [FK_InstVehicle_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[InstitutionVehicle] CHECK CONSTRAINT [FK_InstVehicle_SysUser]
GO
ALTER TABLE [inst_basic].[PersonCompSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonCompSkillCompSkill] FOREIGN KEY([CompSkillID])
REFERENCES [inst_nom].[CompSkill] ([CompSkillID])
GO
ALTER TABLE [inst_basic].[PersonCompSkill] CHECK CONSTRAINT [FK_PersonCompSkillCompSkill]
GO
ALTER TABLE [inst_basic].[PersonCompSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonCompSkillCompSkillLevel] FOREIGN KEY([CompSkillLevelID])
REFERENCES [inst_nom].[CompSkillLevel] ([CompSkillLevelID])
GO
ALTER TABLE [inst_basic].[PersonCompSkill] CHECK CONSTRAINT [FK_PersonCompSkillCompSkillLevel]
GO
ALTER TABLE [inst_basic].[PersonCompSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonCompSkillPerson] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [inst_basic].[PersonCompSkill] CHECK CONSTRAINT [FK_PersonCompSkillPerson]
GO
ALTER TABLE [inst_basic].[PersonCompSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonCompSkillSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[PersonCompSkill] CHECK CONSTRAINT [FK_PersonCompSkillSysUser]
GO
ALTER TABLE [inst_basic].[PersonDetail]  WITH CHECK ADD  CONSTRAINT [FK_PersonDetailPerson] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [inst_basic].[PersonDetail] CHECK CONSTRAINT [FK_PersonDetailPerson]
GO
ALTER TABLE [inst_basic].[PersonDetail]  WITH CHECK ADD  CONSTRAINT [FK_PersonDetailSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[PersonDetail] CHECK CONSTRAINT [FK_PersonDetailSysUser]
GO
ALTER TABLE [inst_basic].[PersonFLSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonFLSkillFL] FOREIGN KEY([FLID])
REFERENCES [inst_nom].[FL] ([FLID])
GO
ALTER TABLE [inst_basic].[PersonFLSkill] CHECK CONSTRAINT [FK_PersonFLSkillFL]
GO
ALTER TABLE [inst_basic].[PersonFLSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonFLSkillFLLevel] FOREIGN KEY([FLLevelID])
REFERENCES [inst_nom].[FLLevel] ([FLLevelID])
GO
ALTER TABLE [inst_basic].[PersonFLSkill] CHECK CONSTRAINT [FK_PersonFLSkillFLLevel]
GO
ALTER TABLE [inst_basic].[PersonFLSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonFLSkillPerson] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [inst_basic].[PersonFLSkill] CHECK CONSTRAINT [FK_PersonFLSkillPerson]
GO
ALTER TABLE [inst_basic].[PersonFLSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonFLSkillSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[PersonFLSkill] CHECK CONSTRAINT [FK_PersonFLSkillSysUser]
GO
ALTER TABLE [inst_basic].[PersonOKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonOKSEducationGradeType] FOREIGN KEY([EducationGradeTypeID])
REFERENCES [inst_nom].[EducationGradeType] ([EducationGradeTypeID])
GO
ALTER TABLE [inst_basic].[PersonOKS] CHECK CONSTRAINT [FK_PersonOKSEducationGradeType]
GO
ALTER TABLE [inst_basic].[PersonOKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonOKSPerson] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [inst_basic].[PersonOKS] CHECK CONSTRAINT [FK_PersonOKSPerson]
GO
ALTER TABLE [inst_basic].[PersonOKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonOKSSpecialityOrdType] FOREIGN KEY([SpecialityOrdTypeID])
REFERENCES [inst_nom].[SpecialityOrdType] ([SpecialityOrdTypeID])
GO
ALTER TABLE [inst_basic].[PersonOKS] CHECK CONSTRAINT [FK_PersonOKSSpecialityOrdType]
GO
ALTER TABLE [inst_basic].[PersonOKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonOKSSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[PersonOKS] CHECK CONSTRAINT [FK_PersonOKSSysUser]
GO
ALTER TABLE [inst_basic].[PersonOKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonOKSUniversity] FOREIGN KEY([UniversityID])
REFERENCES [inst_nom].[University] ([UniversityID])
GO
ALTER TABLE [inst_basic].[PersonOKS] CHECK CONSTRAINT [FK_PersonOKSUniversity]
GO
ALTER TABLE [inst_basic].[PersonOKSSubjectGroup]  WITH CHECK ADD  CONSTRAINT [FK_PersonOKSSubjectGroupPersonOKS] FOREIGN KEY([PersonOKSID])
REFERENCES [inst_basic].[PersonOKS] ([PersonOKSID])
GO
ALTER TABLE [inst_basic].[PersonOKSSubjectGroup] CHECK CONSTRAINT [FK_PersonOKSSubjectGroupPersonOKS]
GO
ALTER TABLE [inst_basic].[PersonOKSSubjectGroup]  WITH CHECK ADD  CONSTRAINT [FK_PersonOKSSubjectGroupSubjectGroup] FOREIGN KEY([SubjectGroupID])
REFERENCES [inst_nom].[SubjectGroup] ([SubjectGroupID])
GO
ALTER TABLE [inst_basic].[PersonOKSSubjectGroup] CHECK CONSTRAINT [FK_PersonOKSSubjectGroupSubjectGroup]
GO
ALTER TABLE [inst_basic].[PersonOKSSubjectGroup]  WITH CHECK ADD  CONSTRAINT [FK_PersonOKSSubjectGroupSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[PersonOKSSubjectGroup] CHECK CONSTRAINT [FK_PersonOKSSubjectGroupSysUser]
GO
ALTER TABLE [inst_basic].[PersonPKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonPKSPerson] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [inst_basic].[PersonPKS] CHECK CONSTRAINT [FK_PersonPKSPerson]
GO
ALTER TABLE [inst_basic].[PersonPKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonPKSPKSType] FOREIGN KEY([PKSTypeID])
REFERENCES [inst_nom].[PKSType] ([PKSTypeID])
GO
ALTER TABLE [inst_basic].[PersonPKS] CHECK CONSTRAINT [FK_PersonPKSPKSType]
GO
ALTER TABLE [inst_basic].[PersonPKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonPKSSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[PersonPKS] CHECK CONSTRAINT [FK_PersonPKSSysUser]
GO
ALTER TABLE [inst_basic].[PersonPKS]  WITH CHECK ADD  CONSTRAINT [FK_PersonPKSUniversity] FOREIGN KEY([UniversityID])
REFERENCES [inst_nom].[University] ([UniversityID])
GO
ALTER TABLE [inst_basic].[PersonPKS] CHECK CONSTRAINT [FK_PersonPKSUniversity]
GO
ALTER TABLE [inst_basic].[PersonQCourse]  WITH CHECK ADD  CONSTRAINT [FK_PersonQCoursePerson] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [inst_basic].[PersonQCourse] CHECK CONSTRAINT [FK_PersonQCoursePerson]
GO
ALTER TABLE [inst_basic].[PersonQCourse]  WITH CHECK ADD  CONSTRAINT [FK_PersonQCourseQCourseActType] FOREIGN KEY([QCourseActTypeID])
REFERENCES [inst_nom].[QCourseActType] ([QCourseActTypeID])
GO
ALTER TABLE [inst_basic].[PersonQCourse] CHECK CONSTRAINT [FK_PersonQCourseQCourseActType]
GO
ALTER TABLE [inst_basic].[PersonQCourse]  WITH CHECK ADD  CONSTRAINT [FK_PersonQCourseQCourseBudgetSourceType] FOREIGN KEY([QCourseBudgetSourceTypeID])
REFERENCES [inst_nom].[QCourseBudgetSourceType] ([QCourseBudgetSourceTypeID])
GO
ALTER TABLE [inst_basic].[PersonQCourse] CHECK CONSTRAINT [FK_PersonQCourseQCourseBudgetSourceType]
GO
ALTER TABLE [inst_basic].[PersonQCourse]  WITH CHECK ADD  CONSTRAINT [FK_PersonQCourseQCourseDurationType] FOREIGN KEY([QCourseDurationTypeID])
REFERENCES [inst_nom].[QCourseDurationType] ([QCourseDurationTypeID])
GO
ALTER TABLE [inst_basic].[PersonQCourse] CHECK CONSTRAINT [FK_PersonQCourseQCourseDurationType]
GO
ALTER TABLE [inst_basic].[PersonQCourse]  WITH CHECK ADD  CONSTRAINT [FK_PersonQCourseQCourseType] FOREIGN KEY([QCourseTypeID])
REFERENCES [inst_nom].[QCourseType] ([QCourseTypeID])
GO
ALTER TABLE [inst_basic].[PersonQCourse] CHECK CONSTRAINT [FK_PersonQCourseQCourseType]
GO
ALTER TABLE [inst_basic].[PersonQCourse]  WITH CHECK ADD  CONSTRAINT [FK_PersonQCourseSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[PersonQCourse] CHECK CONSTRAINT [FK_PersonQCourseSysUser]
GO
ALTER TABLE [inst_basic].[PersonQCourse]  WITH CHECK ADD  CONSTRAINT [FK_PersonQCourseUniversity] FOREIGN KEY([UniversityID])
REFERENCES [inst_nom].[University] ([UniversityID])
GO
ALTER TABLE [inst_basic].[PersonQCourse] CHECK CONSTRAINT [FK_PersonQCourseUniversity]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPerson] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPerson]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionCategoryStaffType] FOREIGN KEY([CategoryStaffTypeID])
REFERENCES [inst_nom].[CategoryStaffType] ([CategoryStaffTypeID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionCategoryStaffType]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionClassGroup] FOREIGN KEY([MainClassID])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionClassGroup]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionContractReason] FOREIGN KEY([ContractReasonID])
REFERENCES [inst_nom].[ContractReason] ([ContractReasonID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionContractReason]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionContractType] FOREIGN KEY([ContractTypeID])
REFERENCES [inst_nom].[ContractType] ([ContractTypeID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionContractType]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionContractWith] FOREIGN KEY([ContractWithID])
REFERENCES [inst_nom].[ContractWith] ([ContractWithID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionContractWith]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionInstitution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionInstitution]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionNKPDPosition] FOREIGN KEY([NKPDPositionID])
REFERENCES [inst_nom].[NKPDPosition] ([NKPDPositionID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionNKPDPosition]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionPerson] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionPerson]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionPersonOKS] FOREIGN KEY([MainPersonOKSID])
REFERENCES [inst_basic].[PersonOKS] ([PersonOKSID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionPersonOKS]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionPositionKind] FOREIGN KEY([PositionKindID])
REFERENCES [inst_nom].[PositionKind] ([PositionKindID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionPositionKind]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionStaffType] FOREIGN KEY([StaffTypeID])
REFERENCES [inst_nom].[StaffType] ([StaffTypeID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionStaffType]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionSubjectGroup] FOREIGN KEY([PositionSubjectGroupID])
REFERENCES [inst_nom].[SubjectGroup] ([SubjectGroupID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionSubjectGroup]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionSysUser]
GO
ALTER TABLE [inst_basic].[StaffPosition]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionTerminationReason] FOREIGN KEY([TerminationReasonID])
REFERENCES [inst_nom].[TerminationReason] ([TerminationReasonID])
GO
ALTER TABLE [inst_basic].[StaffPosition] CHECK CONSTRAINT [FK_StaffPositionTerminationReason]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 - неналети в EducationalState, 2 - налети' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'changeYearEduStateTemp', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1- ъпдейт, 2 - делийт' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'changeYearEduStateTemp', @level2type=N'COLUMN',@level2name=N'OperationTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 - старт, 2 - край' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'changeYearLogTemp', @level2type=N'COLUMN',@level2name=N'OperationID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'InstitutionDepartmentID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименование на другия адрес н' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на държава' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'CountryID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на населено място' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'TownID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на район' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'LocalAreaID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Адрес' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'Address'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пощенски код' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'PostCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Основен адрес"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'IsMain'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Активен адрес' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment', @level2type=N'COLUMN',@level2name=N'IsValid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Други адреси за осъществяване ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDepartment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Електронна поща' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'Email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Интернет адрес' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'Website'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Година на основаване' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'EstablishedYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Акт за учредяване или преобраз' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'ConstitActFirst'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Акт за учредяване или собствен' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'ConstitActLast'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Към ДГ има яслена груп' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'IsODZ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Осигурява професионалн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'IsProfSchool'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "С национално значение"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'IsNational'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Oрганизира допълнителн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'IsProvideEduServ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "На делегиран бюджет"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'IsDelegateBudget'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Към училището има неса' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'IsNonIndDormitory'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'не се използва' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'IsInternContract'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Банкова сметка на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'BankIBAN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на банката' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'BankBIC'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименование на банката' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'BankName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Титуляр на банковата сметка' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'BankAccountHolder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "ДГ прилага иновативна ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail', @level2type=N'COLUMN',@level2name=N'IsAppInnovSystem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Допълнителни данни за институц' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionDetail'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionInnovation', @level2type=N'COLUMN',@level2name=N'InstitutionInnovationID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionInnovation', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид иновация' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionInnovation', @level2type=N'COLUMN',@level2name=N'InnovationTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Коментар' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionInnovation', @level2type=N'COLUMN',@level2name=N'Notes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionInnovation', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionInnovation', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionInnovation', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Иновации' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionInnovation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'InstitutionPhoneID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на адреса на провеждане на' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'DepartmentID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Тип телефон' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'PhoneTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код за АТИ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'PhoneCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Телефонен номер' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'PhoneNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Контакт' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'ContactKind'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Телефон на институцият' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'IsInstitution'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Основен за институцият' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'IsMain'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Телефони' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPhone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'InstitutionProjectID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид на проект' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ProjectTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид програма' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ProjectProgramTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименование' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Резюме (описание) на проекта' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Сайт на проекта' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'Website'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Начална дата на проекта' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'StartDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Крайна дата на проекта' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'EndDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца  (планиран' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'PlanPreSchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца/ученици (п' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'PlanPrimarySchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца/ученици (п' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'PlanSecondarySchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца/ученици (п' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'PlanHighSchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца/ученици (п' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'PlanCountAll'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца (реализира' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ActualPreSchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца/ученици (р' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ActualPrimarySchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца/ученици (р' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ActualSecondarySchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца/ученици (р' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ActualHighSchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'брой обхванати деца/ученици (р' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ActualCountAll'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'в т.ч. обхванати в международн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'InterPreSchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'в т.ч. обхванати в международн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'InterPrimarySchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'в т.ч. обхванати в международн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'InterSecondarySchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'в т.ч. обхванати в международн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'InterHighSchoolCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'в т.ч. обхванати в международн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'InterCountAll'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Цели и резултати на проекта' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'Goals'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Проектът е архивиран"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'IsArchive'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Проекти' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProject'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'InstitutionProjectPartnerID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид партньор' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'ProjectPartnerTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на прое' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'InstitutionProjectID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Булстат на организацията - пар' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'Eik'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Име на организацията - партньо' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Координатор"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'IsCoordinator'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Проекти - Партньори' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPartner'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPriorityArea', @level2type=N'COLUMN',@level2name=N'InstitutionProjectPriorityAreaID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на прое' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPriorityArea', @level2type=N'COLUMN',@level2name=N'ProjectID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Приоритетна област' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPriorityArea', @level2type=N'COLUMN',@level2name=N'ProjectPriorityAreaTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPriorityArea', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPriorityArea', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPriorityArea', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Проекти - Приоритетни области' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionProjectPriorityArea'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'InstitutionPublicCouncilID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Име' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'FirstName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Презиме' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'MiddleName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Фамилия' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'FamilyName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Институция, на която е предста' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'PublicCouncilTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Роля' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'PublicCouncilRoleID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Електронна поща' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'Email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Телефонен номер' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'PhoneNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Обществен съвет' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionPublicCouncil'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'InstitutionVehicleID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Начин на придобиване' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'AcquisitionWayTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид на документа за придобиван' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'DocumentTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Номер на документа' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'DocumentNum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата на документа' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'DocumentDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Регистрационен номер' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'RegistrationNum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Година на производство' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'ProducedYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой места' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'PlaceCountTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Автобуси' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'InstitutionVehicle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill', @level2type=N'COLUMN',@level2name=N'PersonCompSkillID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към човек' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Компютърно умение' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill', @level2type=N'COLUMN',@level2name=N'CompSkillID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ниво' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill', @level2type=N'COLUMN',@level2name=N'CompSkillLevelID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill', @level2type=N'COLUMN',@level2name=N'Notes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Персонал - Компютърни умения' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonCompSkill'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'PersonDetailID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към човек' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Титла' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Телефонен номер' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'PhoneNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Продължава образоаниет' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'IsExtendStudent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Пенсионер"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'IsPensioneer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Допълнителни данни за човек' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonDetail'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill', @level2type=N'COLUMN',@level2name=N'PersonFLSkillID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към човек' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Чужд език' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill', @level2type=N'COLUMN',@level2name=N'FLID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ниво на владеене' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill', @level2type=N'COLUMN',@level2name=N'FLLevelID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill', @level2type=N'COLUMN',@level2name=N'Notes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Персонал - Чуждоезикови умения' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonFLSkill'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'PersonOKSID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към човек' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Образователна степен' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'EducationGradeTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Поредна специалност' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'SpecialityOrdTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Образователната институция, в ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'UniversityID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'UniversityNotes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Номер на документа' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'CertifcateNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Година на завършване' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'YearOfGraduation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименование на специалността' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'Speciality'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Професионална квалификация' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'AcquiredPK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "ПК учител"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'IsPKTeacher'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Персонал - Образование' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKSSubjectGroup', @level2type=N'COLUMN',@level2name=N'PersonOKSSubjectGroupID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към образование' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKSSubjectGroup', @level2type=N'COLUMN',@level2name=N'PersonOKSID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към група предмети' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKSSubjectGroup', @level2type=N'COLUMN',@level2name=N'SubjectGroupID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKSSubjectGroup', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKSSubjectGroup', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKSSubjectGroup', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Персонал - Образование - Учите' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonOKSSubjectGroup'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'PersonPKSID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към човек' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ПКС' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'PKSTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Образователната институция, в ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'UniversityID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'UniversityNotes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Номер на документа' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'CertifcateNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Година на завършване' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'YearOfGraduation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименование на специалността' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'Speciality'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N' Персонал - ПКС' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonPKS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'PersonQCourseID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към човек' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Тип курс' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseActTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Година на завършване' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид на курса' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseTypeNotes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Източник на финансиране/Органи' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseBudgetSourceTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Цена на курса' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'InternalCoursePrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Образователната институция, в ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'UniversityID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'UniversityNotes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Тема на курса' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseTopic'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Продължителност на курса' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseDurationTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой кредити' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseDurationCredits'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой часове' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'QCourseDurationHours'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Документ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'DocumentType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Персонал - Квалификационни кур' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'PersonQCourse'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'StaffPositionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пореден номер' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'StaffOrd'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към човек' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Година на постъпване' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'WorkStartYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Общ трудов стаж (г.)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'WorkExpTotalYears'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Трудов стаж по специалността (' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'WorkExpSpecYears'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учителски трудов стаж (г.)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'WorkExpTeachYears'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Пътуващ"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'IsTravel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пореден номер на длъжността' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'StaffPositionNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид на договора' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'ContractTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Основание за сключване' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'ContractReasonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Номер на договора' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'ContractNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Година на договора' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'ContractYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки по договора' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'ContractNotes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Назначен към:' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'ContractWithID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Материално-отговорно л' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'IsAccountablePerson'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Категория персонал (обобщена)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'StaffTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Категория персонал (детайлна)' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'CategoryStaffTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код по НКПД на длъжността' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'NKPDPositionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Титуляр/заместник' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'PositionKindID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки по длъжността' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'PositionNotes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Щат' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'PositionCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Основен етап, в който преподав' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'TeachStageID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Mаркер "Болничен учител"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'IsHospital'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Mаркер "Учител - наставник"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'IsMentor'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Mаркер "Учител - стажант"' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'IsTrainee'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Не отговаря на изисква' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'IsNotMeetReq'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Длъжността е активна з' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'CurrentlyValid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Основна специалност, по която ' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'MainPersonOKSID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Прилагат се условията по чл.4,' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'HasTELK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Назначен на щатно място по' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition', @level2type=N'COLUMN',@level2name=N'PositionSubjectGroupID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Персонал - Длъжности' , @level0type=N'SCHEMA',@level0name=N'inst_basic', @level1type=N'TABLE',@level1name=N'StaffPosition'
GO
