import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DivideMiPreviewRiDocumentComponent } from './divide-mi-preview-ri-document.component';

describe('DivideMiPreviewRiProcedureComponent', () => {
  let component: DivideMiPreviewRiDocumentComponent;
  let fixture: ComponentFixture<DivideMiPreviewRiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DivideMiPreviewRiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DivideMiPreviewRiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
