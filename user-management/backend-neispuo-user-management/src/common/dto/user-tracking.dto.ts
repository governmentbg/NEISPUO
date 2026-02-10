import { IsNotEmpty } from 'class-validator';

export class UserTrackingDto {
    @IsNotEmpty()
    email: string;
}
