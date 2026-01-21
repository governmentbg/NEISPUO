import { HttpParameterCodec, HttpParams } from '@angular/common/http';

/**
 * Custom HttpParameterCodec
 * Workaround for https://github.com/angular/angular/issues/18261
 */
export class CustomHttpParameterCodec implements HttpParameterCodec {
    encodeKey(k: string): string {
        return encodeURIComponent(k);
    }
    encodeValue(v: string): string {
        return encodeURIComponent(v);
    }
    decodeKey(k: string): string {
        return decodeURIComponent(k);
    }
    decodeValue(v: string): string {
        return decodeURIComponent(v);
    }
}

// we cannot create new template files and there is no other private (not exported) file,
// so there is no better place to put these functions

export function mapValues(data: any, fn: (item: any) => any) {
    if (data == null) {
        return data;
    }
    return Object.keys(data).reduce(
        (acc, key) => ({ ...acc, [key]: fn(data[key]) }),
        {}
    );
}

const pad = (n: any) => {
  if (n < 10) {
    return '0' + n;
  }
  return n;
};

export const dateToLocalIsoString = (d: any) =>
    d == null ?
        d :
        d.getFullYear() +
            '-' + pad(d.getMonth() + 1) +
            '-' + pad(d.getDate());

export const dateTimeToLocalIsoString = (d: any) =>
    d == null ?
        d :
        d.getFullYear() +
            '-' + pad(d.getMonth() + 1) +
            '-' + pad(d.getDate()) +
            'T' + pad(d.getHours()) +
            ':' + pad(d.getMinutes()) +
            ':' + pad(d.getSeconds());

export const isoStringToLocalDate = (s: any) =>
    s == null ?
        s :
        new Date(s.substr(0, 10) + 'T00:00:00');

export const isoStringToLocalDateTime = (s: any) =>
    s == null ?
        s :
        new Date(s.indexOf('T') > -1 ? s.substr(0, 19) : s.substr(0, 10) + 'T00:00:00');

// Taken directly from angular
// https://github.com/angular/angular/blob/00f293c35f74a71d08e8f66efa50fe7194377355/packages/common/http/src/request.ts#L250
export const urlWithParams = ({ url, httpParams }: { url: string; httpParams?: HttpParams }) => {
  if (!httpParams) {
    return url;
  } else {
    // Encode the parameters to a string in preparation for inclusion in the URL.
    const params = httpParams.toString();
    if (params.length === 0) {
      // No parameters, the visible URL is just the URL given at creation time.
      return url;
    } else {
      // Does the URL already have query parameters? Look for '?'.
      const qIdx = url.indexOf('?');
      // There are 3 cases to handle:
      // 1) No existing parameters -> append '?' followed by params.
      // 2) '?' exists and is followed by existing query string ->
      //    append '&' followed by params.
      // 3) '?' exists at the end of the url -> append params directly.
      // This basically amounts to determining the character, if any, with
      // which to join the URL and parameters.
      const sep: string = qIdx === -1 ? '?' : qIdx < url.length - 1 ? '&' : '';
      return url + sep + params;
    }
  }
};
