import { Component, Inject, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-modal",
  templateUrl: "./modal.component.html",
  styleUrls: ["./modal.component.scss"]
})
export class ModalComponent implements OnInit {
  constructor(
    private dialogRef: MatDialogRef<ModalComponent>,
    @Inject(MAT_DIALOG_DATA)
    private data: {
      message: string;
      confirmBtnLbl: string;
      cancelBtnLbl: string;
      innerHtml?: any;
      hasLocalServerLink?: boolean;
    }
  ) {
    this.dialogRef.disableClose = true;
  }

  message: string;
  confirmButtonLabel: string;
  cancelButtonLabel: string;
  helpText: any;
  hasLocalServerLink: boolean;

  ngOnInit() {
    this.message = this.data.message;
    this.confirmButtonLabel = this.data.confirmBtnLbl;
    this.cancelButtonLabel = this.data.cancelBtnLbl;
    this.helpText = this.data.innerHtml;
    this.hasLocalServerLink = this.data.hasLocalServerLink;
  }

  closeDialog(choice: boolean) {
    this.dialogRef.close(choice);
  }
}
