namespace SB.Domain;
using System;

public static class ParentMeetingsModelSample
{
    public static readonly ParentMeetingsModel Sample =
        new(
            new ParentMeetingsModelParentMeeting[]
            {
                new ParentMeetingsModelParentMeeting(new DateTime(2021, 9, 20), "Първа родителска среща", "Запознаване с преподавателите и учебния процес"),
                new ParentMeetingsModelParentMeeting(new DateTime(2021, 11, 21), "Родителска среща ноември", null),
                new ParentMeetingsModelParentMeeting(new DateTime(2022, 1, 15), "Родителска среща първи срок", "Обсъждане на поведението и успеха на учениците за първи срок"),
                new ParentMeetingsModelParentMeeting(new DateTime(2022, 5, 20), "Родителска среща втори срок", "Обсъждане на поведението и успеха на учениците за втори срок")
            }
        );
}
