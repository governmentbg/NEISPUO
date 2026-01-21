import { Component, Input, OnInit } from '@angular/core';
import { NewModuleInfoQuery } from '@reporting/new-module-info/new-module-info.query';

@Component({
  selector: 'app-new-module-info',
  templateUrl: './new-module-info.component.html',
  styleUrls: ['./new-module-info.component.scss']
})
export class NewModuleInfoComponent implements OnInit {
  @Input() showNewModuleInfoModal;

  constructor(public newModuleInfoQuery: NewModuleInfoQuery) {}

  ngOnInit(): void {}
}
