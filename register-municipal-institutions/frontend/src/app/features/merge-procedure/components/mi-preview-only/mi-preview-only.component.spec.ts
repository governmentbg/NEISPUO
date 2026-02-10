import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MIPreviewOnlyComponent } from './mi-preview-only.component';

describe('MiPreviewExistingComponent', () => {
  let component: MIPreviewOnlyComponent;
  let fixture: ComponentFixture<MIPreviewOnlyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MIPreviewOnlyComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MIPreviewOnlyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
