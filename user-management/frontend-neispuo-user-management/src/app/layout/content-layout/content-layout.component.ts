import { Component, OnInit, ViewChild } from '@angular/core';
import { SideMenuService } from '@shared/services/side-menu.service';

@Component({
    selector: 'app-content-layout',
    templateUrl: './content-layout.component.html',
    styleUrls: ['./content-layout.component.scss'],
})
export class ContentLayoutComponent implements OnInit {
    displaySidemenu!: boolean;

    constructor(public sideMenuData: SideMenuService) {}

    ngOnInit() {
        this.sideMenuData.displaySidemenu.subscribe((displaySidemenu) => (this.displaySidemenu = displaySidemenu));
    }
}
