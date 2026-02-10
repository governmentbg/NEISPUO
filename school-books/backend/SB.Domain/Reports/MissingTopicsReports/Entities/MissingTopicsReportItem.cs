namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class MissingTopicsReportItem
{
    // EF constructor
    private MissingTopicsReportItem()
    {
        this.ClassBookName = null!;
        this.CurriculumName = null!;
    }

    public MissingTopicsReportItem(
        DateTime date,
        string classBookName,
        string curriculumName,
        string[] teacherNames)
    {
        this.Date = date;
        this.ClassBookName = classBookName;
        this.CurriculumName = curriculumName;
        this.teachers.AddRange(teacherNames.Select(s => new MissingTopicsReportItemTeacher(s)));
    }

    public int MissingTopicsReportId { get; private set; }

    public int SchoolYear { get; private set; }

    public DateTime Date { get; private set; }

    public string ClassBookName { get; private set; }

    public string CurriculumName { get; private set; }

    public int MissingTopicsReportItemId { get; private set; }

    // relations
    private readonly List<MissingTopicsReportItemTeacher> teachers = new();
    public IReadOnlyCollection<MissingTopicsReportItemTeacher> Teachers => this.teachers.AsReadOnly();
}
