import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'sb-reg-book-qualification',
  templateUrl: './reg-book-qualification.component.html'
})
export class RegBookQualificationComponent {
  readonly kontraxStudentsUrl = environment.kontraxStudentsUrl;
}
