namespace MON.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public class CspReportRequest
    {
        /// <summary>
        /// An envelope for the the csp report.
        /// </summary>
        [JsonProperty(PropertyName = "csp-report")]
        [ReadOnly(false)]
        public CspReport CspReport { get; set; }
    }

    public sealed class CspReport
    {
        /// <summary>
        /// The CSP document URI.
        /// </summary>
        /// <remarks>See more at <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP">CSP</a>.</remarks>
        [JsonProperty(PropertyName = "document-uri")]
        [ReadOnly(false)]
        public string DocumentUri { get; set; }

        /// <summary>
        /// The CSP referrer.
        /// </summary>
        /// <remarks>See more at <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/referrer">Referrer</a>.</remarks>
        [JsonProperty(PropertyName = "referrer")]
        [ReadOnly(false)]
        public string Referrer { get; set; }

        /// <summary>
        /// The CSP violated directive.
        /// </summary>
        /// <remarks>See more at <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP">CSP</a>.</remarks>
        [JsonProperty(PropertyName = "violated-directive")]
        [ReadOnly(false)]
        public string ViolatedDirective { get; set; }

        /// <summary>
        /// The CSP effective directive.
        /// </summary>
        /// <remarks>See more at <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP">CSP</a>.</remarks>
        [JsonProperty(PropertyName = "effective-directive")]
        [ReadOnly(false)]
        public string EffectiveDirective { get; set; }

        /// <summary>
        /// The CSP original policy.
        /// </summary>
        /// <remarks>See more at <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP">CSP</a>.</remarks>
        [JsonProperty(PropertyName = "original-policy")]
        [ReadOnly(false)]
        public string OriginalPolicy { get; set; }

        /// <summary>
        /// The CSP blocked URI.
        /// </summary>
        /// <remarks>See more at <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP">CSP</a>.</remarks>
        [JsonProperty(PropertyName = "blocked-uri")]
        [ReadOnly(false)]
        public string BlockedUri { get; set; }

        /// <summary>
        /// The CSP status code.
        /// </summary>
        /// <remarks>See more at <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP">CSP</a>.</remarks>
        [JsonProperty(PropertyName = "status-code")]
        [ReadOnly(false)]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "source-file")]
        [ReadOnly(false)]
        public string SourceFile { get; set; }

        [JsonProperty(PropertyName = "disposition")]
        [ReadOnly(false)]
        public string Disposition { get; set; }

        [JsonProperty(PropertyName = "line-number")]
        [ReadOnly(false)]
        public int? LineNumber { get; set; }

        [JsonProperty(PropertyName = "column-number")]
        [ReadOnly(false)]
        public int? ColumnNumber { get; set; }
    }
}
