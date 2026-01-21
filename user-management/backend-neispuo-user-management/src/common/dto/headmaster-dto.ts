import { IsEmail, IsOptional, IsString } from 'class-validator';

export class HeadmasterDTO {
    @IsString()
    firstName: string;

    @IsString()
    middleName: string;

    @IsString()
    lastName: string;

    @IsOptional()
    @IsEmail()
    email: string;
}
