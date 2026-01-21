namespace SB.Domain;

using System;

public class ExamsReportItem
{
    // EF constructor
    public ExamsReportItem()
    {
        this.ClassBookName = null!;
        this.CurriculumName = null!;
        this.CreatedBySysUserName = null!;
    }

    public ExamsReportItem(
        DateTime date,
        string classBookName,
        BookExamType bookExamType,
        string curriculumName,
        string createdBySysUserName)
    {
        this.Date = date;
        this.ClassBookName = classBookName;
        this.BookExamType = bookExamType;
        this.CurriculumName = curriculumName;
        this.CreatedBySysUserName = createdBySysUserName;
    }

    public int SchoolYear { get; private set; }

    public int ExamsReportId { get; private set; }

    public int ExamsReportItemId { get; private set; }

    public DateTime Date { get; private set; }

    public string ClassBookName { get; private set; }

    public BookExamType BookExamType { get; private set; }

    public string CurriculumName { get; private set; }

    public string CreatedBySysUserName { get; private set; }
}
