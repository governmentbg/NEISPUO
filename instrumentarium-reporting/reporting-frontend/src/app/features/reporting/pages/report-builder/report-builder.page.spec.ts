import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportBuilderPage } from './report-builder.page';

describe('ReportBuilderPage', () => {
  let component: ReportBuilderPage;
  let fixture: ComponentFixture<ReportBuilderPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ReportBuilderPage]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportBuilderPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
