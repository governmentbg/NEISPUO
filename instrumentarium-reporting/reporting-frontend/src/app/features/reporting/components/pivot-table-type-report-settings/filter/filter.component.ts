import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ControlContainer } from '@angular/forms';
import { PrimengConfigService } from '@shared/services/primeng-config.service';
import { SelectItem } from 'primeng/api';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {
  form: any;
  @Input() value;
  @Input() type;
  @Output() removeAppliedFilter = new EventEmitter();

  textOptions: SelectItem[];
  numberOptions: SelectItem[];
  dateOptions: SelectItem[];

  constructor(private primengConfig: PrimengConfigService, private cc: ControlContainer) {}

  ngOnInit(): void {
    this.textOptions = this.primengConfig.getfilterMatchModeOptions().text.map((v) => this.valueToTranslatedLabelValue(v));
    this.numberOptions = this.primengConfig
      .getfilterMatchModeOptions()
      .numeric.map((v) => this.valueToTranslatedLabelValue(v));
    this.dateOptions = this.primengConfig.getfilterMatchModeOptions().date.map((v) => this.valueToTranslatedLabelValue(v));
    this.form = this.cc.control;
  }

  private valueToTranslatedLabelValue(v: string) {
    return { label: this.primengConfig.translate(v), value: v };
  }

  removeFilter() {
    this.removeAppliedFilter.emit();
  }
}
