import { IsNumber } from 'class-validator';

export class DeleteStudentDtoRequest {
    @IsNumber()
    personID: number;
}
