import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetachMiPreviewRiDocumentComponent } from './detach-mi-preview-ri-document.component';

describe('DetachMiPreviewRiDocumentComponent', () => {
  let component: DetachMiPreviewRiDocumentComponent;
  let fixture: ComponentFixture<DetachMiPreviewRiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetachMiPreviewRiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetachMiPreviewRiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
