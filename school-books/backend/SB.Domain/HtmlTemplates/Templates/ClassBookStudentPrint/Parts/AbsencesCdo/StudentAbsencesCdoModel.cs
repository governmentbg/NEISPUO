namespace SB.Domain;

public record StudentAbsencesCdoModel(
    StudentAbsencesItemByTypes SepAbsences,
    StudentAbsencesItemByTypes OctAbsences,
    StudentAbsencesItemByTypes NovAbsences,
    StudentAbsencesItemByTypes DecAbsences,
    StudentAbsencesItemByTypes JanAbsences,
    StudentAbsencesItemByTypes FebAbsences,
    StudentAbsencesItemByTypes MarAbsences,
    StudentAbsencesItemByTypes AprAbsences,
    StudentAbsencesItemByTypes MayAbsences,
    StudentAbsencesItemByTypes JunAbsences,
    StudentAbsencesItemByTypes JulAbsences,
    StudentAbsencesItemByTypes AugAbsences
);

public record StudentAbsencesItemByTypes(
    decimal ExcusedAbsences,
    decimal UnexcusedAbsences
);
