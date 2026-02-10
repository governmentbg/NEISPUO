export class ChangePasswordDTO {
    currentPassword: string;
    newPassword: string;
    passwordConfirmation: string;
    recaptchaToken: string;
}
