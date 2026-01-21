import { Component, OnDestroy, OnInit } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";
import { FileUploadControl, FileUploadValidators } from "@iplab/ngx-file-upload";
import { Subscription } from "rxjs";
import { MessagesService } from "../../services/messages.service";
import { SnackbarService } from "../../services/snackbar.service";
import { FileService } from "../../services/file.service";

@Component({
  selector: "app-upload",
  templateUrl: "./upload.component.html",
  styleUrls: ["./upload.component.scss"]
})
export class UploadComponent implements OnInit, OnDestroy {
  fileUploadControl: FileUploadControl;
  isLoading: boolean = false;
  text: string;
  fileToUpload: File;

  private fileUploadSubscription: Subscription;

  constructor(
    private snackbarService: SnackbarService,
    private msgService: MessagesService,
    private fileService: FileService,
    private dialogRef: MatDialogRef<UploadComponent>
  ) {}

  ngOnInit() {
    this.fileUploadControl = new FileUploadControl([
      FileUploadValidators.sizeRange({ minSize: 10, maxSize: 10 * 1024 * 1024 }), //this is 10MB
      FileUploadValidators.accept([".pdf"])
    ]);

    this.initFileUpload();
  }

  initFileUpload() {
    this.text = this.msgService.fileMessages.fileDragDropText;

    this.fileUploadSubscription = this.fileUploadControl.valueChanges.subscribe(
      (files: File[]) => {
        if (files.length > 0) {
          const validationErrors = this.fileUploadControl.getError();
          if (validationErrors.length > 0) {
            this.manageFileErrors(validationErrors[0]);
          } else {
            this.fileToUpload = files[0];
          }
        } else {
          this.fileToUpload = null;
        }
      },
      err => (this.isLoading = false)
    );
  }

  ngOnDestroy() {
    this.fileUploadSubscription && this.fileUploadSubscription.unsubscribe();
  }

  close(saveInfo: boolean) {
    if (saveInfo) {
      this.fileService.uploadFile(this.fileToUpload).subscribe(
        (res: { name: string; size: number; blobId: number; location: string }) => {
          this.dialogRef.close(res.blobId);
        },
        err => this.snackbarService.openErrorSnackbar(this.msgService.fileMessages.fileUploadErr)
      );
    } else {
      this.dialogRef.close(saveInfo);
    }
  }

  private manageFileErrors(error: any) {
    if (error.sizeRange) {
      this.snackbarService.openErrorSnackbar(this.msgService.fileMessages.fileSize);
    } else if (error.fileTypes) {
      this.snackbarService.openErrorSnackbar(this.msgService.fileMessages.fileType);
    } else {
      this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.error);
    }
    this.fileUploadControl.setValue([]);
  }
}
