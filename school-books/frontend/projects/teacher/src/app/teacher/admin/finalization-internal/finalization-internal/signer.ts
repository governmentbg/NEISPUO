import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { getUnexpectedErrorMessage } from 'projects/shared/utils/error';
import { from, Observable, of, race, throwError, timer } from 'rxjs';
import { fromFetch } from 'rxjs/fetch';
import { catchError, map, switchMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

class DeferredPromise<T> {
  promise: Promise<T>;
  resolve!: (value: T | PromiseLike<T>) => void;
  reject!: (reason?: any) => void;

  constructor() {
    this.promise = new Promise<T>((resolve, reject) => {
      this.resolve = resolve;
      this.reject = reject;
    });
  }
}

class MaxConcurrencyExecutor<T> {
  constructor(private concurrency: number) {}

  enqueue(task: () => Promise<T>): Promise<T> {
    const deferred = new DeferredPromise<T>();
    this.queue.push([task, deferred]);
    this.run();
    return deferred.promise;
  }

  private queue: [() => Promise<T>, DeferredPromise<T>][] = [];
  private running = 0;

  private run() {
    if (this.running >= this.concurrency) {
      return;
    }

    const next = this.queue.shift();
    if (next === undefined) {
      return;
    }

    this.running++;

    const [task, deferred] = next;
    task()
      .then((result) => deferred.resolve(result))
      .catch((error) => deferred.reject(error))
      .finally(() => {
        this.running--;
        this.run();
      });
  }
}

class SignerError {
  constructor(
    public readonly userMessage: string | null,
    public readonly error: any = null,
    public readonly code: string | null = null
  ) {}
}

export class Signer {
  private static readonly signingServiceLocation = environment.signingServerPath;
  private static readonly signingServiceSignPdfEndpoint = `${this.signingServiceLocation}/api/certificate/signPdf`;
  private static readonly signingServiceVersionEndpoint = `${this.signingServiceLocation}/api/server/version`;

  // if the service does not respond in 2sec we assume it does not exist
  private static readonly signingServiceVersionTimeoutMs = 2000;

  private readonly downloadPdfBase64Executor = new MaxConcurrencyExecutor<string>(5);
  private readonly signPdfExecutor = new MaxConcurrencyExecutor<string>(1);

  private lastUsedThumbprint: string | null = null;

  downloadPdfBase64(blobDownloadUrl: string): Promise<string> {
    return this.downloadPdfBase64Executor.enqueue(() =>
      fetch(blobDownloadUrl)
        .then(
          (response) => {
            if (!response.ok) {
              return Promise.reject(new SignerError(null, response));
            }

            return response
              .blob()
              .catch((error) => Promise.reject(new SignerError(null, error, 'downloadpdf_fetch_blob_error')));
          },
          (error) => Promise.reject(new SignerError(null, error, 'downloadpdf_fetch_error'))
        )
        .then(
          (blob) =>
            new Promise<string>((resolve, reject) => {
              const reader = new FileReader();
              reader.onloadend = () => {
                if (reader.error) {
                  return;
                }

                if (typeof reader.result !== 'string' || reader.result.length === 0) {
                  reject(new SignerError(null, null, 'downloadpdf_reader_unexpected_result'));
                  return;
                }

                resolve(reader.result.replace(/data:.+;base64,/, ''));
              };
              reader.onerror = () => {
                reject(new SignerError(null, reader.error, 'downloadpdf_reader_error'));
              };

              reader.readAsDataURL(blob);
            })
        )
        .catch((error) => {
          if (error instanceof SignerError) {
            GlobalErrorHandler.instance.handleError(error.error, true);
            return Promise.reject(error.userMessage || getUnexpectedErrorMessage(error.code || error.error));
          }

          GlobalErrorHandler.instance.handleError(error, true);
          return Promise.reject(getUnexpectedErrorMessage('downloadpdf_unexpected_error'));
        })
    );
  }

  signPdf(pdfBase64: string): Promise<string> {
    return this.signPdfExecutor.enqueue(() =>
      fetch(Signer.signingServiceSignPdfEndpoint, {
        headers: {
          'content-type': 'application/json; charset=UTF-8'
        },
        body: JSON.stringify({
          contents: pdfBase64,
          options: {
            isVisible: true,
            pagePosition: 'first'
          },
          thumbprint: this.lastUsedThumbprint
        }),
        method: 'POST',
        mode: 'cors',
        credentials: 'omit'
      })
        .then(
          (response) => {
            if (!response.ok) {
              return Promise.reject(new SignerError(null, response, 'signpdf_fetch_unexpected_status'));
            }
            return response
              .json()
              .catch((error) => Promise.reject(new SignerError(null, error, 'signpdf_fetch_json_error')));
          },
          (error) => Promise.reject(new SignerError(null, error, 'signpdf_fetch_error'))
        )
        .then((result) => {
          if (!result) {
            return Promise.reject(new SignerError(null, null, 'signpdf_empty_response'));
          }

          if (result.isError) {
            return Promise.reject(new SignerError(result.message));
          }

          let contents: string;
          if (typeof result.contents === 'string' && result.contents.length > 0) {
            contents = result.contents;
          } else {
            return Promise.reject(new SignerError(null, result, 'signpdf_empty_contents'));
          }

          if (typeof result.thumbprint === 'string' && result.thumbprint.length > 0) {
            this.lastUsedThumbprint = result.thumbprint;
          }

          return contents;
        })
        .catch((error) => {
          if (error instanceof SignerError) {
            GlobalErrorHandler.instance.handleError(error.error, true);
            return Promise.reject(error.userMessage || getUnexpectedErrorMessage(error.code || error.error));
          }

          GlobalErrorHandler.instance.handleError(error, true);
          return Promise.reject(getUnexpectedErrorMessage('signpdf_unexpected_error'));
        })
    );
  }

  static signingServiceExists(): Observable<boolean> {
    return race(
      fromFetch(Signer.signingServiceVersionEndpoint, {
        method: 'GET',
        mode: 'cors',
        credentials: 'omit'
      }).pipe(
        switchMap((response) => {
          if (!response.ok) {
            return throwError(response.statusText);
          }
          return from(response.json());
        }),
        map((res) => typeof res === 'string' && res.length > 0),
        catchError(() => of(false))
      ),
      timer(this.signingServiceVersionTimeoutMs).pipe(map(() => false))
    );
  }
}
