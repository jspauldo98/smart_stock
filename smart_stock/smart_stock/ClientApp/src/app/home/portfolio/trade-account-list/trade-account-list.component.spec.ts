import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeAccountListComponent } from './trade-account-list.component';

describe('TradeAccountListComponent', () => {
  let component: TradeAccountListComponent;
  let fixture: ComponentFixture<TradeAccountListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TradeAccountListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeAccountListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
