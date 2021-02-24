import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IPortfolio, ITradeAccount, IUser } from 'src/app/interfaces';
import { LoginService } from 'src/app/services/login.service';
import { PortfolioService } from 'src/app/services/portfolio.service';
import { PortfolioComponent } from '../portfolio.component';

@Component({
  selector: 'app-trade-account',
  templateUrl: './trade-account.component.html',
  styleUrls: ['./trade-account.component.css']
})
export class TradeAccountComponent implements OnInit {
  tradeAccounts : ITradeAccount[];

  constructor(private readonly loginService : LoginService,
    private readonly portfolioService : PortfolioService,
    private PortfolioComponent : PortfolioComponent,
    private toastr : ToastrService) { }

  ngOnInit(): void {    
    this.refreshData();
  }

  refreshData() {
    this.portfolioService.getTradeAccounts(this.PortfolioComponent.portfolio.id).subscribe(res => {
      this.tradeAccounts = res as ITradeAccount[];
      console.log(res);
    })
  }

  createTradeAccount() {
    this.portfolioService.postTradeAccount(this.PortfolioComponent.portfolio.id).subscribe(res => {
      this.tradeAccounts = res as ITradeAccount[];
      this.refreshData();
      this.toastr.success('Created successfully', 'Trade Account');
    });
  }
}
