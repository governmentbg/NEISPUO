import { Component, OnInit } from '@angular/core';
import { ISideMenuConfig, SideMenuConfig } from './sidemenu.config';

@Component({
  selector: 'app-second-nav',
  templateUrl: './second-nav.component.html',
  styleUrls: ['./second-nav.component.scss']
})
export class SecondNavComponent implements OnInit {
  sideMenuList!: ISideMenuConfig[];

  constructor() {}

  ngOnInit(): void {
    this.sideMenuList = SideMenuConfig;
  }
}
