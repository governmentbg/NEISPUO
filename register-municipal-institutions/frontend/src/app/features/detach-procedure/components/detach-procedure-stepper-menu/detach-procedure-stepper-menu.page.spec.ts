import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetachProcedureStepperMenuPage } from './detach-procedure-stepper-menu.page';

describe('DetachProcedureStepperMenuPage', () => {
  let component: DetachProcedureStepperMenuPage;
  let fixture: ComponentFixture<DetachProcedureStepperMenuPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetachProcedureStepperMenuPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetachProcedureStepperMenuPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
