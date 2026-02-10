ALTER TABLE [inst_nom].[DBVersion] ADD  CONSTRAINT [DF_DBVersion_CreateAt]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [inst_nom].[InstAdminData] ADD  DEFAULT ((0)) FOR [IsInnovative]
GO
ALTER TABLE [inst_nom].[InstAdminData] ADD  DEFAULT ((0)) FOR [IsCentral]
GO
ALTER TABLE [inst_nom].[InstAdminData] ADD  DEFAULT ((0)) FOR [IsProtected]
GO
ALTER TABLE [inst_nom].[InstAdminData] ADD  DEFAULT ((0)) FOR [IsStateFunded]
GO
ALTER TABLE [inst_nom].[InstAdminData] ADD  DEFAULT ((0)) FOR [HasMunDecisionFor4]
GO
ALTER TABLE [inst_nom].[NKPDCodeMON] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_nom].[NKPDPositionCodeMON] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_nom].[Subject] ADD  DEFAULT ((0)) FOR [IsMandatory]
GO
ALTER TABLE [inst_nom].[TerminationReason] ADD  DEFAULT ((1)) FOR [IsValid]
GO
ALTER TABLE [inst_nom].[BuildingAreaType]  WITH CHECK ADD  CONSTRAINT [FK_BuildingAreaTypeBuildingType] FOREIGN KEY([BuildingTypeID])
REFERENCES [inst_nom].[BuildingType] ([BuildingTypeID])
GO
ALTER TABLE [inst_nom].[BuildingAreaType] CHECK CONSTRAINT [FK_BuildingAreaTypeBuildingType]
GO
ALTER TABLE [inst_nom].[BuildingRoomType]  WITH CHECK ADD  CONSTRAINT [FK_BuildingRoomTypeAreaType] FOREIGN KEY([BuildingAreaTypeID])
REFERENCES [inst_nom].[BuildingAreaType] ([BuildingAreaTypeID])
GO
ALTER TABLE [inst_nom].[BuildingRoomType] CHECK CONSTRAINT [FK_BuildingRoomTypeAreaType]
GO
ALTER TABLE [inst_nom].[CampaignData]  WITH CHECK ADD  CONSTRAINT [FK_CampaignDataInstNom] FOREIGN KEY([InstType])
REFERENCES [inst_nom].[InstType] ([InstTypeID])
GO
ALTER TABLE [inst_nom].[CampaignData] CHECK CONSTRAINT [FK_CampaignDataInstNom]
GO
ALTER TABLE [inst_nom].[CategoryStaffType]  WITH CHECK ADD  CONSTRAINT [FK_CategoryStaffTypeStaffType] FOREIGN KEY([StaffTypeID])
REFERENCES [inst_nom].[StaffType] ([StaffTypeID])
GO
ALTER TABLE [inst_nom].[CategoryStaffType] CHECK CONSTRAINT [FK_CategoryStaffTypeStaffType]
GO
ALTER TABLE [inst_nom].[CurriculumLibrary]  WITH CHECK ADD  CONSTRAINT [FK_CurriculumLibrary_CurriculumCatalog] FOREIGN KEY([CurriculumCatalogID])
REFERENCES [inst_nom].[CurriculumCatalog] ([CurriculumCatalogID])
GO
ALTER TABLE [inst_nom].[CurriculumLibrary] CHECK CONSTRAINT [FK_CurriculumLibrary_CurriculumCatalog]
GO
ALTER TABLE [inst_nom].[EducationArea]  WITH CHECK ADD  CONSTRAINT [FK_BasicEducationAreaBasicClass] FOREIGN KEY([BasicEducationAreaID])
REFERENCES [inst_nom].[BasicEducationArea] ([BasicEducationAreaID])
GO
ALTER TABLE [inst_nom].[EducationArea] CHECK CONSTRAINT [FK_BasicEducationAreaBasicClass]
GO
ALTER TABLE [inst_nom].[EquipmentType]  WITH CHECK ADD  CONSTRAINT [FK_EquipmentTypeBuildingRoomType] FOREIGN KEY([BuildingRoomTypeID])
REFERENCES [inst_nom].[BuildingRoomType] ([BuildingRoomTypeID])
GO
ALTER TABLE [inst_nom].[EquipmentType] CHECK CONSTRAINT [FK_EquipmentTypeBuildingRoomType]
GO
ALTER TABLE [inst_nom].[NKPDGroup]  WITH CHECK ADD  CONSTRAINT [FK_NKPDGroupNKPDSubClass] FOREIGN KEY([NKPDSubClassID])
REFERENCES [inst_nom].[NKPDSubClass] ([NKPDSubClassID])
GO
ALTER TABLE [inst_nom].[NKPDGroup] CHECK CONSTRAINT [FK_NKPDGroupNKPDSubClass]
GO
ALTER TABLE [inst_nom].[NKPDPosition]  WITH CHECK ADD  CONSTRAINT [FK_NKPDPositionNKPDSubGroup] FOREIGN KEY([NKPDSubGroupID])
REFERENCES [inst_nom].[NKPDSubGroup] ([NKPDSubGroupID])
GO
ALTER TABLE [inst_nom].[NKPDPosition] CHECK CONSTRAINT [FK_NKPDPositionNKPDSubGroup]
GO
ALTER TABLE [inst_nom].[NKPDPositionCodeMON]  WITH CHECK ADD  CONSTRAINT [FK_NKPDPositionCodeMON_NKPDCodeMON] FOREIGN KEY([OccMONID])
REFERENCES [inst_nom].[NKPDCodeMON] ([OccMONID])
GO
ALTER TABLE [inst_nom].[NKPDPositionCodeMON] CHECK CONSTRAINT [FK_NKPDPositionCodeMON_NKPDCodeMON]
GO
ALTER TABLE [inst_nom].[NKPDPositionCodeMON]  WITH CHECK ADD  CONSTRAINT [FK_NKPDPositionCodeMON_NKPDPosition] FOREIGN KEY([NKPDPositionID])
REFERENCES [inst_nom].[NKPDPosition] ([NKPDPositionID])
GO
ALTER TABLE [inst_nom].[NKPDPositionCodeMON] CHECK CONSTRAINT [FK_NKPDPositionCodeMON_NKPDPosition]
GO
ALTER TABLE [inst_nom].[NKPDSubClass]  WITH CHECK ADD  CONSTRAINT [FK_NKPDSubClassNKPDClass] FOREIGN KEY([NKPDClassID])
REFERENCES [inst_nom].[NKPDClass] ([NKPDClassID])
GO
ALTER TABLE [inst_nom].[NKPDSubClass] CHECK CONSTRAINT [FK_NKPDSubClassNKPDClass]
GO
ALTER TABLE [inst_nom].[NKPDSubGroup]  WITH CHECK ADD  CONSTRAINT [FK_NKPDSubGroupNKPDGroup] FOREIGN KEY([NKPDGroupID])
REFERENCES [inst_nom].[NKPDGroup] ([NKPDGroupID])
GO
ALTER TABLE [inst_nom].[NKPDSubGroup] CHECK CONSTRAINT [FK_NKPDSubGroupNKPDGroup]
GO
ALTER TABLE [inst_nom].[QCourseBudgetSourceType]  WITH CHECK ADD  CONSTRAINT [FK_QCourseBudgetSourceTypeQCourseActType] FOREIGN KEY([QCourseActTypeID])
REFERENCES [inst_nom].[QCourseActType] ([QCourseActTypeID])
GO
ALTER TABLE [inst_nom].[QCourseBudgetSourceType] CHECK CONSTRAINT [FK_QCourseBudgetSourceTypeQCourseActType]
GO
ALTER TABLE [inst_nom].[QCourseDurationType]  WITH CHECK ADD  CONSTRAINT [FK_QCourseDurationTypeQCourseActType] FOREIGN KEY([QCourseActTypeID])
REFERENCES [inst_nom].[QCourseActType] ([QCourseActTypeID])
GO
ALTER TABLE [inst_nom].[QCourseDurationType] CHECK CONSTRAINT [FK_QCourseDurationTypeQCourseActType]
GO
ALTER TABLE [inst_nom].[QCourseType]  WITH CHECK ADD  CONSTRAINT [FK_QCourseTypeQCourseActType] FOREIGN KEY([QCourseActTypeID])
REFERENCES [inst_nom].[QCourseActType] ([QCourseActTypeID])
GO
ALTER TABLE [inst_nom].[QCourseType] CHECK CONSTRAINT [FK_QCourseTypeQCourseActType]
GO
ALTER TABLE [inst_nom].[SchoolProfile]  WITH CHECK ADD  CONSTRAINT [FK_SchoolProfileSchoolBasicProfile] FOREIGN KEY([SchoolBasicProfileID])
REFERENCES [inst_nom].[SchoolBasicProfile] ([SchoolBasicProfileID])
GO
ALTER TABLE [inst_nom].[SchoolProfile] CHECK CONSTRAINT [FK_SchoolProfileSchoolBasicProfile]
GO
ALTER TABLE [inst_nom].[SPPOOProfArea]  WITH CHECK ADD  CONSTRAINT [FK_SPPOOProfAreaSPPOOEducArea] FOREIGN KEY([EducAreaID])
REFERENCES [inst_nom].[SPPOOEducArea] ([SPPOOEducAreaID])
GO
ALTER TABLE [inst_nom].[SPPOOProfArea] CHECK CONSTRAINT [FK_SPPOOProfAreaSPPOOEducArea]
GO
ALTER TABLE [inst_nom].[SPPOOProfession]  WITH CHECK ADD  CONSTRAINT [FK_SPPOOProfessionProfArea] FOREIGN KEY([ProfAreaID])
REFERENCES [inst_nom].[SPPOOProfArea] ([SPPOOProfAreaID])
GO
ALTER TABLE [inst_nom].[SPPOOProfession] CHECK CONSTRAINT [FK_SPPOOProfessionProfArea]
GO
ALTER TABLE [inst_nom].[SPPOOSpeciality]  WITH CHECK ADD  CONSTRAINT [FK_SPPOOSpecialityProfession] FOREIGN KEY([ProfessionID])
REFERENCES [inst_nom].[SPPOOProfession] ([SPPOOProfessionID])
GO
ALTER TABLE [inst_nom].[SPPOOSpeciality] CHECK CONSTRAINT [FK_SPPOOSpecialityProfession]
GO
ALTER TABLE [inst_nom].[SubjectType]  WITH CHECK ADD  CONSTRAINT [FK_SubjTypeBaseSubjType] FOREIGN KEY([BasicSubjectTypeID])
REFERENCES [inst_nom].[BasicSubjectType] ([BasicSubjectTypeID])
GO
ALTER TABLE [inst_nom].[SubjectType] CHECK CONSTRAINT [FK_SubjTypeBaseSubjType]
GO
ALTER TABLE [inst_nom].[University]  WITH CHECK ADD  CONSTRAINT [FK_UniversityTown] FOREIGN KEY([TownID])
REFERENCES [location].[Town] ([TownID])
GO
ALTER TABLE [inst_nom].[University] CHECK CONSTRAINT [FK_UniversityTown]
GO
