import { IsNumber, IsString } from 'class-validator';
import { DTO } from '../responses/dto.interface';

export class EnrollmentDeleteRequestDTO implements DTO {
    @IsString()
    classs: string;

    @IsNumber()
    school: number;

    @IsNumber()
    user: number;
}
