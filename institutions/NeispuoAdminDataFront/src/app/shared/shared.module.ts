import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MaterialModule } from "./material.module";
import { LoaderComponent } from "./loader/loader.component";
import { MatMultiSortModule } from "./multisort/mat-multi-sort.module";
import { EgnValidator } from "./egn-validator.directive";
import { ScrollingModule } from "@angular/cdk/scrolling";
import { InnerLoaderComponent } from "./inner-loader/inner-loader.component";
import { InfoTooltipComponent } from "./info-tooltip/info-tooltip.component";

@NgModule({
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, MatMultiSortModule],
  declarations: [LoaderComponent, EgnValidator, InnerLoaderComponent, InfoTooltipComponent],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    MatMultiSortModule,
    ScrollingModule,
    LoaderComponent,
    EgnValidator,
    InnerLoaderComponent,
    InfoTooltipComponent
  ]
})
export class SharedModule {}
