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
  public isEdit : boolean = false;

  constructor(private readonly portfolioComponent : PortfolioComponent) { }

  ngOnInit(): void {
    this.tradeAccount = this.portfolioComponent.tradeAccount;
    console.log(this.tradeAccount);
  }

  backToPortfolio() {
    this.portfolioComponent.content = 0;
  }

  editTa() {
    this.isEdit = true;
  }

}
