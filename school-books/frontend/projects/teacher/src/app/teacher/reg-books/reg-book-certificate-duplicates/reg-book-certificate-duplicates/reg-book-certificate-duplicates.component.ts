import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'sb-reg-book-certificate-duplicates',
  templateUrl: './reg-book-certificate-duplicates.component.html'
})
export class RegBookCertificateDuplicatesComponent {
  readonly kontraxStudentsUrl = environment.kontraxStudentsUrl;
}
