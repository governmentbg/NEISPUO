import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RmtSelectionComponent } from './rmt-selection.component';

describe('RmtSelectionComponent', () => {
  let component: RmtSelectionComponent;
  let fixture: ComponentFixture<RmtSelectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RmtSelectionComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RmtSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
