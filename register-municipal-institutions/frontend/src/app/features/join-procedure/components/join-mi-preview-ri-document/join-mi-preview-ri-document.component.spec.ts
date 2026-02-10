import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinMiPreviewRiDocumentComponent } from './join-mi-preview-ri-document.component';

describe('JoinMiPreviewRiDocumentComponent', () => {
  let component: JoinMiPreviewRiDocumentComponent;
  let fixture: ComponentFixture<JoinMiPreviewRiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JoinMiPreviewRiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JoinMiPreviewRiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
