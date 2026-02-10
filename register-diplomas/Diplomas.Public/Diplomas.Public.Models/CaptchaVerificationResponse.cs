using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Diplomas.Public.Models
{
    public class CaptchaVerificationResponse
    {
        public bool Success { get; set; }

        /// <summary>
        /// timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
        /// </summary>
        [JsonProperty("challenge_ts")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// the hostname of the site where the reCAPTCHA was solved
        /// </summary>
        public string Hostname { get; set; }

        public float? Score { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// optional
        /// missing-input-secret	The secret parameter is missing.
        /// invalid-input-secret	The secret parameter is invalid or malformed.
        /// missing-input-response	The response parameter is missing.
        /// invalid-input-response	The response parameter is invalid or malformed.
        /// bad-request	The request is invalid or malformed.
        /// timeout-or-duplicate	The response is no longer valid: either is too old or has been used previously.
        /// </summary>
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
