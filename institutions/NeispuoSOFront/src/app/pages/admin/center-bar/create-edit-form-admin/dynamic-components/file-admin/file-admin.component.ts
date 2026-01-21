import { Component, OnInit } from "@angular/core";
import { UpdateJsonService } from "src/app/services/update-json.service";
import { DynamicComponentAdmin } from "../../dynamic-field/dynamic-component";

@Component({
  selector: "app-file-admin",
  templateUrl: "./file-admin.component.html",
  styleUrls: ["./file-admin.component.scss"]
})
export class FileAdminComponent extends DynamicComponentAdmin implements OnInit {
  constructor(public jsonService: UpdateJsonService) {
    super(jsonService);
  }

  ngOnInit(): void {}
}
