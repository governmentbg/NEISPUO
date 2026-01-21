import { Component, Input, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ModalComponent } from "src/app/shared/modal/modal.component";

@Component({
  selector: "app-help",
  templateUrl: "./help.component.html",
  styleUrls: ["./help.component.scss"]
})
export class HelpComponent implements OnInit {
  opened: boolean;

  @Input() helpText;

  constructor(private dialog: MatDialog) {}

  ngOnInit() {}

  openModal() {
    this.dialog.open(ModalComponent, { panelClass: "l-modal-help", data: { innerHtml: this.helpText, cancelBtnLbl: "Затвори" } });
  }
}
