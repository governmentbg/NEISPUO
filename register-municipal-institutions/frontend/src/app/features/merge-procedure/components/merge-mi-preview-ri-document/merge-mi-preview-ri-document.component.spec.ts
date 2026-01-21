import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MergeMiPreviewRiDocumentComponent } from './merge-mi-preview-ri-document.component';

describe('MergeMiPreviewRiDocumentComponent', () => {
  let component: MergeMiPreviewRiDocumentComponent;
  let fixture: ComponentFixture<MergeMiPreviewRiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MergeMiPreviewRiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MergeMiPreviewRiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
