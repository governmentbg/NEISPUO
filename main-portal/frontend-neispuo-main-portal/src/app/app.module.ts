import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '@shared/shared.module';
import { NgxScrollTopModule } from 'ngx-scrolltop';
import { AuthenticationModule } from '@authentication/authentication.module';
import { AkitaNgRouterStoreModule } from '@datorama/akita-ng-router-store';

import { HttpClientModule } from '@angular/common/http';
import { JwtConfig, JwtModule, JWT_OPTIONS } from '@auth0/angular-jwt';
import { PortalModule } from '@angular/cdk/portal';
import { AuthGuard } from '@authentication/guards/auth.guard';
import { NoAuthGuard } from '@authentication/guards/no-auth.guard';
import { ParentGuard } from '@authentication/guards/parent.guard';
import { AuthStore } from '@authentication/auth-state-manager/auth.store';
import { AppInitService } from '@shared/services/app-init.service';

export function jwtOptionsFactory(authStore: AuthStore) {
  return {
    tokenGetter: () => {
      return authStore.getValue().oidcAccessToken;
    },
    allowedDomains: [],
    disallowedRoutes: []
  };
}

export function initEnv(appInit: AppInitService, jwtConfig: JwtConfig) {
  return async () => {
    const config = await appInit.loadConfiguration();
    jwtConfig.allowedDomains?.push(...config.ALLOWED_DOMAINS.split(','));
    jwtConfig.disallowedRoutes?.push(...config.DISALLOWED_ROUTES.split(','));
  };
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AuthenticationModule,
    SharedModule,
    AppRoutingModule,
    NgxScrollTopModule,
    AkitaNgRouterStoreModule,
    HttpClientModule,
    PortalModule,
    JwtModule.forRoot({
      jwtOptionsProvider: {
        provide: JWT_OPTIONS,
        useFactory: jwtOptionsFactory,
        deps: [AuthStore]
      }
    })
  ],
  providers: [
    AuthGuard,
    NoAuthGuard,
    ParentGuard,
    {
      provide: APP_INITIALIZER,
      useFactory: initEnv,
      deps: [AppInitService, JWT_OPTIONS],
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
