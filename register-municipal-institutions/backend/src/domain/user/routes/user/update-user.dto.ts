import { Exclude } from 'class-transformer';

export class UpdateUserDto {
    firstName: string;
    middleName: string;
    lastName: string;
    email: string;
    phone: string;
    address: string;

    @Exclude() // Prevent user from giving himself roles. No role change feature for now...
    roles?: any[];
}

export class UserResponseDTO {}
