import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MaterialModule } from "./material.module";
import { LoaderComponent } from "./loader/loader.component";
import { MatMultiSortModule } from "./multisort/mat-multi-sort.module";

@NgModule({
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, MatMultiSortModule],
  declarations: [LoaderComponent],
  exports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, MatMultiSortModule, LoaderComponent]
})
export class SharedModule {}
