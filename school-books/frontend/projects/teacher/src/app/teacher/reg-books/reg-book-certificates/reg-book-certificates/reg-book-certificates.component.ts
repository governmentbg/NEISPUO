import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'sb-reg-book-certificates',
  templateUrl: './reg-book-certificates.component.html'
})
export class RegBookCertificatesComponent {
  readonly kontraxStudentsUrl = environment.kontraxStudentsUrl;
}
