import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegBookCertificatesComponent } from './reg-book-certificates/reg-book-certificates.component';

const routes: Routes = [{ path: '', component: RegBookCertificatesComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RegBookCertificatesRoutingModule {}
