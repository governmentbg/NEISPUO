import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PaginatorItem } from './paginator-item';

@Component({
  selector: 'sb-paginator',
  templateUrl: './paginator.component.html',
  styleUrls: ['./paginator.component.scss']
})
export class PaginatorComponent {
  @Input() items: PaginatorItem[] = [];
  @Output() itemClick = new EventEmitter<PaginatorItem>();
}
