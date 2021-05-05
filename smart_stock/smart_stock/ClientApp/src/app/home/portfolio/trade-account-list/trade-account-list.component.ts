import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IPortfolio, ITradeAccount, IUser } from 'src/app/interfaces';
import { LoginService } from 'src/app/services/login.service';
import { PortfolioService } from 'src/app/services/portfolio.service';
import { SubSink } from 'subsink';
import { PortfolioComponent } from '../portfolio.component';
import { Chart } from 'angular-highcharts';
import { DatePipe } from '@angular/common';
import { HistoryService } from 'src/app/services/history.service';

@Component({
  selector: 'app-trade-account-list',
  templateUrl: './trade-account-list.component.html',
  styleUrls: ['./trade-account-list.component.css']
})
export class TradeAccountListComponent implements OnInit, OnDestroy {
  private subs = new SubSink();
  public tradeAccounts : ITradeAccount[];
  public chart : any;
  public hourHeaders = new Array();
  public hourData = new Array();

  constructor(private readonly loginService : LoginService,
    private readonly portfolioService : PortfolioService,
    private readonly historyService : HistoryService,
    private portfolioComponent : PortfolioComponent,
    private datepipe : DatePipe) { }

  ngOnInit(): void {    
    this.refreshData();
  }

  refreshData() {
    this.subs.add(
      this.portfolioService.getTradeAccounts(this.portfolioComponent.portfolio.id).subscribe(res => {
        this.tradeAccounts = res as ITradeAccount[];
        res.forEach(e => {
          this.historyService.getHourData(e.id).subscribe(res => {
            res.forEach(element => {
              this.hourHeaders.push(this.datepipe.transform(new Date(element.date), 'HH:mm'));
              this.hourData.push(element.tradeAccountAmount);          
            });
            this.hour();
          })
        })
      })
    );
  }

  hour() {
    this.chart = new Chart({
      chart: {
        type: 'line'
      },
      title : null,
      credits: {
        enabled: false
      },
      legend : {
        enabled : false
      },
      yAxis : {
        title : null,
        labels : {
          enabled : false
        }
      },
      xAxis : {
        title : null,
        labels : {
          enabled : false
        },
        categories : this.hourHeaders
      },
      plotOptions : {
        line : {
          marker : {
            enabled : false
          }
        }
      },
      series: [
        {
          name: 'Equity',
          type: 'line',
          data: this.hourData
        },
      ]
    });
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
