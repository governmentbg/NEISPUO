namespace SB.Domain;
using System;

public static class StudentSanctionsModelSample
{
    public static readonly StudentSanctionsModel Sample =
        new(
            new StudentSanctionsModelSanction[]
            {
                new StudentSanctionsModelSanction(
                    "чл.199 ал.1 т.1 Забележка",
                    "431a",
                    new DateTime(2022, 7, 5),
                    "431a",
                    new DateTime(2022, 7, 15)
                ),
                new StudentSanctionsModelSanction(
                    "чл.199 ал.1 т.1 Забележка",
                    "431a",
                    new DateTime(2022, 5, 7),
                    "431a",
                    new DateTime(2022, 7, 15)
                )
            }
        );
}
