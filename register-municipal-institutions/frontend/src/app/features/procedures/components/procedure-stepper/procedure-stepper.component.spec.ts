import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcedureStepperComponent } from './procedure-stepper.component';

describe('ProcedureStepperComponent', () => {
  let component: ProcedureStepperComponent;
  let fixture: ComponentFixture<ProcedureStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProcedureStepperComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcedureStepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
