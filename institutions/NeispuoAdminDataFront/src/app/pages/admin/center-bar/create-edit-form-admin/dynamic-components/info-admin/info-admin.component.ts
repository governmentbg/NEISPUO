import { Component, OnInit } from "@angular/core";
import { UpdateJsonService } from "src/app/services/update-json.service";
import { DynamicComponentAdmin } from "../../dynamic-field/dynamic-component";

@Component({
  selector: "app-info-admin",
  templateUrl: "./info-admin.component.html",
  styleUrls: ["./info-admin.component.scss"]
})
export class InfoAdminComponent extends DynamicComponentAdmin implements OnInit {
  constructor(public jsonService: UpdateJsonService) {
    super(jsonService);
  }

  ngOnInit(): void {}
}
