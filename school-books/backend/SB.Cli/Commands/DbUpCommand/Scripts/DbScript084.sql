GO

ALTER TABLE [school_books].[ClassBookStudentGradeless]
DROP CONSTRAINT [CHK_ClassBookStudentGradeless_WithoutFirstTermGrade_WithoutSecondTermGrade_WithoutFinalGrade];
GO

ALTER TABLE [school_books].[ClassBookStudentGradeless]
ADD CONSTRAINT [CHK_ClassBookStudentGradeless_WithoutFirstTermGrade_WithoutSecondTermGrade_WithoutFinalGrade]
        CHECK (
            -- if all fields are FALSE (0) the record should not exist
            [WithoutFirstTermGrade] != 0 OR [WithoutSecondTermGrade] != 0 OR [WithoutFinalGrade] != 0
        )
GO
