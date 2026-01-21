GO

EXEC sp_rename 'school_books.SchoolYearDateInfoIdSequence', 'SchoolYearSettingsIdSequence', 'OBJECT'
EXEC sp_rename 'school_books.SchoolYearDateInfo.SchoolYearDateInfoId', 'SchoolYearSettingsId', 'COLUMN'
EXEC sp_rename 'school_books.SchoolYearDateInfo.PK_SchoolYearDateInfo', 'PK_SchoolYearSettings'
EXEC sp_rename 'school_books.FK_SchoolYearDateInfo_InstId_SchoolYear', 'FK_SchoolYearSettings_InstId_SchoolYear', 'OBJECT'
EXEC sp_rename 'school_books.FK_SchoolYearDateInfo_CreatedBySysUserId', 'FK_SchoolYearSettings_CreatedBySysUserId', 'OBJECT'
EXEC sp_rename 'school_books.FK_SchoolYearDateInfo_ModifiedBySysUserId', 'FK_SchoolYearSettings_ModifiedBySysUserId', 'OBJECT'
EXEC sp_rename 'school_books.SchoolYearDateInfo.IX_SchoolYearDateInfo', 'IX_SchoolYearSettings', 'INDEX'
EXEC sp_rename 'school_books.SchoolYearDateInfo.UQ_SchoolYearDateInfo_SchoolYear_InstId_IsForAllClasses', 'UQ_SchoolYearSettings_SchoolYear_InstId_IsForAllClasses', 'INDEX'
EXEC sp_rename 'school_books.SchoolYearDateInfo', 'SchoolYearSettings'
GO

EXEC sp_rename 'school_books.SchoolYearDateInfoClass.SchoolYearDateInfoId', 'SchoolYearSettingsId', 'COLUMN'
EXEC sp_rename 'school_books.SchoolYearDateInfoClass.PK_SchoolYearDateInfoClass', 'PK_SchoolYearSettingsClass'
EXEC sp_rename 'school_books.FK_SchoolYearDateInfoClass_SchoolYearDateInfo', 'FK_SchoolYearSettingsClass_SchoolYearSettings', 'OBJECT'
EXEC sp_rename 'school_books.FK_SchoolYearDateInfoClass_BasicClass', 'FK_SchoolYearSettingsClass_BasicClass', 'OBJECT'
EXEC sp_rename 'school_books.SchoolYearDateInfoClass', 'SchoolYearSettingsClass'
GO

EXEC sp_rename 'school_books.SchoolYearDateInfoClassBook.SchoolYearDateInfoId', 'SchoolYearSettingsId', 'COLUMN'
EXEC sp_rename 'school_books.SchoolYearDateInfoClassBook.PK_SchoolYearDateInfoClassBook', 'PK_SchoolYearSettingsClassBook'
EXEC sp_rename 'school_books.FK_SchoolYearDateInfoClassBook_SchoolYearDateInfo', 'FK_SchoolYearSettingsClassBook_SchoolYearSettings', 'OBJECT'
EXEC sp_rename 'school_books.FK_SchoolYearDateInfoClassBook_ClassBook', 'FK_SchoolYearSettingsClassBook_ClassBook', 'OBJECT'
EXEC sp_rename 'school_books.SchoolYearDateInfoClassBook', 'SchoolYearSettingsClassBook'
GO

EXEC sp_rename 'school_books.SchoolYearDateInfoDefault.PK_SchoolYearDateInfoDefault', 'PK_SchoolYearSettingsDefault'
EXEC sp_rename 'school_books.SchoolYearDateInfoDefault', 'SchoolYearSettingsDefault'
GO

EXEC sp_rename 'school_books.ClassBookSchoolYearDateInfo.SchoolYearDateInfoId', 'SchoolYearSettingsId', 'COLUMN'
EXEC sp_rename 'school_books.ClassBookSchoolYearDateInfo.PK_ClassBookSchoolYearDateInfo', 'PK_ClassBookSchoolYearSettings'
EXEC sp_rename 'school_books.FK_ClassBookSchoolYearDateInfo_ClassBook', 'FK_ClassBookSchoolYearSettings_ClassBook', 'OBJECT'
EXEC sp_rename 'school_books.FK_ClassBookSchoolYearDateInfo_SchoolYearDateInfo', 'FK_ClassBookSchoolYearSettings_SchoolYearSettings', 'OBJECT'
EXEC sp_rename 'school_books.ClassBookSchoolYearDateInfo', 'ClassBookSchoolYearSettings'
GO
