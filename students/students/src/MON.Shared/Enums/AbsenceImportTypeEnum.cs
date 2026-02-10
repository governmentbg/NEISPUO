using System.ComponentModel;

namespace MON.Shared.Enums
{
    public enum AbsenceImportTypeEnum
    {
        [Description("Импорт на отсъствия от дневник")]
        SchoolBook = 1,
        [Description("Импорт на отсъствия от файл")]
        File = 2,
        [Description("Ръчно въвеждане")]
        Manual = 3
    }
}
