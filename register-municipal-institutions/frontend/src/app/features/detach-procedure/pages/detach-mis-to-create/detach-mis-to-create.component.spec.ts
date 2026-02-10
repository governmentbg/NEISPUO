import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetachMIsToCreateComponent } from './detach-mis-to-create.component';

describe('DetachMIsToCreateComponent', () => {
  let component: DetachMIsToCreateComponent;
  let fixture: ComponentFixture<DetachMIsToCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetachMIsToCreateComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetachMIsToCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
