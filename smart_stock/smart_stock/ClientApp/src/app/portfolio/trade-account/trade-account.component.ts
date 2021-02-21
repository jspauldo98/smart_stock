import { Component, OnInit } from '@angular/core';
import { ITradeAccount } from 'src/app/interfaces';
import { PortfolioService } from 'src/app/services/portfolio.service';

@Component({
  selector: 'app-trade-account',
  templateUrl: './trade-account.component.html',
  styleUrls: ['./trade-account.component.css']
})
export class TradeAccountComponent implements OnInit {

  constructor(public portfolioService : PortfolioService) { }
  public test : ITradeAccount[];

  ngOnInit(): void {
    this.portfolioService.getTradeAccounts();
  }
}
