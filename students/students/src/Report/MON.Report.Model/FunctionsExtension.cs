namespace MON.Report.Model
{
    using MON.Report.Model.Diploma;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class FunctionsExtension
    {
        /// <summary>
        /// Средно съотношение на главни букви към малки
        /// </summary>
        private const double UpperCaseRatio = 1.4;
        private const string NotFoundDefaultString = "-";

        public static string TryGetValue2(Dictionary<string, string> source, string key)
        {
            if (source == null) return "";

            source.TryGetValue(key != null ? key : "", out string value);
            return value;
        }

        public static string TryGetValue(Dictionary<string, string> source, string key)
        {
            if (source == null) return "";

            source.TryGetValue(key != null ? key : "", out string value);
            return value;
        }

        public static string TryGetFullName(Dictionary<string, string> source)
        {
            if (source == null) return "";

            source.TryGetValue("firstName", out string firstName);
            source.TryGetValue("middleName", out string middleName);
            source.TryGetValue("lastName", out string lastName);
            return $"{firstName} {middleName} {lastName}";
        }

        public static string TryGetSubjectName(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int index)
        {
            return source.GetDiplomaGradeModel(diplomaPartCode, index)?.SubjectName ?? NotFoundDefaultString;
        }

        public static string TryGetSubjectName(Dictionary<string, Dictionary<string, List<DiplomaGradeModel>>> source, string basicSubjectType, int index)
        {
            return source.GetGradesGroupedBySubjectName(basicSubjectType, index).Key ?? NotFoundDefaultString;
        }

        public static string TryGetGrade(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int index)
        {
            DiplomaGradeModel grade = source.GetDiplomaGradeModel(diplomaPartCode, index);

            return grade != null && grade.Grade > 0
                ? grade.Grade.Value.ToString("N2")
                : NotFoundDefaultString;
        }

        public static string TryGetGrade(Dictionary<string, Dictionary<string, List<DiplomaGradeModel>>> source, string basicSubjectType, int index, string basicClass)
        {
            KeyValuePair<string, List<DiplomaGradeModel>> kvp = source.GetGradesGroupedBySubjectName(basicSubjectType, index);
            if (kvp.Value == null) return NotFoundDefaultString;
            DiplomaGradeModel grade = kvp.Value.FirstOrDefault(x => x.BasicClass != null && x.BasicClass == basicClass);

            return grade != null && grade.Grade > 0
                ? grade.Grade.Value.ToString("N2")
                : NotFoundDefaultString;
        }

        public static string TryGetQualitativeGrade(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int index)
        {
            DiplomaGradeModel grade = source.GetDiplomaGradeModel(diplomaPartCode, index);

            return grade != null && grade.Grade > 0
                ? $"{grade.GradeText} {grade.Grade.Value.ToString("N0")}"
                : NotFoundDefaultString;
        }

        public static string TryGetQualitativeGrade(Dictionary<string, Dictionary<string, List<DiplomaGradeModel>>> source, string basicSubjectType, int index, string basicClass)
        {
            KeyValuePair<string, List<DiplomaGradeModel>> kvp = source.GetGradesGroupedBySubjectName(basicSubjectType, index);
            if (kvp.Value == null) return NotFoundDefaultString;
            DiplomaGradeModel grade = kvp.Value.FirstOrDefault(x => x.BasicClass != null && x.BasicClass == basicClass);

            return grade != null && grade.Grade > 0
                ? $"{grade.GradeText} {grade.Grade.Value.ToString("N0")}"
                : NotFoundDefaultString;
        }

        public static string TryGetGradeText(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int index)
        {
            return source.GetDiplomaGradeModel(diplomaPartCode, index)?.GradeText ?? NotFoundDefaultString;
        }

        public static string TryGetGradeText(Dictionary<string, Dictionary<string, List<DiplomaGradeModel>>> source, string basicSubjectType, int index, string basicClass)
        {
            KeyValuePair<string, List<DiplomaGradeModel>> kvp = source.GetGradesGroupedBySubjectName(basicSubjectType, index);
            if (kvp.Value == null) return NotFoundDefaultString;
            DiplomaGradeModel grade = kvp.Value.FirstOrDefault(x => x.BasicClass != null && x.BasicClass == basicClass);

            return grade?.GradeText ?? NotFoundDefaultString;
        }

        public static string TryGetHorarium(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int index)
        {
            DiplomaGradeModel grade = source.GetDiplomaGradeModel(diplomaPartCode, index);

            return grade != null && grade.Horarium.HasValue
                ? grade.Horarium.Value.ToString("N0")
                : NotFoundDefaultString;
        }

        public static string TryGetHorarium(Dictionary<string, Dictionary<string, List<DiplomaGradeModel>>> source, string basicSubjectType, int index, string basicClass)
        {
            KeyValuePair<string, List<DiplomaGradeModel>> kvp = source.GetGradesGroupedBySubjectName(basicSubjectType, index);
            if (kvp.Value == null) return NotFoundDefaultString;
            DiplomaGradeModel grade = kvp.Value.FirstOrDefault(x => x.BasicClass != null && x.BasicClass == basicClass);

            return grade != null && grade.Horarium.HasValue
                ? grade.Horarium.Value.ToString("N0")
                : NotFoundDefaultString;
        }

        public static string TryGetSubjectNameByPosition(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int position)
        {
            return source.GetDiplomaGradeModel(diplomaPartCode, position, true)?.SubjectName ?? NotFoundDefaultString;
        }

        public static string TryGetGradeByPosition(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int position)
        {
            DiplomaGradeModel grade = source.GetDiplomaGradeModel(diplomaPartCode, position, true);

            return grade != null && grade.Grade > 0
                ? grade.Grade.Value.ToString("N2")
                : NotFoundDefaultString;
        }

        public static string TryGetQualitativeGradeByPosition(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int position)
        {
            DiplomaGradeModel grade = source.GetDiplomaGradeModel(diplomaPartCode, position, true);

            return grade != null && grade.Grade > 0
                ? $"{grade.GradeText} {grade.Grade.Value.ToString("N0")}"
                : NotFoundDefaultString;
        }

        public static string TryGetGradeTextByPosition(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int position)
        {
            return source.GetDiplomaGradeModel(diplomaPartCode, position, true)?.GradeText ?? NotFoundDefaultString;
        }

        public static string TryGetHorariumByPosition(Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int position)
        {
            DiplomaGradeModel grade = source.GetDiplomaGradeModel(diplomaPartCode, position, true);

            return grade != null && grade.Horarium.HasValue
                ? grade.Horarium.Value.ToString("N0")
                : NotFoundDefaultString;
        }

        public static string TryGetFormattedDate(Dictionary<string, string> source, string key, string format)
        {
            if (source == null) return "";

            if (source.TryGetValue(key ?? "", out string value)
                && DateTime.TryParse(value, out DateTime date))
            {
                return string.IsNullOrWhiteSpace(format)
                    ? date.ToString("dd.MM.yyyy")
                    : date.ToString(format);
            }

            return "";
        }

        public static string TryGetSchoolYearPart(string schoolYearStr, int index)
        {
            string[] arr = (schoolYearStr ?? "").Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (index < arr.Length)
            {
                return arr[index];
            }

            return "";
        }

        public static string TryGetPointsByKey(this Dictionary<string, List<DiplomaGradeModel>> source, string key, int index)
        {
            DiplomaGradeModel grade = source.GetDiplomaGradeModel(key, index);

            return grade != null && grade.Points.HasValue && grade.Points.Value > 0
                ? grade.Points.Value.ToString("N0")
                : NotFoundDefaultString;
        }

        private static DiplomaGradeModel GetDiplomaGradeModel(this Dictionary<string, List<DiplomaGradeModel>> source, string diplomaPartCode, int index, bool usePosition = false)
        {
            if (source == null) return null;

            if (source.TryGetValue(diplomaPartCode, out List<DiplomaGradeModel> grades))
            {
                return usePosition
                    ? grades.FirstOrDefault(x => x.Position == index)
                    : grades.ElementAtOrDefault(index);
            }

            return null;
        }

        /// <summary>
        /// Връща KeyValuePair<string, List<DiplomaGradeModel>>, търсейки ключ basicSubjectType и взимайки ElementAtOrDefault(index).
        /// source e речник от групирани по basicSubjectType и послед по subjectName оценки.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="basicSubjectType"></param>
        /// <param name="index"></param>
        /// <returns>default(KeyValuePair<string, List<DiplomaGradeModel>>) if the source is null or empty and basicSubjectType is missing.</returns>
        private static KeyValuePair<string, List<DiplomaGradeModel>> GetGradesGroupedBySubjectName(this Dictionary<string, Dictionary<string, List<DiplomaGradeModel>>> source, string basicSubjectType, int index)
        {
            if (source == null) return default(KeyValuePair<string, List<DiplomaGradeModel>>);

            if (source.TryGetValue(basicSubjectType, out Dictionary<string, List<DiplomaGradeModel>> gradesByBasicSubjectTypeDict))
            {
                KeyValuePair<string, List<DiplomaGradeModel>> kvp = gradesByBasicSubjectTypeDict.ElementAtOrDefault(index);
                return kvp;
            }

            return default(KeyValuePair<string, List<DiplomaGradeModel>>);
        }

        public static List<MultiYearDiplomaGradeModel> GetGradesSkipTake(List<MultiYearDiplomaGradeModel> grades, int skip, int take)
        {
            if (grades != null)
            {
                if (take == 0 && skip == 0)
                {
                    return grades;
                }
                if (take == 0)
                {
                    return grades.Skip(skip).ToList();
                }
                else if (skip == 0)
                {
                    return grades.Take(take).ToList();
                }
                else
                {
                    return grades.Skip(skip).Take(take).ToList();
                }
            }
            else
            {
                return grades;
            }
        }

        /// <summary>
        /// Взимане на оценки, разделени по дължина на предмета
        /// </summary>
        /// <param name="grades">Списък с оценки</param>
        /// <param name="skip">Пропусни</param>
        /// <param name="take">Вземи</param>
        /// <param name="autoFill">Попълване с празни елементи, ако са по-малко</param>
        /// <param name="exceptions">Изключения по SubjectId, разделени със запетая</param>
        /// <returns></returns>
        public static List<DiplomaGradeModel> GetGradesSkipTakeByLength(List<DiplomaGradeModel> grades, int skip, int take, int maxLength, bool autoFill, string exceptions)
        {
            List<int?> exceptionsList = new List<int?>();
            if (!String.IsNullOrWhiteSpace(exceptions))
            {
                var listIds = exceptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                exceptionsList = listIds.Select(i => (int?)Int32.Parse(i)).ToList();
            }

            var gradesByLength = GetGradesByLineLength(grades.Where(i => !exceptionsList.Contains(i.SubjectId)).ToList(), maxLength);
            var gradesFiltered = GetGradesSkipTake(gradesByLength, skip, take);
            if (autoFill)
            {
                gradesFiltered = AutoFillGrades(gradesFiltered, take);
            }

            return gradesFiltered;
        }

        /// <summary>
        /// Взимане на оценки, разделени по дължина на предмета с изключване на повтарящи се DocumentPartName за да няма повторения при повторно извикване на функцията 
        /// !!! ВАЖНО: Когато се използват различни списъци за grades -> (Fields.MandatoryGrades, Fields.NonMandatoryGrades) извикването на фунцкията и за двете трябва да започва винаги с (skip = 0, take = нужната бройка от редове(grades))
        /// !!! Параметър: 'bool clearDocumentPartName = true', задължително в Telerik Reporting Designer трябва да се въвежда винаги "true" !!!
        /// </summary>
        /// <param name="grades">Списък с оценки</param>
        /// <param name="skip">Пропусни</param>
        /// <param name="take">Вземи</param>
        /// <param name="autoFill">Попълване с празни елементи, ако са по-малко</param>
        /// <param name="exceptions">Изключения по SubjectId, разделени със запетая</param>
        /// <param name="maxLength">Максимална дължина на реда</param>
        /// <param name="clearDocumentPartName">Флаг за изчистване на documentPartName при повторение на функцията</param>
        /// <returns></returns>
        public static List<DiplomaGradeModel> GetGradesSkipTakeByLength(List<DiplomaGradeModel> grades, int skip, int take, int maxLength, bool autoFill, string exceptions, bool clearDocumentPartName = true)
        {
            List<int?> exceptionsList = new List<int?>();

            if (!String.IsNullOrWhiteSpace(exceptions))
            {
                string[] listIds = exceptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                exceptionsList = listIds.Select(i => (int?)Int32.Parse(i)).ToList();
            }

            List<DiplomaGradeModel> gradesByLength = GetGradesByLineLength(grades.Where(i => !exceptionsList.Contains(i.SubjectId)).ToList(), maxLength);
            List<DiplomaGradeModel> gradesFiltered = GetGradesSkipTake(gradesByLength, skip, take);

            //проверяваме дали clearDocumentPart е true\false и gradesFiltered да не е празен
            if (clearDocumentPartName && gradesFiltered.Count > 0)
            {
                //създаваме и използваме HashSet<string> за да събираме и сравняваме данни за вече съществуващи DocumentPartName
                HashSet<string> existingPartsName = new HashSet<string>();
                //ако skip е по-голям от 0, създаваме List<DiplomaGradeModel> в който добавяме DocumentPartName, които вече са минали (с повторно извикване на функцията рекурсивно)
                if (skip > 0)
                {
                    //на мястото на 'skip' параметъра винаги е 0 а на 'take' = 'skip' за да сравнява предишно извиканите елементи на метода от предходната функция
                    List<DiplomaGradeModel> existing = GetGradesSkipTakeByLength(grades, 0, skip, maxLength, autoFill, exceptions, false);
                    foreach (var docPartName in existing.Select(x => x.DocumentPartName))
                    {
                        existingPartsName.Add(docPartName);
                    }
                }

                //проверяваме дали в existingPartsName има данни (съответно дали има повтаряши се DocumentPartName, ако има, за всеки предмет от gradesFiltered зануляваме стойността на DocumentPartName
                if (existingPartsName.Count > 0)
                {
                    foreach (var grade in gradesFiltered)
                    {
                        if (existingPartsName.Contains(grade.DocumentPartName))
                        {
                            grade.DocumentPartName = "";
                        }
                    }
                }
            }

            if (autoFill)
            {
                gradesFiltered = AutoFillGrades(gradesFiltered, take);
            }

            return gradesFiltered;
        }

        /// <summary>
        /// OVERLOAD за MultiYearDiplomaGradeModel
        /// Взимане на оценки, разделени по дължина на предмета с изключване на повтарящи се DocumentPartName за да няма повторения при повторно извикване на функцията 
        /// !!! ВАЖНО: Когато се използват различни списъци за grades -> (Fields.MandatoryGrades, Fields.NonMandatoryGrades) извикването на фунцкията и за двете трябва да започва винаги с (skip = 0, take = нужната бройка от редове(grades))
        /// !!! Параметър: 'bool clearDocumentPartName = true', задължително в Telerik Reporting Designer трябва да се въвежда винаги "true" !!!
        /// </summary>
        /// <param name="grades">Списък с оценки</param>
        /// <param name="skip">Пропусни</param>
        /// <param name="take">Вземи</param>
        /// <param name="autoFill">Попълване с празни елементи, ако са по-малко</param>
        /// <param name="exceptions">Изключения по SubjectId, разделени със запетая</param>
        /// <param name="maxLength">Максимална дължина на реда</param>
        /// <param name="clearDocumentPartName">Флаг за изчистване на documentPartName при повторение на функцията</param>
        /// <returns></returns>
        public static List<MultiYearDiplomaGradeModel> GetGradesSkipTakeByLength(List<MultiYearDiplomaGradeModel> grades, int skip, int take, int maxLength, bool autoFill, string exceptions, bool clearDocumentPartName = true)
        {
            List<int?> exceptionsList = new List<int?>();

            if (!String.IsNullOrWhiteSpace(exceptions))
            {
                string[] listIds = exceptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                exceptionsList = listIds.Select(i => (int?)Int32.Parse(i)).ToList();
            }

            List<MultiYearDiplomaGradeModel> gradesByLength = GetGradesByLineLength(grades.Where(i => !exceptionsList.Contains(i.SubjectId)).ToList(), maxLength);
            List<MultiYearDiplomaGradeModel> gradesFiltered = GetGradesSkipTake(gradesByLength, skip, take);

            //проверяваме дали clearDocumentPart е true\false и gradesFiltered да не е празен
            if (clearDocumentPartName && gradesFiltered.Count > 0)
            {
                //създаваме и използваме HashSet<string> за да събираме и сравняваме данни за вече съществуващи DocumentPartName
                HashSet<string> existingPartsName = new HashSet<string>();
                //ако skip е по-голям от 0, създаваме List<DiplomaGradeModel> в който добавяме DocumentPartName, които вече са минали (с повторно извикване на функцията рекурсивно)
                if (skip > 0)
                {
                    //на мястото на 'skip' параметъра винаги е 0 а на 'take' = 'skip' за да сравнява предишно извиканите елементи на метода от предходната функция
                    List<MultiYearDiplomaGradeModel> existing = GetGradesSkipTakeByLength(grades, 0, skip, maxLength, autoFill, exceptions, false);
                    foreach (var docPartName in existing.Select(x => x.DocumentPartName))
                    {
                        existingPartsName.Add(docPartName);
                    }
                }

                //проверяваме дали в existingPartsName има данни (съответно дали има повтаряши се DocumentPartName, ако има, за всеки предмет от gradesFiltered зануляваме стойността на DocumentPartName
                if (existingPartsName.Count > 0)
                {
                    foreach (var grade in gradesFiltered)
                    {
                        if (existingPartsName.Contains(grade.DocumentPartName))
                        {
                            grade.DocumentPartName = "";
                        }
                    }
                }
            }

            if (autoFill)
            {
                gradesFiltered = AutoFillGrades(gradesFiltered, take);
            }

            return gradesFiltered;
        }

        public static List<Competency> GetCompetenciesSkipTake(List<Competency> competencies, int skip, int take)
        {
            if (competencies != null)
            {
                if (take == 0 && skip == 0)
                {
                    return competencies;
                }
                if (take == 0)
                {
                    return competencies.Skip(skip).ToList();
                }
                else if (skip == 0)
                {
                    return competencies.Take(take).ToList();
                }
                else
                {
                    return competencies.Skip(skip).Take(take).ToList();
                }
            }
            else
            {
                return competencies;
            }
        }

        /// <summary>
        /// Взимане на компетенции, разделени по дължина на вписаната компетенция
        /// </summary>
        /// <param name="competencies">Списък с компетенции</param>
        /// <param name="skip">Пропусни</param>
        /// <param name="take">Вземи</param>
        /// <param name="maxLength">Дължина на Name, по който ще се разцепят</param>
        /// <param name="autoFill">Попълване с празни елементи, ако са по-малко</param>
        /// <returns></returns>
        public static List<Competency> GetCompetenciesSkipTakeByLength(List<Competency> competencies, int skip, int take, int maxLength, bool autoFill)
        {
            List<Competency> competenciesByLength = GetCompetenciesByLineLength(competencies, maxLength);
            List<Competency> competenciesFiltered = GetCompetenciesSkipTake(competenciesByLength, skip, take);

            if (autoFill)
            {
                competenciesFiltered = AutoFillCompetencies(competenciesFiltered, take);
            }

            return competenciesFiltered;
        }

        /// <summary>
        /// Автоматично допълва колекцията с празни, така че да има поне minCount елемента
        /// </summary>
        /// <param name="competencies"></param>
        /// <param name="minCount"></param>
        /// <returns></returns>
        public static List<Competency> AutoFillCompetencies(List<Competency> competencies, int minCount)
        {
            List<Competency> autoFilledCompetencies = new List<Competency>();
            if (competencies != null)
            {
                for (int i = 0; i < competencies.Count; i++)
                {
                    autoFilledCompetencies.Add(competencies[i]);
                }

                for (int i = competencies.Count; i < Math.Max(competencies.Count, minCount); i++)
                {
                    autoFilledCompetencies.Add(new Competency());
                }
            }
            else
            {
                for (int i = 0; i < minCount; i++)
                {
                    autoFilledCompetencies.Add(new Competency());
                }
            }

            return new List<Competency>(autoFilledCompetencies);
        }

        /// <summary>
        /// Автоматично допълва колекцията с празни(null) стрингове, за да има minCount елемента
        /// </summary>
        /// <param name="commissionMembers"></param>
        /// <param name="minCount"></param>
        /// <returns></returns>
        public static List<CommissionMember> AutoFillCommissionMembers(List<CommissionMember> commissionMembers, int minCount)
        {
            List<CommissionMember> autoFilledCommissionMember = new List<CommissionMember>();
            if (commissionMembers != null)
            {
                for (int i = 0; i < commissionMembers.Count; i++)
                {
                    autoFilledCommissionMember.Add(commissionMembers[i]);
                }

                for (int i = commissionMembers.Count; i < Math.Max(commissionMembers.Count, minCount); i++)
                {
                    autoFilledCommissionMember.Add(new CommissionMember() { Name = null });
                }
            }
            else
            {
                for (int i = 0; i < minCount; i++)
                {
                    autoFilledCommissionMember.Add(new CommissionMember() { Name = null });
                }
            }

            return new List<CommissionMember>(autoFilledCommissionMember);
        }

        /// <summary>
        /// Разделя компетенциите по дължината на Name и прави толкова компетенции, колкото са необходими
        /// </summary>
        /// <param name="competencies"></param>
        /// <param name="maxLineLength"></param>
        /// <returns></returns>
        public static List<Competency> GetCompetenciesByLineLength(List<Competency> competencies, int maxLineLength)
        {
            List<Competency> resultCompetencies = new List<Competency>();
            foreach (Competency competency in competencies)
            {
                string competencyName = competency.Name;
                int charCount = 0;

                List<string> lines = competencyName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .GroupBy(w => (charCount += (int)GetLengthByUpperLowerCase(w, UpperCaseRatio) + 1) / maxLineLength)
                    .Select(g => string.Join(" ", g))
                    .ToList();

                int index = 0;
                int linesCount = lines.Count();
                foreach (string line in lines)
                {
                    index++;
                    Competency cmp = new Competency()
                    {
                        Name = line,
                    };

                    if (index == 1)
                    {
                        cmp.Code = competency.Code;
                    }

                    resultCompetencies.Add(cmp);
                }
            }

            return resultCompetencies;
        }

        /// <summary>
        /// Взимане на оценки, разделени по дължина на предмета
        /// </summary>
        /// <param name="grades">Списък с оценки</param>
        /// <param name="skip">Пропусни</param>
        /// <param name="take">Вземи</param>
        /// <param name="maxLength">Дължина на SubjectName, по който ще се разцепят</param>
        /// <param name="autoFill">Попълване с празни елементи, ако са по-малко</param>
        /// <param name="exceptions">Изключения по SubjectId, разделени със запетая</param>
        /// <returns></returns>
        public static List<MultiYearDiplomaGradeModel> GetGradesSkipTakeByLength(List<MultiYearDiplomaGradeModel> grades, int skip, int take, int maxLength, bool autoFill, string exceptions)
        {
            var gradesByLength = GetGradesByLineLength(grades, maxLength);
            var gradesFiltered = GetGradesSkipTake(gradesByLength, skip, take);
            if (autoFill)
            {
                gradesFiltered = AutoFillGrades(gradesFiltered, take);
            }
            return gradesFiltered;
        }

        public static List<DiplomaGradeModel> GetGradesSkipTake(List<DiplomaGradeModel> grades, int skip, int take)
        {
            if (grades != null)
            {
                if (take == 0 && skip == 0)
                {
                    return grades;
                }
                if (take == 0)
                {
                    return grades.Skip(skip).ToList();
                }
                else if (skip == 0)
                {
                    return grades.Take(take).ToList();
                }
                else
                {
                    return grades.Skip(skip).Take(take).ToList();
                }
            }
            else
            {
                return grades;
            }
        }

        public static string Capitalize(string text)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                return text.Trim().ToUpper();
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// Автоматично допълва колекцията с празни, така че да има поне minCount елемента
        /// </summary>
        /// <param name="grades"></param>
        /// <param name="minCount"></param>
        /// <returns></returns>
        public static List<DiplomaGradeModel> AutoFillGrades(List<DiplomaGradeModel> grades, int minCount)
        {
            var autoFilledGrades = new List<DiplomaGradeModel>();
            if (grades != null)
            {
                for (var i = 0; i < grades.Count; i++)
                {
                    autoFilledGrades.Add(grades[i]);
                }

                for (var i = grades.Count; i < Math.Max(grades.Count, minCount); i++)
                {
                    autoFilledGrades.Add(new DiplomaGradeModel());
                }
            }
            else
            {
                for (var i = 0; i < minCount; i++)
                {
                    autoFilledGrades.Add(new DiplomaGradeModel());
                }
            }

            return new List<DiplomaGradeModel>(autoFilledGrades);
        }

        public static List<MultiYearDiplomaGradeModel> AutoFillGrades(List<MultiYearDiplomaGradeModel> grades, int minCount)
        {
            var autoFilledGrades = new List<MultiYearDiplomaGradeModel>();
            if (grades != null)
            {
                for (var i = 0; i < grades.Count; i++)
                {
                    autoFilledGrades.Add(grades[i]);
                }

                for (var i = grades.Count; i < Math.Max(grades.Count, minCount); i++)
                {
                    autoFilledGrades.Add(new MultiYearDiplomaGradeModel());
                }
            }
            else
            {
                for (var i = 0; i < minCount; i++)
                {
                    autoFilledGrades.Add(new MultiYearDiplomaGradeModel());
                }
            }

            return new List<MultiYearDiplomaGradeModel>(autoFilledGrades);
        }

        public static List<DiplomaGradeModel> GetGradesByExternalEvaluation(List<DiplomaGradeModel> grades, int externalEvaluationTypeId, string exceptions)
        {
            List<int> exceptionsList = new List<int>();

            if (!String.IsNullOrWhiteSpace(exceptions))
            {
                var listIds = exceptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                exceptionsList = listIds.Select(i => Int32.Parse(i)).ToList();
            }
            var filteredGrades = grades?.Where(i => i.ExternalEvaluationTypeId == externalEvaluationTypeId && (exceptionsList == null || (i.SubjectId.HasValue && !exceptionsList.Contains(i.SubjectId.Value)))).ToList();
            return filteredGrades;
        }

        public static List<DiplomaGradeModel> Union(List<DiplomaGradeModel> grades1, List<DiplomaGradeModel> grades2)
        {
            return grades1.Union(grades2).ToList();
        }

        public static List<MultiYearDiplomaGradeModel> Union(List<MultiYearDiplomaGradeModel> grades1, List<MultiYearDiplomaGradeModel> grades2)
        {
            return grades1.Union(grades2).ToList();
        }

        public static List<DiplomaGradeModel> GetGradesByExternalEvaluationSubjectType(List<DiplomaGradeModel> grades, int externalEvaluationTypeId, string exceptions)
        {
            List<(int, int)> exceptionsList = new List<(int, int)>();

            if (!String.IsNullOrWhiteSpace(exceptions))
            {
                var listIds = exceptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                exceptionsList = listIds.Select(i =>
                {
                    var items = i.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    return (Int32.Parse(items[0]), Int32.Parse(items[1]));
                }
                ).ToList();
            }
            var filteredGrades = grades?.Where(i => i.ExternalEvaluationTypeId == externalEvaluationTypeId && (exceptionsList == null || (i.SubjectId.HasValue && !exceptionsList.Contains((i.SubjectId.Value, i.SubjectTypeId.Value))))).ToList();
            return filteredGrades;
        }


        /// <summary>
        /// Връща всички оценки филтрирани по DocumentPartId
        /// </summary>
        /// <param name="grades"></param>
        /// <param name="documentPartId"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public static List<DiplomaGradeModel> GetGradesByDocumentPartId(List<DiplomaGradeModel> grades, int documentPartId, string exceptions)
        {
            List<int> exceptionsList = new List<int>();

            if (!String.IsNullOrWhiteSpace(exceptions))
            {
                string[] listIds = exceptions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                exceptionsList = listIds.Select(i => Int32.Parse(i)).ToList();
            }

            List<DiplomaGradeModel> filteredGrades = grades?.Where(i => i.DocumentPartId == documentPartId).ToList(); //temporary fix for 3-22
            //            List<DiplomaGradeModel> filteredGrades = grades?.Where(i => i.DocumentPartId == documentPartId && (exceptionsList == null || (i.SubjectId.HasValue && !exceptionsList.Contains(i.SubjectId.Value)))).ToList();

            return filteredGrades;
        }

        public static DiplomaGradeModel GetGradeByExternalEvaluation(List<DiplomaGradeModel> grades, int subjectId, int externalEvaluationTypeId)
        {
            var grade = grades?.FirstOrDefault(i => i.SubjectId == subjectId && i.ExternalEvaluationTypeId == externalEvaluationTypeId);
            return grade;
        }

        public static DiplomaGradeModel GetGradeByExternalEvaluationAndSubjectType(List<DiplomaGradeModel> grades, int subjectId, int subjectTypeId, int externalEvaluationTypeId)
        {
            var grade = grades?.FirstOrDefault(i => i.SubjectId == subjectId && i.SubjectTypeId == subjectTypeId && i.ExternalEvaluationTypeId == externalEvaluationTypeId);
            return grade;
        }

        public static DiplomaGradeModel GetGradeByDocumentPart(List<DiplomaGradeModel> grades, int subjectId, int basicDocumentPartId)
        {
            var grade = grades.FirstOrDefault(i => i.SubjectId == subjectId && i.DocumentPartId == basicDocumentPartId);
            return grade;
        }


        /// <summary>
        /// Изчислява коригирана дължина на текстов низ, спрямо коефицент на главните букви
        /// </summary>
        /// <param name="text"></param>
        /// <param name="coeff">Коефицент за главни букви</param>
        private static double GetLengthByUpperLowerCase(string text, double coeff)
        {
            double length = 0;
            foreach (char c in text)
            {
                length += Char.IsUpper(c) ? coeff : 1;
            }
            return length;
        }

        private static bool LastDocumentPartNameIsDuplicate(List<DiplomaGradeModel> grades, List<DiplomaGradeModel> skipTakeGrades)
        {
            bool hasDuplicates = grades.GroupBy(x => x.DocumentPartName).Any(g => g.Count() > 1);

            return hasDuplicates;
        }

        /// <summary>
        /// Разделя предметите по дължината на SubjectName и прави толкова предмета, колкото са необходими
        /// </summary>
        /// <param name="grades"></param>
        /// <param name="maxLineLength"></param>
        /// <returns></returns>
        public static List<DiplomaGradeModel> GetGradesByLineLength(List<DiplomaGradeModel> grades, int maxLineLength)
        {
            List<DiplomaGradeModel> resultGrades = new List<DiplomaGradeModel>();

            foreach (var grade in grades)
            {
                string subjectName = grade.SubjectName;
                var charCount = 0;

                var lines = subjectName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .GroupBy(w => (charCount += (int)GetLengthByUpperLowerCase(w, UpperCaseRatio) + 1) / maxLineLength)
                    .Select(g => string.Join(" ", g))
                    .ToList();

                int index = 0;
                int linesCount = lines.Count();
                foreach (var line in lines)
                {

                    index++;

                    DiplomaGradeModel gm = new DiplomaGradeModel()
                    {
                        SubjectName = line,
                        ExternalEvaluationTypeId = grade.ExternalEvaluationTypeId,
                        GradeCategory = grade.GradeCategory,
                        DocumentPartId = grade.DocumentPartId,
                        Position = grade.Position,
                        DocumentPartName = grade.DocumentPartName,
                        Points = grade.Points,
                    };

                    if (index == linesCount)
                    {
                        // Последен елемент
                        gm.Grade = grade.Grade;
                        gm.GradeText = grade.GradeText;
                        gm.Horarium = grade.Horarium;
                        gm.FLLevel = grade.FLLevel;
                    }
                    else
                    {
                    }

                    resultGrades.Add(gm);
                }
            }

            return resultGrades;
        }

        /// <summary>
        /// Разделя предметите по дължината на SubjectName и прави толкова предмета, колкото са необходими
        /// </summary>
        /// <param name="grades"></param>
        /// <param name="maxLineLength"></param>
        /// <returns></returns>
        public static List<MultiYearDiplomaGradeModel> GetGradesByLineLength(List<MultiYearDiplomaGradeModel> grades, int maxLineLength)
        {
            List<MultiYearDiplomaGradeModel> resultGrades = new List<MultiYearDiplomaGradeModel>();
            foreach (var grade in grades)
            {
                string subjectName = grade.SubjectName;
                var charCount = 0;

                var lines = subjectName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .GroupBy(w => (charCount += (int)GetLengthByUpperLowerCase(w, UpperCaseRatio) + 1) / maxLineLength)
                    .Select(g => string.Join(" ", g))
                    .ToList();

                int index = 0;
                int linesCount = lines.Count();
                foreach (var line in lines)
                {
                    index++;
                    MultiYearDiplomaGradeModel gm = new MultiYearDiplomaGradeModel()
                    {
                        SubjectName = line,
                        DocumentPartId = grade.DocumentPartId,
                        Position = grade.Position,
                        DocumentPartName = grade.DocumentPartName,
                    };

                    if (index == linesCount)
                    {
                        // Последен елемент
                        gm.Grades = grade.Grades;
                    }
                    else
                    {
                    }

                    resultGrades.Add(gm);
                }
            }

            return resultGrades;
        }

        public static string CharacterSpacing(string text, int spacing)
        {
            if (!string.IsNullOrEmpty(text))
            {
                char[] characters = text.ToCharArray();

                string spacingString = "";
                spacingString = spacingString.PadRight(spacing);

                return string.Join(spacingString, characters);
            }

            return NotFoundDefaultString;
        }

        public static string SpecialNeedsLegend(List<MultiYearDiplomaGradeModel> grades)
        {
            var allGrades = grades.Where(i => i.Grades != null).SelectMany(i => i.Grades);
            bool hasAchievesTheRequirementsGrades = allGrades.Any(i => i.GradeText == "ПИ");
            bool hasCopesGrades = allGrades.Any(i => i.GradeText == "СС");
            bool hasEncountersDifficultiesGrades = allGrades.Any(i => i.GradeText == "СЗ");

            List<string> legengs = new List<string>();
            if (hasEncountersDifficultiesGrades) legengs.Add("СЗ: Среща затруднения");
            if (hasCopesGrades) legengs.Add("СС: Справя се");
            if (hasAchievesTheRequirementsGrades) legengs.Add("ПИ: Постига изискванията");

            return string.Join(",", legengs);
        }

        public static string FormatStringByLength(string text, int characterLengthLimit, int index)
        {
            return FormatStringByLength(text, characterLengthLimit, index, NotFoundDefaultString);
        }

        //Overload на FormatStringByLength - с цел избягване на проблеми с Telerik Report Designer-a
        public static string FormatStringByLength(string text, int characterLengthLimit, int index, string emptyChar = "")
        {
            if (!string.IsNullOrEmpty(text))
            {
                StringBuilder stringBuild = new StringBuilder();
                text = text.Replace(",", ", ")
                    .Replace("-", "- ")
                    .Replace(":", ": ")
                    .Replace(".", ". ");
                string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int charCount = 0;
                List<string> lines = words
                    .GroupBy(w => (charCount += (int)GetLengthByUpperLowerCase(w, UpperCaseRatio) + 1) / characterLengthLimit)
                    .Select(g => string.Join(" ", g))
                    .ToList();

                if (lines.Count > index)
                {
                    return lines[index];
                }
                else return emptyChar;
            }
            else
            {
                return emptyChar;
            }
        }

        public static int GetStringLength(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                int textLength = text.Length;

                return textLength;
            }

            return 0;
        }

        public static string GetEctsGrade(decimal grade)
        {
            string ectsGrade = grade switch
            {
                decimal gr when gr == 6.00m => "A",
                decimal gr when gr >= 5.50m && gr <= 5.99m => "B",
                decimal gr when gr >= 4.50m && gr <= 5.49m => "C",
                decimal gr when gr >= 3.50m && gr <= 4.49m => "D",
                decimal gr when gr >= 3.00m && gr <= 3.49m => "E",
                _ => string.Empty,
            };

            return ectsGrade;
        }

        public static string GetRomeNameByBasicClassId(int basicClassId)
        {
            string romanResult = "";

            Dictionary<string, int> romanNumbersDictionary = new Dictionary<string, int> {
            {
                "I",
                1
            },
            {
                "II",
                2
            },
            {
                "III",
                3
            },
            {
                "IV",
                4
            },
            {
                "V",
                5
            },
            {
                "VI",
                6
            },
            {
                "VII",
                7
            },
            {
                "VIII",
                8
            },
            {
                "IX",
                9
            },
            {
                "X",
                10
            },
            {
                "XI",
                11
            },
            {
                "XII",
                12
            },
            {
                "XIII",
                13
            },
           };

            foreach (var item in romanNumbersDictionary.Reverse())
            {
                if (basicClassId <= 0) break;
                while (basicClassId >= item.Value)
                {
                    romanResult += item.Key;
                    basicClassId -= item.Value;
                }
            }

            return romanResult;
        }
    }
}
