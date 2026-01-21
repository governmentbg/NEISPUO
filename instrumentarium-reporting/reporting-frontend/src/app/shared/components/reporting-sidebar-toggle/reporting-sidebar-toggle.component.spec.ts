import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportingSidebarToggleComponent } from './reporting-sidebar-toggle.component';

describe('ReportingSidebarToggleComponent', () => {
  let component: ReportingSidebarToggleComponent;
  let fixture: ComponentFixture<ReportingSidebarToggleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportingSidebarToggleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportingSidebarToggleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
