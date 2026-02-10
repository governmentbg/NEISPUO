import { HttpErrorResponse } from '@angular/common/http';

export function getRequestId(error: unknown) {
  if (error instanceof HttpErrorResponse || error instanceof Response) {
    return error.headers.get('X-Sb-Request-Id');
  } else if (typeof error === 'string' && error.length > 0) {
    return error;
  } else {
    return null;
  }
}

export function getUnexpectedErrorMessage(error: unknown) {
  const requestId = getRequestId(error);

  return requestId
    ? `Възникна грешка! Моля, презаредете страницата и опитайте отново. Номер на грешка: ${requestId}`
    : 'Възникна грешка! Моля, презаредете страницата и опитайте отново.';
}
