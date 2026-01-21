import { Component, Input, OnInit } from "@angular/core";
import { FileService } from "../../../services/file.service";
import { Mode } from "src/app/enums/mode.enum";
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
  @Input() procId: number = null;

  get modes() {
    return Mode;
  }

  constructor(private fileService: FileService, private route: ActivatedRoute) {}

  ngOnInit() {}

  download() {
    this.fileService.getFile(this.fileId, this.route.snapshot.params.id, this.procId || this.route.snapshot.queryParams.procID).subscribe(blob => {
      const name = this.fileLabel || "document";
      const file = new File([blob], `${name}.pdf`, { type: ".pdf" });
      const anchor = document.createElement("a");
      anchor.href = window.URL.createObjectURL(file);
      anchor.download = `${name}.pdf`;
      anchor.click();
    });
  }
}
