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
  title = '\'s History';
  type = 'LineChart';
  data = [
     ["Jan",  7.0, -0.2, -0.9, 3.9],
     ["Feb",  6.9, 0.8, 0.6, 4.2],
     ["Mar",  9.5,  5.7, 3.5, 5.7],
     ["Apr",  14.5, 11.3, 8.4, 8.5],
     ["May",  18.2, 17.0, 13.5, 11.9],
     ["Jun",  21.5, 22.0, 17.0, 15.2],
     ["Jul",  25.2, 24.8, 18.6, 17.0],
     ["Aug",  26.5, 24.1, 17.9, 16.6],
     ["Sep",  23.3, 20.1, 14.3, 14.2],
     ["Oct",  18.3, 14.1, 9.0, 10.3],
     ["Nov",  13.9,  8.6, 3.9, 6.6],
     ["Dec",  9.6,  2.5,  1.0, 4.8]
  ];
  columnNames = ["Month", "Tokyo", "New York","Berlin", "Paris"];
  options = {   
     hAxis: {
        title: 'Month'
     },
     vAxis:{
        title: 'Temperature'
     },
  };
  width = 400;
  height = 200;
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

  transfer() {
    this.portfolioComponent.content = 3;
  }

  viewTa(ta : ITradeAccount) {
    this.portfolioComponent.content = 1;
    this.portfolioComponent.tradeAccount = ta;
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
