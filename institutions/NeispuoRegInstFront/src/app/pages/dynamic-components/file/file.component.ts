import { Component, Input, OnInit } from "@angular/core";
import { FileService } from "../../../services/file.service";
import { environment } from "../../../../environments/environment";
import { Mode } from "src/app/enums/mode.enum";
import { features } from "process";
import { FormDataService } from "../../../services/form-data.service";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-file",
  templateUrl: "./file.component.html",
  styleUrls: ["./file.component.scss"]
})
export class FileComponent implements OnInit {
  @Input() fileLabel: string;
  @Input() fileId: number;
  @Input() mode: Mode;

  get modes() {
    return Mode;
  }

  constructor(private fileService: FileService, private formDataService: FormDataService, private route: ActivatedRoute) {}

  ngOnInit() {}

  download() {
    const queryParams = environment.production
      ? this.formDataService.decodeParams(this.route.snapshot.queryParams.q || "")
      : this.route.snapshot.queryParams;

    this.fileService.getFile(queryParams.instid, this.fileId).subscribe(blob => {
      const name = this.fileLabel || "document";
      const file = new File([blob], `${name}.pdf`, { type: ".pdf" });
      const anchor = document.createElement("a");
      anchor.href = window.URL.createObjectURL(file);
      anchor.download = `${name}.pdf`;
      anchor.click();
    });
  }
}
