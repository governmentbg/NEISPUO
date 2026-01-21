import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegBookCertificateDuplicatesComponent } from './reg-book-certificate-duplicates/reg-book-certificate-duplicates.component';

const routes: Routes = [{ path: '', component: RegBookCertificateDuplicatesComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RegBookCertificateDuplicatesRoutingModule {}
