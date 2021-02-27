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
    private PortfolioComponent : PortfolioComponent,
    private toastr : ToastrService) { }

  ngOnInit(): void {    
    this.refreshData();
  }

  refreshData() {
    this.subs.add(
      this.portfolioService.getTradeAccounts(this.PortfolioComponent.portfolio.id).subscribe(res => {
        this.tradeAccounts = res as ITradeAccount[];
        console.log(res);
      })
    );
  }

  createTradeAccount() {
    this.subs.add(
      this.portfolioService.postTradeAccount(this.PortfolioComponent.portfolio.id).subscribe(res => {
        this.tradeAccounts = res as ITradeAccount[];
        this.refreshData();
        this.toastr.success('Created successfully', 'Trade Account');
      })
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
