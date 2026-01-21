import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { JwtConfig, JwtModule, JWT_OPTIONS } from '@auth0/angular-jwt';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '@shared/shared.module';
import { NgxScrollTopModule } from 'ngx-scrolltop';
import { AuthenticationModule } from 'src/app/core/authentication/authentication.module';
import { AkitaNgRouterStoreModule } from '@datorama/akita-ng-router-store';
import { MunicipalInstitutionModule } from '@municipal-institutions/municipal-institutions.module';
import { ToastModule } from 'primeng/toast';
import { ProceduresModule } from '@procedures/procedures.module';
import { HttpClientModule } from '@angular/common/http';
import { AppInitService } from '@core/services/app-init.service';
import { AuthStore } from '@core/authentication/auth-state-manager/auth.store';
import { FlexFieldsModule } from '@flex-fields/flex-fields.module';
import { JoinProcedureModule } from './features/join-procedure/join-procedure.module';
import { MergeProcedureModule } from './features/merge-procedure/merge-procedure.module';
import { DivideProcedureModule } from './features/divide-procedure/divide-procedure.module';
import { DetachProcedureModule } from './features/detach-procedure/detach-procedure.module';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

export function jwtOptionsFactory(authStore: AuthStore) {
  return {
    tokenGetter: () => authStore.getValue().oidcAccessToken,
    allowedDomains: [],
    disallowedRoutes: [],
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
    ToastModule,
    AuthenticationModule,
    SharedModule,
    MunicipalInstitutionModule,
    ProceduresModule,
    JoinProcedureModule,
    FlexFieldsModule,
    MergeProcedureModule,
    DivideProcedureModule,
    DetachProcedureModule,
    AppRoutingModule,
    NgxScrollTopModule,
    AkitaNgRouterStoreModule,
    HttpClientModule,
    JwtModule.forRoot({
      jwtOptionsProvider: {
        provide: JWT_OPTIONS,
        useFactory: jwtOptionsFactory,
        deps: [AuthStore],
        multi: false,
      },
    }),
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initEnv,
      deps: [AppInitService, JWT_OPTIONS],
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
