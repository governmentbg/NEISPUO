import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { Mode } from "src/app/enums/mode.enum";
import { Table } from "src/app/models/table.interface";
import { FormDataService } from "src/app/services/form-data.service";
import { HelperService } from "src/app/services/helpers.service";
import { SnackbarService } from "src/app/services/snackbar.service";
import { environment } from "../../../../environments/environment";

@Component({
  selector: "app-multiple-add",
  templateUrl: "./multiple-add.component.html",
  styleUrls: ["./multiple-add.component.scss"]
})
export class MultipleAddComponent implements OnInit {
  table: Table;
  mode: Mode;
  isLoading: boolean = true;
  checkActive: boolean = false;
  checkedList: string[];
  labelList: string[] = [];
  parentGroup: FormGroup;

  constructor(
    private fb: FormBuilder,
    private formDataService: FormDataService,
    private helperService: HelperService,
    private dialogRef: MatDialogRef<MultipleAddComponent>,
    private route: ActivatedRoute,
    private snackbarService: SnackbarService,
    @Inject(MAT_DIALOG_DATA)
    private data: { table: Table; mode: Mode; checkedList: string[]; dependsOn: Object }
  ) {
    this.dialogRef.disableClose = true;
  }

  ngOnInit() {
    this.table = this.data.table;
    this.table.hasEditButton = false;
    this.mode = this.data.mode;
    this.checkedList = this.data.checkedList;
    this.parentGroup = this.fb.group({});

    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    this.formDataService.getMultiAddTableValues(this.table, { ...queryParams, ...this.data.dependsOn }).subscribe(
      res => {
        this.isLoading = false;

        if (this.table.multiAdd.listColumns) {
          for (const fieldVal of this.checkedList) {
            const fldName = this.table.multiAdd.comparisonColumn;
            const row = fldName
              ? this.table.values.find(row => {
                  const fldVal = row.fields.find(fld => fld.name === fldName).value;
                  return fldVal == fieldVal;
                })
              : this.table.values.find(row => row.id == fieldVal);

            if (row) {
              let label = "";
              for (const column of this.table.multiAdd.listColumns) {
                const field = row.fields.find(fld => fld.name === column);
                label += field.value + " ";
              }

              this.labelList.push(label);
            }
          }
        }
      },
      err => {
        this.isLoading = false;
        this.dialogRef.close(null);
      }
    );
  }

  closeDialog(save: boolean) {
    if (save) {
      this.dialogRef.close({ values: this.table.values, checkedList: this.checkedList });
    } else {
      this.dialogRef.close(null);
    }
  }

  onCheck(event: { checked: boolean; id: string; label: string }) {
    if (event.checked) {
      this.checkedList.push(event.id + "");
      this.table.multiAdd.listColumns && this.labelList.push(event.label);
    } else {
      const index = this.checkedList.findIndex(elem => elem == event.id);
      this.checkedList.splice(index, 1);
      this.table.multiAdd.listColumns && this.labelList.splice(index, 1);
    }
  }

  onAllChecked(event: { idList: string[]; labelList: string[] }) {
    this.checkedList = event.idList;
    this.labelList = event.labelList;
  }

  remove(index: number) {
    if (!this.table.multiAdd.checkDeleteProcedure) {
      this.checkedList.splice(index, 1);
      this.labelList.splice(index, 1);
    } else {
      this.isLoading = true;
      this.checkActive = true;
      const operationType = 13;

      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      let formValues: any = { ...queryParams };
      formValues.id = this.checkedList[index];
      formValues.forceOperation = 0;

      this.formDataService
        .submitForm({
          data: formValues,
          procedureName: this.table.multiAdd.checkDeleteProcedure,
          operationType
        })
        .subscribe((res: any) => {
          res && (res = JSON.parse(res));
          if ((res && res.length && res[0].OperationResultType === 2) || (res && res.OperationResultType === 2)) {
            this.formDataService.getMessages().subscribe(messages => {
              const index = res.MessageCode || res[0].MessageCode;
              this.isLoading = false;
              this.checkActive = false;
              this.snackbarService.openErrorSnackbar(messages[index]);
            });
          } else {
            this.checkedList.splice(index, 1);
            this.labelList.splice(index, 1);
            this.isLoading = false;
            this.checkActive = false;
          }
        });
    }
  }
}
