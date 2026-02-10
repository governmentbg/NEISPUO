DECLARE @schoolYears TABLE (SchoolYear INT);
INSERT INTO @schoolYears
SELECT SchoolYear FROM (VALUES (2022)) AS v(SchoolYear);

DECLARE @instIds TABLE (InstId INT);
INSERT INTO @instIds
SELECT InstId FROM (VALUES (300125)) AS v(InstId);

DELETE a FROM school_books.Absence a
JOIN school_books.ClassBook c ON a.SchoolYear = c.SchoolYear AND a.ClassBookId = c.ClassBookId
WHERE c.SchoolYear IN (SELECT SchoolYear FROM @schoolYears) AND
    c.InstId IN (SELECT InstId FROM @instIds)

DELETE g FROM school_books.Grade g
JOIN school_books.ClassBook c ON g.SchoolYear = c.SchoolYear AND g.ClassBookId = c.ClassBookId
WHERE c.SchoolYear IN (SELECT SchoolYear FROM @schoolYears) AND
    c.InstId IN (SELECT InstId FROM @instIds)

DELETE t FROM school_books.Topic t
JOIN school_books.ClassBook c ON t.SchoolYear = c.SchoolYear AND  t.ClassBookId = c.ClassBookId
WHERE c.SchoolYear IN (SELECT SchoolYear FROM @schoolYears) AND
    c.InstId IN (SELECT InstId FROM @instIds)

DELETE schL FROM school_books.ScheduleLesson schL
JOIN school_books.Schedule sch ON schL.SchoolYear = sch.SchoolYear AND schL.ScheduleId = sch.ScheduleId
JOIN school_books.ClassBook c ON sch.SchoolYear = c.SchoolYear AND sch.ClassBookId = c.ClassBookId
WHERE c.SchoolYear IN (SELECT SchoolYear FROM @schoolYears) AND
    c.InstId IN (SELECT InstId FROM @instIds)

DELETE schD FROM school_books.ScheduleDate schD
JOIN school_books.Schedule sch ON schD.SchoolYear = sch.SchoolYear AND schD.ScheduleId = sch.ScheduleId
JOIN school_books.ClassBook c ON sch.SchoolYear = c.SchoolYear AND sch.ClassBookId = c.ClassBookId
WHERE c.SchoolYear IN (SELECT SchoolYear FROM @schoolYears) AND
    c.InstId IN (SELECT InstId FROM @instIds)

DELETE schH FROM school_books.ScheduleHour schH
JOIN school_books.Schedule sch ON schH.SchoolYear = sch.SchoolYear AND schH.ScheduleId = sch.ScheduleId
JOIN school_books.ClassBook c ON sch.SchoolYear = c.SchoolYear AND sch.ClassBookId = c.ClassBookId
WHERE c.SchoolYear IN (SELECT SchoolYear FROM @schoolYears) AND
    c.InstId IN (SELECT InstId FROM @instIds)

DELETE sch FROM school_books.Schedule sch
JOIN school_books.ClassBook c ON sch.SchoolYear = c.SchoolYear AND  sch.ClassBookId = c.ClassBookId
WHERE c.SchoolYear IN (SELECT SchoolYear FROM @schoolYears) AND
    c.InstId IN (SELECT InstId FROM @instIds)

DELETE c FROM school_books.ClassBook c
WHERE c.SchoolYear IN (SELECT SchoolYear FROM @schoolYears) AND
    c.InstId IN (SELECT InstId FROM @instIds)
GO
