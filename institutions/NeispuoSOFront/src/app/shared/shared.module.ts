import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MaterialModule } from "./material.module";
import { LoaderComponent } from "./loader/loader.component";
import { MatMultiSortModule } from "./multisort/mat-multi-sort.module";
import { UploadComponent } from "./upload/upload.component";
import { FileUploadModule } from "@iplab/ngx-file-upload";
import { FileService } from "../services/file.service";
import { EgnValidator } from "./egn-validator.directive";
import { ScrollingModule } from "@angular/cdk/scrolling";
import { InnerLoaderComponent } from "./inner-loader/inner-loader.component";
import { InfoTooltipComponent } from "./info-tooltip/info-tooltip.component";
import { RegixService } from "../services/regix.service";
import { LnchValidator } from "./lnch-validator.directive";
import { NotLnchValidator } from "./not-lnch-validator.directive";
import { NotEgnValidator } from "./not-egn-validator.directive";

@NgModule({
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, MatMultiSortModule, FileUploadModule],
  declarations: [
    LoaderComponent,
    UploadComponent,
    EgnValidator,
    LnchValidator,
    NotEgnValidator,
    NotLnchValidator,
    InnerLoaderComponent,
    InfoTooltipComponent
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    MatMultiSortModule,
    FileUploadModule,
    ScrollingModule,
    LoaderComponent,
    UploadComponent,
    EgnValidator,
    LnchValidator,
    NotEgnValidator,
    NotLnchValidator,
    InnerLoaderComponent,
    InfoTooltipComponent
  ],
  providers: [FileService, RegixService]
})
export class SharedModule {}
