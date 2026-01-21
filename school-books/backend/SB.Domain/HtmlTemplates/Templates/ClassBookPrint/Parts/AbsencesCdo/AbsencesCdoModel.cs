namespace SB.Domain;

public record AbsencesCdoModel(
    AbsencesCdoModelAbsence[] Absences
);

public record AbsencesCdoModelAbsence(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    AbsencesItemByTypes SepAbsences,
    AbsencesItemByTypes OctAbsences,
    AbsencesItemByTypes NovAbsences,
    AbsencesItemByTypes DecAbsences,
    AbsencesItemByTypes JanAbsences,
    AbsencesItemByTypes FebAbsences,
    AbsencesItemByTypes MarAbsences,
    AbsencesItemByTypes AprAbsences,
    AbsencesItemByTypes MayAbsences,
    AbsencesItemByTypes JunAbsences,
    AbsencesItemByTypes JulAbsences,
    AbsencesItemByTypes AugAbsences
);

public record AbsencesItemByTypes(
    decimal ExcusedAbsences,
    decimal UnexcusedAbsences
);
