import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { NeispuoModule } from '@portal/neispuo-modules/neispuo-module.interface';

@Component({
  selector: 'app-neispuo-module-card',
  templateUrl: './neispuo-module-card.component.html',
  styleUrls: ['./neispuo-module-card.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class NeispuoModuleCardComponent implements OnInit {

  @Input() module: NeispuoModule;
  constructor() { }

  ngOnInit(): void {
  }

}
