import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SingleMiChoiceComponent } from './single-mi-choice.component';

describe('SingleMiChoiceComponent', () => {
  let component: SingleMiChoiceComponent;
  let fixture: ComponentFixture<SingleMiChoiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SingleMiChoiceComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SingleMiChoiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
