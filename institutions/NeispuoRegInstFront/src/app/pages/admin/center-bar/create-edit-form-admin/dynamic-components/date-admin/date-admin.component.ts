import { Component, OnInit } from "@angular/core";
import { UpdateJsonService } from "src/app/services/update-json.service";
import { DynamicComponentAdmin } from "../../dynamic-field/dynamic-component";

@Component({
  selector: "app-date-admin",
  templateUrl: "./date-admin.component.html",
  styleUrls: ["./date-admin.component.scss"]
})
export class DateAdminComponent extends DynamicComponentAdmin implements OnInit {
  constructor(public jsonService: UpdateJsonService) {
    super(jsonService);
  }

  ngOnInit(): void {}
}
