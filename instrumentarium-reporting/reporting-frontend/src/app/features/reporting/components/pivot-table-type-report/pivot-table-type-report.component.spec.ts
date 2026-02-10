import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PivotTableTypeReportComponent } from './pivot-table-type-report.component';

describe('PivotTableTypeReportComponent', () => {
  let component: PivotTableTypeReportComponent;
  let fixture: ComponentFixture<PivotTableTypeReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PivotTableTypeReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PivotTableTypeReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
