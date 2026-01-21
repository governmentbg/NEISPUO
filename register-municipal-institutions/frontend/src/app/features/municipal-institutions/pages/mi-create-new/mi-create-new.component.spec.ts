import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiCreateNewComponent } from './mi-create-new.component';

describe('MiCreateNewComponent', () => {
  let component: MiCreateNewComponent;
  let fixture: ComponentFixture<MiCreateNewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MiCreateNewComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiCreateNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
