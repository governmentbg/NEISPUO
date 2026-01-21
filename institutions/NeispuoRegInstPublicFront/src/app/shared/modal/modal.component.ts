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
    @Inject(MAT_DIALOG_DATA) private data: { message: string }
  ) {
    this.dialogRef.disableClose = true;
  }

  message: string;

  ngOnInit() {
    this.message = this.data.message;
  }

  closeDialog(choice: boolean) {
    this.dialogRef.close(choice);
  }
}
