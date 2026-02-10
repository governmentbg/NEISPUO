import { HttpClient, HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { JWT_OPTIONS, JwtConfig, JwtModule } from '@auth0/angular-jwt';
import { AuthStore } from '@core/authentication/auth.store';
import { AkitaNgRouterStoreModule } from '@datorama/akita-ng-router-store';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MessageService } from 'primeng/api';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { AuthGuard } from './core/guard/auth.guard';
import { NoAuthGuard } from './core/guard/no-auth.guard';
import { AppInitService } from './core/services/app-init.service';
import { ContentLayoutModule } from './layout/content-layout/content-layout.module';
import { LoginLayoutModule } from './layout/login-layout/login-layout.module';
import { SharedModule } from './shared/shared.module';

export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

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
        CoreModule,
        SharedModule,
        ContentLayoutModule,
        LoginLayoutModule,
        AppRoutingModule,
        HttpClientModule,
        AkitaNgRouterStoreModule,

        JwtModule.forRoot({
            jwtOptionsProvider: {
                provide: JWT_OPTIONS,
                useFactory: jwtOptionsFactory,
                deps: [AuthStore],
                multi: false,
            },
        }),
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [HttpClient],
            },
        }),
    ],
    providers: [
        NoAuthGuard,
        AuthGuard,
        MessageService,
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
