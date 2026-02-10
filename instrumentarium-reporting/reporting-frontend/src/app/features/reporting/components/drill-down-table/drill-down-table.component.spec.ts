import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrillDownTableComponent } from './drill-down-table.component';

describe('DrillDownTableComponent', () => {
  let component: DrillDownTableComponent;
  let fixture: ComponentFixture<DrillDownTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DrillDownTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DrillDownTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
