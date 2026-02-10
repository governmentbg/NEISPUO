import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiBulstatLoaderComponent } from './mi-bulstat-loader.component';

describe('MiBulstatLoaderComponent', () => {
  let component: MiBulstatLoaderComponent;
  let fixture: ComponentFixture<MiBulstatLoaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MiBulstatLoaderComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiBulstatLoaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
