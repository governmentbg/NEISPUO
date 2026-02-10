// Display error message if needed
const URLparams = new URLSearchParams(document.location.search.substring(1));
const error = URLparams.get('error');
const errorElement = !!error && document.querySelector('#errorMsg');
if (error) {
  errorElement.setAttribute('style', 'display:block;');
}

if (error === 'invalid_recaptcha') {
  errorElement.innerText = ' Невалидна recaptcha';
}

if (error === 'forgot_pass_token_invalid') {
  errorElement.innerText = 'Кодът за промяна на парола е изтекъл';
}

if (error === 'password_mismatch') {
  errorElement.innerText = 'Парола и потвърждение на парола трябва да съвпадат';
}

if (error === 'password_not_valid') {
  errorElement.innerText = 'Паролата трябва да има поне 8 символа и да съдържа числа и букви';
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
