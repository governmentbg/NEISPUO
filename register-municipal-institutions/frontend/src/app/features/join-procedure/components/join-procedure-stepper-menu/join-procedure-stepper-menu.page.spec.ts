import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinProcedureStepperMenuPage } from './join-procedure-stepper-menu.page';

describe('JoinProcedureStepperComponent', () => {
  let component: JoinProcedureStepperMenuPage;
  let fixture: ComponentFixture<JoinProcedureStepperMenuPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JoinProcedureStepperMenuPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JoinProcedureStepperMenuPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
