import { Component, Inject, OnDestroy, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { FileUploadControl, FileUploadValidators } from "@iplab/ngx-file-upload";
import { forkJoin, Subscription } from "rxjs";
import { Mode } from "../../enums/mode.enum";
import { FileService } from "../../services/file.service";
import { MessagesService } from "../../services/messages.service";
import { SnackbarService } from "../../services/snackbar.service";
import * as html2pdf from "html2pdf.js";

@Component({
  selector: "app-documents-modal",
  templateUrl: "./documents-modal.component.html",
  styleUrls: ["./documents-modal.component.scss"]
})
export class DocumentsModalComponent implements OnInit, OnDestroy {
  fileUploadControl: FileUploadControl;
  isLoading: boolean = false;
  text: string;
  uploadCopy = false;

  private fileUploadSubscription: Subscription;
  private initialCertificateId: number;
  private initialCopyCertificateId: number;

  constructor(
    private snackbarService: SnackbarService,
    private msgService: MessagesService,
    private fileService: FileService,
    private dialogRef: MatDialogRef<DocumentsModalComponent>,
    @Inject(MAT_DIALOG_DATA)
    private data: {
      subsection: {
        certificateId: number;
        certificateLabel: string;
        copyCertificateId: number;
        copyCertificateLabel: string;
      };
      formGroup: FormGroup;
      instid: string | number;
      procID: string | number;
    }
  ) {
    this.dialogRef.disableClose = true;
  }

  get mode() {
    return Mode;
  }

  subsection: {
    certificateId: number;
    certificateLabel: string;
    copyCertificateId: number;
    copyCertificateLabel: string;
  };

  ngOnInit() {
    this.subsection = this.data.subsection;
    this.initialCertificateId = this.subsection.certificateId;
    this.initialCopyCertificateId = this.subsection.copyCertificateId;

    this.fileUploadControl = new FileUploadControl([
      FileUploadValidators.accept([".pdf"]),
      FileUploadValidators.fileSize(10 * 1024 * 1024) //this is 10MB
    ]);

    this.initFileUpload();
  }

  closeDialog() {
    if (
      this.initialCertificateId !== this.subsection.certificateId ||
      this.initialCopyCertificateId !== this.subsection.copyCertificateId
    ) {
      this.dialogRef.close(this.subsection);
    } else {
      this.dialogRef.close();
    }
  }

  initFileUpload() {
    this.text = this.msgService.fileMessages.fileDragDropText;

    this.fileUploadSubscription = this.fileUploadControl.valueChanges.subscribe((files: File[]) => {
      if (files.length > 0) {
        const validationErrors = this.fileUploadControl.getError();
        if (validationErrors.length > 0) {
          this.manageFileErrors(validationErrors[0]);
        } else {
          const fileToUpload = files[0];
          this.isLoading = true;
          this.fileService.uploadFile(fileToUpload).subscribe(
            (res: { name: string; size: number; blobId: number; location: string }) => {
              this.fileUploadControl.clear();
              this.uploadCopy = false;
              this.fileService
                .saveCertificateData({
                  data: {
                    procID: this.data.procID,
                    blobID: res.blobId
                  },
                  procedureName: "reginst_basic.documentSave",
                  operationType: 6
                })
                .subscribe(
                  result => {
                    this.subsection.copyCertificateId = res.blobId;
                    this.subsection.copyCertificateLabel = `udost_${this.data.instid}_${res.blobId}.pdf`;
                    this.isLoading = false;
                  },
                  err => (this.isLoading = false)
                );
            },
            err => {
              this.fileUploadControl.clear();
              this.snackbarService.openErrorSnackbar(this.msgService.fileMessages.fileUploadErr);
              this.isLoading = false;
            }
          );
        }
      }
    });
  }

  ngOnDestroy() {
    this.fileUploadSubscription && this.fileUploadSubscription.unsubscribe();
  }

  getCertificate() {
    this.isLoading = true;

    this.fileService
      .saveCertificateData({
        data: { procID: this.data.procID },
        procedureName: "reginst_basic.documentSave",
        operationType: 1
      })
      .subscribe(
        () => {
          forkJoin([
            this.fileService.getCertificateData({ instid: this.data.instid, procID: this.data.procID }),
            this.fileService.getCertificateTemplate()
          ]).subscribe(
            ([certificateData, blob]: [any, Blob]) => {
              this.generateCertificate(certificateData[0], blob);
            },
            err => (this.isLoading = false)
          );
        },
        err => (this.isLoading = false)
      );
  }

  private manageFileErrors(error: any) {
    if (error.fileSize) {
      this.snackbarService.openErrorSnackbar(this.msgService.fileMessages.fileSize);
    } else if (error.fileTypes) {
      this.snackbarService.openErrorSnackbar(this.msgService.fileMessages.fileType);
    } else {
      this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.error);
    }
    this.fileUploadControl.setValue([]);
  }

  private generateCertificate(certificateData, blob) {
    let fileReader: FileReader = new FileReader();
    fileReader.onloadend = async () => {
      let htmlTemplate = fileReader.result;

      htmlTemplate = this.fileService.substituteTemplateInfo(<string>htmlTemplate, certificateData);

      const opt = {
        filename: `certificate.pdf`,
        image: { type: "jpeg", quality: 1 },
        html2canvas: { scale: 2 }
      };

      const blob = await html2pdf().from(htmlTemplate).set(opt).toPdf().get("pdf").output("blob");

      const file = new File([blob], `certificate.pdf`, { type: "application/pdf" });
      this.uploadFile(file);
    };

    fileReader.readAsText(blob);
  }

  private uploadFile(file: File) {
    this.fileService.uploadFile(file).subscribe(
      (res: { name: string; size: number; blobId: number; location: string }) => {
        this.fileService
          .saveCertificateData({
            data: {
              procID: this.data.procID,
              blobID: res.blobId
            },
            procedureName: "reginst_basic.documentSave",
            operationType: 5
          })
          .subscribe(
            result => {
              this.subsection.certificateId = res.blobId;
              this.subsection.certificateLabel = `udost_${this.data.instid}_${res.blobId}.pdf`;
              this.isLoading = false;
            },
            err => (this.isLoading = false)
          );
      },
      err => {
        this.snackbarService.openErrorSnackbar(this.msgService.fileMessages.certificateUploadErr);
        this.isLoading = false;
      }
    );
  }
}
