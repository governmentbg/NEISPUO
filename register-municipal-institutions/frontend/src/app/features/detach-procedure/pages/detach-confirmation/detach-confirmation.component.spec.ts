import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetachConfirmationComponent } from './detach-confirmation.component';

describe('DivideProcedureConfirmationComponent', () => {
  let component: DetachConfirmationComponent;
  let fixture: ComponentFixture<DetachConfirmationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetachConfirmationComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetachConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
