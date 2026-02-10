namespace SB.Domain;

public static class StudentPgResultsModelSample
{
    public static readonly StudentPgResultsModel Sample =
        new(
            new StudentPgResultsModelPgResult[]
            {
                new StudentPgResultsModelPgResult(
                    "Математика - добро ниво; Български език - добро ниво; Природни науки - много добро ниво",
                    "Математика - много добро ниво; Български език - много добро ниво; Природни науки - много добро ниво"),
                new StudentPgResultsModelPgResult(
                    "Математика - добро ниво; Български език - добро ниво; Природни науки - много добро ниво",
                    null),
                new StudentPgResultsModelPgResult(
                    "Математика - много добро ниво; Български език - много добро ниво; Природни науки - много добро ниво",
                    "Математика - много добро ниво; Български език - добро ниво; Природни науки - много добро ниво"),
                new StudentPgResultsModelPgResult(
                    "Математика - добро ниво; Български език - много добро ниво; Природни науки - много добро ниво",
                    "Математика - добро ниво; Български език -много добро ниво; Природни науки - много добро ниво")
            }
        );
}
