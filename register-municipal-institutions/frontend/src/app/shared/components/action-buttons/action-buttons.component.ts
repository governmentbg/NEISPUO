import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-action-buttons',
  templateUrl: './action-buttons.component.html',
  styleUrls: ['./action-buttons.component.scss'],
})
export class ActionButtonsComponent implements OnInit {
  @Input() showBack: boolean;

  @Input() backLabel: string;

  @Input() backRouterLink: string | Array<string>;

  @Input() backwardCallback = (event: any) => { };

  @Input() showEdit: boolean;

  @Input() editLabel: string;

  @Input() editCallback = (event: any) => { };

  @Input() showDelete: boolean;

  @Input() deleteLabel: string;

  @Input() deleteCallback = (event: any) => { };

  constructor() { }

  ngOnInit(): void {
  }
}
