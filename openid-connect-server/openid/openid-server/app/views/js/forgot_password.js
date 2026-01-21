// Display error message if needed
const URLparams = new URLSearchParams(document.location.search.substring(1));
const error = URLparams.get('error');
const errorElement = !!error && document.querySelector('#errorMsg');
if (error) {
  errorElement.setAttribute('style', 'display:block;');
}

if (error === 'invalid_username_or_password') {
  errorElement.innerText = ' Невалиден имейл или парола.';
}

if (error === 'invalid_recaptcha') {
  errorElement.innerText = ' Невалидна recaptcha.';
}

// Text field animations
const { MDCTextField } = mdc.textField;
const labels = document.querySelectorAll('label');
for (const label of Array.from(labels)) {
  new MDCTextField(label);
}

// Button ripple
const { MDCRipple } = mdc.ripple;
new MDCRipple(document.querySelector('.mdc-button'));
