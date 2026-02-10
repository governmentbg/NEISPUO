import { DTO } from './dto.interface';

export class DBErrorsDTO implements DTO {
    errorProcedure: string;

    data: string;
}
