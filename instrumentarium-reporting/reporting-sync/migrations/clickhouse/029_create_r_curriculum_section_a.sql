-- Migration: Create R_Curriculum_Section_A table
-- Based on reporting.R_Curriculum_Section_A view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Curriculum_Section_A (
    `Код по НЕИСПУО` Int32,
    `Наименование` String,
    `Населено място` Nullable(String),
    `Община` Nullable(String),
    `Област` Nullable(String),
    `RegionID` Nullable(Int32),
    `Район` Nullable(String),
    `Вид по чл. 35-36 (според собствеността)` Nullable(String),
    `Вид по чл. 38 (детайлен)` Nullable(String),
    `Финансиращ орган` Nullable(String),
    `Вид по чл. 37 (общ, според вида на подготовката)` Nullable(String),
    `Email` Nullable(String),
    `Учебна година` Nullable(Int16),
    `Етап на обучение` Nullable(String),
    `Наименование на випуск/клас` Nullable(String),
    `Учебен предмет` Nullable(String),
    `Начин на изучаване` Nullable(String),
    `Чужд език` Nullable(String),
    `I срок УС` Nullable(Int16),
    `I срок ЧС` Nullable(Float32),
    `II срок УС` Nullable(Int16),
    `II срок ЧС` Nullable(Float32),
    `Общ бр.ч.` Nullable(Int32),
    `Общ брой ученици` Nullable(Int32),
    `Преподавател` Nullable(String)
) ENGINE = MergeTree()
ORDER BY (`Код по НЕИСПУО`, `Наименование`);
