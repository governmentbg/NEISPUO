namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class Note : IAggregateRoot
{
    // EF constructor
    private Note()
    {
        this.Description = null!;
        this.Version = null!;
    }

    public Note(
        int schoolYear,
        int classBookId,
        int[] studentIds,
        string description,
        bool isForAllStudents,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.Description = description;

        if (!isForAllStudents)
        {
            this.students.AddRange(studentIds.Select(id => new NoteStudent(this, id)));
        }

        this.IsForAllStudents = isForAllStudents;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }

    public int SchoolYear { get; private set; }
    public int NoteId { get; private set; }
    public int ClassBookId { get; private set; }
    public string Description { get; private set; }
    public bool IsForAllStudents { get; private set; }
    public DateTime CreateDate { get; private set; }
    public int CreatedBySysUserId { get; private set; }
    public DateTime ModifyDate { get; private set; }
    public int ModifiedBySysUserId { get; private set; }
    public byte[] Version { get; private set; }

    // relations
    private readonly List<NoteStudent> students = new();
    public IReadOnlyCollection<NoteStudent> Students => this.students.AsReadOnly();

    public void UpdateData(int[] studentIds, string description, bool isForAllStudents, int modifiedBySysUserId)
    {
        this.students.Clear();

        if (!isForAllStudents)
        {
            this.students.AddRange(studentIds.Select(id => new NoteStudent(this, id)));
        }

        this.Description = description;

        this.IsForAllStudents = isForAllStudents;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }
}
