import { Component, OnInit } from '@angular/core';
import { ITradeAccount } from 'src/app/interfaces';
import { PortfolioComponent } from '../portfolio.component';

@Component({
  selector: 'app-trade-account',
  templateUrl: './trade-account.component.html',
  styleUrls: ['./trade-account.component.css']
})
export class TradeAccountComponent implements OnInit {
  public tradeAccount : ITradeAccount;

  constructor(private readonly portfolioComponent : PortfolioComponent) { }

  ngOnInit(): void {
    this.tradeAccount = this.portfolioComponent.tradeAccount;
  }

  backToPortfolio() {
    this.portfolioComponent.content = 0;
  }

}
