import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MaterialModule } from "./material.module";
import { LoaderComponent } from "./loader/loader.component";
import { MatMultiSortModule } from "./multisort/mat-multi-sort.module";
import { UploadComponent } from "./upload/upload.component";
import { FileUploadModule } from "@iplab/ngx-file-upload";
import { FileService } from "../services/file.service";
import { InfoTooltipComponent } from "./info-tooltip/info-tooltip.component";

@NgModule({
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, MatMultiSortModule, FileUploadModule],
  declarations: [LoaderComponent, UploadComponent, InfoTooltipComponent],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    MatMultiSortModule,
    FileUploadModule,
    LoaderComponent,
    UploadComponent,
    InfoTooltipComponent
  ],
  providers: [FileService]
})
export class SharedModule {}
