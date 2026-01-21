namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

public class EmailTemplateConfig
{
    public static readonly EmailTemplateConfig NewGrade = new(
        "NewGrade",
        "NewGrade.cshtml",
        "noreply.neispuo@edu.mon.bg",
        (model) => $"Нова оценка по {(string)model["curriculumName"]!} {" на " + (string)model["studentName"]!} в НЕИСПУО",
        true,
        JObject.FromObject(
            new
            {
                gradeText = "Мн.Добър(5)",
                curriculumName = "Математика",
                studentName = "Иван Иванов"
            }
        ));

    public static readonly EmailTemplateConfig NewAbsence = new (
        "NewAbsence",
        "NewAbsence.cshtml",
        "noreply.neispuo@edu.mon.bg",
        (model) => $"Новo {(string)model["absenceTypeText"]!} {" на " + (string)model["studentName"]!} в НЕИСПУО",
        true,
        JObject.FromObject(
            new
            {
                absenceTypeText = "Отсъствие по неуважителни причини",
                curriculumName = "Математика",
                studentName = "Иван Иванов"
            }
        ));

    public static readonly EmailTemplateConfig NewRemark = new (
        "NewRemark",
        "NewRemark.cshtml",
        "noreply.neispuo@edu.mon.bg",
        (model) => $"Нова {(string)model["remarkTypeText"]!} {" на " + (string)model["studentName"]!} в НЕИСПУО",
        true,
        JObject.FromObject(
            new
            {
                remarkTypeText = "забележка \"Лоша дисциплина\"",
                description = "- не слуша\n- използва си телефона",
                curriculumName = "Математика",
                studentName = "Иван Иванов"
            }
        ));

    public static readonly EmailTemplateConfig NewAttendanceAbsence = new (
        "NewAttendanceAbsence",
        "NewAttendanceAbsence.cshtml",
        "noreply.neispuo@edu.mon.bg",
        (model) => $"Новo {(string)model["attendanceTypeText"]!} {" на " + (string)model["studentName"]!} в НЕИСПУО",
        true,
        JObject.FromObject(
            new
            {
                attendanceTypeText = "Отсъствие по неуважителни причини",
                studentName = "Иван Иванов"
            }
        ));

    public static readonly EmailTemplateConfig NewMessage = new(
        "NewMessage",
        "NewMessage.cshtml",
        "noreply.neispuo@edu.mon.bg",
        (model) => $"Ново съобщение в системата на НЕИСПУО",
        true,
        new JObject());

    public static readonly IReadOnlyDictionary<string, EmailTemplateConfig> AllTemplates =
        new ReadOnlyDictionary<string, EmailTemplateConfig>(
            new Dictionary<string, EmailTemplateConfig>(StringComparer.InvariantCultureIgnoreCase)
            {
                { NewGrade.TemplateName, NewGrade },
                { NewAbsence.TemplateName, NewAbsence },
                { NewRemark.TemplateName, NewRemark },
                { NewAttendanceAbsence.TemplateName, NewAttendanceAbsence },
                { NewMessage.TemplateName, NewMessage },
            }
        );

    private EmailTemplateConfig(string templateName, string templateFileName, string sender, Func<JObject, string> mailSubject, bool isBodyHtml, JObject sampleModel)
    {
        this.TemplateName = templateName;
        this.TemplateFileName = templateFileName;
        this.Sender = sender;
        this.MailSubject = mailSubject;
        this.IsBodyHtml = isBodyHtml;
        this.SampleModel = sampleModel;
    }

    public string TemplateName { get; private set; }

    public string TemplateFileName { get; private set; }

    public string Sender { get; private set; }

    public Func<JObject, string> MailSubject { get; private set; }

    public bool IsBodyHtml { get; private set; }

    public JObject SampleModel { get; private set; }

    public static EmailTemplateConfig Get(string name)
    {
        return AllTemplates[name];
    }
}
