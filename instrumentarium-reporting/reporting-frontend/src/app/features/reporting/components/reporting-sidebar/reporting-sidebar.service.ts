import { EventEmitter, Injectable, Output } from '@angular/core';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { SidebarToggleOption } from '@shared/models/sidebar-toggle-option.interface';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportingSidebarService {
  private _options = new BehaviorSubject<SidebarToggleOption>(null);
  private _visualization = new BehaviorSubject<VisualizationTypeEnum>(null);
  private _isSidebarOpen = new BehaviorSubject<boolean>(false);

  set options(response) {
    this._options.next(response);
  }

  get options() {
    return this._options.getValue();
  }

  set visualization(response) {
    this._visualization.next(response);
  }

  get visualization() {
    return this._visualization.getValue();
  }

  set isSidebarOpen(response) {
    this._isSidebarOpen.next(response);
  }

  get isSidebarOpen() {
    return this._isSidebarOpen.getValue();
  }

  @Output() change: EventEmitter<{
    isSidebarOpen: boolean;
    visualization: VisualizationTypeEnum;
    options: SidebarToggleOption;
  }> = new EventEmitter();

  constructor() {}

  toggle(options) {
    if (this.options?.value === options.value && this.isSidebarOpen) {
      this.isSidebarOpen = false;
      this.options = null;
    } else {
      this.isSidebarOpen = true;
      this.options = options;
    }

    this.change.emit({ isSidebarOpen: this.isSidebarOpen, visualization: this.visualization, options: this.options });
  }
}
