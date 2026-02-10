import { HttpStatusCode } from '@angular/common/http';

export class UserManagementResponse<T> {
    status!: HttpStatusCode;

    message: string = '';

    payload!: T;
}
