import { HttpClient, HttpHandler, HttpXhrBackend } from '@angular/common/http';
import { enableProdMode, Injector } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { AppInitService } from './app/core/services/app-init.service';

const injector = Injector.create({
  providers: [
    { provide: HttpClient, deps: [HttpHandler] },
    {
      provide: HttpHandler,
      useValue: new HttpXhrBackend({ build: () => new XMLHttpRequest() })
    },
    { provide: AppInitService, deps: [HttpClient] }
  ]
});
const appInitService = injector.get(AppInitService);

async function setApplicationMode() {
  const config = await appInitService.getConfiguration();
  if (config.production) {
    enableProdMode();
  }
}
setApplicationMode().then(() => {
  platformBrowserDynamic()
    .bootstrapModule(AppModule)
    .catch((err) => console.error(err));
});
