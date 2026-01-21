using Diplomas.Public.Models;
using Diplomas.Public.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Diplomas.Public.Services
{
    public class CaptchaVerificationService
    {
        private readonly CaptchaSettings _captchaSettings;
        private readonly ILogger<CaptchaVerificationService> _logger;

        public CaptchaVerificationService(IOptions<CaptchaSettings> captchaSettings,
            ILogger<CaptchaVerificationService> logger)
        {
            _captchaSettings = captchaSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token">Client reCaptcha response token.</param>
        /// <param name="minScore">Minimum points for validation to be considered successful.</param>
        /// <param name="actionName">Action name to compare with the client reCaptcha response.</param>
        /// <returns></returns>
        public async Task<bool> IsCaptchaValid(string token, float minScore = 0.5f, string actionName = "")
        {
            var result = false;

            var googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";

            try
            {
                using var client = new HttpClient();

                var response = await client.PostAsync($"{googleVerificationUrl}?secret={_captchaSettings.SecretKey}&response={token}", null);
                var jsonString = await response.Content.ReadAsStringAsync();
                var captchaVerfication = JsonConvert.DeserializeObject<CaptchaVerificationResponse>(jsonString);

                result = captchaVerfication.Success
                    && (captchaVerfication.Score ?? 0f) >= minScore
                    && (captchaVerfication.Action ?? "").Equals(actionName, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to process captcha validation", e);
            }

            return result;
        }
    }
}
