import { Component, OnInit } from "@angular/core";
import { FileService } from "../../../services/file.service";
import { Mode } from "src/app/enums/mode.enum";
import { ModalComponent } from "src/app/shared/modal/modal.component";
import { MatDialog } from "@angular/material/dialog";
import { DynamicComponent } from "src/app/shared/dynamic-component";
import { UploadComponent } from "src/app/shared/upload/upload.component";
import { environment } from "../../../../environments/environment";
import { HelperService } from "../../../services/helpers.service";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-file",
  templateUrl: "./file.component.html",
  styleUrls: ["./file.component.scss"]
})
export class FileComponent extends DynamicComponent implements OnInit {
  constructor(
    private fileService: FileService,
    private dialog: MatDialog,
    private helperService: HelperService,
    private route: ActivatedRoute
  ) {
    super();
  }

  get modes() {
    return Mode;
  }

  ngOnInit() {}

  download() {
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    const initialOwnershipDoc = this.field.name === "initialOwnershipDoc" ? this.field.value : 0;
    const latestOwnershipDoc = this.field.name === "latestOwnershipDoc" ? this.field.value : 0;

    if (this.field.name === "initialOwnershipDoc" || this.field.name === "latestOwnershipDoc") {
      this.fileService
        .getBuildingDocument(queryParams.buildingID, initialOwnershipDoc, latestOwnershipDoc)
        .subscribe(blob => this.fileService.downloadPDF(blob, this.field.name));
    } else if (this.field.name === "NPO109Doc") {
      this.fileService
        .getClassGroupDocument(this.field.value, queryParams.classid)
        .subscribe(blob => this.fileService.downloadPDF(blob, this.field.name));
    }
  }

  openUploadModal() {
    if (this.field.value) {
      const dRef = this.dialog.open(ModalComponent, {
        width: "45%",
        data: {
          message: "Качване на нов документ, ще замести предходния. Искате ли да продължите?",
          confirmBtnLbl: "Продължи",
          cancelBtnLbl: "Откажи"
        }
      });

      dRef.afterClosed().subscribe((res: boolean) => {
        if (res) {
          this.openHelperModal();
        }
      });
    } else {
      this.openHelperModal();
    }
  }

  private openHelperModal() {
    const dialogRef = this.dialog.open(UploadComponent, {
      width: "29%",
      panelClass: "l-modal-custom"
    });

    dialogRef.afterClosed().subscribe((fileId: number) => {
      if (fileId) {
        this.field.value = fileId;
        this.group.get(this.field.name).setValue(fileId);
      }
    });
  }
}
