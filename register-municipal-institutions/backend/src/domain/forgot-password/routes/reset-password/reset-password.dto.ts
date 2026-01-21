export class ResetPasswordDTO {
    userUuid: string;
    token: string;
    newPassword: string;
    passwordConfirmation: string;
    used: boolean;
    recaptchaToken: string;
}
