import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportBuilderContentHeaderComponent } from './report-builder-content-header.component';

describe('ReportBuilderContentHeaderComponent', () => {
  let component: ReportBuilderContentHeaderComponent;
  let fixture: ComponentFixture<ReportBuilderContentHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportBuilderContentHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportBuilderContentHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
