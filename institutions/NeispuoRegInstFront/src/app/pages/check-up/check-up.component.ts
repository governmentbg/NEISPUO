import { Component, OnInit } from "@angular/core";
import { Checkup, CheckupFileName } from "../../enums/check-up-names.enum";
import { FormDataService } from "../../services/form-data.service";
import * as XLSX from "xlsx";

@Component({
  selector: "app-check-up",
  templateUrl: "./check-up.component.html",
  styleUrls: ["./check-up.component.scss"]
})
export class CheckUpComponent implements OnInit {
  constructor(private formDataService: FormDataService) {}

  get checkup() {
    return Checkup;
  }

  ngOnInit() {}

  exportExcel(xlsName: string) {
    const fileName = CheckupFileName[Checkup[xlsName]] + ".xlsx";
    this.formDataService.getData(xlsName, {}).subscribe((res: any[]) => {
      const tableData = this.transformData(res, xlsName);
      const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(tableData);
      const wb: XLSX.WorkBook = XLSX.utils.book_new();
      ws["!cols"] = tableData[0].map((col, i) => ({
        wch: Math.max(...tableData.map(row => (row && row[i] ? row[i].toString().length : 0)))
      }));

      XLSX.utils.book_append_sheet(wb, ws, "Report");
      XLSX.writeFile(wb, fileName);
    });
  }

  private transformData(data: any[], xlsxName: string) {
    let fieldsOrder = [];

    switch (xlsxName) {
      case "reportStatActiveInstByTypeRegion":
        fieldsOrder = ["regName", "cntSchool", "cntKindergarden", "cntCPLR", "cntCSOP", "cntSOZ"];
        break;
      case "reportStatProcTypeByRegion":
        fieldsOrder = [
          "regName",
          "schoolNew",
          "schoolChange",
          "schoolArchived",
          "kindergardenNew",
          "kindergardenChange",
          "kindergardenArchived",
          "cplrNew",
          "cplrChange",
          "cplrArchived",
          "csopNew",
          "csopChange",
          "csopArchived",
          "sozNew",
          "sozChange",
          "sozArchived"
        ];
        break;
      case "reportProcTypeByRegion":
        fieldsOrder = ["regName", "munName", "townName", "institutionID", "institutionName", "institutionType", "transformType"];
        break;
      case "reportActiveInstitutions":
        fieldsOrder = [
          "regName",
          "munName",
          "townName",
          "institutionID",
          "institutionName",
          "institutionType",
          "institutionAddress",
          "headMaster"
        ];
        break;
    }

    const tableData = [];

    for (let row of data) {
      const tableRow = [];
      for (let i = 0; i < fieldsOrder.length; i++) {
        tableRow.push(row[fieldsOrder[i]]);
      }
      tableData.push(tableRow);
    }

    return tableData;
  }
}
