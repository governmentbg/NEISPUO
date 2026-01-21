import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { FormTypeInt } from "../../../enums/formType.enum";
import { Menu } from "../../../enums/menu.enum";
import { Subsection } from "../../../models/subsection.interface";
import { DocumentsModalComponent } from "../../documents-modal/documents-modal.component";
import { environment } from "../../../../environments/environment";
import { FormDataService } from "src/app/services/form-data.service";
import { AuthService } from "src/app/auth/auth.service";

@Component({
  selector: "app-control-component",
  templateUrl: "./control-component.component.html",
  styleUrls: ["./control-component.component.scss"]
})
export class ControlComponentComponent implements OnInit {
  @Input() historyMode = false;
  @Input() path: string;
  @Input() dataName: string;
  @Input() instKind: string;
  @Input() certificateSubsection: Subsection = null;
  @Input() fg: FormGroup;
  @Input() instid: string | number;
  @Input() procID: string | number;
  @Input() permissions;
  @Input() canBeAnnulled: boolean = true;
  @Input() canBeRenewed: boolean = false;
  @Input() hideCertificateBtn: boolean = false;

  @Output() isHistoryActive: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() certificateChange: EventEmitter<any> = new EventEmitter<any>();

  showButtons = true;

  get menu() {
    return Menu;
  }

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private formDataService: FormDataService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.showButtons = this.permissions.includes(+this.instKind);
  }

  historyChanged(active: boolean) {
    this.historyMode = active;
    this.isHistoryActive.emit(active);
  }

  navigate(statusCode, isCorrection, operationType) {
    const params = environment.production
      ? this.formDataService.decodeParams(this.route.snapshot.queryParams["q"] || "")
      : this.route.snapshot.queryParams;

    const formName = this.route.snapshot.params["formName"];
    const instType = FormTypeInt[this.route.snapshot.params["type"]] + 1;

    let queryParams: any = {
      instid: params.instid,
      procType: statusCode,
      isCorrection,
      operationType,
      instKind: this.instKind,
      procID: params.procID,
      sysuserid: this.authService.getSysUserId(),
      region: this.authService.getRegion(),
      instType
    };

    environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

    this.router.navigate([`/${Menu.CreateProcedure}/${formName}`], { queryParams });
  }

  openModal() {
    const dialogRef = this.dialog.open(DocumentsModalComponent, {
      width: "65%",
      panelClass: "l-modal-custom",
      data: {
        subsection: {
          certificateId: this.certificateSubsection.certificateId,
          certificateLabel: this.certificateSubsection.certificateLabel,
          copyCertificateId: this.certificateSubsection.copyCertificateId,
          copyCertificateLabel: this.certificateSubsection.copyCertificateLabel
        },
        formGroup: this.fg,
        instid: this.instid,
        procID: this.procID
      }
    });

    dialogRef.afterClosed().subscribe((values: any) => {
      this.certificateChange.emit(values);
    });
  }
}
