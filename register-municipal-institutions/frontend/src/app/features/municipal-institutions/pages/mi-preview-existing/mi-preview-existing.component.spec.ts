import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiPreviewExistingComponent } from './mi-preview-existing.component';

describe('MiPreviewExistingComponent', () => {
  let component: MiPreviewExistingComponent;
  let fixture: ComponentFixture<MiPreviewExistingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MiPreviewExistingComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiPreviewExistingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
