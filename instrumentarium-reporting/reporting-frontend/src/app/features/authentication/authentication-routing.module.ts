import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NoAuthGuard } from './guards/no-auth.guard';
import { LoginPage } from './pages/login/login.page';
import { SigninCallbackPage } from './pages/signin-callback/signin-callback.page';
import { SilentSigninCallbackPage } from './pages/silent-signin-callback/silent-signin-callback.page';

const routes: Routes = [
  { path: 'login', component: LoginPage, canActivate: [NoAuthGuard] },
  { path: 'signin-callback', component: SigninCallbackPage },
  { path: 'silent-signin-callback', component: SilentSigninCallbackPage }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthenticationRoutingModule {}
