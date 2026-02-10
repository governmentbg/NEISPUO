-- A copy of this script is located in Teams at "Поддръжка > Files > Скриптове при refresh на neipuo-test"

UPDATE
    [school_books].[ClassBookExtProvider]
SET
    [ExtSystemId] = NULL
WHERE
    [SchoolYear] IN (2022, 2023, 2024, 2025)
    AND [InstId] IN (
        300125,
        2206409,
        200277,
        100006,
        607055,
        1690180,
        2000218,
        300110
    );
