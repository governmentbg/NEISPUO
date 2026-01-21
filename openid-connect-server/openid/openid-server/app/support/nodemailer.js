const nodemailer = require('nodemailer');

let transporter = nodemailer.createTransport({
  pool: process.env.EMAIL_POOL === 'true',
  host: process.env.EMAIL_HOST,
  port: +process.env.EMAIL_PORT,
  secure: +process.env.EMAIL_PORT === 465, // true for 465, false for other ports
  auth: {
    user: process.env.EMAIL_USER,
    pass: process.env.EMAIL_PASSWORD,
  },
});

async function sendEmail({
  from = process.env.EMAIL_FROM, // sender address
  to = undefined, // list of receivers 'email@example.com, email2@example.com'
  subject = undefined, // Subject line
  text = undefined, // plain text body
  html = undefined, // html body
}) {
  if (process.env.STOP_EMAILS === 'true') {
    console.log(`Skipping email to ${to} due to STOP_EMAILS=true.`);
    return;
  }

  if (process.env.DEV_INBOX) {
    to = process.env.DEV_INBOX;
  }

  try {
    // send mail with defined transport object
    /* const info = */ await transporter.sendMail({ from, to, subject, text, html });

    console.log(`Email sent to "${to}" with subject "${subject}"`);
  } catch (error) {
    const errStr = error ? JSON.stringify(error) : '';
    console.log(errStr);
    console.log((error && error.message) || error);
    console.error(`Failed to send email to ${to}. See logs above`);
    throw error;
  }
}

module.exports = { sendEmail };
