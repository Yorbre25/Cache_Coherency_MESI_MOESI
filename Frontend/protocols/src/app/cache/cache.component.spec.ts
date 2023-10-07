import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CacheComponent } from './cache.component';

describe('CacheComponent', () => {
  let component: CacheComponent;
  let fixture: ComponentFixture<CacheComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CacheComponent]
    });
    fixture = TestBed.createComponent(CacheComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
