import { IsEmail, IsNotEmpty, IsString } from 'class-validator';

export class GetParentInfoRequestDto {
    @IsNotEmpty()
    @IsString()
    @IsEmail()
    email: string;
}
