import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { getUnexpectedErrorMessage } from 'projects/shared/utils/error';
import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .catch((err) => {
    // error should be logged into console by defaultErrorLogger,
    // so no extra logging is necessary here
    const errorMsgElement = document.querySelector('#errorMsgElement');
    if (errorMsgElement) {
      errorMsgElement.textContent = getUnexpectedErrorMessage(err);
    }
  });
