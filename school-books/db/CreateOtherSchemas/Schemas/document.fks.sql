ALTER TABLE [document].[BasicDocument] ADD  CONSTRAINT [DF__BasicDocu__Valid__5E01903C]  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [document].[BasicDocument] ADD  CONSTRAINT [DF__BasicDocu__Valid__5EF5B475]  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [document].[BasicDocument] ADD  CONSTRAINT [DF_BasicDocument_IsValidation]  DEFAULT ((0)) FOR [IsValidation]
GO
ALTER TABLE [document].[BasicDocument] ADD  DEFAULT ((0)) FOR [IsAppendix]
GO
ALTER TABLE [document].[BasicDocument] ADD  DEFAULT ((0)) FOR [HasFactoryNumber]
GO
ALTER TABLE [document].[BasicDocument] ADD  DEFAULT ((0)) FOR [IsUniqueForStudent]
GO
ALTER TABLE [document].[BasicDocument] ADD  DEFAULT ((1)) FOR [HasSubjects]
GO
ALTER TABLE [document].[BasicDocument] ADD  DEFAULT ((0)) FOR [IsDuplicate]
GO
ALTER TABLE [document].[BasicDocument] ADD  DEFAULT ((0)) FOR [IsIncludedInRegister]
GO
ALTER TABLE [document].[BasicDocument] ADD  CONSTRAINT [DF_BasicDocument_IsRuoDoc]  DEFAULT ((0)) FOR [IsRuoDoc]
GO
ALTER TABLE [document].[BasicDocumentMargin] ADD  DEFAULT ((0)) FOR [Left1Margin]
GO
ALTER TABLE [document].[BasicDocumentMargin] ADD  DEFAULT ((0)) FOR [Top1Margin]
GO
ALTER TABLE [document].[BasicDocumentMargin] ADD  DEFAULT ((0)) FOR [Left2Margin]
GO
ALTER TABLE [document].[BasicDocumentMargin] ADD  DEFAULT ((0)) FOR [Top2Margin]
GO
ALTER TABLE [document].[BasicDocumentPart] ADD  DEFAULT ('') FOR [SubjectTypesList]
GO
ALTER TABLE [document].[BasicDocumentSubject] ADD  DEFAULT ((0)) FOR [SubjectCanChange]
GO
ALTER TABLE [document].[Diploma] ADD  CONSTRAINT [DF__Diploma__CreateD__7C86175C]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [document].[Diploma] ADD  CONSTRAINT [DF_Diploma_IsSigned]  DEFAULT ((0)) FOR [IsSigned]
GO
ALTER TABLE [document].[Diploma] ADD  CONSTRAINT [DF_Diploma_IsPublic]  DEFAULT ((0)) FOR [IsPublic]
GO
ALTER TABLE [document].[Diploma] ADD  CONSTRAINT [DF_Diploma_IsDiplomaFormPrinted1]  DEFAULT ((0)) FOR [IsDiplomaFormPrinted]
GO
ALTER TABLE [document].[Diploma] ADD  CONSTRAINT [DF__Diploma__IsCance__533D1662]  DEFAULT ((0)) FOR [IsCancelled]
GO
ALTER TABLE [document].[Diploma] ADD  DEFAULT ((0)) FOR [IsEditable]
GO
ALTER TABLE [document].[Diploma] ADD  DEFAULT ((0)) FOR [IsMigrated]
GO
ALTER TABLE [document].[DiplomaAdditionalDocument] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [document].[DiplomaCreateRequest] ADD  DEFAULT ((0)) FOR [IsGranted]
GO
ALTER TABLE [document].[DiplomaCreateRequest] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [document].[DiplomaCreateRequest] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [document].[DiplomaDocument] ADD  DEFAULT ((1)) FOR [Position]
GO
ALTER TABLE [document].[DiplomaSubject] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [document].[DiplomaSubject] ADD  DEFAULT ((1)) FOR [GradeCategory]
GO
ALTER TABLE [document].[EducationType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [document].[EducationType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [document].[EKRType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [document].[EKRType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [document].[ITLevel] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [document].[ITLevel] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [document].[NKRType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [document].[NKRType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [document].[PrintTemplate] ADD  DEFAULT ((0)) FOR [Left1Margin]
GO
ALTER TABLE [document].[PrintTemplate] ADD  DEFAULT ((0)) FOR [Top1Margin]
GO
ALTER TABLE [document].[PrintTemplate] ADD  DEFAULT ((0)) FOR [Left2Margin]
GO
ALTER TABLE [document].[PrintTemplate] ADD  DEFAULT ((0)) FOR [Top2Margin]
GO
ALTER TABLE [document].[Template] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [document].[TemplateSubject] ADD  DEFAULT ((0)) FOR [SubjectCanChange]
GO
ALTER TABLE [document].[TemplateSubject] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [document].[TemplateSubject] ADD  DEFAULT ((1)) FOR [GradeCategory]
GO
ALTER TABLE [document].[BarcodeYear]  WITH CHECK ADD  CONSTRAINT [FK_BarcodeYear_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[BarcodeYear] CHECK CONSTRAINT [FK_BarcodeYear_BasicDocument]
GO
ALTER TABLE [document].[BarcodeYear]  WITH CHECK ADD  CONSTRAINT [FK_BarcodeYear_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[BarcodeYear] CHECK CONSTRAINT [FK_BarcodeYear_Creator_SysUser]
GO
ALTER TABLE [document].[BarcodeYear]  WITH CHECK ADD  CONSTRAINT [FK_BarcodeYear_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[BarcodeYear] CHECK CONSTRAINT [FK_BarcodeYear_Updater_SysUser]
GO
ALTER TABLE [document].[BasicDocumentGenerator]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentGenerator_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[BasicDocumentGenerator] CHECK CONSTRAINT [FK_BasicDocumentGenerator_BasicDocument]
GO
ALTER TABLE [document].[BasicDocumentGenerator]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentGenerator_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [document].[BasicDocumentGenerator] CHECK CONSTRAINT [FK_BasicDocumentGenerator_InstitutionSchoolYear]
GO
ALTER TABLE [document].[BasicDocumentGenerator]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentGenerator_Region] FOREIGN KEY([RegionId])
REFERENCES [location].[Region] ([RegionID])
GO
ALTER TABLE [document].[BasicDocumentGenerator] CHECK CONSTRAINT [FK_BasicDocumentGenerator_Region]
GO
ALTER TABLE [document].[BasicDocumentLimit]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentToDetailedSchoolType_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[BasicDocumentLimit] CHECK CONSTRAINT [FK_BasicDocumentToDetailedSchoolType_BasicDocument]
GO
ALTER TABLE [document].[BasicDocumentLimit]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentToDetailedSchoolType_DetailedSchoolType] FOREIGN KEY([DetailedSchoolTypeId])
REFERENCES [noms].[DetailedSchoolType] ([DetailedSchoolTypeID])
GO
ALTER TABLE [document].[BasicDocumentLimit] CHECK CONSTRAINT [FK_BasicDocumentToDetailedSchoolType_DetailedSchoolType]
GO
ALTER TABLE [document].[BasicDocumentMargin]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentMargin_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[BasicDocumentMargin] CHECK CONSTRAINT [FK_BasicDocumentMargin_BasicDocument]
GO
ALTER TABLE [document].[BasicDocumentMargin]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentMargin_BasicDocumentPrintForm] FOREIGN KEY([BasicDocumentPrintFormId])
REFERENCES [document].[BasicDocumentPrintForm] ([Id])
GO
ALTER TABLE [document].[BasicDocumentMargin] CHECK CONSTRAINT [FK_BasicDocumentMargin_BasicDocumentPrintForm]
GO
ALTER TABLE [document].[BasicDocumentMargin]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentMargin_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[BasicDocumentMargin] CHECK CONSTRAINT [FK_BasicDocumentMargin_Creator_SysUser]
GO
ALTER TABLE [document].[BasicDocumentMargin]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentMargin_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[BasicDocumentMargin] CHECK CONSTRAINT [FK_BasicDocumentMargin_Updater_SysUser]
GO
ALTER TABLE [document].[BasicDocumentPart]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentPart_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [document].[BasicDocumentPart] CHECK CONSTRAINT [FK_BasicDocumentPart_BasicClass]
GO
ALTER TABLE [document].[BasicDocumentPart]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentPart_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[BasicDocumentPart] CHECK CONSTRAINT [FK_BasicDocumentPart_BasicDocument]
GO
ALTER TABLE [document].[BasicDocumentPart]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentPart_BasicSubjectType] FOREIGN KEY([BasicSubjectTypeId])
REFERENCES [inst_nom].[BasicSubjectType] ([BasicSubjectTypeID])
GO
ALTER TABLE [document].[BasicDocumentPart] CHECK CONSTRAINT [FK_BasicDocumentPart_BasicSubjectType]
GO
ALTER TABLE [document].[BasicDocumentPrintForm]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentPrintForm_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[BasicDocumentPrintForm] CHECK CONSTRAINT [FK_BasicDocumentPrintForm_BasicDocument]
GO
ALTER TABLE [document].[BasicDocumentRegDate]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentRegDate_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[BasicDocumentRegDate] CHECK CONSTRAINT [FK_BasicDocumentRegDate_BasicDocument]
GO
ALTER TABLE [document].[BasicDocumentRegDate]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentRegDate_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[BasicDocumentRegDate] CHECK CONSTRAINT [FK_BasicDocumentRegDate_Creator_SysUser]
GO
ALTER TABLE [document].[BasicDocumentRegDate]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentRegDate_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[BasicDocumentRegDate] CHECK CONSTRAINT [FK_BasicDocumentRegDate_Updater_SysUser]
GO
ALTER TABLE [document].[BasicDocumentSequence]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentSequence_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[BasicDocumentSequence] CHECK CONSTRAINT [FK_BasicDocumentSequence_BasicDocument]
GO
ALTER TABLE [document].[BasicDocumentSequence]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentSequence_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [document].[BasicDocumentSequence] CHECK CONSTRAINT [FK_BasicDocumentSequence_InstitutionSchoolYear]
GO
ALTER TABLE [document].[BasicDocumentSequence]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentSequence_Region] FOREIGN KEY([RegionId])
REFERENCES [location].[Region] ([RegionID])
GO
ALTER TABLE [document].[BasicDocumentSequence] CHECK CONSTRAINT [FK_BasicDocumentSequence_Region]
GO
ALTER TABLE [document].[BasicDocumentSubject]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentSubject_BasicDocumentPart] FOREIGN KEY([BasicDocumentPartId])
REFERENCES [document].[BasicDocumentPart] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [document].[BasicDocumentSubject] CHECK CONSTRAINT [FK_BasicDocumentSubject_BasicDocumentPart]
GO
ALTER TABLE [document].[BasicDocumentSubject]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentSubject_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [document].[BasicDocumentSubject] CHECK CONSTRAINT [FK_BasicDocumentSubject_Subject]
GO
ALTER TABLE [document].[BasicDocumentSubject]  WITH CHECK ADD  CONSTRAINT [FK_BasicDocumentSubject_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [document].[BasicDocumentSubject] CHECK CONSTRAINT [FK_BasicDocumentSubject_SubjectType]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_BasicClass]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_BasicDocument]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_CancelledSysUser] FOREIGN KEY([CancelledBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_CancelledSysUser]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_ClassType] FOREIGN KEY([ClassTypeId])
REFERENCES [inst_nom].[ClassType] ([ClassTypeID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_ClassType]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_Diploma] FOREIGN KEY([OriginalDiplomaId])
REFERENCES [document].[Diploma] ([Id])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_Diploma]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_EditableSetBySysUserId] FOREIGN KEY([EditableSetBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_EditableSetBySysUserId]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_EducationType] FOREIGN KEY([EducationTypeId])
REFERENCES [document].[EducationType] ([Id])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_EducationType]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_FLLevel] FOREIGN KEY([FLGELevelId])
REFERENCES [inst_nom].[FLLevel] ([FLLevelID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_FLLevel]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_Institution] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_Institution]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_ITLevel] FOREIGN KEY([ITLevelId])
REFERENCES [document].[ITLevel] ([Id])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_ITLevel]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_Ministry] FOREIGN KEY([MinistryId])
REFERENCES [student].[Ministry] ([Id])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_Ministry]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_ModifiedBySysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_ModifiedBySysUser]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_SignedBySysUserID] FOREIGN KEY([SignedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_SignedBySysUserID]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_SPPOOProfession] FOREIGN KEY([SPPOOProfessionId])
REFERENCES [inst_nom].[SPPOOProfession] ([SPPOOProfessionID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_SPPOOProfession]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_SPPOOSpeciality] FOREIGN KEY([SPPOOSpecialityId])
REFERENCES [inst_nom].[SPPOOSpeciality] ([SPPOOSpecialityID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_SPPOOSpeciality]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Diploma_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Diploma_SysUser]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Document_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Document_Person]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Document_Template] FOREIGN KEY([TemplateId])
REFERENCES [document].[Template] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Document_Template]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Person_Country] FOREIGN KEY([NationalityID])
REFERENCES [location].[Country] ([CountryID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Person_Country]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Person_Gender] FOREIGN KEY([Gender])
REFERENCES [noms].[Gender] ([GenderID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Person_Gender]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [FK_Person_PersonalIDType] FOREIGN KEY([PersonalIDType])
REFERENCES [noms].[PersonalIDType] ([PersonalIDTypeID])
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [FK_Person_PersonalIDType]
GO
ALTER TABLE [document].[DiplomaAdditionalDocument]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaAdditionalDocument_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[DiplomaAdditionalDocument] CHECK CONSTRAINT [FK_DiplomaAdditionalDocument_BasicDocument]
GO
ALTER TABLE [document].[DiplomaAdditionalDocument]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaAdditionalDocument_Creator] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[DiplomaAdditionalDocument] CHECK CONSTRAINT [FK_DiplomaAdditionalDocument_Creator]
GO
ALTER TABLE [document].[DiplomaAdditionalDocument]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaAdditionalDocument_Diploma] FOREIGN KEY([DiplomaId])
REFERENCES [document].[Diploma] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [document].[DiplomaAdditionalDocument] CHECK CONSTRAINT [FK_DiplomaAdditionalDocument_Diploma]
GO
ALTER TABLE [document].[DiplomaAdditionalDocument]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaAdditionalDocument_MainDiploma] FOREIGN KEY([MainDiplomaId])
REFERENCES [document].[Diploma] ([Id])
GO
ALTER TABLE [document].[DiplomaAdditionalDocument] CHECK CONSTRAINT [FK_DiplomaAdditionalDocument_MainDiploma]
GO
ALTER TABLE [document].[DiplomaAdditionalDocument]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaAdditionalDocument_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[DiplomaAdditionalDocument] CHECK CONSTRAINT [FK_DiplomaAdditionalDocument_Updater]
GO
ALTER TABLE [document].[DiplomaCreateRequest]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaCreateRequest_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[DiplomaCreateRequest] CHECK CONSTRAINT [FK_DiplomaCreateRequest_BasicDocument]
GO
ALTER TABLE [document].[DiplomaCreateRequest]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaCreateRequest_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[DiplomaCreateRequest] CHECK CONSTRAINT [FK_DiplomaCreateRequest_CreatedSysUser]
GO
ALTER TABLE [document].[DiplomaCreateRequest]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaCreateRequest_Diploma] FOREIGN KEY([DiplomaId])
REFERENCES [document].[Diploma] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [document].[DiplomaCreateRequest] CHECK CONSTRAINT [FK_DiplomaCreateRequest_Diploma]
GO
ALTER TABLE [document].[DiplomaCreateRequest]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaCreateRequest_InstitutionSchoolYear_CurrentInstitution] FOREIGN KEY([CurrentInstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [document].[DiplomaCreateRequest] CHECK CONSTRAINT [FK_DiplomaCreateRequest_InstitutionSchoolYear_CurrentInstitution]
GO
ALTER TABLE [document].[DiplomaCreateRequest]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaCreateRequest_InstitutionSchoolYear_RequestingInstitution] FOREIGN KEY([RequestingInstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [document].[DiplomaCreateRequest] CHECK CONSTRAINT [FK_DiplomaCreateRequest_InstitutionSchoolYear_RequestingInstitution]
GO
ALTER TABLE [document].[DiplomaCreateRequest]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaCreateRequest_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[DiplomaCreateRequest] CHECK CONSTRAINT [FK_DiplomaCreateRequest_ModifiedSysUser]
GO
ALTER TABLE [document].[DiplomaCreateRequest]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaCreateRequest_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [document].[DiplomaCreateRequest] CHECK CONSTRAINT [FK_DiplomaCreateRequest_Person]
GO
ALTER TABLE [document].[DiplomaDocument]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaDocument_Diploma] FOREIGN KEY([DiplomaId])
REFERENCES [document].[Diploma] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [document].[DiplomaDocument] CHECK CONSTRAINT [FK_DiplomaDocument_Diploma]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaSubject_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DiplomaSubject_BasicClass]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaSubject_Creator] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DiplomaSubject_Creator]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaSubject_FLSubject] FOREIGN KEY([FlSubjectId])
REFERENCES [inst_nom].[FL] ([FLID])
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DiplomaSubject_FLSubject]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaSubject_Parent] FOREIGN KEY([ParentId])
REFERENCES [document].[DiplomaSubject] ([Id])
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DiplomaSubject_Parent]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaSubject_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DiplomaSubject_SubjectType]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DiplomaSubject_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DiplomaSubject_Updater]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DocumentSubject_BasicDocumentPart] FOREIGN KEY([BasicDocumentPartId])
REFERENCES [document].[BasicDocumentPart] ([Id])
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DocumentSubject_BasicDocumentPart]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DocumentSubject_BasicDocumentSubject] FOREIGN KEY([BasicDocumentSubjectId])
REFERENCES [document].[BasicDocumentSubject] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DocumentSubject_BasicDocumentSubject]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DocumentSubject_Document] FOREIGN KEY([DiplomaId])
REFERENCES [document].[Diploma] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DocumentSubject_Document]
GO
ALTER TABLE [document].[DiplomaSubject]  WITH CHECK ADD  CONSTRAINT [FK_DocumentSubject_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [document].[DiplomaSubject] CHECK CONSTRAINT [FK_DocumentSubject_Subject]
GO
ALTER TABLE [document].[GraduationCommissionMember]  WITH CHECK ADD  CONSTRAINT [FK_GraduationCommissionMember_Diploma] FOREIGN KEY([DiplomaId])
REFERENCES [document].[Diploma] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [document].[GraduationCommissionMember] CHECK CONSTRAINT [FK_GraduationCommissionMember_Diploma]
GO
ALTER TABLE [document].[GraduationCommissionMember]  WITH CHECK ADD  CONSTRAINT [FK_GraduationCommissionMember_Template] FOREIGN KEY([TemplateId])
REFERENCES [document].[Template] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [document].[GraduationCommissionMember] CHECK CONSTRAINT [FK_GraduationCommissionMember_Template]
GO
ALTER TABLE [document].[OriginalDiploma]  WITH CHECK ADD  CONSTRAINT [FK_OriginalDiploma_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[OriginalDiploma] CHECK CONSTRAINT [FK_OriginalDiploma_Creator_SysUser]
GO
ALTER TABLE [document].[OriginalDiploma]  WITH CHECK ADD  CONSTRAINT [FK_OriginalDiploma_Diploma] FOREIGN KEY([DiplomaId])
REFERENCES [document].[Diploma] ([Id])
GO
ALTER TABLE [document].[OriginalDiploma] CHECK CONSTRAINT [FK_OriginalDiploma_Diploma]
GO
ALTER TABLE [document].[OriginalDiploma]  WITH CHECK ADD  CONSTRAINT [FK_Originaliploma_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[OriginalDiploma] CHECK CONSTRAINT [FK_Originaliploma_Updater_SysUser]
GO
ALTER TABLE [document].[PrintTemplate]  WITH CHECK ADD  CONSTRAINT [FK_PrintTemplate_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[PrintTemplate] CHECK CONSTRAINT [FK_PrintTemplate_BasicDocument]
GO
ALTER TABLE [document].[PrintTemplate]  WITH CHECK ADD  CONSTRAINT [FK_PrintTemplate_BasicDocumentPrintForm] FOREIGN KEY([BasicDocumentPrintFormId])
REFERENCES [document].[BasicDocumentPrintForm] ([Id])
GO
ALTER TABLE [document].[PrintTemplate] CHECK CONSTRAINT [FK_PrintTemplate_BasicDocumentPrintForm]
GO
ALTER TABLE [document].[PrintTemplate]  WITH CHECK ADD  CONSTRAINT [FK_PrintTemplate_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[PrintTemplate] CHECK CONSTRAINT [FK_PrintTemplate_Creator_SysUser]
GO
ALTER TABLE [document].[PrintTemplate]  WITH CHECK ADD  CONSTRAINT [FK_PrintTemplate_Institution] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [document].[PrintTemplate] CHECK CONSTRAINT [FK_PrintTemplate_Institution]
GO
ALTER TABLE [document].[PrintTemplate]  WITH CHECK ADD  CONSTRAINT [FK_PrintTemplate_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[PrintTemplate] CHECK CONSTRAINT [FK_PrintTemplate_Updater_SysUser]
GO
ALTER TABLE [document].[Template]  WITH CHECK ADD  CONSTRAINT [FK_Template_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [document].[Template] CHECK CONSTRAINT [FK_Template_BasicClass]
GO
ALTER TABLE [document].[Template]  WITH CHECK ADD  CONSTRAINT [FK_Template_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [document].[Template] CHECK CONSTRAINT [FK_Template_BasicDocument]
GO
ALTER TABLE [document].[Template]  WITH CHECK ADD  CONSTRAINT [FK_Template_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[Template] CHECK CONSTRAINT [FK_Template_CreatedSysUser]
GO
ALTER TABLE [document].[Template]  WITH CHECK ADD  CONSTRAINT [FK_Template_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [document].[Template] CHECK CONSTRAINT [FK_Template_InstitutionSchoolYear]
GO
ALTER TABLE [document].[Template]  WITH CHECK ADD  CONSTRAINT [FK_Template_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[Template] CHECK CONSTRAINT [FK_Template_ModifiedSysUser]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_BasicClass]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_BasicDocumentPart] FOREIGN KEY([BasicDocumentPartId])
REFERENCES [document].[BasicDocumentPart] ([Id])
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_BasicDocumentPart]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_BasicDocumentSubject] FOREIGN KEY([BasicDocumentSubjectId])
REFERENCES [document].[BasicDocumentSubject] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_BasicDocumentSubject]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_Creator] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_Creator]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_FLSubject] FOREIGN KEY([FlSubjectId])
REFERENCES [inst_nom].[FL] ([FLID])
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_FLSubject]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_Parent] FOREIGN KEY([ParentId])
REFERENCES [document].[TemplateSubject] ([Id])
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_Parent]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_Subject]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_SubjectType]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_Template] FOREIGN KEY([TemplateId])
REFERENCES [document].[Template] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_Template]
GO
ALTER TABLE [document].[TemplateSubject]  WITH CHECK ADD  CONSTRAINT [FK_TemplateSubject_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [document].[TemplateSubject] CHECK CONSTRAINT [FK_TemplateSubject_Updater]
GO
ALTER TABLE [document].[BasicDocumentGenerator]  WITH CHECK ADD  CONSTRAINT [BasicDocumentGeneratorHasInstitution] CHECK  (([SchoolYear] IS NOT NULL AND ([InstitutionId] IS NOT NULL OR [RegionId] IS NOT NULL)))
GO
ALTER TABLE [document].[BasicDocumentGenerator] CHECK CONSTRAINT [BasicDocumentGeneratorHasInstitution]
GO
ALTER TABLE [document].[BasicDocumentSequence]  WITH CHECK ADD  CONSTRAINT [BasicDocumentSequenceHasInstitution] CHECK  (([SchoolYear] IS NOT NULL AND ([InstitutionId] IS NOT NULL OR [RegionId] IS NOT NULL)))
GO
ALTER TABLE [document].[BasicDocumentSequence] CHECK CONSTRAINT [BasicDocumentSequenceHasInstitution]
GO
ALTER TABLE [document].[Diploma]  WITH CHECK ADD  CONSTRAINT [DiplomaHasInstitution] CHECK  (([SchoolYear] IS NOT NULL AND ([InstitutionId] IS NOT NULL OR [RuoRegId] IS NOT NULL)))
GO
ALTER TABLE [document].[Diploma] CHECK CONSTRAINT [DiplomaHasInstitution]
GO
