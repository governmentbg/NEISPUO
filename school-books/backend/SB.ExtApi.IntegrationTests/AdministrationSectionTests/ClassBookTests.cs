namespace SB.ExtApi.IntegrationTests;

using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using System;
using System.Collections.Generic;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class ClassBookTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classIdWithNoClassBook;
    private readonly int classIdOnChildLevel;
    private readonly int classIdOnParentLevel;
    private readonly bool isLevel2Class;
    private readonly GradeDO grade;
    private readonly ExtApiWebApplicationFactory appFactory;

    public ClassBookTests(
        OrderedFixtures<
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                ExtApiWebApplicationFactory
            >> fixtures)
    {
        DataFixture_V_XII data = fixtures.Values.Item2;
        ExtApiWebApplicationFactory appFactory = fixtures.Values.Item5;

        this.appFactory = appFactory;
        this.schoolYear = data.SchoolYear;
        this.institutionId = data.InstitutionId;
        this.classIdWithNoClassBook = data.ClassIdWithNoClassBook;

        this.classIdOnChildLevel = fixtures.Values.Item4.ClassIdOnChildLevel;
        this.classIdOnParentLevel = fixtures.Values.Item4.ClassIdOnParentLevel;
        this.isLevel2Class = fixtures.Values.Item4.IsLevel2Class;
        this.grade =
            new GradeDO
            {
                PersonId = data.PersonId,
                CurriculumId = data.ScheduleLessonCurriculumId,
                Date = data.ScheduleLessonDate,
                Category = GradeCategory.Decimal,
                Type = GradeType.Homework,
                DecimalGrade = 5m,
                QualitativeGrade = null,
                SpecialGrade = null,
                Comment = null,
                ScheduleLessonId = data.ScheduleLessonId
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbooks_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGetAsync(this.schoolYear, this.institutionId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_and_remove_classbook()
    {
        var classBookId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPostAsync(this.schoolYear, this.institutionId, this.classIdWithNoClassBook);
        Assert.InRange(classBookId, 1, int.MaxValue);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksDeleteAsync(this.schoolYear, this.institutionId, classBookId);

        var classBookDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGetAsync(this.schoolYear, this.institutionId))
            .Where(s => s.ClassBookId == classBookId)
            .SingleOrDefault();

        Assert.Null(classBookDeleted);
    }

    //TODO: Find workaround to test it with the current data setup
    //[Fact]
    //public async Task Should_combine_and_after_that_separate_class_book()
    //{
    //    var apiClient = this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider);

    //    if (this.isLevel2Class)
    //    {
    //        // Test combining class 
    //        var classBookIdAfterCombine = await apiClient
    //            .ClassBooksCombineClassBooksAsync(
    //                this.schoolYear,
    //                this.institutionId,
    //                new ClassBookCombineDO
    //                {
    //                    ParentClassId = this.classIdOnParentLevel,
    //                    ChildClassIdForDataTransfer = this.classIdOnChildLevel
    //                });

    //        Assert.InRange(classBookIdAfterCombine, 1, int.MaxValue);

    //        var schedulesAfterCombine = await apiClient.ClassBooksSchedulesGetAsync(this.schoolYear, this.institutionId, classBookIdAfterCombine);
    //        Assert.NotEmpty(schedulesAfterCombine);

    //        var classBooksAfterCombine = await apiClient.ClassBooksGetAsync(this.schoolYear, this.institutionId);
    //        var oldClassBookBeforeCombine = classBooksAfterCombine.First(cb => cb.ClassId == this.classIdOnChildLevel);

    //        Assert.False(oldClassBookBeforeCombine.IsValid);

    //        // Test separate
    //        var classBookIdsAfterSeparate = await apiClient
    //            .ClassBooksSeparateClassBooksAsync(
    //                this.schoolYear,
    //                this.institutionId,
    //                new ClassBookSeparateDO
    //                {
    //                    ParentClassId = this.classIdOnParentLevel,
    //                    ChildClassIds = new List<int> { this.classIdOnChildLevel }
    //                });

    //        Assert.NotNull(classBookIdsAfterSeparate);

    //        foreach (var classBookIdAfterSeparate in classBookIdsAfterSeparate)
    //        {
    //            var schedulesAfterSeparate = await apiClient.ClassBooksSchedulesGetAsync(this.schoolYear, this.institutionId, classBookIdAfterSeparate);
    //            Assert.NotEmpty(schedulesAfterSeparate);
    //        }
    //    }
    //    else
    //    {
    //        // Test separate
    //        var classBookIdsAfterSeparate = await apiClient
    //            .ClassBooksSeparateClassBooksAsync(
    //                this.schoolYear,
    //                this.institutionId,
    //                new ClassBookSeparateDO
    //                {
    //                    ParentClassId = this.classIdOnParentLevel,
    //                    ChildClassIds = new List<int> { this.classIdOnChildLevel }
    //                });

    //        Assert.NotNull(classBookIdsAfterSeparate);

    //        foreach (var classBookIdAfterSeparate in classBookIdsAfterSeparate)
    //        {
    //            var schedulesAfterSeparate = await apiClient.ClassBooksSchedulesGetAsync(this.schoolYear, this.institutionId, classBookIdAfterSeparate);
    //            Assert.NotEmpty(schedulesAfterSeparate);
    //        }

    //        var classBooksAfterSeparate = await apiClient.ClassBooksGetAsync(this.schoolYear, this.institutionId);
    //        var oldClassBookBeforeSeparate = classBooksAfterSeparate.First(cb => cb.ClassId == this.classIdOnParentLevel);

    //        Assert.False(oldClassBookBeforeSeparate.IsValid);

    //        // Test combining class 
    //        var classBookIdAfterCombine = await apiClient
    //            .ClassBooksCombineClassBooksAsync(
    //                this.schoolYear,
    //                this.institutionId,
    //                new ClassBookCombineDO
    //                {
    //                    ParentClassId = this.classIdOnParentLevel,
    //                    ChildClassIdForDataTransfer = this.classIdOnChildLevel
    //                });

    //        Assert.InRange(classBookIdAfterCombine, 1, int.MaxValue);

    //        var schedulesAfterCombine = await apiClient.ClassBooksSchedulesGetAsync(this.schoolYear, this.institutionId, classBookIdAfterCombine);
    //        Assert.NotEmpty(schedulesAfterCombine);

    //        var classBooksAfterCombine = await apiClient.ClassBooksGetAsync(this.schoolYear, this.institutionId);
    //        var oldClassBookBeforeCombine = classBooksAfterCombine.First(cb => cb.ClassId == this.classIdOnChildLevel);

    //        Assert.False(oldClassBookBeforeCombine.IsValid);
    //    }
    //}

    //TODO: Find workaround to test it with the current data setup
    //[Fact]
    //public async Task Should_throw_exception_after_marked_as_invalid_class_book()
    //{
    //    var apiClient = this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider);

    //    if (this.isLevel2Class)
    //    {
    //        await apiClient
    //            .ClassBooksCombineClassBooksAsync(
    //                this.schoolYear,
    //                this.institutionId,
    //                new ClassBookCombineDO
    //                {
    //                    ParentClassId = this.classIdOnParentLevel,
    //                    ChildClassIdForDataTransfer = this.classIdOnChildLevel
    //                });

    //        var classBookId = (await apiClient.ClassBooksGetAsync(this.schoolYear, this.institutionId)).First(cb => cb.IsValid == false).ClassBookId ?? 0;

    //        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
    //            async () => await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
    //                .ClassBooksGradesPostAsync(
    //                    this.schoolYear,
    //                    this.institutionId,
    //                    classBookId,
    //                    this.grade));

    //        Assert.Equal(400, ex.StatusCode);
    //        Assert.Contains("The classbook is marked as invalid (archived).", ex.Message);
    //    }
    //    else
    //    {
    //        await apiClient
    //            .ClassBooksSeparateClassBooksAsync(
    //                this.schoolYear,
    //                this.institutionId,
    //                new ClassBookSeparateDO
    //                {
    //                    ParentClassId = this.classIdOnParentLevel,
    //                    ChildClassIds = new List<int> { this.classIdOnChildLevel }
    //                });

    //        var classBookId = (await apiClient.ClassBooksGetAsync(this.schoolYear, this.institutionId)).First(cb => cb.IsValid == false).ClassBookId ?? 0;

    //        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
    //            async () => await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
    //                .ClassBooksGradesPostAsync(
    //                    this.schoolYear,
    //                    this.institutionId,
    //                    classBookId,
    //                    this.grade));

    //        Assert.Equal(400, ex.StatusCode);
    //        Assert.Contains("The classbook is marked as invalid (archived).", ex.Message);
    //    }
    //}

    // Note! This test requires the blobs service to be running
    [Theory,
        InlineData("Files/print_signed.pdf"),
        InlineData("Files/print_multiple_signed.pdf")
    ]
    public async Task Finalize_should_succeed_on_validly_signed_pdf(string file)
    {
        // Setup
        using Stream printFileStream = EmbeddedFilesHelper.CreateEmbeddedFileReadStream(file);

        var classBookId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPostAsync(this.schoolYear, this.institutionId, this.classIdWithNoClassBook);
        Assert.InRange(classBookId, 1, int.MaxValue);

        // Act
        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
            .ClassBooksFinalizeAsync(
                this.schoolYear,
                this.institutionId,
                classBookId,
                new FileParameter(
                    printFileStream,
                    "print.pdf",
                    "application/pdf"
                ));

        // Assert
        var classBook = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGetAsync(this.schoolYear, this.institutionId))
            .Where(s => s.ClassBookId == classBookId)
            .SingleOrDefault();

        Assert.NotNull(classBook);
        Assert.True(classBook.IsFinalized);
    }

    // Note! This test requires the blobs service to be running
    [Theory,
        InlineData("Files/print_unsigned.pdf", "missing_required_signature"),
        InlineData("Files/print_selfsigned.pdf", "signature_validation_failed")]
    public async Task Finalize_should_fail_on_unsigned_pdf(string file, string error)
    {
        // Setup
        using Stream printFileStream = EmbeddedFilesHelper.CreateEmbeddedFileReadStream(file);

        var classBookId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPostAsync(this.schoolYear, this.institutionId, this.classIdWithNoClassBook);
        Assert.InRange(classBookId, 1, int.MaxValue);

        // Act && Assert
        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () => await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
                .ClassBooksFinalizeAsync(
                    this.schoolYear,
                    this.institutionId,
                    classBookId,
                    new FileParameter(
                        printFileStream,
                        "print.pdf",
                        "application/pdf"
                    )));
        Assert.Equal(400, ex.StatusCode);
        Assert.Contains(error, ex.Message);
    }
}
