namespace SB.Domain;

using System;

public static class ScheduleAndAbsencesModelSample
{
    public static readonly ScheduleAndAbsencesModel Sample =
        new(
            false,
            new ScheduleAndAbsencesModelWeek[]
            {
                new ScheduleAndAbsencesModelWeek(
                    "14.09 - 20.09",
                    null,
                    new [] { "Допълнителна дейност 1", "Допълнителна дейност 2" },
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 14),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1", "Тема 2" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 15),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 16),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 17),
                            "Четвъртък",
                            false,
                            Array.Empty<ScheduleAndAbsencesModelWeekDayHour>()),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 18),
                            "Петък",
                            false,
                            Array.Empty<ScheduleAndAbsencesModelWeekDayHour>()),
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "21.09 - 27.09",
                    null,
                    Array.Empty<string>(),
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 21),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 22),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 23),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 24),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 26),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "28.09 - 04.10",
                    null,
                    new [] { "Допълнителна дейност 1" },
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 28),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null),
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 29),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 9, 30),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 1),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 2),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "05.10 - 11.10",
                    null,
                    Array.Empty<string>(),
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 5),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 6),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 7),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 8),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 9),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "12.10 - 18.10",
                    null,
                    Array.Empty<string>(),
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 12),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 13),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 14),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 15),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 16),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "19.10 - 25.10",
                    null,
                    Array.Empty<string>(),
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 19),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 20),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 21),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 22),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 23),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "26.10 - 01.11",
                    null,
                    new [] { "Допълнителна дейност 1", "Допълнителна дейност 2" },
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 26),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 27),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 28),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 29),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 10, 30),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "02.11 - 08.11",
                    null,
                    Array.Empty<string>(),
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 2),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 3),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 4),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 5),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 6),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "09.11 - 15.11",
                    null,
                    Array.Empty<string>(),
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 9),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 10),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 11),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 12),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 13),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    }),
                new ScheduleAndAbsencesModelWeek(
                    "16.11 - 22.11",
                    null,
                    new [] { "Допълнителна дейност 1", "Допълнителна дейност 2", "Допълнителна дейност 3" },
                    new ScheduleAndAbsencesModelWeekDay[]
                    {
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 16),
                            "Понеделник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Изобразително изкуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително изкуство", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Български език и литература", "ООП", "Р. Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(7, null, null, "Час на класа", "...", "Р. Костадинова", "", "", "", "", "", Array.Empty<string>(), null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 17),
                            "Вторник",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Татковина", "ДП/ДрУП", "Румяна Костадинова", "", "", "4, 11", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Grammar and Vocabulary Revision" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Музика", "ООП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 18),
                            "Сряда",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Физическо възпитание и спорт", "ДП/ДрУП", "Румяна Костадинова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Български език и литература", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Математика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Контролна работа за определяне на входно равнище" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Технологии и предприемачество", "РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Околен свят", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Родина", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 19),
                            "Четвъртък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "ООП", "Румяна Костадинова", "", "", "", "", "", new[] { "Понятие за езикова норма" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Музика", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Спортни дейности (лека атлетика)", "...", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Физическо възпитание и спорт", "ООП", "Иванка Димова", "", "", "", "", "", Array.Empty<string>(), null),
                                new ScheduleAndAbsencesModelWeekDayHour(6, null, null, "Трудово", "ДП/ДрУП", "Велка Герова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            }),
                        new ScheduleAndAbsencesModelWeekDay(
                            new DateTime(2022, 11, 20),
                            "Петък",
                            false,
                            new ScheduleAndAbsencesModelWeekDayHour[]
                            {
                                new ScheduleAndAbsencesModelWeekDayHour(1, null, null, "Български език и литература", "РП/УП-А", "Румяна Костадинова", "", "", "", "", "", new[] { "Публично изказване по граждански проблем" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(2, null, null, "Математика", "ООП", "Иван Радев", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(3, null, null, "Чужд език - Английски език", "ООП", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(4, null, null, "Чужд език - Английски език", " РП/УП-А", "Ваня Стефанова", "", "", "", "", "", new[] { "Тема 1" }, null),
                                new ScheduleAndAbsencesModelWeekDayHour(5, null, null, "Изобразително искуство", "ООП", "Иванка Димова", "", "", "", "", "", new[] { "Тема 1" }, null)
                            })
                    })
            },
            Array.Empty<ScheduleAndAbsencesModelWeek>()
        );
}
