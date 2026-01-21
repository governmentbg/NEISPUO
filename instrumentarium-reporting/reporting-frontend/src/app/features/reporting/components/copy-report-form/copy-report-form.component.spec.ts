import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CopyReportFormComponent } from './copy-report-form.component';

describe('CopyReportFormComponent', () => {
  let component: CopyReportFormComponent;
  let fixture: ComponentFixture<CopyReportFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CopyReportFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CopyReportFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
