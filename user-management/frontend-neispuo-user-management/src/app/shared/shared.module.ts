import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ConfirmDialogModule } from '@shared/components/confirm-dialog/confirm-dialog.module';
import { ExportButtonModule } from './components/export-button/export-button.module';
import { HeadingModule } from './components/heading/heading.module';
import { PrimeNGComponentsModule } from './components/primeng-components/primeng-components.module';
import { SpinnerModule } from './components/spinner/spinner.module';
import { BooleanPipe } from './pipes/boolean.pipe';
import { DotNotatedPipe } from './pipes/dot-notated.pipe';
import { DotNotationPipe } from './pipes/dot-notation.pipe';
import { EnumPipe } from './pipes/enum.pipe';
import { SideMenuService } from './services/side-menu.service';

@NgModule({
    declarations: [DotNotationPipe, DotNotatedPipe, BooleanPipe, EnumPipe],
    imports: [
        CommonModule,
        FormsModule,
        PrimeNGComponentsModule,
        HeadingModule,
        TranslateModule,
        ConfirmDialogModule,
        SpinnerModule,
        ExportButtonModule,
        ReactiveFormsModule,
    ],
    exports: [
        CommonModule,
        FormsModule,
        PrimeNGComponentsModule,
        DotNotationPipe,
        BooleanPipe,
        HeadingModule,
        EnumPipe,
        TranslateModule,
        ConfirmDialogModule,
        SpinnerModule,
        ExportButtonModule,
        ReactiveFormsModule,
    ],
    providers: [SideMenuService],
})
export class SharedModule {}
