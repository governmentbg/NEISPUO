import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { responseType } from "src/app/enums/responseType";
import { HelperService } from "src/app/services/helpers.service";
import { ModalComponent } from "src/app/shared/modal/modal.component";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-list-view",
  templateUrl: "./list-view.component.html",
  styleUrls: ["./list-view.component.scss"]
})
export class ListViewComponent implements OnInit {
  checkTriggered: boolean = false;

  @Input() infoLs: { label: string; text: string; list: string; resultType?: responseType }[];

  @Output() onGetListView: EventEmitter<void> = new EventEmitter<void>();

  constructor(private dialog: MatDialog, private helperService: HelperService, private route: ActivatedRoute) {}

  get resultType() {
    return responseType;
  }

  ngOnInit() {
    this.route.queryParams.subscribe(queryParams => {
      const queryParamsDecoded = environment.production ? this.helperService.decodeParams(queryParams.q) : { ...queryParams };

      if (queryParamsDecoded.check || this.checkTriggered) {
        this.onCheck();
      }
    });
  }

  onCheck() {
    this.checkTriggered = true;

    this.onGetListView.emit();
  }

  openAdditionalInfo(infoList: string) {
    this.dialog.open(ModalComponent, {
      panelClass: "l-modal-help",
      maxWidth: "80%",
      minWidth: "35%",
      data: { innerHtml: infoList, cancelBtnLbl: "Затвори" }
    });
  }
}
