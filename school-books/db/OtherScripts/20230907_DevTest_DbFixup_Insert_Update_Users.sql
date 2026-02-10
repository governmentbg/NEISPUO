-- A copy of this script is located in Teams at "Поддръжка > Files > Скриптове при refresh на neipuo-test"

INSERT INTO [core].[SysUserSysRole]([SysUserID], [SysRoleID], [InstitutionID])
VALUES
-- neispuo_ciela@edu.mon.bg роля директор
(881627, 0, 300125),  -- СУ "Вела Благоева"
(881627, 0, 2206409), -- ПГТЕ "Хенри Форд"
(881627, 0, 200277),  -- Детска градина "Ханс Кристиан Андерсен"
(881627, 0, 100006),  -- Детска градина "Звездичка"
(881627, 0, 607055),  -- Център за специална образователна подкрепа "Д-р Петър Берон"
(881627, 0, 1690180), -- Регионален център за подкрепа на процеса на приобщаващо образование - Пловдив
(881627, 0, 2000218), -- ЦПЛР Котел
(881627, 0, 300110),   -- ОУ "Св.Иван Рилски", с. Балван
--(881627, 6, NULL), -- роля ученик
(881627, 7, NULL)  -- роля родител

-- подмяна на паролите за няколко учителя с паролата, която е пинната в Teams, канал #DnevnikDev
UPDATE u
SET
    [Password] = '$2a$12$Wd6uFSqUM/YbwxE4ZPEhJuoPvx9F9dImtNp7c.LlNP3/KVzFkHnU.',
    [IsAzureUser] = 0
FROM [core].[SysUser] u
JOIN (VALUES
    (84368694), -- katya.bakalova@edu.mon.bg
    (84414911), -- milena.trendafilova@edu.mon.bg
    (84613549), -- yanina.grudeva@edu.mon.bg
    (85366174), -- liana.liu.todorova@edu.mon.bg
    (85753100), -- lidiia.zaharieva-trichkova@edu.mon.bg
    (85774665) -- tsvetelina.st.ilieva@edu.mon.bg
) AS p([PersonID]) ON u.[PersonID] = p.[PersonID]

UPDATE
    [core].[SysUser]
SET
    [PersonId] = 83664365
WHERE
    [PersonId] = 86227714
    AND [SysUserID] = 881627
    AND [Username] = 'neispuo_ciela@edu.mon.bg';

INSERT INTO
    [core].[ParentChildSchoolBookAccess] ([ChildID], [ParentID], [HasAccess])
VALUES
    (83664365, 83664365, 1),
    (83735309, 83664365, 1),
    (84232841, 83664365, 1),
    (83568047, 83664365, 1);
