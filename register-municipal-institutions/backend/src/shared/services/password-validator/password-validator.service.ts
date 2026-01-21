const passwordValidator = new (require('password-validator'))();

/* Docs: https://www.npmjs.com/package/password-validator */
passwordValidator
    .is()
    .min(8)
    .has()
    .not()
    .spaces();

export class PasswordValidator {
    validate(string: string) {
        // eslint-disable-next-line no-lone-blocks
        {
            const errors = passwordValidator.validate(string, { list: true });
            const result = { passwordValid: false, message: '' };

            if (!errors.length) {
                result.passwordValid = true;
                return result;
            }
            for (const error of errors) {
                switch (error) {
                    case 'min':
                        result.message += 'Password must be at least 8 characters long.\n';
                        break;
                    default:
                        result.message += 'Error validating your password.\n';
                        break;
                }
            }

            return result;
        }
    }
}
