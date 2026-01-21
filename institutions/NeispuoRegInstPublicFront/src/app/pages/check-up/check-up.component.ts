import { Component, OnInit } from "@angular/core";
import { Checkup, CheckupFileName } from "../../enums/check-up-names.enum";
import { FileService } from "../../services/file.service";

@Component({
  selector: "app-check-up",
  templateUrl: "./check-up.component.html",
  styleUrls: ["./check-up.component.scss"]
})
export class CheckUpComponent implements OnInit {
  constructor(private fileService: FileService) {}

  get checkup() {
    return Checkup;
  }

  ngOnInit() {}

  openSnackbar(xlsName: string) {
    this.fileService.getXlsx(xlsName).subscribe(blob => {
      const file = new File([blob], `${CheckupFileName[Checkup[xlsName]]}.xlsx`, { type: ".xlsx" });
      const anchor = document.createElement("a");
      anchor.href = window.URL.createObjectURL(file);
      anchor.download = `${CheckupFileName[Checkup[xlsName]]}.xlsx`;
      anchor.click();
    });
  }
}
