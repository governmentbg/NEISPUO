import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MergeProcedureStepperMenuPage } from './merge-procedure-stepper-menu.page';

describe('MergeProcedureStepperMenuPage', () => {
  let component: MergeProcedureStepperMenuPage;
  let fixture: ComponentFixture<MergeProcedureStepperMenuPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MergeProcedureStepperMenuPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MergeProcedureStepperMenuPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
