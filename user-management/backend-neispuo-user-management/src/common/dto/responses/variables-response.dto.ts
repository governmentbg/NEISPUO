import { IsString } from 'class-validator';
import { DTO } from './dto.interface';

export class VariablesResponseDTO implements DTO {
    @IsString()
    name?: string;

    @IsString()
    value?: string;
}
