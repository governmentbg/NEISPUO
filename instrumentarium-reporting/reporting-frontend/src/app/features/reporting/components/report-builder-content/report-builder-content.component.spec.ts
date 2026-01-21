import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportBuilderContentComponent } from './report-builder-content.component';

describe('ReportBuilderContentComponent', () => {
  let component: ReportBuilderContentComponent;
  let fixture: ComponentFixture<ReportBuilderContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportBuilderContentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportBuilderContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
