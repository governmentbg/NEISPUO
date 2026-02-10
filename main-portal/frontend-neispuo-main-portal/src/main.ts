import { HttpClient, HttpHandler, HttpXhrBackend } from '@angular/common/http';
import { enableProdMode, Injector } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppInitService } from '@shared/services/app-init.service';

import { AppModule } from './app/app.module';
import { persistState } from '@datorama/akita';

const storage = persistState({ include: ['start-user-tour.startUserTour', 'is-user-guide-clicked.isUserGuideClicked'] });

const injector = Injector.create({
  providers: [
    { provide: HttpClient, deps: [HttpHandler] },
    {
      provide: HttpHandler,
      useValue: new HttpXhrBackend({ build: () => new XMLHttpRequest() })
    },
    { provide: AppInitService, deps: [HttpClient] },
    { provide: 'persistStorage', useValue: storage }
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
