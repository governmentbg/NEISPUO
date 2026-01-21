ALTER TABLE [core].[ExtSystemServices] ADD  CONSTRAINT [DF_ExtSystemServices_IsReturnArray]  DEFAULT ((1)) FOR [IsReturnArray]
GO
ALTER TABLE [core].[ExtSystemServices] ADD  CONSTRAINT [DF_ExtSystemServices_IsValid]  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [core].[InstitutionConfData] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [core].[InstitutionConfData] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [core].[InstitutionExtSystem] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [core].[InstitutionExtSystem] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [core].[InstitutionSchoolYear] ADD  CONSTRAINT [DF_InstitutionSchoolYear_IsFinalized]  DEFAULT ((0)) FOR [IsFinalized]
GO
ALTER TABLE [core].[InstitutionSchoolYear] ADD  CONSTRAINT [DF_InstitutionSchoolYear_IsCurrent]  DEFAULT ((0)) FOR [IsCurrent]
GO
ALTER TABLE [core].[InstitutionSchoolYear] ADD  CONSTRAINT [DF_InstitutionSchoolYear_ValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [core].[InstitutionSchoolYear] ADD  CONSTRAINT [DF_InstitutionSchoolYear_ValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [core].[ParentChildSchoolBookAccess] ADD  DEFAULT ((0)) FOR [HasAccess]
GO
ALTER TABLE [core].[Person] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [core].[Person] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [core].[Person] ADD  DEFAULT (NULL) FOR [SysUserType]
GO
ALTER TABLE [core].[Position] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [core].[Position] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [core].[SystemUserMessage] ADD  DEFAULT (sysutcdatetime()) FOR [StartDate]
GO
ALTER TABLE [core].[SystemUserMessage] ADD  DEFAULT (sysutcdatetime()) FOR [EndDate]
GO
ALTER TABLE [core].[SysUser] ADD  DEFAULT ('FALSE') FOR [IsAzureUser]
GO
ALTER TABLE [core].[SysUser] ADD  DEFAULT ((-1)) FOR [PersonID]
GO
ALTER TABLE [core].[SysUser] ADD  CONSTRAINT [isAzureSyncedNotNull]  DEFAULT ((0)) FOR [isAzureSynced]
GO
ALTER TABLE [core].[SysUser] ADD  CONSTRAINT [InitialPasswordNull]  DEFAULT (NULL) FOR [InitialPassword]
GO
ALTER TABLE [core].[SysUser] ADD  CONSTRAINT [DeletedOnSysUserDefault]  DEFAULT (NULL) FOR [DeletedOn]
GO
ALTER TABLE [noms].[BaseSchoolType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [noms].[BaseSchoolType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [noms].[BudgetingInstitution] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [noms].[BudgetingInstitution] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [noms].[Currency] ADD  CONSTRAINT [DF_Currency_IsMain]  DEFAULT ((0)) FOR [IsMain]
GO
ALTER TABLE [noms].[Currency] ADD  CONSTRAINT [DF_Currency_IsValid]  DEFAULT ((0)) FOR [IsValid]
GO
ALTER TABLE [noms].[Currency] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [noms].[DetailedSchoolType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [noms].[DetailedSchoolType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [noms].[FinancialSchoolType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [noms].[FinancialSchoolType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [noms].[Gender] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [noms].[Gender] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [noms].[PersonalIDType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [noms].[PersonalIDType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [family].[EducationType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [family].[EducationType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [family].[Relative] ADD  CONSTRAINT [DF_RelativeValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [family].[Relative] ADD  CONSTRAINT [DF_RelativeValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [core].[Certificate]  WITH CHECK ADD  CONSTRAINT [FK_Certificate_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [core].[Certificate] CHECK CONSTRAINT [FK_Certificate_Creator_SysUser]
GO
ALTER TABLE [core].[Certificate]  WITH CHECK ADD  CONSTRAINT [FK_Certificate_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [core].[Certificate] CHECK CONSTRAINT [FK_Certificate_Updater_SysUser]
GO
ALTER TABLE [core].[EducationalState]  WITH CHECK ADD  CONSTRAINT [FK_EducationalState_Institution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [core].[EducationalState] CHECK CONSTRAINT [FK_EducationalState_Institution]
GO
ALTER TABLE [core].[EducationalState]  WITH CHECK ADD  CONSTRAINT [FK_EducationalState_Person] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [core].[EducationalState] CHECK CONSTRAINT [FK_EducationalState_Person]
GO
ALTER TABLE [core].[EducationalState]  WITH CHECK ADD  CONSTRAINT [FK_EducationalState_Position] FOREIGN KEY([PositionID])
REFERENCES [core].[Position] ([PositionID])
GO
ALTER TABLE [core].[EducationalState] CHECK CONSTRAINT [FK_EducationalState_Position]
GO
ALTER TABLE [core].[ExtSystem]  WITH CHECK ADD  CONSTRAINT [FK_ExtSystem_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [core].[ExtSystem] CHECK CONSTRAINT [FK_ExtSystem_SysUser]
GO
ALTER TABLE [core].[ExtSystemAccess]  WITH CHECK ADD  CONSTRAINT [FK_ExtSystemAccess] FOREIGN KEY([ExtSystemID])
REFERENCES [core].[ExtSystem] ([ExtSystemID])
GO
ALTER TABLE [core].[ExtSystemAccess] CHECK CONSTRAINT [FK_ExtSystemAccess]
GO
ALTER TABLE [core].[ExtSystemAccess]  WITH CHECK ADD  CONSTRAINT [FK_ExtSystemType] FOREIGN KEY([ExtSystemType])
REFERENCES [noms].[ExtSystemType] ([ExtSystemTypeID])
GO
ALTER TABLE [core].[ExtSystemAccess] CHECK CONSTRAINT [FK_ExtSystemType]
GO
ALTER TABLE [core].[ExtSystemCertificate]  WITH CHECK ADD  CONSTRAINT [FK_ExtSystemCertificate] FOREIGN KEY([ExtSystemID])
REFERENCES [core].[ExtSystem] ([ExtSystemID])
GO
ALTER TABLE [core].[ExtSystemCertificate] CHECK CONSTRAINT [FK_ExtSystemCertificate]
GO
ALTER TABLE [core].[ExtSystemServicesMap]  WITH CHECK ADD  CONSTRAINT [FK_ExtSystem] FOREIGN KEY([ExtSystemID])
REFERENCES [core].[ExtSystem] ([ExtSystemID])
GO
ALTER TABLE [core].[ExtSystemServicesMap] CHECK CONSTRAINT [FK_ExtSystem]
GO
ALTER TABLE [core].[ExtSystemServicesMap]  WITH CHECK ADD  CONSTRAINT [FK_ExtSystemServiceMap] FOREIGN KEY([ExtServiceID])
REFERENCES [core].[ExtSystemServices] ([ExtServiceID])
GO
ALTER TABLE [core].[ExtSystemServicesMap] CHECK CONSTRAINT [FK_ExtSystemServiceMap]
GO
ALTER TABLE [core].[Institution]  WITH CHECK ADD  CONSTRAINT [FK_Institution_BaseSchoolType] FOREIGN KEY([BaseSchoolTypeID])
REFERENCES [noms].[BaseSchoolType] ([BaseSchoolTypeID])
GO
ALTER TABLE [core].[Institution] CHECK CONSTRAINT [FK_Institution_BaseSchoolType]
GO
ALTER TABLE [core].[Institution]  WITH CHECK ADD  CONSTRAINT [FK_Institution_BudgetingSchoolType] FOREIGN KEY([BudgetingSchoolTypeID])
REFERENCES [noms].[BudgetingInstitution] ([BudgetingInstitutionID])
GO
ALTER TABLE [core].[Institution] CHECK CONSTRAINT [FK_Institution_BudgetingSchoolType]
GO
ALTER TABLE [core].[Institution]  WITH CHECK ADD  CONSTRAINT [FK_Institution_Country] FOREIGN KEY([CountryID])
REFERENCES [location].[Country] ([CountryID])
GO
ALTER TABLE [core].[Institution] CHECK CONSTRAINT [FK_Institution_Country]
GO
ALTER TABLE [core].[Institution]  WITH CHECK ADD  CONSTRAINT [FK_Institution_DetailedSchoolType] FOREIGN KEY([DetailedSchoolTypeID])
REFERENCES [noms].[DetailedSchoolType] ([DetailedSchoolTypeID])
GO
ALTER TABLE [core].[Institution] CHECK CONSTRAINT [FK_Institution_DetailedSchoolType]
GO
ALTER TABLE [core].[Institution]  WITH CHECK ADD  CONSTRAINT [FK_Institution_FinancialSchoolType] FOREIGN KEY([FinancialSchoolTypeID])
REFERENCES [noms].[FinancialSchoolType] ([FinancialSchoolTypeID])
GO
ALTER TABLE [core].[Institution] CHECK CONSTRAINT [FK_Institution_FinancialSchoolType]
GO
ALTER TABLE [core].[Institution]  WITH CHECK ADD  CONSTRAINT [FK_Institution_LocalArea] FOREIGN KEY([LocalAreaID])
REFERENCES [location].[LocalArea] ([LocalAreaID])
GO
ALTER TABLE [core].[Institution] CHECK CONSTRAINT [FK_Institution_LocalArea]
GO
ALTER TABLE [core].[Institution]  WITH CHECK ADD  CONSTRAINT [FK_Institution_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [core].[Institution] CHECK CONSTRAINT [FK_Institution_SysUser]
GO
ALTER TABLE [core].[Institution]  WITH CHECK ADD  CONSTRAINT [FK_Institution_Town] FOREIGN KEY([TownID])
REFERENCES [location].[Town] ([TownID])
GO
ALTER TABLE [core].[Institution] CHECK CONSTRAINT [FK_Institution_Town]
GO
ALTER TABLE [core].[InstitutionConfData]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionConfData] FOREIGN KEY([SOExtProviderID])
REFERENCES [core].[ExtSystem] ([ExtSystemID])
GO
ALTER TABLE [core].[InstitutionConfData] CHECK CONSTRAINT [FK_InstitutionConfData]
GO
ALTER TABLE [core].[InstitutionConfData]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionConfDataCB] FOREIGN KEY([CBExtProviderID])
REFERENCES [core].[ExtSystem] ([ExtSystemID])
GO
ALTER TABLE [core].[InstitutionConfData] CHECK CONSTRAINT [FK_InstitutionConfDataCB]
GO
ALTER TABLE [core].[InstitutionConfData]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionConfDataInstitution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [core].[InstitutionConfData] CHECK CONSTRAINT [FK_InstitutionConfDataInstitution]
GO
ALTER TABLE [core].[InstitutionConfData]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionConfDataSchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [core].[InstitutionConfData] CHECK CONSTRAINT [FK_InstitutionConfDataSchoolYear]
GO
ALTER TABLE [core].[InstitutionConfData]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionConfDataSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [core].[InstitutionConfData] CHECK CONSTRAINT [FK_InstitutionConfDataSysUser]
GO
ALTER TABLE [core].[InstitutionExtSystem]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionExtSystem] FOREIGN KEY([ExtSystemID])
REFERENCES [core].[ExtSystem] ([ExtSystemID])
GO
ALTER TABLE [core].[InstitutionExtSystem] CHECK CONSTRAINT [FK_InstitutionExtSystem]
GO
ALTER TABLE [core].[InstitutionExtSystem]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionExtSystemInstitution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [core].[InstitutionExtSystem] CHECK CONSTRAINT [FK_InstitutionExtSystemInstitution]
GO
ALTER TABLE [core].[InstitutionExtSystem]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionExtSystemSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [core].[InstitutionExtSystem] CHECK CONSTRAINT [FK_InstitutionExtSystemSysUser]
GO
ALTER TABLE [core].[InstitutionExtSystem]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionExtSystemType] FOREIGN KEY([ExtSystemTypeID])
REFERENCES [noms].[ExtSystemType] ([ExtSystemTypeID])
GO
ALTER TABLE [core].[InstitutionExtSystem] CHECK CONSTRAINT [FK_InstitutionExtSystemType]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_BaseSchoolType] FOREIGN KEY([BaseSchoolTypeID])
REFERENCES [noms].[BaseSchoolType] ([BaseSchoolTypeID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_BaseSchoolType]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_BudgetingSchoolType] FOREIGN KEY([BudgetingSchoolTypeID])
REFERENCES [noms].[BudgetingInstitution] ([BudgetingInstitutionID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_BudgetingSchoolType]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_Country] FOREIGN KEY([CountryID])
REFERENCES [location].[Country] ([CountryID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_Country]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_CurrentYear]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_DetailedSchoolType] FOREIGN KEY([DetailedSchoolTypeID])
REFERENCES [noms].[DetailedSchoolType] ([DetailedSchoolTypeID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_DetailedSchoolType]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_FinancialSchoolType] FOREIGN KEY([FinancialSchoolTypeID])
REFERENCES [noms].[FinancialSchoolType] ([FinancialSchoolTypeID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_FinancialSchoolType]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_LocalArea] FOREIGN KEY([LocalAreaID])
REFERENCES [location].[LocalArea] ([LocalAreaID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_LocalArea]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_SysUser]
GO
ALTER TABLE [core].[InstitutionSchoolYear]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionSchoolYear_Town] FOREIGN KEY([TownID])
REFERENCES [location].[Town] ([TownID])
GO
ALTER TABLE [core].[InstitutionSchoolYear] CHECK CONSTRAINT [FK_InstitutionSchoolYear_Town]
GO
ALTER TABLE [core].[ParentChildSchoolBookAccess]  WITH CHECK ADD  CONSTRAINT [FK_ParentChildSchoolBookAccess_Child_Person] FOREIGN KEY([ChildID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [core].[ParentChildSchoolBookAccess] CHECK CONSTRAINT [FK_ParentChildSchoolBookAccess_Child_Person]
GO
ALTER TABLE [core].[ParentChildSchoolBookAccess]  WITH CHECK ADD  CONSTRAINT [FK_ParentChildSchoolBookAccess_Parent_Person] FOREIGN KEY([ParentID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [core].[ParentChildSchoolBookAccess] CHECK CONSTRAINT [FK_ParentChildSchoolBookAccess_Parent_Person]
GO
ALTER TABLE [core].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Country] FOREIGN KEY([NationalityID])
REFERENCES [location].[Country] ([CountryID])
GO
ALTER TABLE [core].[Person] CHECK CONSTRAINT [FK_Person_Country]
GO
ALTER TABLE [core].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Country_BirthPlace] FOREIGN KEY([BirthPlaceCountry])
REFERENCES [location].[Country] ([CountryID])
GO
ALTER TABLE [core].[Person] CHECK CONSTRAINT [FK_Person_Country_BirthPlace]
GO
ALTER TABLE [core].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Gender] FOREIGN KEY([Gender])
REFERENCES [noms].[Gender] ([GenderID])
GO
ALTER TABLE [core].[Person] CHECK CONSTRAINT [FK_Person_Gender]
GO
ALTER TABLE [core].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_PersonalIDType] FOREIGN KEY([PersonalIDType])
REFERENCES [noms].[PersonalIDType] ([PersonalIDTypeID])
GO
ALTER TABLE [core].[Person] CHECK CONSTRAINT [FK_Person_PersonalIDType]
GO
ALTER TABLE [core].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Town_BirthPlace] FOREIGN KEY([BirthPlaceTownID])
REFERENCES [location].[Town] ([TownID])
GO
ALTER TABLE [core].[Person] CHECK CONSTRAINT [FK_Person_Town_BirthPlace]
GO
ALTER TABLE [core].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Town_Current] FOREIGN KEY([CurrentTownID])
REFERENCES [location].[Town] ([TownID])
GO
ALTER TABLE [core].[Person] CHECK CONSTRAINT [FK_Person_Town_Current]
GO
ALTER TABLE [core].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Town_Permanent] FOREIGN KEY([PermanentTownID])
REFERENCES [location].[Town] ([TownID])
GO
ALTER TABLE [core].[Person] CHECK CONSTRAINT [FK_Person_Town_Permanent]
GO
ALTER TABLE [core].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_SysRole] FOREIGN KEY([SysRoleID])
REFERENCES [core].[SysRole] ([SysRoleID])
GO
ALTER TABLE [core].[Position] CHECK CONSTRAINT [FK_Position_SysRole]
GO
ALTER TABLE [core].[RelativeChild]  WITH CHECK ADD  CONSTRAINT [FK_RelativeChild_RelativeType] FOREIGN KEY([RelativeTypeID])
REFERENCES [family].[RelativeType] ([RelativeTypeID])
GO
ALTER TABLE [core].[RelativeChild] CHECK CONSTRAINT [FK_RelativeChild_RelativeType]
GO
ALTER TABLE [core].[SysUser]  WITH CHECK ADD  CONSTRAINT [FK_SysUser_Person] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [core].[SysUser] CHECK CONSTRAINT [FK_SysUser_Person]
GO
ALTER TABLE [core].[SysUserSysRole]  WITH CHECK ADD  CONSTRAINT [FK_SysUserSysRole_BudgetingInstitution] FOREIGN KEY([BudgetingInstitutionID])
REFERENCES [noms].[BudgetingInstitution] ([BudgetingInstitutionID])
GO
ALTER TABLE [core].[SysUserSysRole] CHECK CONSTRAINT [FK_SysUserSysRole_BudgetingInstitution]
GO
ALTER TABLE [core].[SysUserSysRole]  WITH CHECK ADD  CONSTRAINT [FK_SysUserSysRole_Institution] FOREIGN KEY([InstitutionID])
REFERENCES [core].[Institution] ([InstitutionID])
GO
ALTER TABLE [core].[SysUserSysRole] CHECK CONSTRAINT [FK_SysUserSysRole_Institution]
GO
ALTER TABLE [core].[SysUserSysRole]  WITH CHECK ADD  CONSTRAINT [FK_SysUserSysRole_Municipality] FOREIGN KEY([MunicipalityID])
REFERENCES [location].[Municipality] ([MunicipalityID])
GO
ALTER TABLE [core].[SysUserSysRole] CHECK CONSTRAINT [FK_SysUserSysRole_Municipality]
GO
ALTER TABLE [core].[SysUserSysRole]  WITH CHECK ADD  CONSTRAINT [FK_SysUserSysRole_Region] FOREIGN KEY([RegionID])
REFERENCES [location].[Region] ([RegionID])
GO
ALTER TABLE [core].[SysUserSysRole] CHECK CONSTRAINT [FK_SysUserSysRole_Region]
GO
ALTER TABLE [core].[SysUserSysRole]  WITH CHECK ADD  CONSTRAINT [FK_SysUserSysRole_SysRole] FOREIGN KEY([SysRoleID])
REFERENCES [core].[SysRole] ([SysRoleID])
GO
ALTER TABLE [core].[SysUserSysRole] CHECK CONSTRAINT [FK_SysUserSysRole_SysRole]
GO
ALTER TABLE [core].[SysUserSysRole]  WITH CHECK ADD  CONSTRAINT [FK_SysUserSysRole_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [core].[SysUserSysRole] CHECK CONSTRAINT [FK_SysUserSysRole_SysUser]
GO
ALTER TABLE [noms].[DetailedSchoolType]  WITH CHECK ADD  CONSTRAINT [FK_DetailedSchoolType_BaseSchoolType] FOREIGN KEY([BaseSchoolTypeID])
REFERENCES [noms].[BaseSchoolType] ([BaseSchoolTypeID])
GO
ALTER TABLE [noms].[DetailedSchoolType] CHECK CONSTRAINT [FK_DetailedSchoolType_BaseSchoolType]
GO
ALTER TABLE [location].[LocalArea]  WITH CHECK ADD  CONSTRAINT [FK_LocalArea_Town] FOREIGN KEY([TownCode])
REFERENCES [location].[Town] ([TownID])
GO
ALTER TABLE [location].[LocalArea] CHECK CONSTRAINT [FK_LocalArea_Town]
GO
ALTER TABLE [location].[Municipality]  WITH CHECK ADD  CONSTRAINT [FK_Municipality_Region] FOREIGN KEY([RegionID])
REFERENCES [location].[Region] ([RegionID])
GO
ALTER TABLE [location].[Municipality] CHECK CONSTRAINT [FK_Municipality_Region]
GO
ALTER TABLE [location].[Town]  WITH CHECK ADD  CONSTRAINT [FK_Town_Municipality] FOREIGN KEY([MunicipalityID])
REFERENCES [location].[Municipality] ([MunicipalityID])
GO
ALTER TABLE [location].[Town] CHECK CONSTRAINT [FK_Town_Municipality]
GO
ALTER TABLE [family].[Relative]  WITH CHECK ADD  CONSTRAINT [FK_Relative_EducationType] FOREIGN KEY([EducationTypeId])
REFERENCES [family].[EducationType] ([Id])
GO
ALTER TABLE [family].[Relative] CHECK CONSTRAINT [FK_Relative_EducationType]
GO
ALTER TABLE [family].[Relative]  WITH CHECK ADD  CONSTRAINT [FK_Relative_Person] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [family].[Relative] CHECK CONSTRAINT [FK_Relative_Person]
GO
ALTER TABLE [family].[Relative]  WITH CHECK ADD  CONSTRAINT [FK_Relative_PersonalIDType] FOREIGN KEY([PersonalIDType])
REFERENCES [noms].[PersonalIDType] ([PersonalIDTypeID])
GO
ALTER TABLE [family].[Relative] CHECK CONSTRAINT [FK_Relative_PersonalIDType]
GO
ALTER TABLE [family].[Relative]  WITH CHECK ADD  CONSTRAINT [FK_Relative_RelativeType] FOREIGN KEY([RelativeTypeID])
REFERENCES [family].[RelativeType] ([RelativeTypeID])
GO
ALTER TABLE [family].[Relative] CHECK CONSTRAINT [FK_Relative_RelativeType]
GO
ALTER TABLE [family].[Relative]  WITH CHECK ADD  CONSTRAINT [FK_Relative_WorkStatus] FOREIGN KEY([WorkStatusID])
REFERENCES [family].[WorkStatus] ([WorkStatusID])
GO
ALTER TABLE [family].[Relative] CHECK CONSTRAINT [FK_Relative_WorkStatus]
GO
ALTER TABLE [core].[Person]  WITH CHECK ADD  CONSTRAINT [ck_unique_person] CHECK  (([core].[fn_check_unique_person]([FirstName],[MiddleName],[LastName],[BirthDate],[PersonalIdType],[SysUserType],[ValidFrom])=(0)))
GO
ALTER TABLE [core].[Person] CHECK CONSTRAINT [ck_unique_person]
GO
ALTER TABLE [core].[SysUserSysRole]  WITH NOCHECK ADD  CONSTRAINT [ValidateConditionalNotNulls] CHECK  ((NOT ([SysRoleId]=(8) OR [SysRoleId]=(6) OR [SysRoleId]=(5)) AND (((((case when ([SysRoleID]=(14) OR [SysRoleID]=(0) OR [SysRoleID]=(20)) AND [InstitutionID] IS NOT NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end+case when ([SysRoleID]=(22) OR [SysRoleID]=(21) OR [SysRoleID]=(19) OR [SysRoleID]=(18) OR [SysRoleID]=(17) OR [SysRoleID]=(16) OR [SysRoleID]=(15) OR [SysRoleID]=(14) OR [SysRoleID]=(13) OR [SysRoleID]=(12) OR [SysRoleID]=(11) OR [SysRoleID]=(10) OR [SysRoleID]=(1)) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)+case when ([SysRoleID]=(9) OR [SysRoleID]=(2)) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NOT NULL then (1) else (0) end)+case when [SysRoleID]=(3) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NOT NULL AND [RegionID] IS NULL then (1) else (0) end)+case when [SysRoleID]=(4) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NOT NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)+case when [SysRoleID]=(7) AND [InstitutionID] IS NULL AND [BudgetingInstitutionID] IS NULL AND [MunicipalityID] IS NULL AND [RegionID] IS NULL then (1) else (0) end)=(1)))
GO
ALTER TABLE [core].[SysUserSysRole] CHECK CONSTRAINT [ValidateConditionalNotNulls]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'EducationalState', @level2type=N'COLUMN',@level2name=N'EducationalStateID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка човек' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'EducationalState', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'EducationalState', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Позиция в образователната сист' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'EducationalState', @level2type=N'COLUMN',@level2name=N'PositionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Актуален статус в образователн' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'EducationalState'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пълно наименование' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Кратко наименование' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'Abbreviation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Булстат/ЕИК' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'Bulstat'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на държава' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'CountryID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на район (основен адрес)' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'LocalAreaID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Източник на финансиране' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'FinancialSchoolTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид по чл. 38 (детайлен)' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'DetailedSchoolTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид по чл. 35-36 (според собст' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'BudgetingSchoolTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на населеното място (основ' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'TownID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид по чл. 37 (общ, според вид' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution', @level2type=N'COLUMN',@level2name=N'BaseSchoolTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Данни за институциите' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Institution'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionConfData', @level2type=N'COLUMN',@level2name=N'InstitutionConfDataID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionConfData', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionConfData', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Версия на данните за СО' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionConfData', @level2type=N'COLUMN',@level2name=N'SOVersion'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Версия на данните за ЕД' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionConfData', @level2type=N'COLUMN',@level2name=N'CBVersion'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на външен доставчик на СО' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionConfData', @level2type=N'COLUMN',@level2name=N'SOExtProviderID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на външен доставчик на ЕД' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionConfData', @level2type=N'COLUMN',@level2name=N'CBExtProviderID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Конфигурационни данни за текущ' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionConfData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'InstitutionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Финализирана' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'IsFinalized'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименование' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Кратко наименование' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'Abbreviation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'БУЛСТАТ' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'Bulstat'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на държава' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'CountryID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на район (основен адрес)' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'LocalAreaID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Източник на финансиране' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'FinancialSchoolTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид по чл. 38 (детайлен)' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'DetailedSchoolTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид по чл. 35-36 (според собст' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'BudgetingSchoolTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на града (основен адрес)' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'TownID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид по чл. 37 (общ, според вид' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'BaseSchoolTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Основни данни за институцията ' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'InstitutionSchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Име' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'FirstName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Презиме' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'MiddleName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Фамилия' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'LastName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Постоянен адрес' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'PermanentAddress'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Постоянен адрес - населено мяс' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'PermanentTownID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Настоящ адрес' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'CurrentAddress'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Настоящ адрес - населено място' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'CurrentTownID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Личен образователен номер' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'PublicEduNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид идентификатор' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'PersonalIDType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Гражданство' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'NationalityID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ЕГН/ЛНЧ/Личен номер на чуждене' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'PersonalID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата на раждане' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'BirthDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Месторождение - населено място' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'BirthPlaceTownID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Месторождение - Държава' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'BirthPlaceCountry'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пол' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person', @level2type=N'COLUMN',@level2name=N'Gender'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Персонал, ученици, родители - ' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'Person'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребителско име' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Username'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Парола' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'Password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Валиден от' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Валиден до' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'SysUser', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребители' , @level0type=N'SCHEMA',@level0name=N'core', @level1type=N'TABLE',@level1name=N'SysUser'
GO
