import { Component, Input } from '@angular/core';

@Component({
  selector: 'sb-placeholder-field',
  templateUrl: './placeholder-field.component.html'
})
export class PlaceholderFieldComponent {
  @Input() label?: string;
}
