import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MergeMiPreviewDeleteComponent } from './merge-mi-preview-delete.component';

describe('MiPreviewExistingComponent', () => {
  let component: MergeMiPreviewDeleteComponent;
  let fixture: ComponentFixture<MergeMiPreviewDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MergeMiPreviewDeleteComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MergeMiPreviewDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
