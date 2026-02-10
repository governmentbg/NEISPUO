import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { SideMenuComponent } from './side-menu.component';

@NgModule({
    declarations: [SideMenuComponent],
    imports: [CommonModule, SharedModule],
    exports: [SideMenuComponent],
})
export class SideMenuModule {}
