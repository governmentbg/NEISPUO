import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaveReportButtonComponent } from './save-report-button.component';

describe('SaveReportButtonComponent', () => {
  let component: SaveReportButtonComponent;
  let fixture: ComponentFixture<SaveReportButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SaveReportButtonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SaveReportButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
