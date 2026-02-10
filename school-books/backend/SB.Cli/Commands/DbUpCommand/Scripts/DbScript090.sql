ALTER TABLE [school_books].[GradeChangeExamsAdmProtocolStudentSubject]
ADD
    [SubjectTypeId] INT NOT NULL CONSTRAINT DEFAULT_SubjectTypeId DEFAULT -1,
    CONSTRAINT [FK_GradeChangeExamsAdmProtocolStudentSubject_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [school_books].[GradeChangeExamsAdmProtocolStudentSubject]
DROP
    CONSTRAINT DEFAULT_SubjectTypeId,
    CONSTRAINT PK_GradeChangeExamsAdmProtocolStudentSubject
GO
ALTER TABLE [school_books].[GradeChangeExamsAdmProtocolStudentSubject]
ADD CONSTRAINT [PK_GradeChangeExamsAdmProtocolStudentSubject] PRIMARY KEY ([SchoolYear], [GradeChangeExamsAdmProtocolId], [ClassId], [PersonId], [SubjectId], [SubjectTypeId])

ALTER TABLE [school_books].[StateExamsAdmProtocolStudentSubject]
ADD
    [SubjectTypeId] INT NOT NULL CONSTRAINT DEFAULT_SubjectTypeId DEFAULT -1,
    CONSTRAINT [FK_StateExamsAdmProtocolStudentSubject_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
ALTER TABLE [school_books].[StateExamsAdmProtocolStudentSubject]
DROP
    CONSTRAINT DEFAULT_SubjectTypeId,
    CONSTRAINT PK_StateExamsAdmProtocolStudentSubject
GO
ALTER TABLE [school_books].[StateExamsAdmProtocolStudentSubject]
ADD CONSTRAINT [PK_StateExamsAdmProtocolStudentSubject] PRIMARY KEY ([SchoolYear], [StateExamsAdmProtocolId], [ClassId], [PersonId], [SubjectId], [SubjectTypeId], [IsAdditional])

ALTER TABLE [school_books].[StateExamsAdmProtocolStudent]
ADD
    [SecondMandatorySubjectTypeId] INT NULL,
    CONSTRAINT [FK_StateExamsAdmProtocolStudent_SubjectType] FOREIGN KEY ([SecondMandatorySubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID])
GO
