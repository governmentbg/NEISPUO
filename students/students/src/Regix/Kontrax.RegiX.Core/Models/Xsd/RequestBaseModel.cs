using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kontrax.RegiX.Core.TestStandard.Models
{
    public abstract class RequestBaseModel
    {
        [Display(Name = "Справка")]
        public AllowedDependencyViewModel Dependency { get; set; }

        [Display(Name = "Заявена от")]
        public string UserDisplayName { get; set; }

        [DisplayFormat(DataFormatString = Shared.Format.DateTime)]
        public DateTime? StartDateTime { get; set; }

        [DisplayFormat(DataFormatString = Shared.Format.DateTime)]
        public DateTime? EndDateTime { get; set; }

        public bool IsCanceled { get; set; }

        public string Error { get; set; }

        public bool IsPending
        {
            get { return !StartDateTime.HasValue && !IsCanceled; }
        }

        public bool IsDone
        {
            get { return EndDateTime.HasValue; }
        }

        public string CssClass
        {
            get { return IsCanceled ? "default" : !StartDateTime.HasValue ? "info" : !string.IsNullOrEmpty(Error) ? "danger" : IsDone ? "success" : "warning"; }
        }

        [Display(Name = "Състояние")]
        public string StatusText
        {
            get
            {
                StringBuilder text = new StringBuilder();

                if (!string.IsNullOrEmpty(Error))
                {
                    text.Append("Грешка: ");
                    text.AppendLine(Error);
                }

                bool isCanceled = IsCanceled;
                if (isCanceled)
                {
                    text.AppendLine("Отказано.");
                }

                DateTime? start = StartDateTime;
                DateTime? end = EndDateTime;
                if (start.HasValue)
                {
                    if (IsDone)
                    {
                        TimeSpan duration = end.Value - start.Value;
                        if (duration.TotalSeconds > 2.0)  // Само за по-бавните отговори се изписва продължителността.
                        {
                            string durationText = duration.TotalMinutes >= 2.0
                                ? $"{(int)Math.Round(duration.TotalMinutes)} минути и {duration.Seconds} секунди"
                                : $"{(int)Math.Round(duration.TotalSeconds)} секунди";
                            text.AppendFormat("Завършено за {0}.", durationText);
                        }
                        else if (string.IsNullOrEmpty(Error))
                        {
                            text.Append("Завършено.");
                        }
                    }
                    else
                    {
                        text.Append("Заявено, няма отговор.");
                    }
                }
                else if (!isCanceled)
                {
                    text.Append("Незаявено.");
                }

                return text.ToString();
            }
        }
    }
}
