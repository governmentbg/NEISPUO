export function isHttpSuccessfulResponse(statusCode: number | undefined): boolean {
  return !!(statusCode && statusCode >= 200 && statusCode < 300);
}
