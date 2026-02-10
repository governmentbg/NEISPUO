import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'sb-reg-book-qualification-duplicates',
  templateUrl: './reg-book-qualification-duplicates.component.html'
})
export class RegBookQualificationDuplicatesComponent {
  readonly kontraxStudentsUrl = environment.kontraxStudentsUrl;
}
