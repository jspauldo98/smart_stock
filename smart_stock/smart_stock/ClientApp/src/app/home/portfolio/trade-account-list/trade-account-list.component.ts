import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IPortfolio, ITradeAccount, IUser } from 'src/app/interfaces';
import { LoginService } from 'src/app/services/login.service';
import { PortfolioService } from 'src/app/services/portfolio.service';
import { SubSink } from 'subsink';
import { PortfolioComponent } from '../portfolio.component';

@Component({
  selector: 'app-trade-account-list',
  templateUrl: './trade-account-list.component.html',
  styleUrls: ['./trade-account-list.component.css']
})
export class TradeAccountListComponent implements OnInit, OnDestroy {
  private subs = new SubSink();
  public tradeAccounts : ITradeAccount[];

  constructor(private readonly loginService : LoginService,
    private readonly portfolioService : PortfolioService,
    private portfolioComponent : PortfolioComponent,
    private toastr : ToastrService) { }

  ngOnInit(): void {    
    this.refreshData();
  }

  refreshData() {
    this.subs.add(
      this.portfolioService.getTradeAccounts(this.portfolioComponent.portfolio.id).subscribe(res => {
        this.tradeAccounts = res as ITradeAccount[];
        console.log(res);
      })
    );
  }

  createTradeAccount() {
    this.portfolioComponent.content = 2;
  }

  viewTa(ta : ITradeAccount) {
    this.portfolioComponent.content = 1;
    this.portfolioComponent.tradeAccount = ta;
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
