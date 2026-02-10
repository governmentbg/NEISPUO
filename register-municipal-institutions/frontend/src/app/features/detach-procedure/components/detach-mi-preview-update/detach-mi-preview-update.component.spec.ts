import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetachMiPreviewUpdateComponent } from './detach-mi-preview-update.component';

describe('DetachMiPreviewUpdateComponent', () => {
  let component: DetachMiPreviewUpdateComponent;
  let fixture: ComponentFixture<DetachMiPreviewUpdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetachMiPreviewUpdateComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetachMiPreviewUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
