import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcedureStepperMenuPage } from './procedure-stepper-menu.page';

describe('DivideProcedureStepperComponent', () => {
  let component: ProcedureStepperMenuPage;
  let fixture: ComponentFixture<ProcedureStepperMenuPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProcedureStepperMenuPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcedureStepperMenuPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
