namespace SB.Domain;

using SB.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class ClassBook : IAggregateRoot
{
    [GeneratedRegex("CN=([^,]+)", RegexOptions.IgnoreCase)]
    private static partial Regex CommonNameRegex();

    public const int UndoIntervalInMinutes = 60;
    public const int FirstGradeBasicClassId = 1;
    public const int SecondGradeBasicClassId = 2;
    public const int ThirdGradeBasicClassId = 3;

    public static bool CheckBookTypeAllowsGrades(ClassBookType bookType)
        => CheckBookTypeAllowsDecimalGrades(bookType) ||
            CheckBookTypeAllowsQualitativeGrades(bookType) ||
            CheckBookTypeAllowsSpecialGrades(bookType);

    public static bool CheckBookTypeAllowsDecimalGrades(ClassBookType bookType)
        => bookType == ClassBookType.Book_IV ||
            bookType == ClassBookType.Book_V_XII ||
            bookType == ClassBookType.Book_CSOP;

    public static bool CheckBookTypeAllowsQualitativeGrades(ClassBookType bookType)
        => bookType == ClassBookType.Book_I_III ||
            bookType == ClassBookType.Book_CSOP;

    public static bool CheckBookTypeAllowsSpecialGrades(ClassBookType bookType)
        => bookType == ClassBookType.Book_I_III ||
            bookType == ClassBookType.Book_IV ||
            bookType == ClassBookType.Book_V_XII ||
            bookType == ClassBookType.Book_CSOP;

    public static bool CheckBookTypeAllowsAbsences(ClassBookType bookType)
        => bookType == ClassBookType.Book_I_III ||
            bookType == ClassBookType.Book_IV ||
            bookType == ClassBookType.Book_V_XII ||
            bookType == ClassBookType.Book_CSOP ||
            bookType == ClassBookType.Book_CDO;

    public static bool CheckBookTypeAllowsDplrAbsences(ClassBookType bookType)
        => bookType == ClassBookType.Book_DPLR;

    public static bool CheckBookTypeAllowsAttendances(ClassBookType bookType)
        => bookType == ClassBookType.Book_PG;

    public static bool CheckBookTypeAllowsAdditionalActivities(ClassBookType bookType)
        => bookType == ClassBookType.Book_PG;

    public static bool CheckBookTypeAllowsStudentActivities(ClassBookType bookType)
        => bookType == ClassBookType.Book_PG ||
            bookType == ClassBookType.Book_I_III ||
            bookType == ClassBookType.Book_IV;

    // Note! the CheckBook[X] methods have identical
    // versions in the frontend in frontend/projects/shared/utils/book.ts

    public static bool CheckBookAllowsModifications(
        bool schoolYearIsFinalized,
        bool classBookIsFinalized)
        => !schoolYearIsFinalized &&
           !classBookIsFinalized;

    public static bool CheckBookAllowsGradeModifications(bool schoolYearIsFinalized,
        bool classBookIsFinalized,
        bool hasFutureEntryLock,
        int? pastMonthLockDay,
        DateTime now,
        GradeType gradeType,
        DateTime gradeDate)
        => !schoolYearIsFinalized &&
            !classBookIsFinalized &&
            (!Grade.GradeTypeRequiresScheduleLesson(gradeType) ||
                (!CheckBookHasFutureEntryLock(hasFutureEntryLock, now, gradeDate) &&
                    !CheckBookHasPastMonthLock(pastMonthLockDay, now, gradeDate)));

    public static bool CheckBookAllowsAttendanceAbsenceTopicModifications(
        bool schoolYearIsFinalized,
        bool classBookIsFinalized,
        bool hasFutureEntryLock,
        int? pastMonthLockDay,
        DateTime now,
        DateTime entryDate)
        => !schoolYearIsFinalized &&
            !classBookIsFinalized &&
            !CheckBookHasFutureEntryLock(hasFutureEntryLock, now, entryDate) &&
            !CheckBookHasPastMonthLock(pastMonthLockDay, now, entryDate);

    public static bool CheckBookAllowsAdditionalActivityModifications(
        bool schoolYearIsFinalized,
        bool classBookIsFinalized,
        bool hasFutureEntryLock,
        int? pastMonthLockDay,
        DateTime now,
        int year,
        int weekNumber)
        => !schoolYearIsFinalized &&
            !classBookIsFinalized &&
            !CheckBookHasFutureEntryLock(hasFutureEntryLock, now, DateExtensions.GetDateFromIsoWeek(year, weekNumber, 1)) &&
            !CheckBookHasPastMonthLock(pastMonthLockDay, now, DateExtensions.GetDateFromIsoWeek(year, weekNumber, 7));

    private static bool CheckBookHasFutureEntryLock(
        bool hasFutureEntryLock,
        DateTime now,
        DateTime entryDate)
        => hasFutureEntryLock && entryDate >= now.Date.AddDays(1);

    private static bool CheckBookHasPastMonthLock(
        int? pastMonthLockDay,
        DateTime now,
        DateTime entryDate)
        => pastMonthLockDay != null &&
            entryDate < GetFirstEditableMonthStartDate(pastMonthLockDay.Value, now);

    private static DateTime GetFirstEditableMonthStartDate(
        int pastMonthLockDay,
        DateTime now)
        => now.Date.AddDays(1 - now.Day).AddMonths(now.Day >= pastMonthLockDay ? 0 : -1);

    // EF constructor
    private ClassBook()
    {
        this.BookName = null!;
        this.FullBookName = null!;
    }

    public ClassBook(
        int schoolYear,
        int instId,
        InstType instType,
        ClassGroup classGroup,
        BasicClass? basicClass,
        ClassType? classType,
        (ClassGroup, ClassType?)[] childClassGroups,
        string bookName,
        int createdBySysUserId)
    {
        if (classGroup.IsNotPresentForm == true)
        {
            throw new DomainValidationException("Cannot create class book for ClassGroup with IsNotPresentForm=True");
        }

        // Lvl2 and IsCombined=False ClassGroup
        if (classGroup.ParentClassId != null && (classGroup.IsCombined ?? false) == false)
        {
            throw new DomainValidationException($"Cannot create class book for ClassGroup with ClassId={classGroup.ClassId} because it's Lvl2 and IsCombined=False.");
        }

        var (classBookType, classBookTypeError) =
            GetClassBookTypeForClassGroup(instType, classGroup, classType, childClassGroups);

        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ClassId = classGroup.ClassId;
        this.ClassIsLvl2 = classGroup.ParentClassId != null;
        this.BookType = classBookType ??
            throw new DomainValidationException($"Cannot create class book because book type cannot be determined. Reason: {classBookTypeError}");
        this.BasicClassId = classGroup.BasicClassId;
        this.BasicClassName = basicClass?.Name;
        this.BookName = bookName;
        this.FullBookName = null!;
        this.IsFinalized = false;
        this.IsValid = true;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int InstId { get; private set; }

    public int ClassId { get; private set; }

    public bool ClassIsLvl2 { get; private set; }

    public ClassBookType BookType { get; private set; }

    public int? BasicClassId { get; private set; }

    public string? BasicClassName { get; private set; }

    public string BookName { get; private set; }

    public string? SchoolYearProgram { get; private set; }

    public bool IsFinalized { get; private set; }

    public bool IsValid { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    public bool BookTypeAllowsDecimalGrades => CheckBookTypeAllowsDecimalGrades(this.BookType);

    public bool BookTypeAllowsQualitativeGrades => CheckBookTypeAllowsQualitativeGrades(this.BookType);

    public bool BookTypeAllowsSpecialGrades => CheckBookTypeAllowsSpecialGrades(this.BookType);

    public bool BookTypeAllowsAbsences => CheckBookTypeAllowsAbsences(this.BookType);

    public bool BookTypeAllowsAttendances => CheckBookTypeAllowsAttendances(this.BookType);

    // computed columns
    public string FullBookName { get; private set; }

    // relations
    private readonly List<ClassBookCurriculumGradeless> gradelessCurriculums = new();
    public IReadOnlyCollection<ClassBookCurriculumGradeless> GradelessCurriculums => this.gradelessCurriculums.AsReadOnly();

    private readonly List<ClassBookStudentGradeless> gradelessStudents = new();
    public IReadOnlyCollection<ClassBookStudentGradeless> GradelessStudents => this.gradelessStudents.AsReadOnly();

    private readonly List<ClassBookStudentSpecialNeeds> specialNeedsStudents = new();
    public IReadOnlyCollection<ClassBookStudentSpecialNeeds> SpecialNeedsStudents => this.specialNeedsStudents.AsReadOnly();

    private readonly List<ClassBookStudentFirstGradeResultSpecialNeeds> firstGradeResultSpecialNeedsStudents = new();
    public IReadOnlyCollection<ClassBookStudentFirstGradeResultSpecialNeeds> FirstGradeResultSpecialNeedsStudents => this.firstGradeResultSpecialNeedsStudents.AsReadOnly();

    private readonly List<ClassBookStudentActivity> studentActivities = new();
    public IReadOnlyCollection<ClassBookStudentActivity> StudentActivities => this.studentActivities.AsReadOnly();

    private readonly List<ClassBookStudentCarriedAbsence> studentCarriedAbsences = new();
    public IReadOnlyCollection<ClassBookStudentCarriedAbsence> StudentCarriedAbsences => this.studentCarriedAbsences.AsReadOnly();

    private readonly List<ClassBookPrint> prints = new();
    public IReadOnlyCollection<ClassBookPrint> Prints => this.prints.AsReadOnly();

    private readonly List<ClassBookStudentPrint> studentPrints = new();
    public IReadOnlyCollection<ClassBookStudentPrint> StudentPrints => this.studentPrints.AsReadOnly();

    private readonly List<ClassBookStatusChange> statusChanges = new();
    public IReadOnlyCollection<ClassBookStatusChange> StatusChanges => this.statusChanges.AsReadOnly();

    public void MarkAllStudentsAsSpecialNeeds(
        int[] curriculumIds,
        int[] studentPersonIds)
    {
        this.specialNeedsStudents.Clear();
        foreach (var curriculumId in curriculumIds)
        {
            foreach (int personId in studentPersonIds)
            {
                this.specialNeedsStudents.Add(new ClassBookStudentSpecialNeeds(
                    this,
                    personId,
                    curriculumId));
            }
        }
    }

    public void UpdateData(
        string bookName,
        int modifiedBySysUserId)
    {
        this.BookName = bookName;
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void UpdateCurriculum(int curriculumId, bool withoutGrade, int modifiedBySysUserId)
    {
        var gradelessCurriculum = this.GradelessCurriculums.SingleOrDefault(gc => gc.CurriculumId == curriculumId);

        if (withoutGrade == true && gradelessCurriculum == null)
        {
            this.gradelessCurriculums.Add(new ClassBookCurriculumGradeless(this, curriculumId));
        }
        else if(withoutGrade == false && gradelessCurriculum != null)
        {
            this.gradelessCurriculums.Remove(
                this.gradelessCurriculums.Single(s => s.CurriculumId == curriculumId));
        }

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void UpdateStudent(
        int personId,
        (int curriculumId,
            bool withoutFirstTermGrade,
            bool withoutSecondTermGrade,
            bool withoutFinalGrade)[] gradelessCurriculums,
        int[] specialNeedsCurriculumIds,
        bool hasSpecialNeedFirstGradeResult,
        string? activities,
        (int FirstTermExcusedCount,
            int FirstTermUnexcusedCount,
            int FirstTermLateCount,
            int SecondTermExcusedCount,
            int SecondTermUnexcusedCount,
            int SecondTermLateCount) carriedAbsences,
        int modifiedBySysUserId)
    {
        if (CheckBookTypeAllowsStudentActivities(this.BookType))
        {
            this.UpdateStudentActivities(personId, activities, modifiedBySysUserId);
        }

        if (CheckBookTypeAllowsGrades(this.BookType))
        {
            this.UpdateStudentGradeless(personId, gradelessCurriculums, modifiedBySysUserId);
        }

        if (CheckBookTypeAllowsSpecialGrades(this.BookType))
        {
            this.UpdateStudentSpecialNeeds(personId, specialNeedsCurriculumIds, hasSpecialNeedFirstGradeResult, modifiedBySysUserId);
        }

        if (CheckBookTypeAllowsAbsences(this.BookType))
        {
            this.UpdateStudentCarriedAbsences(personId, carriedAbsences, modifiedBySysUserId);
        }
    }

    public void UpdateStudentActivities(
        int personId,
        string? activities,
        int modifiedBySysUserId)
    {
        if (!CheckBookTypeAllowsStudentActivities(this.BookType))
        {
            throw new DomainValidationException($"Cannot add student activities for the book type of classBookId:{this.ClassBookId}");
        }

        var activity = this.studentActivities.SingleOrDefault(a => a.PersonId == personId);

        if (activity != null)
        {
            if (string.IsNullOrEmpty(activities))
            {
                this.studentActivities.Remove(activity);
            }
            else
            {
                activity.Update(activities);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(activities))
            {
                this.studentActivities.Add(new ClassBookStudentActivity(this, personId, activities));
            }
        }

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void UpdateStudentGradeless(
        int personId,
        (int curriculumId,
            bool withoutFirstTermGrade,
            bool withoutSecondTermGrade,
            bool withoutFinalGrade)[] gradelessCurriculums,
        int modifiedBySysUserId)
    {
        if (!CheckBookTypeAllowsGrades(this.BookType))
        {
            throw new DomainValidationException($"Cannot add student gradeless curriculums for the book type of classBookId:{this.ClassBookId}");
        }

        this.gradelessStudents.RemoveAll(c => c.PersonId == personId);
        foreach (var (curriculumId, withoutFirstTermGrade, withoutSecondTermGrade, withoutFinalGrade) in gradelessCurriculums)
        {
            this.gradelessStudents.Add(
                new ClassBookStudentGradeless(
                    this,
                    personId,
                    curriculumId,
                    withoutFirstTermGrade,
                    withoutSecondTermGrade,
                    withoutFinalGrade));
        }

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void UpdateStudentSpecialNeeds(
        int personId,
        int[] specialNeedsCurriculumIds,
        bool hasSpecialNeedFirstGradeResult,
        int modifiedBySysUserId)
    {
        if (!CheckBookTypeAllowsSpecialGrades(this.BookType))
        {
            throw new DomainValidationException($"Cannot add student special needs curriculums for the book type of classBookId:{this.ClassBookId}");
        }

        if (hasSpecialNeedFirstGradeResult && (this.BookType != ClassBookType.Book_I_III && this.BookType != ClassBookType.Book_CSOP))
        {
            throw new DomainValidationException($"Cannot add student special needs first grade result for the book type of classBookId:{this.ClassBookId}");
        }

        var specialNeedFirstGradeResult = this.firstGradeResultSpecialNeedsStudents.SingleOrDefault(c => c.PersonId == personId);

        if (hasSpecialNeedFirstGradeResult)
        {
            if (specialNeedFirstGradeResult == null)
            {
                this.firstGradeResultSpecialNeedsStudents.Add(new ClassBookStudentFirstGradeResultSpecialNeeds(this, personId));
            }
        }
        else if (specialNeedFirstGradeResult != null)
        {
            this.firstGradeResultSpecialNeedsStudents.Remove(specialNeedFirstGradeResult);
        }

        this.specialNeedsStudents.RemoveAll(c => c.PersonId == personId);
        foreach (var specialNeedsCurriculumId in specialNeedsCurriculumIds)
        {
            this.specialNeedsStudents.Add(new ClassBookStudentSpecialNeeds(this, personId, specialNeedsCurriculumId));
        }

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void UpdateStudentCarriedAbsences(
        int personId,
        (int firstTermExcusedCount,
            int firstTermUnexcusedCount,
            int firstTermLateCount,
            int secondTermExcusedCount,
            int secondTermUnexcusedCount,
            int secondTermLateCount) carriedAbsences,
        int modifiedBySysUserId)
    {
        if (!CheckBookTypeAllowsAbsences(this.BookType))
        {
            throw new DomainValidationException($"Cannot add student absences for the book type of classBookId:{this.ClassBookId}");
        }

        var carriedAbsence = this.studentCarriedAbsences.SingleOrDefault(a => a.PersonId == personId);

        if (carriedAbsence != null)
        {
            if (carriedAbsences == default)
            {
                this.studentCarriedAbsences.Remove(carriedAbsence);
            }
            else
            {
                carriedAbsence.Update(
                    carriedAbsences.firstTermExcusedCount,
                    carriedAbsences.firstTermUnexcusedCount,
                    carriedAbsences.firstTermLateCount,
                    carriedAbsences.secondTermExcusedCount,
                    carriedAbsences.secondTermUnexcusedCount,
                    carriedAbsences.secondTermLateCount);
            }
        }
        else
        {
            if (carriedAbsences != default)
            {
                this.studentCarriedAbsences.Add(
                    new ClassBookStudentCarriedAbsence(
                        this,
                        personId,
                        carriedAbsences.firstTermExcusedCount,
                        carriedAbsences.firstTermUnexcusedCount,
                        carriedAbsences.firstTermLateCount,
                        carriedAbsences.secondTermExcusedCount,
                        carriedAbsences.secondTermUnexcusedCount,
                        carriedAbsences.secondTermLateCount));
            }
        }

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void UpdateSchoolYearProgram(
        string schoolYearProgram,
        int modifiedBySysUserId)
    {
        this.SchoolYearProgram = schoolYearProgram;
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public ClassBookPrint CreateClassBookPrint(int createdBySysUserId)
    {
        return this.CreateClassBookPrint(false, createdBySysUserId);
    }

    private ClassBookPrint CreateClassBookPrint(bool isFinal, int createdBySysUserId)
    {
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = createdBySysUserId;

        ClassBookPrint print = new(this, isFinal, createdBySysUserId);
        this.prints.Add(print);

        return print;
    }

    public void ClassBookPrintMarkProcessed(int classBookPrintId, int blobId, string? contentHash)
    {
        this.ModifyDate = DateTime.Now;

        ClassBookPrint print = this.prints.Single(p => p.ClassBookPrintId == classBookPrintId);
        print.MarkProcessed(blobId, contentHash);
    }

    public void ClassBookPrintMarkErrored(int classBookPrintId)
    {
        this.ModifyDate = DateTime.Now;

        ClassBookPrint print = this.prints.Single(p => p.ClassBookPrintId == classBookPrintId);
        print.MarkErrored();
    }

    public ClassBookStudentPrint CreateClassBookStudentPrint(int personId, int createdBySysUserId)
    {
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = createdBySysUserId;

        ClassBookStudentPrint print = new(this, personId, createdBySysUserId);
        this.studentPrints.Add(print);

        return print;
    }

    public void ClassBookStudentPrintMarkProcessed(int classBookStudentPrintId, int blobId)
    {
        this.ModifyDate = DateTime.Now;

        ClassBookStudentPrint print = this.studentPrints.Single(p => p.ClassBookStudentPrintId == classBookStudentPrintId);
        print.MarkProcessed(blobId);
    }

    public void ClassBookStudentPrintMarkErrored(int classBookStudentPrintId)
    {
        this.ModifyDate = DateTime.Now;

        ClassBookStudentPrint print = this.studentPrints.Single(p => p.ClassBookStudentPrintId == classBookStudentPrintId);
        print.MarkErrored();
    }

    public void Unfinalize(int modifiedBySysUserId)
    {
        this.statusChanges
            .OrderByDescending(s => s.ChangeDate)
            .FirstOrDefault()
            ?.RemoveIsLast();
        this.statusChanges.Add(
            new ClassBookStatusChange(
                this,
                ClassBookStatusChangeType.Unfinalized,
                modifiedBySysUserId,
                null));

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        this.IsFinalized = false;
        this.Prints.Single(p => p.IsFinal).RemoveIsFinal();
    }

    private string GetSignees(
        (string issuer,
        string subject,
        string thumbprint,
        DateTime validFrom,
        DateTime validTo)[] signatures)
    {
        return string.Join(
            ", ",
            signatures
                .Select(s => s.subject)
                .Select(s => CommonNameRegex().Match(s))
                .Where(m => m.Success)
                .Select(m => m.Groups[1].Value));
    }

    public ClassBookPrint Finalize(int modifiedBySysUserId)
    {
        if (this.IsFinalized)
        {
            throw new DomainValidationException("The class book is already finalized.");
        }

        this.statusChanges
            .OrderByDescending(s => s.ChangeDate)
            .FirstOrDefault()
            ?.RemoveIsLast();
        this.statusChanges.Add(
            new ClassBookStatusChange(
                this,
                ClassBookStatusChangeType.Finalized,
                modifiedBySysUserId,
                null));

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        this.IsFinalized = true;
        return this.CreateClassBookPrint(true, modifiedBySysUserId);
    }

    public void MarkAsInvalid(int modifiedBySysUserId)
    {
        this.IsValid = false;
        this.IsFinalized = true;
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public void SignClassBookPrint(
        int classBookPrintId,
        int blobId,
        (string issuer,
        string subject,
        string thumbprint,
        DateTime validFrom,
        DateTime validTo)[] signatures,
        int modifiedBySysUserId)
    {
        this.statusChanges
            .OrderByDescending(s => s.ChangeDate)
            .FirstOrDefault()
            ?.RemoveIsLast();
        this.statusChanges.Add(
            new ClassBookStatusChange(
                this,
                ClassBookStatusChangeType.Signed,
                modifiedBySysUserId,
                this.GetSignees(signatures)));

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        ClassBookPrint print = this.prints.Single(p => p.ClassBookPrintId == classBookPrintId);
        print.Sign(blobId, signatures);
    }

    public ClassBookPrint FinalizeWithSignedPdf(
        int blobId,
        (string issuer,
        string subject,
        string thumbprint,
        DateTime validFrom,
        DateTime validTo)[] signatures,
        int modifiedBySysUserId)
    {
        if (this.IsFinalized)
        {
            throw new DomainValidationException("The class book is already finalized.");
        }

        this.statusChanges
            .OrderByDescending(s => s.ChangeDate)
            .FirstOrDefault()
            ?.RemoveIsLast();
        this.statusChanges.Add(
            new ClassBookStatusChange(
                this,
                ClassBookStatusChangeType.FinalizedAndSigned,
                modifiedBySysUserId,
                this.GetSignees(signatures)));

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        this.IsFinalized = true;

        ClassBookPrint print = new(
            this,
            blobId,
            signatures,
            modifiedBySysUserId);
        this.prints.Add(print);

        return print;
    }

    public record ClassBookTypeResult(ClassBookType? ClassBookType, ClassBookTypeError? ClassBookTypeError);
    public static ClassBookTypeResult GetClassBookTypeForClassGroup(
        InstType instType,
        ClassGroup classGroup,
        ClassType? classType,
        (ClassGroup, ClassType?)[] childClassGroups)
    {
        var isLvl1 = classGroup.ParentClassId == null;
        var IsCombined = classGroup.IsCombined == true;

        // Lvl1 Regular or Lvl2 Combined
        if ((isLvl1 && !IsCombined) || (!isLvl1 && IsCombined))
        {
            if (classGroup.BasicClassId == null)
            {
                return new (null, ClassBookTypeError.Lvl1RegularOrLvl2CombinedMissingBasicClass);
            }

            return GetClassBookType(
                instType,
                classGroup.BasicClassId.Value,
                classType);
        }
        // Lvl1 Combined
        else if (isLvl1 && IsCombined)
        {
            if (childClassGroups.Length == 0)
            {
                return new (null, ClassBookTypeError.Lvl1CombinedHasNoChildClassGroups);
            }

            ClassBookType? commonChildClassBookType = null;
            foreach (var (childClassGroup, childClassType) in childClassGroups)
            {
                var (classBookType, _) =
                    childClassGroup.BasicClassId != null ?
                        GetClassBookType(
                            instType,
                            childClassGroup.BasicClassId.Value,
                            childClassType)
                        : new (null, null);

                if (commonChildClassBookType == null)
                {
                    commonChildClassBookType = classBookType;
                }
                else if (commonChildClassBookType != classBookType)
                {
                    commonChildClassBookType = null;
                    break;
                }
            }

            if (commonChildClassBookType == null)
            {
                return new (null, ClassBookTypeError.NoCommonClassBookType);
            }
            else
            {
                return new (commonChildClassBookType, null);
            }
        }
        // Lvl2 Regular
        else if (!isLvl1 && !IsCombined)
        {
            return new (null, ClassBookTypeError.IsLvl2Regular);
        }

        throw new Exception("Impossible");
    }

    private static ClassBookTypeResult GetClassBookType(
        InstType instType,
        int basicClassId,
        ClassType? classType)
    {
        ClassKind classKind = classType?.ClassKind ?? ClassKind.Class;
        int classTypeId = classType?.ClassTypeId ?? 0;

        return classKind switch
        {
            ClassKind.Class => instType switch
            {
                InstType.CSOP => (basicClassId, classTypeId) switch
                {
                    (basicClassId: < 0 or 21 or 32, _)                              => new (ClassBookType.Book_PG, null),
                    (basicClassId: 25, classTypeId: ClassType.CsopPgClassTypeId)    => new (ClassBookType.Book_PG, null),
                    _                                                               => new (ClassBookType.Book_CSOP, null),
                },
                _ => basicClassId switch
                {
                    < 0 or 21 or 32 => new (ClassBookType.Book_PG, null),
                    >= 1 and <= 3   => new (ClassBookType.Book_I_III, null),
                    4               => new (ClassBookType.Book_IV, null),
                    >= 5 and <= 12  => new (ClassBookType.Book_V_XII, null),
                    _               => new (null, ClassBookTypeError.InvalidBasicClass),
                }
            },
            ClassKind.Cdo => new (ClassBookType.Book_CDO, null),
            ClassKind.Other =>
                ClassType.DormitoryClassTypes.Contains(classTypeId) ?
                    new (ClassBookType.Book_CDO, null) :
                    new (ClassBookType.Book_DPLR, null),
            _ => new (null, ClassBookTypeError.UnknownClassKind),
        };
    }

    public static string? GetSuggestedClassBookNameForClassGroup(ClassGroup classGroup, BasicClass? basicClass)
    {
        if (!string.IsNullOrEmpty(classGroup.ParalellClassName))
        {
            return classGroup.ParalellClassName;
        }

        if (classGroup.ParentClassId != null && classGroup.IsCombined == true)
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(basicClass?.Name) && classGroup.ClassName.StartsWith(basicClass.Name))
        {
            return classGroup.ClassName.Substring(basicClass.Name.Length).Trim(' ', '.');
        }

        return classGroup.ClassName;
    }
}
