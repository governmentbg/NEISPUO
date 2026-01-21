import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportingLayoutPage } from './reporting-layout.page';

describe('ReportingLayoutPage', () => {
  let component: ReportingLayoutPage;
  let fixture: ComponentFixture<ReportingLayoutPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportingLayoutPage ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportingLayoutPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
