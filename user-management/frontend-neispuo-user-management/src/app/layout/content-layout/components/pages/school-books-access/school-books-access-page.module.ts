import { NgModule } from '@angular/core';
import { DialogService } from 'primeng/dynamicdialog';
import { SharedModule } from 'src/app/shared/shared.module';
import { SchoolBooksAccessPageComponent } from './school-books-access-page/school-books-access-page.component';
import { AddSchoolBookAccessComponent } from './school-books-addition-modal/add-school-book-access.component';

@NgModule({
    declarations: [SchoolBooksAccessPageComponent, AddSchoolBookAccessComponent],
    imports: [SharedModule],
    exports: [SchoolBooksAccessPageComponent],
    providers: [DialogService],
})
export class SchoolBooksAccessPageModule {}
