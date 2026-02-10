import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DivideProcedureStepperMenuPage } from './divide-procedure-stepper-menu.page';

describe('DivideProcedureStepperMenuPage', () => {
  let component: DivideProcedureStepperMenuPage;
  let fixture: ComponentFixture<DivideProcedureStepperMenuPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DivideProcedureStepperMenuPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DivideProcedureStepperMenuPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
