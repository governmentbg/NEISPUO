import { APP_BASE_HREF } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, ErrorHandler, NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouteReuseStrategy } from '@angular/router';
import { ServiceWorkerModule } from '@angular/service-worker';
import { OAuthModule, OAuthService } from 'angular-oauth2-oidc';
import { UserConfigService } from 'projects/sb-api-client/src/api/userConfig.service';
import { Configuration } from 'projects/sb-api-client/src/configuration';
import { NotFoundModule } from 'projects/shared/components/not-found/not-found.module';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { AuthInitializerService } from 'projects/shared/services/auth-initializer.service';
import { AuthService } from 'projects/shared/services/auth.service';
import { ConfigService, Project } from 'projects/shared/services/config.service';
import { EventService } from 'projects/shared/services/event.service';
import { PwaServiceModule } from 'projects/shared/services/pwa-service/pwa-service.module';
import { ParamsRouteReuseStrategy } from 'projects/shared/utils/router';
import { disableOptimizations } from 'projects/shared/utils/various';
import { environment } from 'src/environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserAnimationsModule,
    HttpClientModule,
    OAuthModule.forRoot(),
    NotFoundModule,
    PwaServiceModule,
    AppRoutingModule,
    ServiceWorkerModule.register('student-ngsw-worker.js', {
      enabled: environment.production,
      // Register the ServiceWorker as soon as the application is stable
      // or after 20 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:20000'
    })
  ],
  providers: [
    { provide: APP_BASE_HREF, useValue: '/s/' },
    {
      provide: Configuration,
      useFactory: (oauthService: OAuthService) =>
        new Configuration({
          basePath: environment.apiBasePath,
          accessToken: () => oauthService.getAccessToken()
        }),
      deps: [OAuthService],
      multi: false
    },
    {
      provide: APP_INITIALIZER,
      useFactory:
        (
          authInitializerService: AuthInitializerService,
          authService: AuthService,
          configService: ConfigService,
          eventService: EventService
        ) =>
        () =>
          authInitializerService
            .initialize(environment.authServerPath, disableOptimizations(environment.authRequireHttps) !== 'FALSE')
            .then(() => authService.initialize())
            .then(() => configService.initialize())
            .then(() => GlobalErrorHandler.instance.registerServices(eventService, authService)),
      deps: [AuthInitializerService, AuthService, ConfigService],
      multi: true
    },
    {
      provide: ConfigService,
      useFactory: (userConfigService: UserConfigService) =>
        new ConfigService(userConfigService, {
          project: Project.StudentsApp,
          teachersAppUrl: environment.teachersAppUrl,
          studentsAppUrl: environment.studentsAppUrl
        }),
      deps: [UserConfigService]
    },
    {
      provide: RouteReuseStrategy,
      useClass: ParamsRouteReuseStrategy
    },
    {
      provide: ErrorHandler,
      useValue: GlobalErrorHandler.instance
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
