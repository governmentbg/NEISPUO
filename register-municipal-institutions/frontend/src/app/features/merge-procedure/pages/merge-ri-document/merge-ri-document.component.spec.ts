import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MergeRiDocumentComponent } from './merge-ri-document.component';

describe('MergeRiDocumentComponent', () => {
  let component: MergeRiDocumentComponent;
  let fixture: ComponentFixture<MergeRiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MergeRiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MergeRiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
