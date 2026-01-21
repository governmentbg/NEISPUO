import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetachMIModalPreviewComponent } from './detach-mi-modal-preview.component';

describe('DetachMIModalPreviewComponent', () => {
  let component: DetachMIModalPreviewComponent;
  let fixture: ComponentFixture<DetachMIModalPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetachMIModalPreviewComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetachMIModalPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
