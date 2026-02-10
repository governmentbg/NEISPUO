namespace SB.Domain;
using System;

public static class StudentRemarksModelSample
{
    public static readonly StudentRemarksModel TermOneSample =
        new(
            SchoolTerm.TermOne,
            new StudentRemarksModelRemark[]
            {
                new StudentRemarksModelRemark(new DateTime(2023, 3, 15), "Български език и литература", "ООП", "Забележка \"обща забележка\" - Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."),
                new StudentRemarksModelRemark(new DateTime(2023, 3, 16), "Околен свят", "ООП", "Похвала - взема участие учебния процес"),
                new StudentRemarksModelRemark(new DateTime(2023, 3, 17), "Математика", "ООП", "Забележка \"обща забележка\" - не внимава в час"),
                new StudentRemarksModelRemark(new DateTime(2023, 3, 18), "Музика", "ООП", "Запознаване с преподавателите и учебния процес")
            }
        );

    public static readonly StudentRemarksModel TermTwoSample =
       new(
           SchoolTerm.TermOne,
           new StudentRemarksModelRemark[]
           {
                new StudentRemarksModelRemark(new DateTime(2023, 3, 15), "Български език и литература", "ООП", "Забележка \"обща забележка\" - Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")
           }
       );
}
