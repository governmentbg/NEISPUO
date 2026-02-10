using MON.Shared.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MON.Models
{
    public class ApiValidationResult
    {
        public bool IsValid { get; set; }

        public bool HasErrors => Errors != null && Errors.Count > 0;
        public bool HasWarnings => Warnings != null && Warnings.Count > 0;

        public List<string> Messages { get; set; } = new List<string>();

        public ValidationErrorCollection Errors { get; set; } = new ValidationErrorCollection();
        public ValidationErrorCollection Warnings { get; set; } = new ValidationErrorCollection();

        public string Data { get; set; }

        public void Merge(ApiValidationResult newResult)
        {
            if (newResult == null) return;

            IsValid = IsValid && newResult.IsValid;
            if (newResult.Messages != null && newResult.Messages.Count > 0)
            {
                Messages.AddRange(newResult.Messages);
            }

            if (newResult.Errors != null && newResult.Errors.Count > 0)
            {
                for (int i = 0; i < newResult.Errors.Count; i++)
                {
                    Errors.Add(newResult.Errors[i]);
                }
            }

            if (newResult.Warnings != null && newResult.Warnings.Count > 0)
            {
                for (int i = 0; i < newResult.Warnings.Count; i++)
                {
                    Warnings.Add(newResult.Warnings[i]);
                }
            }
        }

        public override string ToString()
        {
            return Messages != null && Messages.Count > 0
                ? string.Join(Environment.NewLine, Messages)
                : "";
        }

        public ValidationErrorCollection GetErrrorFromMessages()
        {
            if (Messages.Count == 0) return Errors;

            foreach (var msg in Messages)
            {
                Errors.Add(msg);
            }

            return Errors;
        }
        public List<DiplomaPerformaceStats> DiplomaPerformaceStats { get; set; } = new List<DiplomaPerformaceStats>();
        public long Duration => DiplomaPerformaceStats.Sum(i => i.Duration);
    }

    public class PerformanceStats
    {
        public string Name { get; set; }
        public long Duration { get; set; }
    }

    public class DiplomaPerformaceStats
    {
        public string Type { get; set; }
        public string PersonalId { get; set; }
        public List<PerformanceStats> PerformanceStats { get; set; } = new List<PerformanceStats>();
        public long Duration  => PerformanceStats.Sum(i => i.Duration);
    }
}
