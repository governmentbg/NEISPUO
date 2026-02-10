import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DivideMIModalPreviewComponent } from './divide-mi-modal-preview.component';

describe('DivideMIModalPreviewComponent', () => {
  let component: DivideMIModalPreviewComponent;
  let fixture: ComponentFixture<DivideMIModalPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DivideMIModalPreviewComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DivideMIModalPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
