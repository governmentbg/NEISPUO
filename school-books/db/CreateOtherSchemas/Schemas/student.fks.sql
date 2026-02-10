ALTER TABLE [student].[Absence] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [student].[AbsenceCampaign] ADD  DEFAULT ((0)) FOR [IsManuallyActivated]
GO
ALTER TABLE [student].[AbsenceCampaign] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[AbsenceExport] ADD  DEFAULT ((0)) FOR [RecordsCount]
GO
ALTER TABLE [student].[AbsenceExport] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [student].[AbsenceExport] ADD  DEFAULT ((0)) FOR [IsFinalized]
GO
ALTER TABLE [student].[AbsenceExport] ADD  DEFAULT ((0)) FOR [IsSigned]
GO
ALTER TABLE [student].[AbsenceImport] ADD  DEFAULT ((0)) FOR [RecordsCount]
GO
ALTER TABLE [student].[AbsenceImport] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [student].[AbsenceImport] ADD  DEFAULT ((0)) FOR [IsFinalized]
GO
ALTER TABLE [student].[AbsenceImport] ADD  DEFAULT ((0)) FOR [IsSigned]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] ADD  CONSTRAINT [DF_AdditionalPersonalDevelopmentSupport_Suspnded]  DEFAULT ((0)) FOR [IsSuspended]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] ADD  CONSTRAINT [DF_AdditionalPersonalDevelopmentSupport_CreateDate]  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem] ADD  CONSTRAINT [DF_AdditionalPersonalDevelopmentSupportItem_Suspnded]  DEFAULT ((0)) FOR [IsSuspended]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[AdditionalSupportType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[AdditionalSupportType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[AdmissionDocument] ADD  DEFAULT ((3)) FOR [PositionId]
GO
ALTER TABLE [student].[AdmissionDocument] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[AdmissionDocument] ADD  DEFAULT ((0)) FOR [HasHealthStatusDocument]
GO
ALTER TABLE [student].[AdmissionDocument] ADD  DEFAULT ((0)) FOR [HasImmunizationStatusDocument]
GO
ALTER TABLE [student].[AdmissionPermissionRequest] ADD  DEFAULT ((0)) FOR [IsPermissionGranted]
GO
ALTER TABLE [student].[AdmissionReasonType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[AdmissionReasonType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[AppSettings] ADD  CONSTRAINT [DF_AppSettings_ValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[AppSettings] ADD  CONSTRAINT [DF_AppSettings_ValidTo]  DEFAULT (CONVERT([datetime2](0),'9999-12-31 23:59:59')) FOR [ValidTo]
GO
ALTER TABLE [student].[AppSettingsForTenant] ADD  DEFAULT ((8)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[AppSettingsForTenant] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[ASPEnrolledStudentsExport] ADD  DEFAULT ((0)) FOR [RecordsCount]
GO
ALTER TABLE [student].[ASPEnrolledStudentsExport] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [student].[ASPMonthlyBenefit] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [student].[ASPMonthlyBenefitInstitution] ADD  DEFAULT ((0)) FOR [IsSigned]
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport] ADD  DEFAULT ((0)) FOR [RecordsCount]
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [student].[AspSubmittedDataHistory] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[AspZpTemp] ADD  DEFAULT ((0)) FOR [IsCorrection]
GO
ALTER TABLE [student].[AuditEntryProperties] ADD  DEFAULT ((0)) FOR [IsKey]
GO
ALTER TABLE [student].[AwardCategory] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[AwardCategory] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[AwardReason] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[AwardReason] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[AwardTypes] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[AwardTypes] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport] ADD  CONSTRAINT [DF_CommonPersonalDevelopmentSupport_CreateDate]  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[CommonSupportType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[CommonSupportType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[CommuterType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[CommuterType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[DischargeDocument] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[DischargeReasonType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[DischargeReasonType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[DischargeReasonType] ADD  DEFAULT ((0)) FOR [IsForDischarge]
GO
ALTER TABLE [student].[DischargeReasonType] ADD  DEFAULT ((0)) FOR [IsForRelocation]
GO
ALTER TABLE [student].[DischargeReasonType] ADD  DEFAULT ((0)) FOR [IsForInternalEnrollment]
GO
ALTER TABLE [student].[Document] ADD  DEFAULT (newid()) FOR [Rowguid]
GO
ALTER TABLE [student].[Document] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[EarlyEvaluationReason] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[EarlyEvaluationReason] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[EqualizationDetails] ADD  DEFAULT ((1)) FOR [Position]
GO
ALTER TABLE [student].[EqualizationDetails] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[ExternalEvaluation] ADD  CONSTRAINT [DF_ExternalEvaluation_IsRepeat]  DEFAULT ((0)) FOR [IsRepeat]
GO
ALTER TABLE [student].[ExternalEvaluationType] ADD  CONSTRAINT [DF_ExternalEvaluationType_IsUnofficial]  DEFAULT ((0)) FOR [IsUnofficial]
GO
ALTER TABLE [student].[FirstToThirdClassGrade] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[FirstToThirdClassGrade] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[Founder] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[Founder] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[Grade] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[Grade] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[Grade] ADD  DEFAULT ((0)) FOR [IsSpecialGrade]
GO
ALTER TABLE [student].[GradeCategory] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [student].[GradeCategory] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[GradeCategory] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[GradeNom] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [student].[GradeNom] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[GradeNom] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[GradeType] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [student].[GradeType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[GradeType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[HealthInsuranceExport] ADD  DEFAULT ((0)) FOR [RecordsCount]
GO
ALTER TABLE [student].[HealthInsuranceExport] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [student].[HealthInsuranceIncomeRate] ADD  CONSTRAINT [DF_HealthInsuranceIncomeRate_Currency]  DEFAULT ('BGN') FOR [Currency]
GO
ALTER TABLE [student].[InstitutionChange] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[Language] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[Language] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[LodAssessment] ADD  DEFAULT ((0)) FOR [IsSelfEduForm]
GO
ALTER TABLE [student].[LodAssessment] ADD  DEFAULT ((0)) FOR [Position]
GO
ALTER TABLE [student].[LodAssessment] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[LodAssessment] ADD  DEFAULT ((0)) FOR [IsImported]
GO
ALTER TABLE [student].[LodAssessmentGrade] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[LodAssessmentTemplate] ADD  DEFAULT ((0)) FOR [IsSelfEduForm]
GO
ALTER TABLE [student].[LodAssessmentTemplate] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[LodEvaluationGeneral] ADD  DEFAULT ((0)) FOR [IsFinalized]
GO
ALTER TABLE [student].[LodEvaluationGeneral] ADD  DEFAULT ((1)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationGeneral] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[LodEvaluationSectionAB] ADD  DEFAULT ((1)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionAB] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[LodEvaluationSectionG] ADD  DEFAULT ((1)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionG] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[LodEvaluationSectionV] ADD  DEFAULT ((1)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionV] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[LodEvaluationsResult] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[LodEvaluationsResult] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[LODFinalization] ADD  CONSTRAINT [DF_LODFinalizationValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[LODFinalization] ADD  CONSTRAINT [DF_LODFinalizationValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] ADD  DEFAULT ((1)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[LodFirstGradeResult] ADD  DEFAULT ((0)) FOR [IsFinalized]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] ADD  DEFAULT ((1)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[Ministry] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[Ministry] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[Month] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[Month] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[NaturalIndicators] ADD  DEFAULT (getdate()) FOR [ExecutionTime]
GO
ALTER TABLE [student].[NaturalIndicatorsPrice] ADD  CONSTRAINT [DF_NaturalIndicatorsColumn_Currency]  DEFAULT ('BGN') FOR [Currency]
GO
ALTER TABLE [student].[Ores] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[OresAttachment] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[OresToEntity] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[ORESType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ORESType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[OtherDocument] ADD  CONSTRAINT [DF__OtherDocu__Creat__528FDD90]  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[OtherInstitution] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[OtherInstitution] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[OtherInstitution] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [student].[OtherInstitution] ADD  DEFAULT (getdate()) FOR [ModifyDate]
GO
ALTER TABLE [student].[Participation] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[Participation] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[Participation] ADD  DEFAULT ((1)) FOR [ValidForStudent]
GO
ALTER TABLE [student].[PersonalDevelopmentSupport] ADD  DEFAULT ((8)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[PersonalDevelopmentSupport] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[PersonalEarlyAssessment] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentLearningDisability] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[ReasonForEqualizationType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ReasonForEqualizationType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[ReasonForReassessmentType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ReasonForReassessmentType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[ReassessmentDetails] ADD  DEFAULT ((1)) FOR [Position]
GO
ALTER TABLE [student].[ReassessmentDetails] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[Recognition] ADD  CONSTRAINT [DF_Recognition_IsSelfEduForm]  DEFAULT ((0)) FOR [IsSelfEduForm]
GO
ALTER TABLE [student].[RecognitionEducationLevel] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[RecognitionEducationLevel] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[RecognitionEqualization] ADD  DEFAULT ((0)) FOR [IsRequired]
GO
ALTER TABLE [student].[RecognitionEqualization] ADD  DEFAULT ((1)) FOR [Position]
GO
ALTER TABLE [student].[RelocationDocument] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] ADD  DEFAULT ((0)) FOR [IsLoadedFromSchoolbook]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [student].[RepeaterReason] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[RepeaterReason] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[ResourceSupportReport] ADD  CONSTRAINT [DF_ResourceSupportReporttValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ResourceSupportReport] ADD  CONSTRAINT [DF_ResourceSupportReportValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [student].[ResourceSupportSpecialist] ADD  DEFAULT ((1)) FOR [WorkPlaceId]
GO
ALTER TABLE [student].[ResourceSupportSpecialist] ADD  CONSTRAINT [DF_ResourceSupportSpecialistValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ResourceSupportSpecialist] ADD  CONSTRAINT [DF_ResourceSupportSpecialistValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [student].[ResourceSupportSpecialistType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ResourceSupportSpecialistType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[ResourceSupportSpecialistWorkPlace] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ResourceSupportSpecialistWorkPlace] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[ResourceSupportType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ResourceSupportType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[SanctionType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SanctionType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[ScholarshipAmount] ADD  CONSTRAINT [DF__Scholarsh__Valid__746F28F1]  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ScholarshipAmount] ADD  CONSTRAINT [DF__Scholarsh__Valid__75634D2A]  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[ScholarshipFinancingOrgan] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ScholarshipFinancingOrgan] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[ScholarshipStudent] ADD  CONSTRAINT [DF__Scholarsh__Valid__27EECCF7]  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ScholarshipStudent] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[ScholarshipStudent] ADD  DEFAULT ((1)) FOR [CreatedBySysUserId]
GO
ALTER TABLE [student].[ScholarshipStudent] ADD  CONSTRAINT [DF_ScholarshipStudentStartSysTime]  DEFAULT (sysutcdatetime()) FOR [StartSysTime]
GO
ALTER TABLE [student].[ScholarshipStudent] ADD  CONSTRAINT [DF_ScholarshipStudentEndSysTime]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [EndSysTime]
GO
ALTER TABLE [student].[ScholarshipStudent] ADD  CONSTRAINT [DF_ScholarshipStudent_Currency]  DEFAULT ('BGN') FOR [Currency]
GO
ALTER TABLE [student].[ScholarshipType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[ScholarshipType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[SchoolTypeLodAccess] ADD  DEFAULT ((0)) FOR [isLodAccessAllowed]
GO
ALTER TABLE [student].[SchoolTypeLodAccess] ADD  CONSTRAINT [DF_SchoolTypeLodAccess_ValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SchoolTypeLodAccess] ADD  CONSTRAINT [DF_SchoolTypeLodAccess_ValidTo]  DEFAULT (CONVERT([datetime2](0),'9999-12-31 23:59:59')) FOR [ValidTo]
GO
ALTER TABLE [student].[SelfGovernment] ADD  DEFAULT ((1)) FOR [CreatedBySysUserID]
GO
ALTER TABLE [student].[SelfGovernment] ADD  CONSTRAINT [DF_StudentCreateDate]  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[SelfGovernment] ADD  CONSTRAINT [DF_SelfGovernmentValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SelfGovernment] ADD  CONSTRAINT [DF_SelfGovernmentValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [student].[SelfGovernmentPosition] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SelfGovernmentPosition] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[SelfGovernmentPosition] ADD  DEFAULT ((0)) FOR [ValidForStudent]
GO
ALTER TABLE [student].[SpecialNeeds] ADD  CONSTRAINT [DF_CreatedBySysUserID]  DEFAULT ((1)) FOR [CreatedBySysUserID]
GO
ALTER TABLE [student].[SpecialNeeds] ADD  CONSTRAINT [DF__SpecialNe__Creat__6E2616C8]  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[SpecialNeeds] ADD  CONSTRAINT [DF_SpecialNeedsValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SpecialNeeds] ADD  CONSTRAINT [DF_SpecialNeedsValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [student].[SpecialNeedsSubType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SpecialNeedsSubType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[SpecialNeedsType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SpecialNeedsType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[SpecialNeedsYear] ADD  CONSTRAINT [DF__SpecialNe__HasSu__6684F500]  DEFAULT ((0)) FOR [HasSuportiveEnvironment]
GO
ALTER TABLE [student].[SpecialNeedsYear] ADD  DEFAULT ((1)) FOR [CreatedBySysUserID]
GO
ALTER TABLE [student].[SpecialNeedsYear] ADD  CONSTRAINT [DF__SpecialNe__Creat__63A88855]  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[SpecialNeedsYear] ADD  CONSTRAINT [DF_SpecialNeedsYearValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SpecialNeedsYear] ADD  CONSTRAINT [DF_SpecialNeedsYearValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [student].[Student] ADD  DEFAULT ((0)) FOR [LivesWithFosterFamily]
GO
ALTER TABLE [student].[Student] ADD  DEFAULT ((0)) FOR [HasParentConsent]
GO
ALTER TABLE [student].[Student] ADD  DEFAULT ((0)) FOR [RepresentedByTheMajor]
GO
ALTER TABLE [student].[Student] ADD  DEFAULT ((0)) FOR [FamilyEducationWeight]
GO
ALTER TABLE [student].[Student] ADD  DEFAULT ((0)) FOR [FamilyWorkStatusWeight]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__HasIn__1C7D1A4B]  DEFAULT ((0)) FOR [IsIndividualCurriculum]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__IsHou__1D713E84]  DEFAULT ((0)) FOR [IsHourlyOrganization]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__IsFor__1E6562BD]  DEFAULT ((0)) FOR [IsForSubmissionToNRA]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__IsCur__1F5986F6]  DEFAULT ((0)) FOR [IsCurrent]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__HasSu__2235F3A1]  DEFAULT ((0)) FOR [HasSuportiveEnvironment]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__Enrol__232A17DA]  DEFAULT (getdate()) FOR [EnrollmentDate]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__Posit__797EC228]  DEFAULT ((3)) FOR [PositionId]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__Basic__25882CEF]  DEFAULT ((1)) FOR [BasicClassId]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF__StudentCl__Class__2864999A]  DEFAULT ((1)) FOR [ClassTypeId]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF_IsNotPresentForm]  DEFAULT ((0)) FOR [IsNotPresentForm]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF_StudentClassValidFrom]  DEFAULT (sysutcdatetime()) FOR [ValidFrom]
GO
ALTER TABLE [student].[StudentClass] ADD  CONSTRAINT [DF_StudentClassValidTo]  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [ValidTo]
GO
ALTER TABLE [student].[StudentClass] ADD  DEFAULT ((0)) FOR [ToDeleteByAdmin]
GO
ALTER TABLE [student].[StudentClassDualFormCompany] ADD  CONSTRAINT [DF_StudentClassDualFormCompany_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [student].[StudentClassDualFormCompany] ADD  CONSTRAINT [DF_StudentClassDualFormCompany_CreateDate]  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[StudentClassHistory] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [student].[StudentClassHistory] ADD  DEFAULT (sysutcdatetime()) FOR [CreateDate]
GO
ALTER TABLE [student].[StudentEvaluation] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [student].[StudentType] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[StudentType] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[SupportPeriod] ADD  DEFAULT (getdate()) FOR [ValidFrom]
GO
ALTER TABLE [student].[SupportPeriod] ADD  DEFAULT (NULL) FOR [ValidTo]
GO
ALTER TABLE [student].[Absence]  WITH CHECK ADD  CONSTRAINT [FK_Absence_AbsenceImport] FOREIGN KEY([AbsenceImportId])
REFERENCES [student].[AbsenceImport] ([Id])
GO
ALTER TABLE [student].[Absence] CHECK CONSTRAINT [FK_Absence_AbsenceImport]
GO
ALTER TABLE [student].[Absence]  WITH CHECK ADD  CONSTRAINT [FK_Absence_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Absence] CHECK CONSTRAINT [FK_Absence_CreatedSysUser]
GO
ALTER TABLE [student].[Absence]  WITH CHECK ADD  CONSTRAINT [FK_Absence_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[Absence] CHECK CONSTRAINT [FK_Absence_InstitutionSchoolYear]
GO
ALTER TABLE [student].[Absence]  WITH CHECK ADD  CONSTRAINT [FK_Absence_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Absence] CHECK CONSTRAINT [FK_Absence_ModifiedSysUser]
GO
ALTER TABLE [student].[Absence]  WITH CHECK ADD  CONSTRAINT [FK_Absence_Month] FOREIGN KEY([Month])
REFERENCES [student].[Month] ([Id])
GO
ALTER TABLE [student].[Absence] CHECK CONSTRAINT [FK_Absence_Month]
GO
ALTER TABLE [student].[Absence]  WITH CHECK ADD  CONSTRAINT [FK_Absence_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[Absence] CHECK CONSTRAINT [FK_Absence_Person]
GO
ALTER TABLE [student].[Absence]  WITH CHECK ADD  CONSTRAINT [FK_StudentAbsence_ClassGroup] FOREIGN KEY([ClassId])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [student].[Absence] CHECK CONSTRAINT [FK_StudentAbsence_ClassGroup]
GO
ALTER TABLE [student].[AbsenceCampaign]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceCampaign_Creator] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AbsenceCampaign] CHECK CONSTRAINT [FK_AbsenceCampaign_Creator]
GO
ALTER TABLE [student].[AbsenceCampaign]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceCampaign_CurrentSchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[AbsenceCampaign] CHECK CONSTRAINT [FK_AbsenceCampaign_CurrentSchoolYear]
GO
ALTER TABLE [student].[AbsenceCampaign]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceCampaign_Month] FOREIGN KEY([Month])
REFERENCES [student].[Month] ([Id])
GO
ALTER TABLE [student].[AbsenceCampaign] CHECK CONSTRAINT [FK_AbsenceCampaign_Month]
GO
ALTER TABLE [student].[AbsenceCampaign]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceCampaign_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AbsenceCampaign] CHECK CONSTRAINT [FK_AbsenceCampaign_Updater]
GO
ALTER TABLE [student].[AbsenceExport]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceExport_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AbsenceExport] CHECK CONSTRAINT [FK_AbsenceExport_CreatedSysUser]
GO
ALTER TABLE [student].[AbsenceExport]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceExport_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AbsenceExport] CHECK CONSTRAINT [FK_AbsenceExport_ModifiedSysUser]
GO
ALTER TABLE [student].[AbsenceExport]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceExport_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[AbsenceExport] CHECK CONSTRAINT [FK_AbsenceExport_SchoolYear]
GO
ALTER TABLE [student].[AbsenceHistory]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceHistory_Absence] FOREIGN KEY([AbsenceId])
REFERENCES [student].[Absence] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[AbsenceHistory] CHECK CONSTRAINT [FK_AbsenceHistory_Absence]
GO
ALTER TABLE [student].[AbsenceHistory]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceHistory_ModifiedSysUser] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AbsenceHistory] CHECK CONSTRAINT [FK_AbsenceHistory_ModifiedSysUser]
GO
ALTER TABLE [student].[AbsenceImport]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceImport_AbsenceCampaign] FOREIGN KEY([SchoolYear], [Month])
REFERENCES [student].[AbsenceCampaign] ([SchoolYear], [Month])
GO
ALTER TABLE [student].[AbsenceImport] CHECK CONSTRAINT [FK_AbsenceImport_AbsenceCampaign]
GO
ALTER TABLE [student].[AbsenceImport]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceImport_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AbsenceImport] CHECK CONSTRAINT [FK_AbsenceImport_CreatedSysUser]
GO
ALTER TABLE [student].[AbsenceImport]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceImport_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[AbsenceImport] CHECK CONSTRAINT [FK_AbsenceImport_InstitutionSchoolYear]
GO
ALTER TABLE [student].[AbsenceImport]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceImport_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AbsenceImport] CHECK CONSTRAINT [FK_AbsenceImport_ModifiedSysUser]
GO
ALTER TABLE [student].[AbsenceImport]  WITH CHECK ADD  CONSTRAINT [FK_AbsenceImport_Month] FOREIGN KEY([Month])
REFERENCES [student].[Month] ([Id])
GO
ALTER TABLE [student].[AbsenceImport] CHECK CONSTRAINT [FK_AbsenceImport_Month]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_CreatedBySysUserId]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_FinalSchoolYear] FOREIGN KEY([FinalSchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_FinalSchoolYear]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_ModifiedBySysUserId]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_PeriodType] FOREIGN KEY([PeriodTypeId])
REFERENCES [student].[SupportPeriod] ([Id])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_PeriodType]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_Person]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([TempCurrentYearID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_SchoolYear]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_StudentType] FOREIGN KEY([StudentTypeId])
REFERENCES [student].[StudentType] ([Id])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupport_StudentType]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportAttachment_Creator] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportAttachment_Creator]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportAttachment_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportAttachment_Document]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportAttachment_Support] FOREIGN KEY([AdditionalPersonalDevelopmentSupportId])
REFERENCES [student].[AdditionalPersonalDevelopmentSupport] ([Id])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportAttachment_Support]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportAttachment_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportAttachment_Updater]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItem_Creator] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItem_Creator]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItem_Support] FOREIGN KEY([AdditionalPersonalDevelopmentSupportId])
REFERENCES [student].[AdditionalPersonalDevelopmentSupport] ([Id])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItem_Support]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItem_Type] FOREIGN KEY([TypeId])
REFERENCES [student].[AdditionalSupportType] ([Id])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItem_Type]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItem_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItem] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItem_Updater]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItemAttachment_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItemAttachment_Document]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItemAttachment_SupportItem] FOREIGN KEY([AdditionalPersonalDevelopmentSupportItemId])
REFERENCES [student].[AdditionalPersonalDevelopmentSupportItem] ([Id])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItemAttachment_SupportItem]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItemAttachment_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItemAttachment_Updater]
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItemAttachmentt_Creator] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment] CHECK CONSTRAINT [FK_AdditionalPersonalDevelopmentSupportItemAttachmentt_Creator]
GO
ALTER TABLE [student].[AdmissionDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocument_AdmissionReasonType] FOREIGN KEY([AdmissionReasonTypeId])
REFERENCES [student].[AdmissionReasonType] ([Id])
GO
ALTER TABLE [student].[AdmissionDocument] CHECK CONSTRAINT [FK_AdmissionDocument_AdmissionReasonType]
GO
ALTER TABLE [student].[AdmissionDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocument_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdmissionDocument] CHECK CONSTRAINT [FK_AdmissionDocument_Creator_SysUser]
GO
ALTER TABLE [student].[AdmissionDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocument_Institution_SchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[AdmissionDocument] CHECK CONSTRAINT [FK_AdmissionDocument_Institution_SchoolYear]
GO
ALTER TABLE [student].[AdmissionDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocument_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[AdmissionDocument] CHECK CONSTRAINT [FK_AdmissionDocument_Person]
GO
ALTER TABLE [student].[AdmissionDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocument_RelocationDocument] FOREIGN KEY([RelocationDocumentId])
REFERENCES [student].[RelocationDocument] ([Id])
GO
ALTER TABLE [student].[AdmissionDocument] CHECK CONSTRAINT [FK_AdmissionDocument_RelocationDocument]
GO
ALTER TABLE [student].[AdmissionDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocument_StudentClass] FOREIGN KEY([CurrentStudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[AdmissionDocument] CHECK CONSTRAINT [FK_AdmissionDocument_StudentClass]
GO
ALTER TABLE [student].[AdmissionDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocument_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdmissionDocument] CHECK CONSTRAINT [FK_AdmissionDocument_Updater_SysUserd]
GO
ALTER TABLE [student].[AdmissionDocumentDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocumentDocument_AdmissionDocument] FOREIGN KEY([AdmissionDocumentId])
REFERENCES [student].[AdmissionDocument] ([Id])
GO
ALTER TABLE [student].[AdmissionDocumentDocument] CHECK CONSTRAINT [FK_AdmissionDocumentDocument_AdmissionDocument]
GO
ALTER TABLE [student].[AdmissionDocumentDocument]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionDocumentDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[AdmissionDocumentDocument] CHECK CONSTRAINT [FK_AdmissionDocumentDocument_Document]
GO
ALTER TABLE [student].[AdmissionPermissionRequest]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequest_AdmissionDocument] FOREIGN KEY([AdmissionDocumentId])
REFERENCES [student].[AdmissionDocument] ([Id])
GO
ALTER TABLE [student].[AdmissionPermissionRequest] CHECK CONSTRAINT [FK_AdmissionPermissionRequest_AdmissionDocument]
GO
ALTER TABLE [student].[AdmissionPermissionRequest]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequest_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdmissionPermissionRequest] CHECK CONSTRAINT [FK_AdmissionPermissionRequest_Creator_SysUser]
GO
ALTER TABLE [student].[AdmissionPermissionRequest]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequest_DischageDocument] FOREIGN KEY([DischargeDocumentId])
REFERENCES [student].[DischargeDocument] ([Id])
GO
ALTER TABLE [student].[AdmissionPermissionRequest] CHECK CONSTRAINT [FK_AdmissionPermissionRequest_DischageDocument]
GO
ALTER TABLE [student].[AdmissionPermissionRequest]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequest_InstitutionSchoolYear_AuthorizingInstitution] FOREIGN KEY([AuthorizingInstitutionId], [AuthorizingInstitutionSchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[AdmissionPermissionRequest] CHECK CONSTRAINT [FK_AdmissionPermissionRequest_InstitutionSchoolYear_AuthorizingInstitution]
GO
ALTER TABLE [student].[AdmissionPermissionRequest]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequest_InstitutionSchoolYear_RequestingInstitution] FOREIGN KEY([RequestingInstitutionId], [RequestingInstitutionSchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[AdmissionPermissionRequest] CHECK CONSTRAINT [FK_AdmissionPermissionRequest_InstitutionSchoolYear_RequestingInstitution]
GO
ALTER TABLE [student].[AdmissionPermissionRequest]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequest_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[AdmissionPermissionRequest] CHECK CONSTRAINT [FK_AdmissionPermissionRequest_Person]
GO
ALTER TABLE [student].[AdmissionPermissionRequest]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequest_RelocationDocument] FOREIGN KEY([RelocationDocumentId])
REFERENCES [student].[RelocationDocument] ([Id])
GO
ALTER TABLE [student].[AdmissionPermissionRequest] CHECK CONSTRAINT [FK_AdmissionPermissionRequest_RelocationDocument]
GO
ALTER TABLE [student].[AdmissionPermissionRequest]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequest_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AdmissionPermissionRequest] CHECK CONSTRAINT [FK_AdmissionPermissionRequest_Updater_SysUser]
GO
ALTER TABLE [student].[AdmissionPermissionRequestAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequestAttachment_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[AdmissionPermissionRequestAttachment] CHECK CONSTRAINT [FK_AdmissionPermissionRequestAttachment_Document]
GO
ALTER TABLE [student].[AdmissionPermissionRequestAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdmissionPermissionRequestAttachment_Equalization] FOREIGN KEY([AdmissionPermissionRequestId])
REFERENCES [student].[AdmissionPermissionRequest] ([Id])
GO
ALTER TABLE [student].[AdmissionPermissionRequestAttachment] CHECK CONSTRAINT [FK_AdmissionPermissionRequestAttachment_Equalization]
GO
ALTER TABLE [student].[AppSettingsForTenant]  WITH CHECK ADD  CONSTRAINT [FK_AppSettingsForTenant_SysUser_Creator] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AppSettingsForTenant] CHECK CONSTRAINT [FK_AppSettingsForTenant_SysUser_Creator]
GO
ALTER TABLE [student].[AppSettingsForTenant]  WITH CHECK ADD  CONSTRAINT [FK_AppSettingsForTenant_SysUser_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AppSettingsForTenant] CHECK CONSTRAINT [FK_AppSettingsForTenant_SysUser_Updater]
GO
ALTER TABLE [student].[ASPEnrolledStudentsExport]  WITH CHECK ADD  CONSTRAINT [FK_ASPEnrolledStudentsExport_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPEnrolledStudentsExport] CHECK CONSTRAINT [FK_ASPEnrolledStudentsExport_CreatedSysUser]
GO
ALTER TABLE [student].[ASPEnrolledStudentsExport]  WITH CHECK ADD  CONSTRAINT [FK_ASPEnrolledStudentsExport_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPEnrolledStudentsExport] CHECK CONSTRAINT [FK_ASPEnrolledStudentsExport_ModifiedSysUser]
GO
ALTER TABLE [student].[ASPMonthlyBenefit]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefit_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[ASPMonthlyBenefit] CHECK CONSTRAINT [FK_ASPMonthlyBenefit_Person]
GO
ALTER TABLE [student].[ASPMonthlyBenefit]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefits_ASPMonthlyBenefitsImport] FOREIGN KEY([ASPMonthlyBenefitsImportId])
REFERENCES [student].[ASPMonthlyBenefitsImport] ([Id])
GO
ALTER TABLE [student].[ASPMonthlyBenefit] CHECK CONSTRAINT [FK_ASPMonthlyBenefits_ASPMonthlyBenefitsImport]
GO
ALTER TABLE [student].[ASPMonthlyBenefit]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefits_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPMonthlyBenefit] CHECK CONSTRAINT [FK_ASPMonthlyBenefits_CreatedSysUser]
GO
ALTER TABLE [student].[ASPMonthlyBenefit]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefits_CurrentInstitutionSchoolYear] FOREIGN KEY([CurrentInstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[ASPMonthlyBenefit] CHECK CONSTRAINT [FK_ASPMonthlyBenefits_CurrentInstitutionSchoolYear]
GO
ALTER TABLE [student].[ASPMonthlyBenefit]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefits_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[ASPMonthlyBenefit] CHECK CONSTRAINT [FK_ASPMonthlyBenefits_InstitutionSchoolYear]
GO
ALTER TABLE [student].[ASPMonthlyBenefit]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefits_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPMonthlyBenefit] CHECK CONSTRAINT [FK_ASPMonthlyBenefits_ModifiedSysUser]
GO
ALTER TABLE [student].[ASPMonthlyBenefitInstitution]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefitImportInsitution_Import] FOREIGN KEY([ASPMonthlyBenefitImportId])
REFERENCES [student].[ASPMonthlyBenefitsImport] ([Id])
GO
ALTER TABLE [student].[ASPMonthlyBenefitInstitution] CHECK CONSTRAINT [FK_ASPMonthlyBenefitImportInsitution_Import]
GO
ALTER TABLE [student].[ASPMonthlyBenefitInstitution]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefitInstitution_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPMonthlyBenefitInstitution] CHECK CONSTRAINT [FK_ASPMonthlyBenefitInstitution_Creator_SysUser]
GO
ALTER TABLE [student].[ASPMonthlyBenefitInstitution]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefitInstitution_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPMonthlyBenefitInstitution] CHECK CONSTRAINT [FK_ASPMonthlyBenefitInstitution_Updater_SysUser]
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefitsImport_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport] CHECK CONSTRAINT [FK_ASPMonthlyBenefitsImport_CreatedSysUser]
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefitsImport_ExportedSysUser] FOREIGN KEY([ExportedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport] CHECK CONSTRAINT [FK_ASPMonthlyBenefitsImport_ExportedSysUser]
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefitsImport_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport] CHECK CONSTRAINT [FK_ASPMonthlyBenefitsImport_ModifiedSysUser]
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport]  WITH CHECK ADD  CONSTRAINT [FK_ASPMonthlyBenefitsImport_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport] CHECK CONSTRAINT [FK_ASPMonthlyBenefitsImport_SchoolYear]
GO
ALTER TABLE [student].[AspSubmittedDataHistory]  WITH CHECK ADD  CONSTRAINT [FK_AspExportedDataHistory_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AspSubmittedDataHistory] CHECK CONSTRAINT [FK_AspExportedDataHistory_CreatedSysUser]
GO
ALTER TABLE [student].[AspSubmittedDataHistory]  WITH CHECK ADD  CONSTRAINT [FK_AspSubmittedDataHistory_Month] FOREIGN KEY([Month])
REFERENCES [student].[Month] ([Id])
GO
ALTER TABLE [student].[AspSubmittedDataHistory] CHECK CONSTRAINT [FK_AspSubmittedDataHistory_Month]
GO
ALTER TABLE [student].[AspSubmittedDataHistory]  WITH CHECK ADD  CONSTRAINT [FK_AspSubmittedDataHistory_PersonalIdType] FOREIGN KEY([PersonalIdType])
REFERENCES [noms].[PersonalIDType] ([PersonalIDTypeID])
GO
ALTER TABLE [student].[AspSubmittedDataHistory] CHECK CONSTRAINT [FK_AspSubmittedDataHistory_PersonalIdType]
GO
ALTER TABLE [student].[AuditEntryProperties]  WITH CHECK ADD  CONSTRAINT [FK_student.AuditEntryProperties_student.AuditEntries_AuditEntryID] FOREIGN KEY([AuditEntryID])
REFERENCES [student].[AuditEntries] ([AuditEntryID])
ON DELETE CASCADE
GO
ALTER TABLE [student].[AuditEntryProperties] CHECK CONSTRAINT [FK_student.AuditEntryProperties_student.AuditEntries_AuditEntryID]
GO
ALTER TABLE [student].[AvailableArchitecture]  WITH CHECK ADD  CONSTRAINT [FK_AvailableArchitecture_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AvailableArchitecture] CHECK CONSTRAINT [FK_AvailableArchitecture_Creator_SysUser]
GO
ALTER TABLE [student].[AvailableArchitecture]  WITH CHECK ADD  CONSTRAINT [FK_AvailableArchitecture_ModernizationDegree] FOREIGN KEY([ModernizationDegreeId])
REFERENCES [inst_nom].[ModernizationDegree] ([ModernizationDegreeID])
GO
ALTER TABLE [student].[AvailableArchitecture] CHECK CONSTRAINT [FK_AvailableArchitecture_ModernizationDegree]
GO
ALTER TABLE [student].[AvailableArchitecture]  WITH CHECK ADD  CONSTRAINT [FK_AvailableArchitecture_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[AvailableArchitecture] CHECK CONSTRAINT [FK_AvailableArchitecture_Student]
GO
ALTER TABLE [student].[AvailableArchitecture]  WITH CHECK ADD  CONSTRAINT [FK_AvailableArchitecture_StudentClassId] FOREIGN KEY([StudentClassID])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[AvailableArchitecture] CHECK CONSTRAINT [FK_AvailableArchitecture_StudentClassId]
GO
ALTER TABLE [student].[AvailableArchitecture]  WITH CHECK ADD  CONSTRAINT [FK_AvailableArchitecture_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[AvailableArchitecture] CHECK CONSTRAINT [FK_AvailableArchitecture_Updater_SysUser]
GO
ALTER TABLE [student].[Award]  WITH CHECK ADD  CONSTRAINT [FK_Award_AwardCategory] FOREIGN KEY([AwardCategoryId])
REFERENCES [student].[AwardCategory] ([Id])
GO
ALTER TABLE [student].[Award] CHECK CONSTRAINT [FK_Award_AwardCategory]
GO
ALTER TABLE [student].[Award]  WITH CHECK ADD  CONSTRAINT [FK_Award_AwardReason] FOREIGN KEY([AwardReasonId])
REFERENCES [student].[AwardReason] ([Id])
GO
ALTER TABLE [student].[Award] CHECK CONSTRAINT [FK_Award_AwardReason]
GO
ALTER TABLE [student].[Award]  WITH CHECK ADD  CONSTRAINT [FK_Award_AwardType] FOREIGN KEY([AwardTypeId])
REFERENCES [student].[AwardTypes] ([Id])
GO
ALTER TABLE [student].[Award] CHECK CONSTRAINT [FK_Award_AwardType]
GO
ALTER TABLE [student].[Award]  WITH CHECK ADD  CONSTRAINT [FK_Award_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Award] CHECK CONSTRAINT [FK_Award_Creator_SysUser]
GO
ALTER TABLE [student].[Award]  WITH CHECK ADD  CONSTRAINT [FK_Award_Founder] FOREIGN KEY([FounderId])
REFERENCES [student].[Founder] ([Id])
GO
ALTER TABLE [student].[Award] CHECK CONSTRAINT [FK_Award_Founder]
GO
ALTER TABLE [student].[Award]  WITH CHECK ADD  CONSTRAINT [FK_Award_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[Award] CHECK CONSTRAINT [FK_Award_InstitutionSchoolYear]
GO
ALTER TABLE [student].[Award]  WITH CHECK ADD  CONSTRAINT [FK_Award_Student] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[Award] CHECK CONSTRAINT [FK_Award_Student]
GO
ALTER TABLE [student].[Award]  WITH CHECK ADD  CONSTRAINT [FK_Award_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Award] CHECK CONSTRAINT [FK_Award_Updater_SysUser]
GO
ALTER TABLE [student].[AwardDocument]  WITH CHECK ADD  CONSTRAINT [FK_AwardDocument_Award] FOREIGN KEY([AwardId])
REFERENCES [student].[Award] ([Id])
GO
ALTER TABLE [student].[AwardDocument] CHECK CONSTRAINT [FK_AwardDocument_Award]
GO
ALTER TABLE [student].[AwardDocument]  WITH CHECK ADD  CONSTRAINT [FK_AwardDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[AwardDocument] CHECK CONSTRAINT [FK_AwardDocument_Document]
GO
ALTER TABLE [student].[BuildingArea]  WITH CHECK ADD  CONSTRAINT [FK_BuildingArea_BuildingAreaType] FOREIGN KEY([BuildingAreaTypeId])
REFERENCES [inst_nom].[BuildingAreaType] ([BuildingAreaTypeID])
GO
ALTER TABLE [student].[BuildingArea] CHECK CONSTRAINT [FK_BuildingArea_BuildingAreaType]
GO
ALTER TABLE [student].[BuildingArea]  WITH CHECK ADD  CONSTRAINT [FK_BuildingArea_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[BuildingArea] CHECK CONSTRAINT [FK_BuildingArea_Creator_SysUser]
GO
ALTER TABLE [student].[BuildingArea]  WITH CHECK ADD  CONSTRAINT [FK_BuildingArea_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[BuildingArea] CHECK CONSTRAINT [FK_BuildingArea_Student]
GO
ALTER TABLE [student].[BuildingArea]  WITH CHECK ADD  CONSTRAINT [FK_BuildingArea_StudentClassId] FOREIGN KEY([StudentClassID])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[BuildingArea] CHECK CONSTRAINT [FK_BuildingArea_StudentClassId]
GO
ALTER TABLE [student].[BuildingArea]  WITH CHECK ADD  CONSTRAINT [FK_BuildingArea_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[BuildingArea] CHECK CONSTRAINT [FK_BuildingArea_Updater_SysUser]
GO
ALTER TABLE [student].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoom_BuildingRoomType] FOREIGN KEY([BuildingRoomTypeID])
REFERENCES [inst_nom].[BuildingRoomType] ([BuildingRoomTypeID])
GO
ALTER TABLE [student].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoom_BuildingRoomType]
GO
ALTER TABLE [student].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoom_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoom_Creator_SysUser]
GO
ALTER TABLE [student].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoom_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoom_Student]
GO
ALTER TABLE [student].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoom_StudentClassId] FOREIGN KEY([StudentClassID])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoom_StudentClassId]
GO
ALTER TABLE [student].[BuildingRoom]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoom_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[BuildingRoom] CHECK CONSTRAINT [FK_BuildingRoom_Updater_SysUser]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupport_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupport_CreatedBySysUserId]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupport_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupport_ModifiedBySysUserId]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupport_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupport_Person]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupport_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupport] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupport_SchoolYear]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupportAttachment_Creator] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupportAttachment_Creator]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupportAttachment_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupportAttachment_Document]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupportAttachment_Support] FOREIGN KEY([CommonPersonalDevelopmentSupportId])
REFERENCES [student].[CommonPersonalDevelopmentSupport] ([Id])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupportAttachment_Support]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupportAttachment_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportAttachment] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupportAttachment_Updater]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportItem]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupportDocument_Type] FOREIGN KEY([TypeId])
REFERENCES [student].[CommonSupportType] ([Id])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportItem] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupportDocument_Type]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportItem]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupportItem_Creator] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportItem] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupportItem_Creator]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportItem]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupportItem_Support] FOREIGN KEY([CommonPersonalDevelopmentSupportId])
REFERENCES [student].[CommonPersonalDevelopmentSupport] ([Id])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportItem] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupportItem_Support]
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportItem]  WITH CHECK ADD  CONSTRAINT [FK_CommonPersonalDevelopmentSupportItem_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[CommonPersonalDevelopmentSupportItem] CHECK CONSTRAINT [FK_CommonPersonalDevelopmentSupportItem_Updater]
GO
ALTER TABLE [student].[DischargeDocument]  WITH CHECK ADD  CONSTRAINT [FK_DischargeDocumen_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[DischargeDocument] CHECK CONSTRAINT [FK_DischargeDocumen_Updater_SysUserd]
GO
ALTER TABLE [student].[DischargeDocument]  WITH CHECK ADD  CONSTRAINT [FK_DischargeDocument_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[DischargeDocument] CHECK CONSTRAINT [FK_DischargeDocument_Creator_SysUser]
GO
ALTER TABLE [student].[DischargeDocument]  WITH CHECK ADD  CONSTRAINT [FK_DischargeDocument_DischargeReasonType] FOREIGN KEY([DischargeReasonTypeId])
REFERENCES [student].[DischargeReasonType] ([Id])
GO
ALTER TABLE [student].[DischargeDocument] CHECK CONSTRAINT [FK_DischargeDocument_DischargeReasonType]
GO
ALTER TABLE [student].[DischargeDocument]  WITH CHECK ADD  CONSTRAINT [FK_DischargeDocument_Institution_SchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[DischargeDocument] CHECK CONSTRAINT [FK_DischargeDocument_Institution_SchoolYear]
GO
ALTER TABLE [student].[DischargeDocument]  WITH CHECK ADD  CONSTRAINT [FK_DischargeDocument_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[DischargeDocument] CHECK CONSTRAINT [FK_DischargeDocument_Person]
GO
ALTER TABLE [student].[DischargeDocument]  WITH CHECK ADD  CONSTRAINT [FK_DischargeDocument_StudentClass] FOREIGN KEY([CurrentStudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[DischargeDocument] CHECK CONSTRAINT [FK_DischargeDocument_StudentClass]
GO
ALTER TABLE [student].[DischargeDocumentDocument]  WITH CHECK ADD  CONSTRAINT [FK_DischargeDocumentDocument_DischargeDocument] FOREIGN KEY([DischargeDocumentId])
REFERENCES [student].[DischargeDocument] ([Id])
GO
ALTER TABLE [student].[DischargeDocumentDocument] CHECK CONSTRAINT [FK_DischargeDocumentDocument_DischargeDocument]
GO
ALTER TABLE [student].[DischargeDocumentDocument]  WITH CHECK ADD  CONSTRAINT [FK_DischargeDocumentDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[DischargeDocumentDocument] CHECK CONSTRAINT [FK_DischargeDocumentDocument_Document]
GO
ALTER TABLE [student].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Document] CHECK CONSTRAINT [FK_Document_Creator_SysUser]
GO
ALTER TABLE [student].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Document] CHECK CONSTRAINT [FK_Document_Updater_SysUserd]
GO
ALTER TABLE [student].[Equalization]  WITH CHECK ADD  CONSTRAINT [FK_Equalization_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Equalization] CHECK CONSTRAINT [FK_Equalization_Creator_SysUser]
GO
ALTER TABLE [student].[Equalization]  WITH CHECK ADD  CONSTRAINT [FK_Equalization_ReasonForEqualizationType] FOREIGN KEY([ReasonId])
REFERENCES [student].[ReasonForEqualizationType] ([Id])
GO
ALTER TABLE [student].[Equalization] CHECK CONSTRAINT [FK_Equalization_ReasonForEqualizationType]
GO
ALTER TABLE [student].[Equalization]  WITH CHECK ADD  CONSTRAINT [FK_Equalization_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[Equalization] CHECK CONSTRAINT [FK_Equalization_SchoolYear]
GO
ALTER TABLE [student].[Equalization]  WITH CHECK ADD  CONSTRAINT [FK_Equalization_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[Equalization] CHECK CONSTRAINT [FK_Equalization_Student]
GO
ALTER TABLE [student].[Equalization]  WITH CHECK ADD  CONSTRAINT [FK_Equalization_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Equalization] CHECK CONSTRAINT [FK_Equalization_Updater_SysUser]
GO
ALTER TABLE [student].[Equalization]  WITH CHECK ADD  CONSTRAINT [FK_Equalizationt_BasicClass] FOREIGN KEY([InClass])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[Equalization] CHECK CONSTRAINT [FK_Equalizationt_BasicClass]
GO
ALTER TABLE [student].[EqualizationAttachment]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationAttachment_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[EqualizationAttachment] CHECK CONSTRAINT [FK_EqualizationAttachment_Document]
GO
ALTER TABLE [student].[EqualizationAttachment]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationAttachment_Equalization] FOREIGN KEY([EqualizationId])
REFERENCES [student].[Equalization] ([Id])
GO
ALTER TABLE [student].[EqualizationAttachment] CHECK CONSTRAINT [FK_EqualizationAttachment_Equalization]
GO
ALTER TABLE [student].[EqualizationDetails]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationDetails_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[EqualizationDetails] CHECK CONSTRAINT [FK_EqualizationDetails_BasicClass]
GO
ALTER TABLE [student].[EqualizationDetails]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationDetails_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[EqualizationDetails] CHECK CONSTRAINT [FK_EqualizationDetails_Creator_SysUser]
GO
ALTER TABLE [student].[EqualizationDetails]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationDetails_Equalization] FOREIGN KEY([EqualizationId])
REFERENCES [student].[Equalization] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[EqualizationDetails] CHECK CONSTRAINT [FK_EqualizationDetails_Equalization]
GO
ALTER TABLE [student].[EqualizationDetails]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationDetails_GradeCategory] FOREIGN KEY([GradeCategory])
REFERENCES [student].[GradeType] ([Id])
GO
ALTER TABLE [student].[EqualizationDetails] CHECK CONSTRAINT [FK_EqualizationDetails_GradeCategory]
GO
ALTER TABLE [student].[EqualizationDetails]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationDetails_GradeCategory_SessionId] FOREIGN KEY([SessionId])
REFERENCES [student].[GradeCategory] ([Id])
GO
ALTER TABLE [student].[EqualizationDetails] CHECK CONSTRAINT [FK_EqualizationDetails_GradeCategory_SessionId]
GO
ALTER TABLE [student].[EqualizationDetails]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationDetails_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[EqualizationDetails] CHECK CONSTRAINT [FK_EqualizationDetails_Subject]
GO
ALTER TABLE [student].[EqualizationDetails]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationDetails_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[EqualizationDetails] CHECK CONSTRAINT [FK_EqualizationDetails_SubjectType]
GO
ALTER TABLE [student].[EqualizationDetails]  WITH CHECK ADD  CONSTRAINT [FK_EqualizationDetails_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[EqualizationDetails] CHECK CONSTRAINT [FK_EqualizationDetails_Updater_SysUserd]
GO
ALTER TABLE [student].[ExternalEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluation_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ExternalEvaluation] CHECK CONSTRAINT [FK_ExternalEvaluation_Creator_SysUser]
GO
ALTER TABLE [student].[ExternalEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluation_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[ExternalEvaluation] CHECK CONSTRAINT [FK_ExternalEvaluation_CurrentYear]
GO
ALTER TABLE [student].[ExternalEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluation_ExternalEvaluationType] FOREIGN KEY([ExternalEvaluationTypeId])
REFERENCES [student].[ExternalEvaluationType] ([Id])
GO
ALTER TABLE [student].[ExternalEvaluation] CHECK CONSTRAINT [FK_ExternalEvaluation_ExternalEvaluationType]
GO
ALTER TABLE [student].[ExternalEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluation_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[ExternalEvaluation] CHECK CONSTRAINT [FK_ExternalEvaluation_Person]
GO
ALTER TABLE [student].[ExternalEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluation_Self] FOREIGN KEY([ParentId])
REFERENCES [student].[ExternalEvaluation] ([Id])
GO
ALTER TABLE [student].[ExternalEvaluation] CHECK CONSTRAINT [FK_ExternalEvaluation_Self]
GO
ALTER TABLE [student].[ExternalEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluation_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ExternalEvaluation] CHECK CONSTRAINT [FK_ExternalEvaluation_Updater_SysUser]
GO
ALTER TABLE [student].[ExternalEvaluationItem]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluationItem_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ExternalEvaluationItem] CHECK CONSTRAINT [FK_ExternalEvaluationItem_Creator_SysUser]
GO
ALTER TABLE [student].[ExternalEvaluationItem]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluationItem_ExternalEvaluation] FOREIGN KEY([ExternalEvaluationId])
REFERENCES [student].[ExternalEvaluation] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[ExternalEvaluationItem] CHECK CONSTRAINT [FK_ExternalEvaluationItem_ExternalEvaluation]
GO
ALTER TABLE [student].[ExternalEvaluationItem]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluationItem_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[ExternalEvaluationItem] CHECK CONSTRAINT [FK_ExternalEvaluationItem_Subject]
GO
ALTER TABLE [student].[ExternalEvaluationItem]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluationItem_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[ExternalEvaluationItem] CHECK CONSTRAINT [FK_ExternalEvaluationItem_SubjectType]
GO
ALTER TABLE [student].[ExternalEvaluationItem]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluationItem_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ExternalEvaluationItem] CHECK CONSTRAINT [FK_ExternalEvaluationItem_Updater_SysUser]
GO
ALTER TABLE [student].[ExternalEvaluationType]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluationType_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[ExternalEvaluationType] CHECK CONSTRAINT [FK_ExternalEvaluationType_BasicClass]
GO
ALTER TABLE [student].[ExternalEvaluationType]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluationType_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ExternalEvaluationType] CHECK CONSTRAINT [FK_ExternalEvaluationType_Creator_SysUser]
GO
ALTER TABLE [student].[ExternalEvaluationType]  WITH CHECK ADD  CONSTRAINT [FK_ExternalEvaluationType_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ExternalEvaluationType] CHECK CONSTRAINT [FK_ExternalEvaluationType_Updater_SysUser]
GO
ALTER TABLE [student].[GradeNom]  WITH CHECK ADD  CONSTRAINT [FK_GradeNom_GradeType] FOREIGN KEY([GradeTypeId])
REFERENCES [student].[GradeType] ([Id])
GO
ALTER TABLE [student].[GradeNom] CHECK CONSTRAINT [FK_GradeNom_GradeType]
GO
ALTER TABLE [student].[GRAOCampaign]  WITH CHECK ADD  CONSTRAINT [FK_GRAOCampaign_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[GRAOCampaign] CHECK CONSTRAINT [FK_GRAOCampaign_Creator_SysUser]
GO
ALTER TABLE [student].[GRAOCampaign]  WITH CHECK ADD  CONSTRAINT [FK_GRAOCampaign_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[GRAOCampaign] CHECK CONSTRAINT [FK_GRAOCampaign_Updater_SysUser]
GO
ALTER TABLE [student].[GRAOPerson]  WITH NOCHECK ADD  CONSTRAINT [FK_GRAOPerson_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[GRAOPerson] CHECK CONSTRAINT [FK_GRAOPerson_Creator_SysUser]
GO
ALTER TABLE [student].[GRAOPerson]  WITH NOCHECK ADD  CONSTRAINT [FK_GRAOPerson_GRAOCampaignID] FOREIGN KEY([GRAOCampaignID])
REFERENCES [student].[GRAOCampaign] ([Id])
GO
ALTER TABLE [student].[GRAOPerson] CHECK CONSTRAINT [FK_GRAOPerson_GRAOCampaignID]
GO
ALTER TABLE [student].[GRAOPerson]  WITH NOCHECK ADD  CONSTRAINT [FK_GRAOPerson_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[GRAOPerson] CHECK CONSTRAINT [FK_GRAOPerson_Updater_SysUser]
GO
ALTER TABLE [student].[HealthInsuranceExport]  WITH CHECK ADD  CONSTRAINT [FK_HealthInsuranceExport_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[HealthInsuranceExport] CHECK CONSTRAINT [FK_HealthInsuranceExport_CreatedSysUser]
GO
ALTER TABLE [student].[HealthInsuranceExport]  WITH CHECK ADD  CONSTRAINT [FK_HealthInsuranceExport_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[HealthInsuranceExport] CHECK CONSTRAINT [FK_HealthInsuranceExport_ModifiedSysUser]
GO
ALTER TABLE [student].[HealthInsuranceExport]  WITH CHECK ADD  CONSTRAINT [FK_HealthInsuranceExport_Month] FOREIGN KEY([Month])
REFERENCES [student].[Month] ([Id])
GO
ALTER TABLE [student].[HealthInsuranceExport] CHECK CONSTRAINT [FK_HealthInsuranceExport_Month]
GO
ALTER TABLE [student].[InstitutionChange]  WITH CHECK ADD  CONSTRAINT [FK_InstitutionChange_InstitutionSchoolYear_Institution] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[InstitutionChange] CHECK CONSTRAINT [FK_InstitutionChange_InstitutionSchoolYear_Institution]
GO
ALTER TABLE [student].[InternationalMobility]  WITH CHECK ADD  CONSTRAINT [FK_InternationalMobility_Country_Country] FOREIGN KEY([CountryId])
REFERENCES [location].[Country] ([CountryID])
GO
ALTER TABLE [student].[InternationalMobility] CHECK CONSTRAINT [FK_InternationalMobility_Country_Country]
GO
ALTER TABLE [student].[InternationalMobility]  WITH CHECK ADD  CONSTRAINT [FK_InternationalMobility_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[InternationalMobility] CHECK CONSTRAINT [FK_InternationalMobility_Creator_SysUser]
GO
ALTER TABLE [student].[InternationalMobility]  WITH CHECK ADD  CONSTRAINT [FK_InternationalMobility_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[InternationalMobility] CHECK CONSTRAINT [FK_InternationalMobility_InstitutionSchoolYear]
GO
ALTER TABLE [student].[InternationalMobility]  WITH CHECK ADD  CONSTRAINT [FK_InternationalMobility_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[InternationalMobility] CHECK CONSTRAINT [FK_InternationalMobility_SchoolYear]
GO
ALTER TABLE [student].[InternationalMobility]  WITH CHECK ADD  CONSTRAINT [FK_InternationalMobility_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[InternationalMobility] CHECK CONSTRAINT [FK_InternationalMobility_Student]
GO
ALTER TABLE [student].[InternationalMobility]  WITH CHECK ADD  CONSTRAINT [FK_InternationalMobility_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[InternationalMobility] CHECK CONSTRAINT [FK_InternationalMobility_Updater_SysUser]
GO
ALTER TABLE [student].[InternationalMobilityDocument]  WITH CHECK ADD  CONSTRAINT [FK_InternationalMobilityDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[InternationalMobilityDocument] CHECK CONSTRAINT [FK_InternationalMobilityDocument_Document]
GO
ALTER TABLE [student].[InternationalMobilityDocument]  WITH CHECK ADD  CONSTRAINT [FK_InternationalMobilityDocument_InternationalMobility] FOREIGN KEY([InternationalMobilityId])
REFERENCES [student].[InternationalMobility] ([Id])
GO
ALTER TABLE [student].[InternationalMobilityDocument] CHECK CONSTRAINT [FK_InternationalMobilityDocument_InternationalMobility]
GO
ALTER TABLE [student].[InternationalProtection]  WITH CHECK ADD  CONSTRAINT [FK_InternationalProtection_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[InternationalProtection] CHECK CONSTRAINT [FK_InternationalProtection_Person]
GO
ALTER TABLE [student].[LeadTeacher]  WITH CHECK ADD  CONSTRAINT [FK_LeadTeacher_ClassGroup] FOREIGN KEY([ClassId])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [student].[LeadTeacher] CHECK CONSTRAINT [FK_LeadTeacher_ClassGroup]
GO
ALTER TABLE [student].[LeadTeacher]  WITH CHECK ADD  CONSTRAINT [FK_LeadTeacher_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LeadTeacher] CHECK CONSTRAINT [FK_LeadTeacher_Creator_SysUser]
GO
ALTER TABLE [student].[LeadTeacher]  WITH CHECK ADD  CONSTRAINT [FK_LeadTeacher_StaffPosition] FOREIGN KEY([StaffPositionId])
REFERENCES [inst_basic].[StaffPosition] ([StaffPositionID])
ON DELETE CASCADE
GO
ALTER TABLE [student].[LeadTeacher] CHECK CONSTRAINT [FK_LeadTeacher_StaffPosition]
GO
ALTER TABLE [student].[LeadTeacher]  WITH CHECK ADD  CONSTRAINT [FK_LeadTeacher_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LeadTeacher] CHECK CONSTRAINT [FK_LeadTeacher_Updater_SysUser]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_BasicClass]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_Creator_SysUser]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_CurriculumPart] FOREIGN KEY([CurriculumPartId])
REFERENCES [inst_nom].[CurriculumPart] ([CurriculumPartID])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_CurriculumPart]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_FLSubject] FOREIGN KEY([FlSubjectId])
REFERENCES [inst_nom].[FL] ([FLID])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_FLSubject]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_InstitutionSchoolYear]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_Person]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_Self] FOREIGN KEY([ParentId])
REFERENCES [student].[LodAssessment] ([Id])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_Self]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_Subject]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_SubjectType]
GO
ALTER TABLE [student].[LodAssessment]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessment_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodAssessment] CHECK CONSTRAINT [FK_LodAssessment_Updater_SysUser]
GO
ALTER TABLE [student].[LodAssessmentGrade]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessmentGrade_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodAssessmentGrade] CHECK CONSTRAINT [FK_LodAssessmentGrade_Creator_SysUser]
GO
ALTER TABLE [student].[LodAssessmentGrade]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessmentGrade_GradeCategory] FOREIGN KEY([GradeCategoryId])
REFERENCES [student].[GradeCategory] ([Id])
GO
ALTER TABLE [student].[LodAssessmentGrade] CHECK CONSTRAINT [FK_LodAssessmentGrade_GradeCategory]
GO
ALTER TABLE [student].[LodAssessmentGrade]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessmentGrade_GradeNom] FOREIGN KEY([GradeId])
REFERENCES [student].[GradeNom] ([Id])
GO
ALTER TABLE [student].[LodAssessmentGrade] CHECK CONSTRAINT [FK_LodAssessmentGrade_GradeNom]
GO
ALTER TABLE [student].[LodAssessmentGrade]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessmentGrade_LodAssessment] FOREIGN KEY([LodAssessmentId])
REFERENCES [student].[LodAssessment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[LodAssessmentGrade] CHECK CONSTRAINT [FK_LodAssessmentGrade_LodAssessment]
GO
ALTER TABLE [student].[LodAssessmentGrade]  WITH CHECK ADD  CONSTRAINT [FK_LodAssessmentGrade_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodAssessmentGrade] CHECK CONSTRAINT [FK_LodAssessmentGrade_Updater_SysUser]
GO
ALTER TABLE [student].[LodAssessmentTemplate]  WITH NOCHECK ADD  CONSTRAINT [FK_LodAssessmentTemplate_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[LodAssessmentTemplate] CHECK CONSTRAINT [FK_LodAssessmentTemplate_BasicClass]
GO
ALTER TABLE [student].[LodAssessmentTemplate]  WITH NOCHECK ADD  CONSTRAINT [FK_LodAssessmentTemplate_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodAssessmentTemplate] CHECK CONSTRAINT [FK_LodAssessmentTemplate_Creator_SysUser]
GO
ALTER TABLE [student].[LodAssessmentTemplate]  WITH NOCHECK ADD  CONSTRAINT [FK_LodAssessmentTemplate_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[LodAssessmentTemplate] CHECK CONSTRAINT [FK_LodAssessmentTemplate_InstitutionSchoolYear]
GO
ALTER TABLE [student].[LodAssessmentTemplate]  WITH NOCHECK ADD  CONSTRAINT [FK_LodAssessmentTemplate_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodAssessmentTemplate] CHECK CONSTRAINT [FK_LodAssessmentTemplate_Updater_SysUser]
GO
ALTER TABLE [student].[LodEvaluationGeneral]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationGeneral_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[LodEvaluationGeneral] CHECK CONSTRAINT [FK_LodEvaluationGeneral_CurrentYear]
GO
ALTER TABLE [student].[LodEvaluationGeneral]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationGeneral_LodEvaluationsResult] FOREIGN KEY([LodEvaluationsResultId])
REFERENCES [student].[LodEvaluationsResult] ([Id])
GO
ALTER TABLE [student].[LodEvaluationGeneral] CHECK CONSTRAINT [FK_LodEvaluationGeneral_LodEvaluationsResult]
GO
ALTER TABLE [student].[LodEvaluationGeneral]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationGeneral_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LodEvaluationGeneral] CHECK CONSTRAINT [FK_LodEvaluationGeneral_Person]
GO
ALTER TABLE [student].[LodEvaluationGeneral]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationGeneral_Person_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodEvaluationGeneral] CHECK CONSTRAINT [FK_LodEvaluationGeneral_Person_CreatedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationGeneral]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationGeneral_Person_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodEvaluationGeneral] CHECK CONSTRAINT [FK_LodEvaluationGeneral_Person_ModifiedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationGeneral]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationGeneral_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[LodEvaluationGeneral] CHECK CONSTRAINT [FK_LodEvaluationGeneral_StudentClass]
GO
ALTER TABLE [student].[LodEvaluationSectionAB]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionAB_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[LodEvaluationSectionAB] CHECK CONSTRAINT [FK_LodEvaluationSectionAB_CurrentYear]
GO
ALTER TABLE [student].[LodEvaluationSectionAB]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionAB_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LodEvaluationSectionAB] CHECK CONSTRAINT [FK_LodEvaluationSectionAB_Person]
GO
ALTER TABLE [student].[LodEvaluationSectionAB]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionAB_Person_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodEvaluationSectionAB] CHECK CONSTRAINT [FK_LodEvaluationSectionAB_Person_CreatedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionAB]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionAB_Person_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodEvaluationSectionAB] CHECK CONSTRAINT [FK_LodEvaluationSectionAB_Person_ModifiedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionAB]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionAb_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[LodEvaluationSectionAB] CHECK CONSTRAINT [FK_LodEvaluationSectionAb_StudentClass]
GO
ALTER TABLE [student].[LodEvaluationSectionAB]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionAB_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[LodEvaluationSectionAB] CHECK CONSTRAINT [FK_LodEvaluationSectionAB_Subject]
GO
ALTER TABLE [student].[LodEvaluationSectionAB]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionAB_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[LodEvaluationSectionAB] CHECK CONSTRAINT [FK_LodEvaluationSectionAB_SubjectType]
GO
ALTER TABLE [student].[LodEvaluationSectionG]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionG_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[LodEvaluationSectionG] CHECK CONSTRAINT [FK_LodEvaluationSectionG_CurrentYear]
GO
ALTER TABLE [student].[LodEvaluationSectionG]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionG_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LodEvaluationSectionG] CHECK CONSTRAINT [FK_LodEvaluationSectionG_Person]
GO
ALTER TABLE [student].[LodEvaluationSectionG]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionG_Person_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodEvaluationSectionG] CHECK CONSTRAINT [FK_LodEvaluationSectionG_Person_CreatedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionG]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionG_Person_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodEvaluationSectionG] CHECK CONSTRAINT [FK_LodEvaluationSectionG_Person_ModifiedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionG]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionG_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[LodEvaluationSectionG] CHECK CONSTRAINT [FK_LodEvaluationSectionG_StudentClass]
GO
ALTER TABLE [student].[LodEvaluationSectionG]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionG_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[LodEvaluationSectionG] CHECK CONSTRAINT [FK_LodEvaluationSectionG_Subject]
GO
ALTER TABLE [student].[LodEvaluationSectionG]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionG_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[LodEvaluationSectionG] CHECK CONSTRAINT [FK_LodEvaluationSectionG_SubjectType]
GO
ALTER TABLE [student].[LodEvaluationSectionV]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionV_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[LodEvaluationSectionV] CHECK CONSTRAINT [FK_LodEvaluationSectionV_CurrentYear]
GO
ALTER TABLE [student].[LodEvaluationSectionV]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionV_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LodEvaluationSectionV] CHECK CONSTRAINT [FK_LodEvaluationSectionV_Person]
GO
ALTER TABLE [student].[LodEvaluationSectionV]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionV_Person_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodEvaluationSectionV] CHECK CONSTRAINT [FK_LodEvaluationSectionV_Person_CreatedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionV]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionV_Person_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodEvaluationSectionV] CHECK CONSTRAINT [FK_LodEvaluationSectionV_Person_ModifiedBySysUserId]
GO
ALTER TABLE [student].[LodEvaluationSectionV]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionV_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[LodEvaluationSectionV] CHECK CONSTRAINT [FK_LodEvaluationSectionV_StudentClass]
GO
ALTER TABLE [student].[LodEvaluationSectionV]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionV_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[LodEvaluationSectionV] CHECK CONSTRAINT [FK_LodEvaluationSectionV_Subject]
GO
ALTER TABLE [student].[LodEvaluationSectionV]  WITH CHECK ADD  CONSTRAINT [FK_LodEvaluationSectionV_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[LodEvaluationSectionV] CHECK CONSTRAINT [FK_LodEvaluationSectionV_SubjectType]
GO
ALTER TABLE [student].[LODFinalization]  WITH CHECK ADD  CONSTRAINT [FK_LODFinalization_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LODFinalization] CHECK CONSTRAINT [FK_LODFinalization_Creator_SysUser]
GO
ALTER TABLE [student].[LODFinalization]  WITH CHECK ADD  CONSTRAINT [FK_LODFinalization_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[LODFinalization] CHECK CONSTRAINT [FK_LODFinalization_Document]
GO
ALTER TABLE [student].[LODFinalization]  WITH CHECK ADD  CONSTRAINT [FK_LODFinalization_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LODFinalization] CHECK CONSTRAINT [FK_LODFinalization_Person]
GO
ALTER TABLE [student].[LODFinalization]  WITH CHECK ADD  CONSTRAINT [FK_Lodfinalization_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[LODFinalization] CHECK CONSTRAINT [FK_Lodfinalization_StudentClass]
GO
ALTER TABLE [student].[LODFinalization]  WITH CHECK ADD  CONSTRAINT [FK_LODFinalization_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LODFinalization] CHECK CONSTRAINT [FK_LODFinalization_Updater_SysUser]
GO
ALTER TABLE [student].[LodFinalizationSignatory]  WITH CHECK ADD  CONSTRAINT [FK_LodFinalizationSignatory_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodFinalizationSignatory] CHECK CONSTRAINT [FK_LodFinalizationSignatory_Creator_SysUser]
GO
ALTER TABLE [student].[LodFinalizationSignatory]  WITH CHECK ADD  CONSTRAINT [FK_LODFinalizationSignatory_LODFinalization] FOREIGN KEY([LODFinalizationId])
REFERENCES [student].[LODFinalization] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[LodFinalizationSignatory] CHECK CONSTRAINT [FK_LODFinalizationSignatory_LODFinalization]
GO
ALTER TABLE [student].[LodFinalizationSignatory]  WITH CHECK ADD  CONSTRAINT [FK_LODFinalizationSignatory_SysRole] FOREIGN KEY([SysRoleId])
REFERENCES [core].[SysRole] ([SysRoleID])
GO
ALTER TABLE [student].[LodFinalizationSignatory] CHECK CONSTRAINT [FK_LODFinalizationSignatory_SysRole]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeEvaluation_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] CHECK CONSTRAINT [FK_LodFirstGradeEvaluation_CurrentYear]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeEvaluation_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] CHECK CONSTRAINT [FK_LodFirstGradeEvaluation_Person]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeEvaluation_Person_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] CHECK CONSTRAINT [FK_LodFirstGradeEvaluation_Person_CreatedBySysUserId]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeEvaluation_Person_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] CHECK CONSTRAINT [FK_LodFirstGradeEvaluation_Person_ModifiedBySysUserId]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeEvaluation_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] CHECK CONSTRAINT [FK_LodFirstGradeEvaluation_StudentClass]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeEvaluation_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] CHECK CONSTRAINT [FK_LodFirstGradeEvaluation_Subject]
GO
ALTER TABLE [student].[LodFirstGradeEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeEvaluation_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[LodFirstGradeEvaluation] CHECK CONSTRAINT [FK_LodFirstGradeEvaluation_SubjectType]
GO
ALTER TABLE [student].[LodFirstGradeResult]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeResult_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodFirstGradeResult] CHECK CONSTRAINT [FK_LodFirstGradeResult_Creator_SysUser]
GO
ALTER TABLE [student].[LodFirstGradeResult]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeResult_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[LodFirstGradeResult] CHECK CONSTRAINT [FK_LodFirstGradeResult_CurrentYear]
GO
ALTER TABLE [student].[LodFirstGradeResult]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeResult_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LodFirstGradeResult] CHECK CONSTRAINT [FK_LodFirstGradeResult_Person]
GO
ALTER TABLE [student].[LodFirstGradeResult]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeResult_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[LodFirstGradeResult] CHECK CONSTRAINT [FK_LodFirstGradeResult_StudentClass]
GO
ALTER TABLE [student].[LodFirstGradeResult]  WITH CHECK ADD  CONSTRAINT [FK_LodFirstGradeResult_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodFirstGradeResult] CHECK CONSTRAINT [FK_LodFirstGradeResult_Updater_SysUser]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodSelfEduFormEvaluation_CurrentYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] CHECK CONSTRAINT [FK_LodSelfEduFormEvaluation_CurrentYear]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodSelfEduFormEvaluation_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] CHECK CONSTRAINT [FK_LodSelfEduFormEvaluation_Person]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodSelfEduFormEvaluation_Person_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] CHECK CONSTRAINT [FK_LodSelfEduFormEvaluation_Person_CreatedBySysUserId]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodSelfEduFormEvaluation_Person_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] CHECK CONSTRAINT [FK_LodSelfEduFormEvaluation_Person_ModifiedBySysUserId]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodSelfEduFormEvaluation_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] CHECK CONSTRAINT [FK_LodSelfEduFormEvaluation_StudentClass]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodSelfEduFormEvaluation_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] CHECK CONSTRAINT [FK_LodSelfEduFormEvaluation_Subject]
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_LodSelfEduFormEvaluation_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[LodSelfEduFormEvaluation] CHECK CONSTRAINT [FK_LodSelfEduFormEvaluation_SubjectType]
GO
ALTER TABLE [student].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Message] CHECK CONSTRAINT [FK_Message_Creator_SysUser]
GO
ALTER TABLE [student].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Receiver_SysUser] FOREIGN KEY([ReceiverId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Message] CHECK CONSTRAINT [FK_Message_Receiver_SysUser]
GO
ALTER TABLE [student].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Sender_SysUser] FOREIGN KEY([SenderId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Message] CHECK CONSTRAINT [FK_Message_Sender_SysUser]
GO
ALTER TABLE [student].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Message] CHECK CONSTRAINT [FK_Message_Updater_SysUser]
GO
ALTER TABLE [student].[MessageAttachment]  WITH CHECK ADD  CONSTRAINT [FK_MessageAttachment_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[MessageAttachment] CHECK CONSTRAINT [FK_MessageAttachment_Creator_SysUser]
GO
ALTER TABLE [student].[MessageAttachment]  WITH CHECK ADD  CONSTRAINT [FK_MessageAttachment_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[MessageAttachment] CHECK CONSTRAINT [FK_MessageAttachment_Updater_SysUser]
GO
ALTER TABLE [student].[MessageAttachment]  WITH CHECK ADD  CONSTRAINT [FK_MessageAttachmentDocument_Message] FOREIGN KEY([MessageId])
REFERENCES [student].[Message] ([Id])
GO
ALTER TABLE [student].[MessageAttachment] CHECK CONSTRAINT [FK_MessageAttachmentDocument_Message]
GO
ALTER TABLE [student].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Note] CHECK CONSTRAINT [FK_Note_Creator_SysUser]
GO
ALTER TABLE [student].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[Note] CHECK CONSTRAINT [FK_Note_InstitutionSchoolYear]
GO
ALTER TABLE [student].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[Note] CHECK CONSTRAINT [FK_Note_Person]
GO
ALTER TABLE [student].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Note] CHECK CONSTRAINT [FK_Note_Updater_SysUser]
GO
ALTER TABLE [student].[NoteDocument]  WITH CHECK ADD  CONSTRAINT [FK_NoteDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[NoteDocument] CHECK CONSTRAINT [FK_NoteDocument_Document]
GO
ALTER TABLE [student].[NoteDocument]  WITH CHECK ADD  CONSTRAINT [FK_NoteDocument_Note] FOREIGN KEY([NoteId])
REFERENCES [student].[Note] ([Id])
GO
ALTER TABLE [student].[NoteDocument] CHECK CONSTRAINT [FK_NoteDocument_Note]
GO
ALTER TABLE [student].[Ores]  WITH CHECK ADD  CONSTRAINT [FK_Ores_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Ores] CHECK CONSTRAINT [FK_Ores_Creator_SysUser]
GO
ALTER TABLE [student].[Ores]  WITH CHECK ADD  CONSTRAINT [FK_Ores_OresType] FOREIGN KEY([OresTypeId])
REFERENCES [student].[ORESType] ([Id])
GO
ALTER TABLE [student].[Ores] CHECK CONSTRAINT [FK_Ores_OresType]
GO
ALTER TABLE [student].[Ores]  WITH CHECK ADD  CONSTRAINT [FK_Ores_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Ores] CHECK CONSTRAINT [FK_Ores_Updater_SysUser]
GO
ALTER TABLE [student].[OresAttachment]  WITH CHECK ADD  CONSTRAINT [FK_Ores_OresAttachment_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[OresAttachment] CHECK CONSTRAINT [FK_Ores_OresAttachment_SysUser]
GO
ALTER TABLE [student].[OresAttachment]  WITH CHECK ADD  CONSTRAINT [FK_OresAttachment_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[OresAttachment] CHECK CONSTRAINT [FK_OresAttachment_Creator_SysUser]
GO
ALTER TABLE [student].[OresAttachment]  WITH CHECK ADD  CONSTRAINT [FK_OresAttachment_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[OresAttachment] CHECK CONSTRAINT [FK_OresAttachment_Document]
GO
ALTER TABLE [student].[OresAttachment]  WITH CHECK ADD  CONSTRAINT [FK_OresAttachment_Ores] FOREIGN KEY([OresId])
REFERENCES [student].[Ores] ([Id])
GO
ALTER TABLE [student].[OresAttachment] CHECK CONSTRAINT [FK_OresAttachment_Ores]
GO
ALTER TABLE [student].[OresToEntity]  WITH CHECK ADD  CONSTRAINT [FK_Ores_OresToEntity_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[OresToEntity] CHECK CONSTRAINT [FK_Ores_OresToEntity_StudentClass]
GO
ALTER TABLE [student].[OresToEntity]  WITH CHECK ADD  CONSTRAINT [FK_Ores_OresToEntity_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[OresToEntity] CHECK CONSTRAINT [FK_Ores_OresToEntity_SysUser]
GO
ALTER TABLE [student].[OresToEntity]  WITH CHECK ADD  CONSTRAINT [FK_OresToEntity_ClassGroup] FOREIGN KEY([ClassId])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [student].[OresToEntity] CHECK CONSTRAINT [FK_OresToEntity_ClassGroup]
GO
ALTER TABLE [student].[OresToEntity]  WITH CHECK ADD  CONSTRAINT [FK_OresToEntity_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[OresToEntity] CHECK CONSTRAINT [FK_OresToEntity_Creator_SysUser]
GO
ALTER TABLE [student].[OresToEntity]  WITH CHECK ADD  CONSTRAINT [FK_OresToEntity_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[OresToEntity] CHECK CONSTRAINT [FK_OresToEntity_InstitutionSchoolYear]
GO
ALTER TABLE [student].[OresToEntity]  WITH CHECK ADD  CONSTRAINT [FK_OresToEntity_Ores] FOREIGN KEY([OresId])
REFERENCES [student].[Ores] ([Id])
GO
ALTER TABLE [student].[OresToEntity] CHECK CONSTRAINT [FK_OresToEntity_Ores]
GO
ALTER TABLE [student].[OresToEntity]  WITH CHECK ADD  CONSTRAINT [FK_OresToEntity_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[OresToEntity] CHECK CONSTRAINT [FK_OresToEntity_Person]
GO
ALTER TABLE [student].[OtherDocument]  WITH CHECK ADD  CONSTRAINT [FK_OtherDocument_BasicDocument] FOREIGN KEY([BasicDocumentId])
REFERENCES [document].[BasicDocument] ([Id])
GO
ALTER TABLE [student].[OtherDocument] CHECK CONSTRAINT [FK_OtherDocument_BasicDocument]
GO
ALTER TABLE [student].[OtherDocument]  WITH CHECK ADD  CONSTRAINT [FK_OtherDocument_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[OtherDocument] CHECK CONSTRAINT [FK_OtherDocument_Creator_SysUser]
GO
ALTER TABLE [student].[OtherDocument]  WITH CHECK ADD  CONSTRAINT [FK_OtherDocument_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[OtherDocument] CHECK CONSTRAINT [FK_OtherDocument_InstitutionSchoolYear]
GO
ALTER TABLE [student].[OtherDocument]  WITH CHECK ADD  CONSTRAINT [FK_OtherDocument_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[OtherDocument] CHECK CONSTRAINT [FK_OtherDocument_Person]
GO
ALTER TABLE [student].[OtherDocument]  WITH CHECK ADD  CONSTRAINT [FK_OtherDocument_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[OtherDocument] CHECK CONSTRAINT [FK_OtherDocument_SchoolYear]
GO
ALTER TABLE [student].[OtherDocumentDocument]  WITH CHECK ADD  CONSTRAINT [FK_OtherDocumentDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[OtherDocumentDocument] CHECK CONSTRAINT [FK_OtherDocumentDocument_Document]
GO
ALTER TABLE [student].[OtherDocumentDocument]  WITH CHECK ADD  CONSTRAINT [FK_OtherDocumentDocument_OtherDocument] FOREIGN KEY([OtherDocumentId])
REFERENCES [student].[OtherDocument] ([Id])
GO
ALTER TABLE [student].[OtherDocumentDocument] CHECK CONSTRAINT [FK_OtherDocumentDocument_OtherDocument]
GO
ALTER TABLE [student].[OtherInstitution]  WITH CHECK ADD  CONSTRAINT [FK_OtherInstitution_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[OtherInstitution] CHECK CONSTRAINT [FK_OtherInstitution_InstitutionSchoolYear]
GO
ALTER TABLE [student].[OtherInstitution]  WITH CHECK ADD  CONSTRAINT [FK_OtherInstitution_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[OtherInstitution] CHECK CONSTRAINT [FK_OtherInstitution_Person]
GO
ALTER TABLE [student].[OtherInstitution]  WITH CHECK ADD  CONSTRAINT [FK_OtherInstitution_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[OtherInstitution] CHECK CONSTRAINT [FK_OtherInstitution_SysUser]
GO
ALTER TABLE [student].[OtherInstitution]  WITH CHECK ADD  CONSTRAINT [FK_OtherInstitution_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[OtherInstitution] CHECK CONSTRAINT [FK_OtherInstitution_Updater_SysUser]
GO
ALTER TABLE [student].[PersonalDevelopmentAdditionalSupportType]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentAdditionalSupportType_AdditionalSupportType] FOREIGN KEY([AdditionalSupportTypeId])
REFERENCES [student].[AdditionalSupportType] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentAdditionalSupportType] CHECK CONSTRAINT [FK_PersonalDevelopmentAdditionalSupportType_AdditionalSupportType]
GO
ALTER TABLE [student].[PersonalDevelopmentAdditionalSupportType]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentAdditionalSupportType_PersonalDevelopmentSupport] FOREIGN KEY([PersonalDevelopmentSupportId])
REFERENCES [student].[PersonalDevelopmentSupport] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentAdditionalSupportType] CHECK CONSTRAINT [FK_PersonalDevelopmentAdditionalSupportType_PersonalDevelopmentSupport]
GO
ALTER TABLE [student].[PersonalDevelopmentCommonSupportType]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentCommonSupportType_CommonSupportType] FOREIGN KEY([CommonSupportTypeId])
REFERENCES [student].[CommonSupportType] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentCommonSupportType] CHECK CONSTRAINT [FK_PersonalDevelopmentCommonSupportType_CommonSupportType]
GO
ALTER TABLE [student].[PersonalDevelopmentCommonSupportType]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentCommonSupportType_PersonalDevelopmentSupport] FOREIGN KEY([PersonalDevelopmentSupportId])
REFERENCES [student].[PersonalDevelopmentSupport] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentCommonSupportType] CHECK CONSTRAINT [FK_PersonalDevelopmentCommonSupportType_PersonalDevelopmentSupport]
GO
ALTER TABLE [student].[PersonalDevelopmentDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentDocument] CHECK CONSTRAINT [FK_PersonalDevelopmentDocument_Document]
GO
ALTER TABLE [student].[PersonalDevelopmentDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentDocument_PersonalDevelopmentSupport] FOREIGN KEY([PersonalDevelopmentSupportId])
REFERENCES [student].[PersonalDevelopmentSupport] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentDocument] CHECK CONSTRAINT [FK_PersonalDevelopmentDocument_PersonalDevelopmentSupport]
GO
ALTER TABLE [student].[PersonalDevelopmentDocument]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentDocument_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalDevelopmentDocument] CHECK CONSTRAINT [FK_PersonalDevelopmentDocument_SysUser]
GO
ALTER TABLE [student].[PersonalDevelopmentEarlyEvaluationReason]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentEarlyEvaluationReason_EarlyEvaluationReason] FOREIGN KEY([EarlyEvaluationReasonId])
REFERENCES [student].[EarlyEvaluationReason] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentEarlyEvaluationReason] CHECK CONSTRAINT [FK_PersonalDevelopmentEarlyEvaluationReason_EarlyEvaluationReason]
GO
ALTER TABLE [student].[PersonalDevelopmentEarlyEvaluationReason]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentEarlyEvaluationReason_PersonalDevelopmentSupport] FOREIGN KEY([PersonalDevelopmentSupportId])
REFERENCES [student].[PersonalDevelopmentSupport] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentEarlyEvaluationReason] CHECK CONSTRAINT [FK_PersonalDevelopmentEarlyEvaluationReason_PersonalDevelopmentSupport]
GO
ALTER TABLE [student].[PersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentSupport_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[PersonalDevelopmentSupport] CHECK CONSTRAINT [FK_PersonalDevelopmentSupport_Person]
GO
ALTER TABLE [student].[PersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentSupport_Person_CreatedBySysUserId] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalDevelopmentSupport] CHECK CONSTRAINT [FK_PersonalDevelopmentSupport_Person_CreatedBySysUserId]
GO
ALTER TABLE [student].[PersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentSupport_Person_ModifiedBySysUserId] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalDevelopmentSupport] CHECK CONSTRAINT [FK_PersonalDevelopmentSupport_Person_ModifiedBySysUserId]
GO
ALTER TABLE [student].[PersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentSupport_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([TempCurrentYearID])
GO
ALTER TABLE [student].[PersonalDevelopmentSupport] CHECK CONSTRAINT [FK_PersonalDevelopmentSupport_SchoolYear]
GO
ALTER TABLE [student].[PersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentSupport_StudentType] FOREIGN KEY([StudentTypeId])
REFERENCES [student].[StudentType] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentSupport] CHECK CONSTRAINT [FK_PersonalDevelopmentSupport_StudentType]
GO
ALTER TABLE [student].[PersonalDevelopmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_PersonalDevelopmentSupport_SupportPeriod] FOREIGN KEY([SupportPeriodTypeId])
REFERENCES [student].[SupportPeriod] ([Id])
GO
ALTER TABLE [student].[PersonalDevelopmentSupport] CHECK CONSTRAINT [FK_PersonalDevelopmentSupport_SupportPeriod]
GO
ALTER TABLE [student].[PersonalEarlyAssessment]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessment_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalEarlyAssessment] CHECK CONSTRAINT [FK_PersonalEarlyAssessment_CreatedSysUser]
GO
ALTER TABLE [student].[PersonalEarlyAssessment]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessment_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalEarlyAssessment] CHECK CONSTRAINT [FK_PersonalEarlyAssessment_ModifiedSysUser]
GO
ALTER TABLE [student].[PersonalEarlyAssessment]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessment_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[PersonalEarlyAssessment] CHECK CONSTRAINT [FK_PersonalEarlyAssessment_Person]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentAttachment]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentAttachment_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentAttachment] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentAttachment_Document]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentAttachment]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentAttachment_PersonalEarlyAssessment] FOREIGN KEY([PersonalEarlyAssessmentId])
REFERENCES [student].[PersonalEarlyAssessment] ([Id])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentAttachment] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentAttachment_PersonalEarlyAssessment]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentDisabilityReaso_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentDisabilityReaso_ModifiedSysUser]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentDisabilityReason_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentDisabilityReason_CreatedSysUser]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentDisabilityReason_EarlyEvaluationReason] FOREIGN KEY([ReasonId])
REFERENCES [student].[EarlyEvaluationReason] ([Id])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentDisabilityReason_EarlyEvaluationReason]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentDisabilityReason_PersonalEarlyAssessmentt] FOREIGN KEY([PersonalEarlyAssessmentId])
REFERENCES [student].[PersonalEarlyAssessment] ([Id])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentDisabilityReason] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentDisabilityReason_PersonalEarlyAssessmentt]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentLearningDisability]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentLearningDisability_CreatedSysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentLearningDisability] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentLearningDisability_CreatedSysUser]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentLearningDisability]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentLearningDisability_PersonalEarlyAssessment] FOREIGN KEY([PersonalEarlyAssessmentId])
REFERENCES [student].[PersonalEarlyAssessment] ([Id])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentLearningDisability] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentLearningDisability_PersonalEarlyAssessment]
GO
ALTER TABLE [student].[PersonalEarlyAssessmentLearningDisability]  WITH CHECK ADD  CONSTRAINT [FK_PersonalEarlyAssessmentLearningDisabilityt_ModifiedSysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PersonalEarlyAssessmentLearningDisability] CHECK CONSTRAINT [FK_PersonalEarlyAssessmentLearningDisabilityt_ModifiedSysUser]
GO
ALTER TABLE [student].[PreSchoolEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_PreSchoolEvaluation_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[PreSchoolEvaluation] CHECK CONSTRAINT [FK_PreSchoolEvaluation_BasicClass]
GO
ALTER TABLE [student].[PreSchoolEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_PreSchoolEvaluation_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PreSchoolEvaluation] CHECK CONSTRAINT [FK_PreSchoolEvaluation_Creator_SysUser]
GO
ALTER TABLE [student].[PreSchoolEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_PreSchoolEvaluation_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[PreSchoolEvaluation] CHECK CONSTRAINT [FK_PreSchoolEvaluation_Person]
GO
ALTER TABLE [student].[PreSchoolEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_PreSchoolEvaluation_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[PreSchoolEvaluation] CHECK CONSTRAINT [FK_PreSchoolEvaluation_SchoolYear]
GO
ALTER TABLE [student].[PreSchoolEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_PreSchoolEvaluation_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[PreSchoolEvaluation] CHECK CONSTRAINT [FK_PreSchoolEvaluation_Subject]
GO
ALTER TABLE [student].[PreSchoolEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_PreSchoolEvaluation_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PreSchoolEvaluation] CHECK CONSTRAINT [FK_PreSchoolEvaluation_Updater_SysUser]
GO
ALTER TABLE [student].[PreSchoolReadiness]  WITH CHECK ADD  CONSTRAINT [FK_PreSchool_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PreSchoolReadiness] CHECK CONSTRAINT [FK_PreSchool_Updater_SysUser]
GO
ALTER TABLE [student].[PreSchoolReadiness]  WITH CHECK ADD  CONSTRAINT [FK_PreSchoolReadiness_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[PreSchoolReadiness] CHECK CONSTRAINT [FK_PreSchoolReadiness_Creator_SysUser]
GO
ALTER TABLE [student].[PreSchoolReadiness]  WITH CHECK ADD  CONSTRAINT [FK_PreSchoolReadiness_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[PreSchoolReadiness] CHECK CONSTRAINT [FK_PreSchoolReadiness_Person]
GO
ALTER TABLE [student].[Reassessment]  WITH CHECK ADD  CONSTRAINT [FK_Reassessment_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[Reassessment] CHECK CONSTRAINT [FK_Reassessment_BasicClass]
GO
ALTER TABLE [student].[Reassessment]  WITH CHECK ADD  CONSTRAINT [FK_Reassessment_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Reassessment] CHECK CONSTRAINT [FK_Reassessment_Creator_SysUser]
GO
ALTER TABLE [student].[Reassessment]  WITH CHECK ADD  CONSTRAINT [FK_Reassessment_ReasonForReassessmentType] FOREIGN KEY([ReasonId])
REFERENCES [student].[ReasonForReassessmentType] ([Id])
GO
ALTER TABLE [student].[Reassessment] CHECK CONSTRAINT [FK_Reassessment_ReasonForReassessmentType]
GO
ALTER TABLE [student].[Reassessment]  WITH CHECK ADD  CONSTRAINT [FK_Reassessment_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[Reassessment] CHECK CONSTRAINT [FK_Reassessment_SchoolYear]
GO
ALTER TABLE [student].[Reassessment]  WITH CHECK ADD  CONSTRAINT [FK_Reassessment_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[Reassessment] CHECK CONSTRAINT [FK_Reassessment_Student]
GO
ALTER TABLE [student].[Reassessment]  WITH CHECK ADD  CONSTRAINT [FK_Reassessment_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Reassessment] CHECK CONSTRAINT [FK_Reassessment_Updater_SysUser]
GO
ALTER TABLE [student].[ReassessmentDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReassessmentDetails_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ReassessmentDetails] CHECK CONSTRAINT [FK_ReassessmentDetails_Creator_SysUser]
GO
ALTER TABLE [student].[ReassessmentDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReassessmentDetails_GradeCategory] FOREIGN KEY([GradeCategory])
REFERENCES [student].[GradeType] ([Id])
GO
ALTER TABLE [student].[ReassessmentDetails] CHECK CONSTRAINT [FK_ReassessmentDetails_GradeCategory]
GO
ALTER TABLE [student].[ReassessmentDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReassessmentDetails_Reassessment] FOREIGN KEY([ReassessmentId])
REFERENCES [student].[Reassessment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[ReassessmentDetails] CHECK CONSTRAINT [FK_ReassessmentDetails_Reassessment]
GO
ALTER TABLE [student].[ReassessmentDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReassessmentDetails_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[ReassessmentDetails] CHECK CONSTRAINT [FK_ReassessmentDetails_Subject]
GO
ALTER TABLE [student].[ReassessmentDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReassessmentDetails_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[ReassessmentDetails] CHECK CONSTRAINT [FK_ReassessmentDetails_SubjectType]
GO
ALTER TABLE [student].[ReassessmentDetails]  WITH CHECK ADD  CONSTRAINT [FK_ReassessmentDetails_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ReassessmentDetails] CHECK CONSTRAINT [FK_ReassessmentDetails_Updater_SysUserd]
GO
ALTER TABLE [student].[ReassessmentDocument]  WITH CHECK ADD  CONSTRAINT [FK_ReassessmentDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[ReassessmentDocument] CHECK CONSTRAINT [FK_ReassessmentDocument_Document]
GO
ALTER TABLE [student].[ReassessmentDocument]  WITH CHECK ADD  CONSTRAINT [FK_ReassessmentDocument_Reassessment] FOREIGN KEY([ReassessmentId])
REFERENCES [student].[Reassessment] ([Id])
GO
ALTER TABLE [student].[ReassessmentDocument] CHECK CONSTRAINT [FK_ReassessmentDocument_Reassessment]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_BasicClass]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_Creator_SysUser]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_InstitutionCountry] FOREIGN KEY([InstitutionCountryId])
REFERENCES [location].[Country] ([CountryID])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_InstitutionCountry]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_RecognitionEducationLevel] FOREIGN KEY([EducationLevelId])
REFERENCES [student].[RecognitionEducationLevel] ([Id])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_RecognitionEducationLevel]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_SchoolYear]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_SPPOOProfession] FOREIGN KEY([SPPOOProfessionId])
REFERENCES [inst_nom].[SPPOOProfession] ([SPPOOProfessionID])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_SPPOOProfession]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_SPPOOSpeciality] FOREIGN KEY([SPPOOSpecialityId])
REFERENCES [inst_nom].[SPPOOSpeciality] ([SPPOOSpecialityID])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_SPPOOSpeciality]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_Student]
GO
ALTER TABLE [student].[Recognition]  WITH CHECK ADD  CONSTRAINT [FK_Recognition_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Recognition] CHECK CONSTRAINT [FK_Recognition_Updater_SysUser]
GO
ALTER TABLE [student].[RecognitionDocument]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[RecognitionDocument] CHECK CONSTRAINT [FK_RecognitionDocument_Document]
GO
ALTER TABLE [student].[RecognitionDocument]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionDocument_Recognition] FOREIGN KEY([RecognitionId])
REFERENCES [student].[Recognition] ([Id])
GO
ALTER TABLE [student].[RecognitionDocument] CHECK CONSTRAINT [FK_RecognitionDocument_Recognition]
GO
ALTER TABLE [student].[RecognitionEqualization]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionEqualization_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[RecognitionEqualization] CHECK CONSTRAINT [FK_RecognitionEqualization_BasicClass]
GO
ALTER TABLE [student].[RecognitionEqualization]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionEqualization_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[RecognitionEqualization] CHECK CONSTRAINT [FK_RecognitionEqualization_Creator_SysUser]
GO
ALTER TABLE [student].[RecognitionEqualization]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionEqualization_GradeCategory] FOREIGN KEY([GradeCategory])
REFERENCES [student].[GradeType] ([Id])
GO
ALTER TABLE [student].[RecognitionEqualization] CHECK CONSTRAINT [FK_RecognitionEqualization_GradeCategory]
GO
ALTER TABLE [student].[RecognitionEqualization]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionEqualization_Recognition] FOREIGN KEY([RecognitionId])
REFERENCES [student].[Recognition] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[RecognitionEqualization] CHECK CONSTRAINT [FK_RecognitionEqualization_Recognition]
GO
ALTER TABLE [student].[RecognitionEqualization]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionEqualization_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[RecognitionEqualization] CHECK CONSTRAINT [FK_RecognitionEqualization_Subject]
GO
ALTER TABLE [student].[RecognitionEqualization]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionEqualization_SubjectType] FOREIGN KEY([SubjectTypeId])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[RecognitionEqualization] CHECK CONSTRAINT [FK_RecognitionEqualization_SubjectType]
GO
ALTER TABLE [student].[RecognitionEqualization]  WITH CHECK ADD  CONSTRAINT [FK_RecognitionEqualization_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[RecognitionEqualization] CHECK CONSTRAINT [FK_RecognitionEqualization_Updater_SysUser]
GO
ALTER TABLE [student].[RelocationDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocument_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[RelocationDocument] CHECK CONSTRAINT [FK_RelocationDocument_Creator_SysUser]
GO
ALTER TABLE [student].[RelocationDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocument_DischargeReasonType] FOREIGN KEY([RelocationReasonTypeId])
REFERENCES [student].[DischargeReasonType] ([Id])
GO
ALTER TABLE [student].[RelocationDocument] CHECK CONSTRAINT [FK_RelocationDocument_DischargeReasonType]
GO
ALTER TABLE [student].[RelocationDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocument_InstitutionSchoolYear_HostInstitution] FOREIGN KEY([HostInstitutionId], [HostInstitutionSchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[RelocationDocument] CHECK CONSTRAINT [FK_RelocationDocument_InstitutionSchoolYear_HostInstitution]
GO
ALTER TABLE [student].[RelocationDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocument_InstitutionSchoolYear_SendingInstitution] FOREIGN KEY([SendingInstitutionId], [SendingInstitutionSchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[RelocationDocument] CHECK CONSTRAINT [FK_RelocationDocument_InstitutionSchoolYear_SendingInstitution]
GO
ALTER TABLE [student].[RelocationDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocument_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[RelocationDocument] CHECK CONSTRAINT [FK_RelocationDocument_Person]
GO
ALTER TABLE [student].[RelocationDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocument_StudentClass] FOREIGN KEY([CurrentStudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[RelocationDocument] CHECK CONSTRAINT [FK_RelocationDocument_StudentClass]
GO
ALTER TABLE [student].[RelocationDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocument_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[RelocationDocument] CHECK CONSTRAINT [FK_RelocationDocument_Updater_SysUserd]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] CHECK CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_Creator_SysUser]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_CurriculumPart] FOREIGN KEY([CurriculumPartID])
REFERENCES [inst_nom].[CurriculumPart] ([CurriculumPartID])
ON DELETE CASCADE
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] CHECK CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_CurriculumPart]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] CHECK CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_InstitutionSchoolYear]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] CHECK CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_Person]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_RelocationDocument] FOREIGN KEY([RelocationDocumentId])
REFERENCES [student].[RelocationDocument] ([Id])
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] CHECK CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_RelocationDocument]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_SubjectType] FOREIGN KEY([SubjectTypeID])
REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] CHECK CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_SubjectType]
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[RelocationDocumentCurrentTermGrades] CHECK CONSTRAINT [FK_RelocationDocumentCurrentTermGrades_Updater_SysUser]
GO
ALTER TABLE [student].[RelocationDocumentDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[RelocationDocumentDocument] CHECK CONSTRAINT [FK_RelocationDocumentDocument_Document]
GO
ALTER TABLE [student].[RelocationDocumentDocument]  WITH CHECK ADD  CONSTRAINT [FK_RelocationDocumentDocument_RelocationDocument] FOREIGN KEY([RelocationDocumentId])
REFERENCES [student].[RelocationDocument] ([Id])
GO
ALTER TABLE [student].[RelocationDocumentDocument] CHECK CONSTRAINT [FK_RelocationDocumentDocument_RelocationDocument]
GO
ALTER TABLE [student].[ResourceSupport]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupport_AdditionalPersonalDevelopmentSupportItem] FOREIGN KEY([AdditionalPersonalDevelopmentSupportItemId])
REFERENCES [student].[AdditionalPersonalDevelopmentSupportItem] ([Id])
GO
ALTER TABLE [student].[ResourceSupport] CHECK CONSTRAINT [FK_ResourceSupport_AdditionalPersonalDevelopmentSupportItem]
GO
ALTER TABLE [student].[ResourceSupport]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupport_ResourceSupportReport] FOREIGN KEY([ResourceSupportReportId])
REFERENCES [student].[ResourceSupportReport] ([Id])
GO
ALTER TABLE [student].[ResourceSupport] CHECK CONSTRAINT [FK_ResourceSupport_ResourceSupportReport]
GO
ALTER TABLE [student].[ResourceSupport]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupport_ResourceSupportType] FOREIGN KEY([ResourceSupportTypeId])
REFERENCES [student].[ResourceSupportType] ([Id])
GO
ALTER TABLE [student].[ResourceSupport] CHECK CONSTRAINT [FK_ResourceSupport_ResourceSupportType]
GO
ALTER TABLE [student].[ResourceSupportDocument]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[ResourceSupportDocument] CHECK CONSTRAINT [FK_ResourceSupportDocument_Document]
GO
ALTER TABLE [student].[ResourceSupportDocument]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportDocument_ResourceSupportReport] FOREIGN KEY([ResourceSupporReportId])
REFERENCES [student].[ResourceSupportReport] ([Id])
GO
ALTER TABLE [student].[ResourceSupportDocument] CHECK CONSTRAINT [FK_ResourceSupportDocument_ResourceSupportReport]
GO
ALTER TABLE [student].[ResourceSupportReport]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportReport_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[ResourceSupportReport] CHECK CONSTRAINT [FK_ResourceSupportReport_Person]
GO
ALTER TABLE [student].[ResourceSupportReport]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportReport_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[ResourceSupportReport] CHECK CONSTRAINT [FK_ResourceSupportReport_SchoolYear]
GO
ALTER TABLE [student].[ResourceSupportReport]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportReport_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ResourceSupportReport] CHECK CONSTRAINT [FK_ResourceSupportReport_SysUser]
GO
ALTER TABLE [student].[ResourceSupportSpecialist]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportSpecialist_Support] FOREIGN KEY([ResourceSupportId])
REFERENCES [student].[ResourceSupport] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[ResourceSupportSpecialist] CHECK CONSTRAINT [FK_ResourceSupportSpecialist_Support]
GO
ALTER TABLE [student].[ResourceSupportSpecialist]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportSpecialist_SysUser] FOREIGN KEY([SysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ResourceSupportSpecialist] CHECK CONSTRAINT [FK_ResourceSupportSpecialist_SysUser]
GO
ALTER TABLE [student].[ResourceSupportSpecialist]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportSpecialist_Type] FOREIGN KEY([ResourceSupportSpecialistTypeId])
REFERENCES [student].[ResourceSupportSpecialistType] ([Id])
GO
ALTER TABLE [student].[ResourceSupportSpecialist] CHECK CONSTRAINT [FK_ResourceSupportSpecialist_Type]
GO
ALTER TABLE [student].[ResourceSupportSpecialist]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportSpecialistWorkPlace] FOREIGN KEY([WorkPlaceId])
REFERENCES [student].[ResourceSupportSpecialistWorkPlace] ([Id])
GO
ALTER TABLE [student].[ResourceSupportSpecialist] CHECK CONSTRAINT [FK_ResourceSupportSpecialistWorkPlace]
GO
ALTER TABLE [student].[ResourceSupportSpecialistWorkPlaceToResourceSupportType]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportSpecialistWorkPlaceToResourceSupportType_ResourceSupportSpecialistWorkPlace] FOREIGN KEY([ResourceSupportSpecialistWorkPlaceId])
REFERENCES [student].[ResourceSupportSpecialistWorkPlace] ([Id])
GO
ALTER TABLE [student].[ResourceSupportSpecialistWorkPlaceToResourceSupportType] CHECK CONSTRAINT [FK_ResourceSupportSpecialistWorkPlaceToResourceSupportType_ResourceSupportSpecialistWorkPlace]
GO
ALTER TABLE [student].[ResourceSupportSpecialistWorkPlaceToResourceSupportType]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSupportSpecialistWorkPlaceToResourceSupportType_ResourceSupportType] FOREIGN KEY([ResourceSupportTypeId])
REFERENCES [student].[ResourceSupportType] ([Id])
GO
ALTER TABLE [student].[ResourceSupportSpecialistWorkPlaceToResourceSupportType] CHECK CONSTRAINT [FK_ResourceSupportSpecialistWorkPlaceToResourceSupportType_ResourceSupportType]
GO
ALTER TABLE [student].[Sanction]  WITH CHECK ADD  CONSTRAINT [FK_Sanction_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Sanction] CHECK CONSTRAINT [FK_Sanction_Creator_SysUser]
GO
ALTER TABLE [student].[Sanction]  WITH CHECK ADD  CONSTRAINT [FK_Sanction_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[Sanction] CHECK CONSTRAINT [FK_Sanction_InstitutionSchoolYear]
GO
ALTER TABLE [student].[Sanction]  WITH CHECK ADD  CONSTRAINT [FK_Sanction_SanctionType] FOREIGN KEY([SanctionTypeId])
REFERENCES [student].[SanctionType] ([Id])
GO
ALTER TABLE [student].[Sanction] CHECK CONSTRAINT [FK_Sanction_SanctionType]
GO
ALTER TABLE [student].[Sanction]  WITH CHECK ADD  CONSTRAINT [FK_Sanction_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[Sanction] CHECK CONSTRAINT [FK_Sanction_SchoolYear]
GO
ALTER TABLE [student].[Sanction]  WITH CHECK ADD  CONSTRAINT [FK_Sanction_Student] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[Sanction] CHECK CONSTRAINT [FK_Sanction_Student]
GO
ALTER TABLE [student].[Sanction]  WITH CHECK ADD  CONSTRAINT [FK_Sanction_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Sanction] CHECK CONSTRAINT [FK_Sanction_Updater_SysUser]
GO
ALTER TABLE [student].[SanctionDocument]  WITH CHECK ADD  CONSTRAINT [FK_SanctionDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[SanctionDocument] CHECK CONSTRAINT [FK_SanctionDocument_Document]
GO
ALTER TABLE [student].[SanctionDocument]  WITH CHECK ADD  CONSTRAINT [FK_SanctionDocument_Sanction] FOREIGN KEY([SanctionId])
REFERENCES [student].[Sanction] ([Id])
GO
ALTER TABLE [student].[SanctionDocument] CHECK CONSTRAINT [FK_SanctionDocument_Sanction]
GO
ALTER TABLE [student].[ScholarshipStudent]  WITH CHECK ADD  CONSTRAINT [FK_FinancingOrgan_ScholarshipStudent] FOREIGN KEY([FinancingOrganId])
REFERENCES [student].[ScholarshipFinancingOrgan] ([Id])
GO
ALTER TABLE [student].[ScholarshipStudent] CHECK CONSTRAINT [FK_FinancingOrgan_ScholarshipStudent]
GO
ALTER TABLE [student].[ScholarshipStudent]  WITH CHECK ADD  CONSTRAINT [FK_ScholarshipStudent_Creator_SysUser] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ScholarshipStudent] CHECK CONSTRAINT [FK_ScholarshipStudent_Creator_SysUser]
GO
ALTER TABLE [student].[ScholarshipStudent]  WITH CHECK ADD  CONSTRAINT [FK_ScholarshipStudent_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[ScholarshipStudent] CHECK CONSTRAINT [FK_ScholarshipStudent_InstitutionSchoolYear]
GO
ALTER TABLE [student].[ScholarshipStudent]  WITH CHECK ADD  CONSTRAINT [FK_ScholarshipStudent_ScholarshipAmount] FOREIGN KEY([ScholarshipAmountID])
REFERENCES [student].[ScholarshipAmount] ([Id])
GO
ALTER TABLE [student].[ScholarshipStudent] CHECK CONSTRAINT [FK_ScholarshipStudent_ScholarshipAmount]
GO
ALTER TABLE [student].[ScholarshipStudent]  WITH CHECK ADD  CONSTRAINT [FK_ScholarshipStudent_ScholarshipType] FOREIGN KEY([ScholarshipTypeId])
REFERENCES [student].[ScholarshipType] ([Id])
GO
ALTER TABLE [student].[ScholarshipStudent] CHECK CONSTRAINT [FK_ScholarshipStudent_ScholarshipType]
GO
ALTER TABLE [student].[ScholarshipStudent]  WITH CHECK ADD  CONSTRAINT [FK_ScholarshipStudent_StudentClassId] FOREIGN KEY([StudentClassID])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[ScholarshipStudent] CHECK CONSTRAINT [FK_ScholarshipStudent_StudentClassId]
GO
ALTER TABLE [student].[ScholarshipStudent]  WITH CHECK ADD  CONSTRAINT [FK_ScholarshipStudent_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[ScholarshipStudent] CHECK CONSTRAINT [FK_ScholarshipStudent_Updater_SysUserd]
GO
ALTER TABLE [student].[ScholarshipStudentDocument]  WITH CHECK ADD  CONSTRAINT [FK_ScholarshipStudentDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[ScholarshipStudentDocument] CHECK CONSTRAINT [FK_ScholarshipStudentDocument_Document]
GO
ALTER TABLE [student].[ScholarshipStudentDocument]  WITH CHECK ADD  CONSTRAINT [FK_ScholarshipStudentDocument_Scholarship] FOREIGN KEY([ScholarshipStudentId])
REFERENCES [student].[ScholarshipStudent] ([Id])
GO
ALTER TABLE [student].[ScholarshipStudentDocument] CHECK CONSTRAINT [FK_ScholarshipStudentDocument_Scholarship]
GO
ALTER TABLE [student].[SchoolTypeLodAccess]  WITH CHECK ADD  CONSTRAINT [FK_SchoolTypeLodAccess_DetailedSchoolType] FOREIGN KEY([DetailedSchoolTypeId])
REFERENCES [noms].[DetailedSchoolType] ([DetailedSchoolTypeID])
GO
ALTER TABLE [student].[SchoolTypeLodAccess] CHECK CONSTRAINT [FK_SchoolTypeLodAccess_DetailedSchoolType]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernment_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernment_Creator_SysUser]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernment_InstitutionSchoolBoard] FOREIGN KEY([InstitutionSchoolBoardId])
REFERENCES [inst_basic].[InstitutionSchoolBoard] ([InstitutionSchoolBoardID])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernment_InstitutionSchoolBoard]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernment_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernment_InstitutionSchoolYear]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernment_Participation] FOREIGN KEY([ParticipationId])
REFERENCES [student].[Participation] ([Id])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernment_Participation]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernment_SelfGovernmentPosition] FOREIGN KEY([PositionId])
REFERENCES [student].[SelfGovernmentPosition] ([Id])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernment_SelfGovernmentPosition]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernment_Student] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernment_Student]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernment_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernment_StudentClass]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernment_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernment_Updater_SysUser]
GO
ALTER TABLE [student].[SelfGovernment]  WITH CHECK ADD  CONSTRAINT [FK_SelfGovernmentStaffPosition] FOREIGN KEY([StaffPositionID])
REFERENCES [inst_basic].[StaffPosition] ([StaffPositionID])
GO
ALTER TABLE [student].[SelfGovernment] CHECK CONSTRAINT [FK_SelfGovernmentStaffPosition]
GO
ALTER TABLE [student].[SpecialEquipment]  WITH CHECK ADD  CONSTRAINT [FK_SpecialEquipment_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[SpecialEquipment] CHECK CONSTRAINT [FK_SpecialEquipment_Creator_SysUser]
GO
ALTER TABLE [student].[SpecialEquipment]  WITH CHECK ADD  CONSTRAINT [FK_SpecialEquipment_EquipmentType] FOREIGN KEY([EquipmentTypeId])
REFERENCES [inst_nom].[EquipmentType] ([EquipmentTypeId])
GO
ALTER TABLE [student].[SpecialEquipment] CHECK CONSTRAINT [FK_SpecialEquipment_EquipmentType]
GO
ALTER TABLE [student].[SpecialEquipment]  WITH CHECK ADD  CONSTRAINT [FK_SpecialEquipment_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[SpecialEquipment] CHECK CONSTRAINT [FK_SpecialEquipment_Student]
GO
ALTER TABLE [student].[SpecialEquipment]  WITH CHECK ADD  CONSTRAINT [FK_SpecialEquipment_StudentClassId] FOREIGN KEY([StudentClassID])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[SpecialEquipment] CHECK CONSTRAINT [FK_SpecialEquipment_StudentClassId]
GO
ALTER TABLE [student].[SpecialEquipment]  WITH CHECK ADD  CONSTRAINT [FK_SpecialEquipment_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[SpecialEquipment] CHECK CONSTRAINT [FK_SpecialEquipment_Updater_SysUser]
GO
ALTER TABLE [student].[SpecialNeeds]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeeds_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[SpecialNeeds] CHECK CONSTRAINT [FK_SpecialNeeds_Creator_SysUser]
GO
ALTER TABLE [student].[SpecialNeeds]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeeds_SpecialNeedsSubType] FOREIGN KEY([SpecialNeedsSubTypeId])
REFERENCES [student].[SpecialNeedsSubType] ([Id])
GO
ALTER TABLE [student].[SpecialNeeds] CHECK CONSTRAINT [FK_SpecialNeeds_SpecialNeedsSubType]
GO
ALTER TABLE [student].[SpecialNeeds]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeeds_SpecialNeedsType] FOREIGN KEY([SpecialNeedsTypeId])
REFERENCES [student].[SpecialNeedsType] ([Id])
GO
ALTER TABLE [student].[SpecialNeeds] CHECK CONSTRAINT [FK_SpecialNeeds_SpecialNeedsType]
GO
ALTER TABLE [student].[SpecialNeeds]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeeds_SpecialNeedsYear] FOREIGN KEY([SpecialNeedsYearId])
REFERENCES [student].[SpecialNeedsYear] ([Id])
GO
ALTER TABLE [student].[SpecialNeeds] CHECK CONSTRAINT [FK_SpecialNeeds_SpecialNeedsYear]
GO
ALTER TABLE [student].[SpecialNeeds]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeeds_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[SpecialNeeds] CHECK CONSTRAINT [FK_SpecialNeeds_Updater_SysUserd]
GO
ALTER TABLE [student].[SpecialNeedsSubType]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeedsSubType_SpecialNeedsType] FOREIGN KEY([SpecialNeedsTypeId])
REFERENCES [student].[SpecialNeedsType] ([Id])
GO
ALTER TABLE [student].[SpecialNeedsSubType] CHECK CONSTRAINT [FK_SpecialNeedsSubType_SpecialNeedsType]
GO
ALTER TABLE [student].[SpecialNeedsYear]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeedsYear_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[SpecialNeedsYear] CHECK CONSTRAINT [FK_SpecialNeedsYear_Creator_SysUser]
GO
ALTER TABLE [student].[SpecialNeedsYear]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeedsYear_Person] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[SpecialNeedsYear] CHECK CONSTRAINT [FK_SpecialNeedsYear_Person]
GO
ALTER TABLE [student].[SpecialNeedsYear]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeedsYear_SchoolYear] FOREIGN KEY([SchoolYear])
REFERENCES [inst_basic].[CurrentYear] ([CurrentYearID])
GO
ALTER TABLE [student].[SpecialNeedsYear] CHECK CONSTRAINT [FK_SpecialNeedsYear_SchoolYear]
GO
ALTER TABLE [student].[SpecialNeedsYear]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeedsYear_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[SpecialNeedsYear] CHECK CONSTRAINT [FK_SpecialNeedsYear_Updater_SysUserd]
GO
ALTER TABLE [student].[SpecialNeedsYearAttachment]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeedsYearAttachment_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[SpecialNeedsYearAttachment] CHECK CONSTRAINT [FK_SpecialNeedsYearAttachment_Document]
GO
ALTER TABLE [student].[SpecialNeedsYearAttachment]  WITH CHECK ADD  CONSTRAINT [FK_SpecialNeedsYearAttachment_SpecialNeedsYear] FOREIGN KEY([SpecialNeedsYearId])
REFERENCES [student].[SpecialNeedsYear] ([Id])
GO
ALTER TABLE [student].[SpecialNeedsYearAttachment] CHECK CONSTRAINT [FK_SpecialNeedsYearAttachment_SpecialNeedsYear]
GO
ALTER TABLE [student].[Student]  WITH CHECK ADD  CONSTRAINT [FK_Student_Language] FOREIGN KEY([NativeLanguageId])
REFERENCES [student].[Language] ([Id])
GO
ALTER TABLE [student].[Student] CHECK CONSTRAINT [FK_Student_Language]
GO
ALTER TABLE [student].[Student]  WITH CHECK ADD  CONSTRAINT [FK_Student_Person] FOREIGN KEY([PersonID])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[Student] CHECK CONSTRAINT [FK_Student_Person]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_Student_RepeaterReason] FOREIGN KEY([RepeaterId])
REFERENCES [student].[RepeaterReason] ([Id])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_Student_RepeaterReason]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_AdmissionDocument] FOREIGN KEY([AdmissionDocumentId])
REFERENCES [student].[AdmissionDocument] ([Id])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_AdmissionDocument]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_BasicClass]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_ClassGroup] FOREIGN KEY([ClassId])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_ClassGroup]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_ClassType] FOREIGN KEY([ClassTypeId])
REFERENCES [inst_nom].[ClassType] ([ClassTypeID])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_ClassType]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_CommuterType] FOREIGN KEY([CommuterTypeId])
REFERENCES [student].[CommuterType] ([Id])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_CommuterType]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_DischargeDocument] FOREIGN KEY([DischargeDocumentId])
REFERENCES [student].[DischargeDocument] ([Id])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_DischargeDocument]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_DischargeReasonType] FOREIGN KEY([DischargeReasonId])
REFERENCES [student].[DischargeReasonType] ([Id])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_DischargeReasonType]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_FromStudentClassId_Self] FOREIGN KEY([FromStudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_FromStudentClassId_Self]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_InstitutionSchoolYear]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_ORESType] FOREIGN KEY([ORESTypeId])
REFERENCES [student].[ORESType] ([Id])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_ORESType]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_PositionId] FOREIGN KEY([PositionId])
REFERENCES [core].[Position] ([PositionID])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_PositionId]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_RelocationDocument] FOREIGN KEY([RelocationDocumentId])
REFERENCES [student].[RelocationDocument] ([Id])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_RelocationDocument]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_Student] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_Student]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_StudentEduForm] FOREIGN KEY([StudentEduFormId])
REFERENCES [inst_nom].[EduForm] ([ClassEduFormID])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_StudentEduForm]
GO
ALTER TABLE [student].[StudentClass]  WITH CHECK ADD  CONSTRAINT [FK_StudentClass_StudentSpeciality] FOREIGN KEY([StudentSpecialityId])
REFERENCES [inst_nom].[SPPOOSpeciality] ([SPPOOSpecialityID])
GO
ALTER TABLE [student].[StudentClass] CHECK CONSTRAINT [FK_StudentClass_StudentSpeciality]
GO
ALTER TABLE [student].[StudentClassDualFormCompany]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassDualFormCompany_Creator] FOREIGN KEY([CreatedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[StudentClassDualFormCompany] CHECK CONSTRAINT [FK_StudentClassDualFormCompany_Creator]
GO
ALTER TABLE [student].[StudentClassDualFormCompany]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassDualFormCompany_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [student].[StudentClassDualFormCompany] CHECK CONSTRAINT [FK_StudentClassDualFormCompany_StudentClass]
GO
ALTER TABLE [student].[StudentClassDualFormCompany]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassDualFormCompany_Updater] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[StudentClassDualFormCompany] CHECK CONSTRAINT [FK_StudentClassDualFormCompany_Updater]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_AdmissionDocument] FOREIGN KEY([AdmissionDocumentId])
REFERENCES [student].[AdmissionDocument] ([Id])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_AdmissionDocument]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_BasicClass]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_ClassGroup] FOREIGN KEY([ClassId])
REFERENCES [inst_year].[ClassGroup] ([ClassID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_ClassGroup]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_ClassType] FOREIGN KEY([ClassTypeId])
REFERENCES [inst_nom].[ClassType] ([ClassTypeID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_ClassType]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_CommuterType] FOREIGN KEY([CommuterTypeId])
REFERENCES [student].[CommuterType] ([Id])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_CommuterType]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_Creator_SysUser]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_DischargeDocument] FOREIGN KEY([DischargeDocumentId])
REFERENCES [student].[DischargeDocument] ([Id])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_DischargeDocument]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_DischargeReasonType] FOREIGN KEY([DischargeReasonId])
REFERENCES [student].[DischargeReasonType] ([Id])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_DischargeReasonType]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_FromStudentClassId] FOREIGN KEY([FromStudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_FromStudentClassId]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_InstitutionSchoolYear] FOREIGN KEY([InstitutionId], [SchoolYear])
REFERENCES [core].[InstitutionSchoolYear] ([InstitutionId], [SchoolYear])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_InstitutionSchoolYear]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_ORESType] FOREIGN KEY([ORESTypeId])
REFERENCES [student].[ORESType] ([Id])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_ORESType]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_PositionId] FOREIGN KEY([PositionId])
REFERENCES [core].[Position] ([PositionID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_PositionId]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_RelocationDocument] FOREIGN KEY([RelocationDocumentId])
REFERENCES [student].[RelocationDocument] ([Id])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_RelocationDocument]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_RepeaterReason] FOREIGN KEY([RepeaterId])
REFERENCES [student].[RepeaterReason] ([Id])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_RepeaterReason]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_Student] FOREIGN KEY([PersonId])
REFERENCES [core].[Person] ([PersonID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_Student]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_StudentClass] FOREIGN KEY([StudentClassId])
REFERENCES [student].[StudentClass] ([ID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_StudentClass]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_StudentEduForm] FOREIGN KEY([StudentEduFormId])
REFERENCES [inst_nom].[EduForm] ([ClassEduFormID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_StudentEduForm]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_StudentSpeciality] FOREIGN KEY([StudentSpecialityId])
REFERENCES [inst_nom].[SPPOOSpeciality] ([SPPOOSpecialityID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_StudentSpeciality]
GO
ALTER TABLE [student].[StudentClassHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudentClassHistory_Updater_SysUserd] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[StudentClassHistory] CHECK CONSTRAINT [FK_StudentClassHistory_Updater_SysUserd]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_1YearAgo_Grade] FOREIGN KEY([OneYearAgoEvaluation])
REFERENCES [student].[Grade] ([Id])
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_1YearAgo_Grade]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_2YearAgo_Grade] FOREIGN KEY([TwoYearsAgoEvaluation])
REFERENCES [student].[Grade] ([Id])
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_2YearAgo_Grade]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_Annual_Grade] FOREIGN KEY([AnnualEvaluation])
REFERENCES [student].[Grade] ([Id])
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_Annual_Grade]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_CurriculumPart] FOREIGN KEY([CurriculumPartId])
REFERENCES [inst_nom].[CurriculumPart] ([CurriculumPartID])
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_CurriculumPart]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_DischargeDocument] FOREIGN KEY([DischargeDocumentId])
REFERENCES [student].[DischargeDocument] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_DischargeDocument]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_RelocationDocument] FOREIGN KEY([RelocationDocumentId])
REFERENCES [student].[RelocationDocument] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_RelocationDocument]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_Subject]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_Term1_Grade] FOREIGN KEY([FirstTermEvaluation])
REFERENCES [student].[Grade] ([Id])
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_Term1_Grade]
GO
ALTER TABLE [student].[StudentEvaluation]  WITH CHECK ADD  CONSTRAINT [FK_StudentEvaluation_Term2_Grade] FOREIGN KEY([SecondTermEvaluation])
REFERENCES [student].[Grade] ([Id])
GO
ALTER TABLE [student].[StudentEvaluation] CHECK CONSTRAINT [FK_StudentEvaluation_Term2_Grade]
GO
ALTER TABLE [student].[Validation]  WITH CHECK ADD  CONSTRAINT [FK_Validation_BasicClass] FOREIGN KEY([BasicClassId])
REFERENCES [inst_nom].[BasicClass] ([BasicClassID])
GO
ALTER TABLE [student].[Validation] CHECK CONSTRAINT [FK_Validation_BasicClass]
GO
ALTER TABLE [student].[Validation]  WITH CHECK ADD  CONSTRAINT [FK_Validation_Creator_SysUser] FOREIGN KEY([CreatedBySysUserID])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Validation] CHECK CONSTRAINT [FK_Validation_Creator_SysUser]
GO
ALTER TABLE [student].[Validation]  WITH CHECK ADD  CONSTRAINT [FK_Validation_Student] FOREIGN KEY([PersonId])
REFERENCES [student].[Student] ([PersonID])
GO
ALTER TABLE [student].[Validation] CHECK CONSTRAINT [FK_Validation_Student]
GO
ALTER TABLE [student].[Validation]  WITH CHECK ADD  CONSTRAINT [FK_Validation_Subject] FOREIGN KEY([SubjectId])
REFERENCES [inst_nom].[Subject] ([SubjectID])
GO
ALTER TABLE [student].[Validation] CHECK CONSTRAINT [FK_Validation_Subject]
GO
ALTER TABLE [student].[Validation]  WITH CHECK ADD  CONSTRAINT [FK_Validation_Updater_SysUser] FOREIGN KEY([ModifiedBySysUserId])
REFERENCES [core].[SysUser] ([SysUserID])
GO
ALTER TABLE [student].[Validation] CHECK CONSTRAINT [FK_Validation_Updater_SysUser]
GO
ALTER TABLE [student].[ValidationDocument]  WITH CHECK ADD  CONSTRAINT [FK_ValidationDocument_Document] FOREIGN KEY([DocumentId])
REFERENCES [student].[Document] ([Id])
GO
ALTER TABLE [student].[ValidationDocument] CHECK CONSTRAINT [FK_ValidationDocument_Document]
GO
ALTER TABLE [student].[ValidationDocument]  WITH CHECK ADD  CONSTRAINT [FK_ValidationDocument_Validation] FOREIGN KEY([ValidationId])
REFERENCES [student].[Validation] ([Id])
GO
ALTER TABLE [student].[ValidationDocument] CHECK CONSTRAINT [FK_ValidationDocument_Validation]
GO
ALTER TABLE [student].[AbsenceCampaign]  WITH CHECK ADD  CONSTRAINT [CHK_AbsenceCampaign_FromDate_ToDate] CHECK  (([FromDate]<=[ToDate]))
GO
ALTER TABLE [student].[AbsenceCampaign] CHECK CONSTRAINT [CHK_AbsenceCampaign_FromDate_ToDate]
GO
ALTER TABLE [student].[AbsenceCampaign]  WITH CHECK ADD  CONSTRAINT [CHK_AbsenceCampaign_Month] CHECK  (([Month]>=(1) AND [Month]<=(12)))
GO
ALTER TABLE [student].[AbsenceCampaign] CHECK CONSTRAINT [CHK_AbsenceCampaign_Month]
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport]  WITH CHECK ADD  CONSTRAINT [CHK_ASPMonthlyBenefitsImport_FromDate_ToDate] CHECK  (([FromDate]<=[ToDate]))
GO
ALTER TABLE [student].[ASPMonthlyBenefitsImport] CHECK CONSTRAINT [CHK_ASPMonthlyBenefitsImport_FromDate_ToDate]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   TRIGGER [student].[tr_StdentClass_IsCurrent_Update] ON [student].[StudentClass]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF (UPDATE(IsCurrent))
    BEGIN
        DELETE cs
        FROM student.StudentClass sc
        INNER JOIN inst_year.CurriculumStudent cs ON cs.StudentID = sc.ID
        INNER JOIN INSERTED i ON sc.ID = i.ID
        WHERE i.IsCurrent = 0



       --UPDATE cs
        --    SET IsValid = 0
        --FROM student.StudentClass sc
        --INNER JOIN inst_year.CurriculumStudent cs ON cs.StudentID = sc.ID
        --INNER JOIN INSERTED i ON sc.ID = i.ID
        --WHERE i.IsCurrent = 0



   END
