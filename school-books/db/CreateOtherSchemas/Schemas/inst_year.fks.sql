ALTER TABLE [inst_year].[ChangeYearData] ADD  CONSTRAINT [DF_ChangeYearData_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [inst_year].[ChangeYearData] ADD  CONSTRAINT [DF_ChangeYearData_ModifiedOn]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ModifiedOn]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  CONSTRAINT [DF_ClassGroup_IsProfModule]  DEFAULT ((0)) FOR [IsProfModule]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  CONSTRAINT [DF_ClassGroup_IsCombined]  DEFAULT ((0)) FOR [IsCombined]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  CONSTRAINT [DF_ClassGroup_IsNoList]  DEFAULT ((0)) FOR [IsNoList]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  CONSTRAINT [DF_ClassGroup_IsSpecNeed]  DEFAULT ((0)) FOR [IsSpecNeed]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  CONSTRAINT [DF_ClassGroup_IsWholeClass]  DEFAULT ((0)) FOR [IsWholeClass]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  CONSTRAINT [DF_ClassGroup_IsNotPresentForm]  DEFAULT ((0)) FOR [IsNotPresentForm]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  CONSTRAINT [ClassGroupValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  CONSTRAINT [ClassGroupValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  DEFAULT ((0)) FOR [IsClosed]
GO
ALTER TABLE [inst_year].[ClassGroup] ADD  DEFAULT ((0)) FOR [IsNotNPO109]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  CONSTRAINT [DF_Curriculum_IsFL]  DEFAULT ((0)) FOR [IsFL]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  CONSTRAINT [DF_Curriculum_IsIndividualLesson]  DEFAULT ((0)) FOR [IsIndividualLesson]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  CONSTRAINT [DF_Curriculum_IsIndividualCurriculum]  DEFAULT ((0)) FOR [IsIndividualCurriculum]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  CONSTRAINT [CurriculumValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  CONSTRAINT [CurriculumValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  CONSTRAINT [DF_IsWholeClass]  DEFAULT ((1)) FOR [IsWholeClass]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  CONSTRAINT [DF_Curriculum_IsAllStudents]  DEFAULT ((0)) FOR [IsAllStudents]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_year].[Curriculum] ADD  DEFAULT ((0)) FOR [IsCombined]
GO
ALTER TABLE [inst_year].[CurriculumClass] ADD  CONSTRAINT [CurriculumClassValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[CurriculumClass] ADD  CONSTRAINT [CurriculumClassValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[CurriculumClass] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_year].[CurriculumStudent] ADD  CONSTRAINT [CurriculumStudentValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[CurriculumStudent] ADD  CONSTRAINT [CurriculumStudentValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[CurriculumStudent] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_year].[CurriculumStudent] ADD  CONSTRAINT [CurriculumStudentIsAzureEnrolledNotNull]  DEFAULT ((0)) FOR [isAzureEnrolled]
GO
ALTER TABLE [inst_year].[CurriculumTeacher] ADD  CONSTRAINT [CurriculumTeacherValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[CurriculumTeacher] ADD  CONSTRAINT [CurriculumTeacherValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[CurriculumTeacher] ADD  CONSTRAINT [DF__Curriculu__IsVal__02E2D33A]  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_year].[CurriculumTeacher] ADD  CONSTRAINT [DF_CurriculumTeacher_NormaTS]  DEFAULT ((0)) FOR [NormaTS]
GO
ALTER TABLE [inst_year].[CurriculumTeacher] ADD  CONSTRAINT [CurriculumTeacherIsAzureEnrolledNotNull]  DEFAULT ((0)) FOR [isAzureEnrolled]
GO
ALTER TABLE [inst_year].[CurriculumTeacher] ADD  DEFAULT ((0)) FOR [NoReplacement]
GO
ALTER TABLE [inst_year].[InstitutionOtherData] ADD  CONSTRAINT [DF__Instituti__Valid__2BC97F7C]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[InstitutionOtherData] ADD  CONSTRAINT [DF__Instituti__Valid__2CBDA3B5]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[PedStaffData] ADD  CONSTRAINT [DF__StaffOthe__Valid__3DF31CAF]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[PedStaffData] ADD  CONSTRAINT [DF__StaffOthe__Valid__3EE740E8]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[PedStaffData] ADD  CONSTRAINT [DF_PedStaffData_IsValid]  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_year].[ProfileClass] ADD  CONSTRAINT [DF__ProfileCl__Valid__7CAF6937]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[ProfileClass] ADD  CONSTRAINT [DF__ProfileCl__Valid__7DA38D70]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[StaffPositionMainClass] ADD  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [inst_year].[StaffPositionMainClass] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [inst_year].[VacantStaff] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_year].[ChangeYearData]  WITH CHECK ADD  CONSTRAINT [FK_ChangeYearDataInstitution] FOREIGN KEY([InstitutionID], [FromSchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [inst_year].[ChangeYearData] CHECK CONSTRAINT [FK_ChangeYearDataInstitution]
GO
ALTER TABLE [inst_year].[ChangeYearData]  WITH CHECK ADD  CONSTRAINT [FK_ChangeYearDataSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_year].[ChangeYearData] CHECK CONSTRAINT [FK_ChangeYearDataSysUser]
GO
ALTER TABLE [inst_year].[ChangeYearData]  WITH CHECK ADD  CONSTRAINT [FK_ChangeYearStatus] FOREIGN KEY([ChangeYearStatusID])
REFERENCES [inst_nom].[ChangeYearStatus] ([ChangeYearStatusID])
GO
ALTER TABLE [inst_year].[ChangeYearData] CHECK CONSTRAINT [FK_ChangeYearStatus]
GO
ALTER TABLE [inst_year].[ChangeYearDataStudentAmendatory]  WITH CHECK ADD  CONSTRAINT [FK_ChangeYearData] FOREIGN KEY([ChangeYearDataID])
REFERENCES [inst_year].[ChangeYearData] ([ChangeYearDataID])
GO
ALTER TABLE [inst_year].[ChangeYearDataStudentAmendatory] CHECK CONSTRAINT [FK_ChangeYearData]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupBasicClass] FOREIGN KEY([BasicClassID])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupBasicClass]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupBudget] FOREIGN KEY([BudgetingClassTypeID])
REFERENCES [noms].[BudgetingInstitution] ([BudgetingInstitutionID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupBudget]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupClassEduDuration] FOREIGN KEY([ClassEduDurationID])
REFERENCES [inst_nom].[ClassEduDuration] ([ClassEduDurationID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupClassEduDuration]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupClassEduForm] FOREIGN KEY([ClassEduFormID])
REFERENCES [inst_nom].[EduForm] ([ClassEduFormID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupClassEduForm]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupClassShift] FOREIGN KEY([ClassShiftID])
REFERENCES [inst_nom].[ClassShift] ([ClassShiftID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupClassShift]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupClassType] FOREIGN KEY([ClassTypeID])
REFERENCES [inst_nom].[ClassType] ([ClassTypeID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupClassType]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupCurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupCurrentYear]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupEducationArea] FOREIGN KEY([AreaID])
REFERENCES [inst_nom].[EducationArea] ([EducationAreaID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupEducationArea]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupEntranceLevel] FOREIGN KEY([EntranceLevelID])
REFERENCES [inst_nom].[EntranceLevel] ([EntranceLevelID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupEntranceLevel]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupFL] FOREIGN KEY([FLID])
REFERENCES [inst_nom].[FL] ([FLID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupFL]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupFLStudyType] FOREIGN KEY([FLTypeID])
REFERENCES [inst_nom].[FLStudyType] ([FLStudyTypeID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupFLStudyType]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupInstitution] FOREIGN KEY([InstitutionID], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupInstitution]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupInstitutionDepartment] FOREIGN KEY([InstitutionDepartmentID])
REFERENCES [inst_basic].[InstitutionDepartment] ([InstitutionDepartmentID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupInstitutionDepartment]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupParentClass] FOREIGN KEY([ParentClassID])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupParentClass]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupSpeciality] FOREIGN KEY([ClassSpecialityID])
REFERENCES [inst_nom].[SPPOOSpeciality] ([SPPOOSpecialityID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupSpeciality]
GO
ALTER TABLE [inst_year].[ClassGroup]  WITH CHECK ADD  CONSTRAINT [FK_ClassGroupSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_year].[ClassGroup] CHECK CONSTRAINT [FK_ClassGroupSysUser]
GO
ALTER TABLE [inst_year].[Curriculum]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumParentCurriculum] FOREIGN KEY([ParentCurriculumID])
REFERENCES [inst_year].[Curriculum] ([CurriculumID])
GO
ALTER TABLE [inst_year].[Curriculum] CHECK CONSTRAINT [FK_CurriculumParentCurriculum]
GO
ALTER TABLE [inst_year].[CurriculumClass]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumClassCurriculum] FOREIGN KEY([CurriculumID])
REFERENCES [inst_year].[Curriculum] ([CurriculumID])
GO
ALTER TABLE [inst_year].[CurriculumClass] CHECK CONSTRAINT [FK_CurriculumClassCurriculum]
GO
ALTER TABLE [inst_year].[CurriculumClass]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumClassGroup] FOREIGN KEY([ClassID])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [inst_year].[CurriculumClass] CHECK CONSTRAINT [FK_CurriculumClassGroup]
GO
ALTER TABLE [inst_year].[CurriculumClass]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumClassSchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [inst_year].[CurriculumClass] CHECK CONSTRAINT [FK_CurriculumClassSchoolYear]
GO
ALTER TABLE [inst_year].[CurriculumClass]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumClassSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_year].[CurriculumClass] CHECK CONSTRAINT [FK_CurriculumClassSysUser]
GO
ALTER TABLE [inst_year].[CurriculumTeacher]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumTeacherCurriculum] FOREIGN KEY([CurriculumID])
REFERENCES [inst_year].[Curriculum] ([CurriculumID])
GO
ALTER TABLE [inst_year].[CurriculumTeacher] CHECK CONSTRAINT [FK_CurriculumTeacherCurriculum]
GO
ALTER TABLE [inst_year].[CurriculumTeacher]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumTeacherSchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [inst_year].[CurriculumTeacher] CHECK CONSTRAINT [FK_CurriculumTeacherSchoolYear]
GO
ALTER TABLE [inst_year].[CurriculumTeacher]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumTeacherStaffPosition] FOREIGN KEY([StaffPositionID])
REFERENCES [inst_basic].[StaffPosition] ([StaffPositionID])
GO
ALTER TABLE [inst_year].[CurriculumTeacher] CHECK CONSTRAINT [FK_CurriculumTeacherStaffPosition]
GO
ALTER TABLE [inst_year].[CurriculumTeacher]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumTeacherSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_year].[CurriculumTeacher] CHECK CONSTRAINT [FK_CurriculumTeacherSysUser]
GO
ALTER TABLE [inst_year].[InstitutionOtherData]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionOtherDataCurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [inst_year].[InstitutionOtherData] CHECK CONSTRAINT [FK_InstitutionOtherDataCurrentYear]
GO
ALTER TABLE [inst_year].[InstitutionOtherData]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionOtherDataInstitution] FOREIGN KEY([InstitutionID], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [inst_year].[InstitutionOtherData] CHECK CONSTRAINT [FK_InstitutionOtherDataInstitution]
GO
ALTER TABLE [inst_year].[InstitutionOtherData]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionOtherDataSchoolShiftType] FOREIGN KEY([SchoolShiftTypeID])
REFERENCES [inst_nom].[SchoolShiftType] ([SchoolShiftTypeID])
GO
ALTER TABLE [inst_year].[InstitutionOtherData] CHECK CONSTRAINT [FK_InstitutionOtherDataSchoolShiftType]
GO
ALTER TABLE [inst_year].[PedStaffData]  WITH CHECK ADD  CONSTRAINT [FK_PedStaffDataStaffPosition] FOREIGN KEY([StaffPositionID])
REFERENCES [inst_basic].[StaffPosition] ([StaffPositionID])
GO
ALTER TABLE [inst_year].[PedStaffData] CHECK CONSTRAINT [FK_PedStaffDataStaffPosition]
GO
ALTER TABLE [inst_year].[PedStaffData]  WITH CHECK ADD  CONSTRAINT [FK_PedStaffDataSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_year].[PedStaffData] CHECK CONSTRAINT [FK_PedStaffDataSysUser]
GO
ALTER TABLE [inst_year].[ProfileClass]  WITH CHECK ADD  CONSTRAINT [FK_ProfileClassClass] FOREIGN KEY([ClassID])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [inst_year].[ProfileClass] CHECK CONSTRAINT [FK_ProfileClassClass]
GO
ALTER TABLE [inst_year].[ProfileClass]  WITH CHECK ADD  CONSTRAINT [FK_ProfileClassCurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [inst_year].[ProfileClass] CHECK CONSTRAINT [FK_ProfileClassCurrentYear]
GO
ALTER TABLE [inst_year].[ProfileClass]  WITH CHECK ADD  CONSTRAINT [FK_ProfileClassParentClass] FOREIGN KEY([ParentClassID])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [inst_year].[ProfileClass] CHECK CONSTRAINT [FK_ProfileClassParentClass]
GO
ALTER TABLE [inst_year].[ProfileClass]  WITH CHECK ADD  CONSTRAINT [FK_ProfileClassProfSubj] FOREIGN KEY([ProfSubjID])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [inst_year].[ProfileClass] CHECK CONSTRAINT [FK_ProfileClassProfSubj]
GO
ALTER TABLE [inst_year].[ProfileClass]  WITH CHECK ADD  CONSTRAINT [FK_ProfileClassProfSubjType] FOREIGN KEY([ProfSubjType])
REFERENCES [inst_nom].[ProfSubjectType] ([ProfSubjectTypeID])
GO
ALTER TABLE [inst_year].[ProfileClass] CHECK CONSTRAINT [FK_ProfileClassProfSubjType]
GO
ALTER TABLE [inst_year].[ProfileClass]  WITH CHECK ADD  CONSTRAINT [FK_ProfileClassSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_year].[ProfileClass] CHECK CONSTRAINT [FK_ProfileClassSysUser]
GO
ALTER TABLE [inst_year].[StaffPositionMainClass]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionMainClassClassGroup] FOREIGN KEY([MainClassID])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [inst_year].[StaffPositionMainClass] CHECK CONSTRAINT [FK_StaffPositionMainClassClassGroup]
GO
ALTER TABLE [inst_year].[StaffPositionMainClass]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionMainClassStaffPosition] FOREIGN KEY([StaffPositionID])
REFERENCES [inst_basic].[StaffPosition] ([StaffPositionID])
GO
ALTER TABLE [inst_year].[StaffPositionMainClass] CHECK CONSTRAINT [FK_StaffPositionMainClassStaffPosition]
GO
ALTER TABLE [inst_year].[StaffPositionMainClass]  WITH CHECK ADD  CONSTRAINT [FK_StaffPositionMainClassSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_year].[StaffPositionMainClass] CHECK CONSTRAINT [FK_StaffPositionMainClassSysUser]
GO
ALTER TABLE [inst_year].[VacantStaff]  WITH CHECK ADD  CONSTRAINT [FK_VacantStaffInstitution] FOREIGN KEY([InstitutionID], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [inst_year].[VacantStaff] CHECK CONSTRAINT [FK_VacantStaffInstitution]
GO
ALTER TABLE [inst_year].[VacantStaff]  WITH CHECK ADD  CONSTRAINT [FK_VacantStaffNKPDPosition] FOREIGN KEY([NKPDPositionID])
REFERENCES [inst_nom].[NKPDPosition] ([NKPDPositionID])
GO
ALTER TABLE [inst_year].[VacantStaff] CHECK CONSTRAINT [FK_VacantStaffNKPDPosition]
GO
ALTER TABLE [inst_year].[VacantStaff]  WITH CHECK ADD  CONSTRAINT [FK_VacantStaffStaffType] FOREIGN KEY([StaffTypeID])
REFERENCES [inst_nom].[StaffType] ([StaffTypeID])
GO
ALTER TABLE [inst_year].[VacantStaff] CHECK CONSTRAINT [FK_VacantStaffStaffType]
GO
ALTER TABLE [inst_year].[VacantStaff]  WITH CHECK ADD  CONSTRAINT [FK_VacantStaffSubjectGroup] FOREIGN KEY([PositionSubjectGroupID])
REFERENCES [inst_nom].[SubjectGroup] ([SubjectGroupID])
GO
ALTER TABLE [inst_year].[VacantStaff] CHECK CONSTRAINT [FK_VacantStaffSubjectGroup]
GO
ALTER TABLE [inst_year].[VacantStaff]  WITH CHECK ADD  CONSTRAINT [FK_VacantStaffSysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [inst_year].[VacantStaff] CHECK CONSTRAINT [FK_VacantStaffSysUser]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на адре' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'InstitutionDepartmentID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пореден номер на групата' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ClassGroupNum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Наименование на паралелката/гр' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ClassName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Име на паралелката/групата (на' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ParalellClassName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на базо' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ParentClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на випуска (възр' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'BasicClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид на паралелката/групата' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ClassTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Профил (отнася се за групите в' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'AreaID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Форма на обучение на паралелка' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ClassEduFormID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Срок на обучение на паралелкат' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ClassEduDurationID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'форма на организация на учебни' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ClassShiftID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'От кого се финансира' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'BudgetingClassTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Прием след:' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'EntranceLevelID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Специалност' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ClassSpecialityID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Начин на изучаване на чуждия е' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'FLTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Чужд език' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'FLID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Професионално обучение' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'IsProfModule'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой места' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'StudentCountPlaces'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'Notes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Слята паралелка"' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'IsCombined'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Не се прилага списък н' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'IsNoList'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Специална паралелка/гр' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'IsSpecNeed'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Цяла паралелка/група"' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'IsWholeClass'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "За неприсъствени форми' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'IsNotPresentForm'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Временно поле, използва се при' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'tmp_ClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Паралелки/Групи' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'ClassGroup'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'CurriculumID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пореден номер на групата по уч' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'CurriculumGroupNum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебен предмет' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'SubjectID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Начин на изучаване' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'SubjectTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой учебни седмици І срок' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'WeeksFirstTerm'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой часове седмично І срок' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'HoursWeeklyFirstTerm'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой учебни седмици ІІ срок' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'WeeksSecondTerm'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой часове седмично ІІ срок' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'HoursWeeklySecondTerm'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Учебен предмет (различ' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'IsFL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Чужд език' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'FLSubjectID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер за индивидуален час' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'IsIndividualLesson'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Норма на предмета' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'NormaS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Адрес, на който се провежда об' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'InstitutionDepartmentID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на основния запи' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'ParentCurriculumID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Раздел от учебния план' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'CurriculumPartID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор за сортиране' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'SortOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер за индивидуален учебен ' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'IsIndividualCurriculum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Предметът се изучава о' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'IsWholeClass'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Предметът се изучава о' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum', @level2type=N'COLUMN',@level2name=N'IsAllStudents'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебен план' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'Curriculum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumClass', @level2type=N'COLUMN',@level2name=N'CurriculumClassesID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на записа от уче' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumClass', @level2type=N'COLUMN',@level2name=N'CurriculumID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на паралелката/г' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumClass', @level2type=N'COLUMN',@level2name=N'ClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumClass', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumClass', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumClass', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumClass', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебен план - Паралелки/Групи' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumClass'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent', @level2type=N'COLUMN',@level2name=N'CurriculumStudentID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на записа от уче' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent', @level2type=N'COLUMN',@level2name=N'CurriculumID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на учен' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent', @level2type=N'COLUMN',@level2name=N'StudentID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Личен образователен номер' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебен план - Ученици' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumStudent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher', @level2type=N'COLUMN',@level2name=N'CurriculumTeacherID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на записа от уче' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher', @level2type=N'COLUMN',@level2name=N'CurriculumID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на записа на пре' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher', @level2type=N'COLUMN',@level2name=N'StaffPositionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер "Не отговаря на изисква' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher', @level2type=N'COLUMN',@level2name=N'IsNotRegularTeacher'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебен план - Преподаватели' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'CurriculumTeacher'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'InstitutionOtherDataID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Сменност на обучение' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'SchoolShiftTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Утвърдена численост - педагоги' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'PedagogStaffCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Утвърдена численост - непедаго' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'NonpedagogStaffCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Утвърдена численост на персона' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'StaffCountAll'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Средна заплата - педагогически' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'PedagogStaffSalary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Средна заплата - непедагогичес' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'NonpedagogStaffSalary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Утвърден бюджет' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData', @level2type=N'COLUMN',@level2name=N'YearlyBudget'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Допълнителни данни за институц' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'InstitutionOtherData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'PedStaffDataID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към  длъжност' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'StaffPositionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Часове по норматив на РМ' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'NormaS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Минимална ЗНПР' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'Norma'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Индивидуална ЗНПР' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'NormaT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Недостиг до минимална ЗНПР' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'DeficitNorma'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Недостиг до индивидуална ЗНПР' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'DeficitNormaT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Коефициент на редукция към ЗНП' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'ReductionCoef'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Часове с различен норматив' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'DiffNorma'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Часове, подлежащи на редукция ' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'HoursForReduction'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Брой часове след редукция' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'ReductionHours'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Лекторски / недостиг - инд.ЗНП' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'Lect'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Лекторски / недостиг - инд.ЗНП' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'LectYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Годишна преподавателска заетос' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'PedStaffData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'StaffPositionMainClass', @level2type=N'COLUMN',@level2name=N'StaffPositionMainClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към длъжност' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'StaffPositionMainClass', @level2type=N'COLUMN',@level2name=N'StaffPositionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към клас/група' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'StaffPositionMainClass', @level2type=N'COLUMN',@level2name=N'MainClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, от която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'StaffPositionMainClass', @level2type=N'COLUMN',@level2name=N'ValidFrom'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата, до която е валидна стойн' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'StaffPositionMainClass', @level2type=N'COLUMN',@level2name=N'ValidTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'StaffPositionMainClass', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Основен клас, в който преподав' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'StaffPositionMainClass'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID на записа (autoincrement)' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'VacantStaffID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'InstitutionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Категория персонал (обобщена)' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'StaffTypeID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код по НКПД на длъжността' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'NKPDPositionID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Щат' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'PositionCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Назначен на щатно място по' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'PositionSubjectGroupID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Флаг за валидна стойност' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff', @level2type=N'COLUMN',@level2name=N'IsValid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Персонал - незаети щатни бройки' , @level0type=N'SCHEMA',@level0name=N'inst_year', @level1type=N'TABLE',@level1name=N'VacantStaff'
GO
