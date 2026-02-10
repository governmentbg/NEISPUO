import { Component, OnInit } from "@angular/core";
import { UpdateJsonService } from "src/app/services/update-json.service";
import { DynamicComponentAdmin } from "../../dynamic-field/dynamic-component";

@Component({
  selector: "app-select-admin",
  templateUrl: "./select-admin.component.html",
  styleUrls: ["./select-admin.component.scss"]
})
export class SelectAdminComponent extends DynamicComponentAdmin implements OnInit {
  appendClass: string;

  constructor(public jsonService: UpdateJsonService) {
    super(jsonService);
  }

  ngOnInit() {
    this.appendClass = ".l-container-card";
  }
}
