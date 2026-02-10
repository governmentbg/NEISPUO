const axios = require('axios');

async function validateRecaptcha(gRecaptchaResponse) {
  // https://developers.google.com/recaptcha/docs/verify
  // https://developers.google.com/recaptcha/docs/invisible
  let success = false;
  try {
    const response = await axios({
      method: 'post',
      url: 'https://www.google.com/recaptcha/api/siteverify',
      params: {
        secret: process.env.RECAPTCHA_SECRET,
        response: gRecaptchaResponse,
      },
    });
    success = response && response.data && response.data.success;
  } catch (e) {
    console.error(`Failed to validate recaptcha. gRecaptchaResponse: ${gRecaptchaResponse}`);
  }

  return success;
}

module.exports = { validateRecaptcha };
