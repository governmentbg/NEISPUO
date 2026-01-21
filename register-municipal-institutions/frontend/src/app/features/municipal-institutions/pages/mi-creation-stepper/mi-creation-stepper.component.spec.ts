import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiCreationStepperComponent } from './mi-creation-stepper.component';

describe('MiCreationStepperComponent', () => {
  let component: MiCreationStepperComponent;
  let fixture: ComponentFixture<MiCreationStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MiCreationStepperComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiCreationStepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
