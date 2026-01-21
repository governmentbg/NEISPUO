import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableTypeReportComponent } from './table-type-report.component';

describe('TableTypeReportComponent', () => {
  let component: TableTypeReportComponent;
  let fixture: ComponentFixture<TableTypeReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableTypeReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TableTypeReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
