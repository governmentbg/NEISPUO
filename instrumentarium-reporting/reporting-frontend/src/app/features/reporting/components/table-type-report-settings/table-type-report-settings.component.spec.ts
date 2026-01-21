import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableTypeReportSettingsComponent } from './table-type-report-settings.component';

describe('TableTypeReportSettingsComponent', () => {
  let component: TableTypeReportSettingsComponent;
  let fixture: ComponentFixture<TableTypeReportSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableTypeReportSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TableTypeReportSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
