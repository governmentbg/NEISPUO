import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'translate'
})
export class TranslatePipe implements PipeTransform {
  translations = {
    /**
     * Roles
     *
     */
    ADMIN: 'Администратор',
  };

  constructor() { }

  transform(value: string, args?: any): string {
    return this.translations[value] || value;
  }

  translateHttpError(err: any) {
    const errorMessage = err?.error?.message || err?.message;
    err.message = this.translations[errorMessage] || 'Възникна грешка. Моля опитайте отново.';
    return err;
  }
}
