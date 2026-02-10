import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CubejsClientModule } from '@cubejs-client/ngx';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AkitaNgRouterStoreModule } from '@datorama/akita-ng-router-store';
import { HttpClientModule } from '@angular/common/http';
import { MultiSelectModule } from 'primeng/multiselect';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { SkeletonModule } from 'primeng/skeleton';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { DropdownModule } from 'primeng/dropdown';
import { ChartModule } from 'primeng/chart';
import { DragDropModule } from 'primeng/dragdrop';
import { AvailableReportsService } from './features/reporting/avalailable-reports/available-report.service';
import { AvailableReportsStore } from './features/reporting/avalailable-reports/available-report.store';
import { AvailableReportsQuery } from './features/reporting/avalailable-reports/available-report.query';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { SidebarModule } from 'primeng/sidebar';
import { DataViewModule } from 'primeng/dataview';
import { AuthStore } from '@authentication/auth-state-manager/auth.store';
import { JwtConfig, JwtModule, JWT_OPTIONS } from '@auth0/angular-jwt';
import { AppInitService } from '@shared/services/app-init.service';
import { AuthenticationModule } from '@authentication/authentication.module';

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
    AppRoutingModule,
    FormsModule,
    DataViewModule,
    CoreModule,
    SharedModule,
    ButtonModule,
    AkitaNgRouterStoreModule,
    CubejsClientModule.forRoot({}),
    JwtModule.forRoot({
      jwtOptionsProvider: {
        provide: JWT_OPTIONS,
        useFactory: jwtOptionsFactory,
        deps: [AuthStore]
      }
    }),
    HttpClientModule,
    MultiSelectModule,
    DropdownModule,
    ButtonModule,
    TableModule,
    CardModule,
    SkeletonModule,
    MessagesModule,
    MessageModule,
    ChartModule,
    DragDropModule,
    SidebarModule
  ],
  providers: [
    AvailableReportsService,
    AvailableReportsStore,
    AvailableReportsQuery,
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