END
GO
ALTER TABLE [student].[StudentClass] DISABLE TRIGGER [tr_StdentClass_IsCurrent_Update]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupport', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид на РП' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupport', @level2type=N'COLUMN',@level2name=N'ResourceSupportTypeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка документ за насочване з' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupport', @level2type=N'COLUMN',@level2name=N'ResourceSupportReportId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Данни за РП' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupport'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportReport', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Номер на документа за насочван' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportReport', @level2type=N'COLUMN',@level2name=N'ReportNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата на документа' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportReport', @level2type=N'COLUMN',@level2name=N'ReportDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportReport', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към човек' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportReport', @level2type=N'COLUMN',@level2name=N'PersonId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'РП - данни за документите за н' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportReport'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Име на специалиста' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Организация-вид' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'OrganizationType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Организация-име' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'OrganizationName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид специалист' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'SpecialistType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид специалист' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'ResourceSupportSpecialistTypeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка към документ за ресурсн' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'ResourceSupportId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Потребител' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'SysUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Работи в' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist', @level2type=N'COLUMN',@level2name=N'WorkPlaceId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'РП - данни за специалистите, к' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ResourceSupportSpecialist'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на учен' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'StudentClassID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на размера на стипендията' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'ScholarshipAmountID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Бележки' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на вида стипендия' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'ScholarshipTypeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Стипендия - сума' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'AmountRate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Период, за който е отпусната' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'Periodicity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Номер на заповедта' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'OrderNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата на заповедта' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'OrderDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'InstitutionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Финансиращ орган' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'FinancingOrganId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на лице' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent', @level2type=N'COLUMN',@level2name=N'PersonId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ученици - стипендии' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'ScholarshipStudent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeeds', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка с SpecialNeedsType' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeeds', @level2type=N'COLUMN',@level2name=N'SpecialNeedsYearId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид СОП' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeeds', @level2type=N'COLUMN',@level2name=N'SpecialNeedsTypeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид СОП - детайлен' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeeds', @level2type=N'COLUMN',@level2name=N'SpecialNeedsSubTypeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'СОП' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeeds'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeedsYear', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeedsYear', @level2type=N'COLUMN',@level2name=N'PersonId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Учебна година' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeedsYear', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'не се използва' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeedsYear', @level2type=N'COLUMN',@level2name=N'HasSuportiveEnvironment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'не се използва' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeedsYear', @level2type=N'COLUMN',@level2name=N'SupportiveEnvironment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'СОП - година' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'SpecialNeedsYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Връзка ученик' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'PersonID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Домашен телефон' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'HomePhone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Служебен телефон' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'WorkPhone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Мобилен телефон' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'MobilePhone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Е-мейл' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'Email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Живее в общежитие, център за з' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'LivesWithFosterFamily'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Родителите са дали съгласие да' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'HasParentConsent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Телефон на личния лекар' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'GPPhone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Име на личния лекар' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'GPName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Детето се представлява от кмет' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'RepresentedByTheMajor'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Език на семейната среда' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'NativeLanguageId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Коефициент на пазара на труда ' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'FamilyEducationWeight'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Коефициент на образователния с' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'FamilyWorkStatusWeight'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Резултат от интеграция' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student', @level2type=N'COLUMN',@level2name=N'UserManagementIntegrationResult'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ученици - допълнителни данни' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Student'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на запи' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на учеб' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'SchoolYear'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на пара' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'ClassId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Уникален идентификатор на лице' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'PersonId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Специалност на ученика' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'StudentSpecialityId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Форма на обучение' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'StudentEduFormId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Номер в класа' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'ClassNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Статус на ученика в класа/груп' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер за "Индивидуален учебен' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'IsIndividualCurriculum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер за "Почасова организаци' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'IsHourlyOrganization'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер дали детето/ченикът се ' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'IsForSubmissionToNRA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Маркер дали детето е записано ' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'IsCurrent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Повторно записан' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'RepeaterId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пътуващ' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'CommuterTypeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Дата на записване в класа/груп' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'EnrollmentDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Документ за записване в инстит' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'AdmissionDocumentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Позиция в институцията' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'PositionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на випуска (възр' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'BasicClassId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Вид на паралелката/групата' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'ClassTypeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Идентификатор на клас, от койт' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'FromStudentClassId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Причина за отписване' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'DischargeReasonId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Документ за отписване' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'DischargeDocumentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Документ за преместване' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'RelocationDocumentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Тип ОРЕС' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'ORESTypeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Код на институцията' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass', @level2type=N'COLUMN',@level2name=N'InstitutionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'деца/ученици, записани в класо' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'StudentClass'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Temp for Azure accounts deletion' , @level0type=N'SCHEMA',@level0name=N'student', @level1type=N'TABLE',@level1name=N'Temp_StdsForDel'
GO
