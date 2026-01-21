import { IsNotEmpty } from 'class-validator';
import { UserDTO } from '../user/user.dto';

export class LoginResponseDTO {
    readonly accessToken: string;
    readonly user: UserDTO;
    constructor(partial: Partial<LoginResponseDTO>) {
        Object.assign(this, partial);
    }
}

export class LoginRequestDTO {
    @IsNotEmpty()
    readonly email: string;
    @IsNotEmpty()
    readonly password: string;
    @IsNotEmpty()
    readonly recaptchaToken: string;
}
