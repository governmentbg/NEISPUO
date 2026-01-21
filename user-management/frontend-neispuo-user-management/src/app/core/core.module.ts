import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AuthService } from './authentication/auth.service';
import { ErrorHandlerModule } from './error/error-handler.module';
import { ApiService } from './services/api.service';
import { OIDCService } from './services/oidc.service';
import { ToastModule } from './toast/toast.module';

@NgModule({
    declarations: [],
    imports: [CommonModule, ToastModule, ErrorHandlerModule],
    providers: [ApiService, OIDCService, AuthService],
    exports: [ToastModule, ErrorHandlerModule],
})
export class CoreModule {}
