import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetachMIToUpdateComponent } from './detach-mi-to-update.component';

describe('DetachMIToUpdateComponent', () => {
  let component: DetachMIToUpdateComponent;
  let fixture: ComponentFixture<DetachMIToUpdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetachMIToUpdateComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetachMIToUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
