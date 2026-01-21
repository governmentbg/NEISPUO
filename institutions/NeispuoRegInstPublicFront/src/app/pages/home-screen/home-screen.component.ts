import { Component, OnInit } from "@angular/core";
import { Menu } from "../../enums/menu.enum";
import { KeepFiltersService } from "../../services/keep-filters.service";
import { MessagesService } from "../../services/messages.service";

@Component({
  selector: "app-home-screen",
  templateUrl: "./home-screen.component.html",
  styleUrls: ["./home-screen.component.scss"]
})
export class HomeScreenComponent implements OnInit {
  isLoading = false;

  get menu() {
    return Menu;
  }

  constructor(
    private msgService: MessagesService,
    private keepFiltersService: KeepFiltersService
  ) {}

  ngOnInit() {
    this.isLoading = true;
    if (!this.msgService.errorMessages) {
      this.msgService.getMessages();
    }
  }

  clear() {
    this.keepFiltersService.table = null;
    this.keepFiltersService.filters = null;
    this.keepFiltersService.keyWord = "";
    this.keepFiltersService.sortDirs = [];
    this.keepFiltersService.sortParams = [];
  }
}
