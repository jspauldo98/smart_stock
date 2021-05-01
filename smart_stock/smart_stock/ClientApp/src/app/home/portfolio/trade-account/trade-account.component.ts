import { Component, OnInit } from '@angular/core';
import { ILog, ITradeAccount } from 'src/app/interfaces';
import { HistoryService } from 'src/app/services/history.service';
import { SubSink } from 'subsink';
import { PortfolioComponent } from '../portfolio.component';
import { Chart } from 'angular-highcharts';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-trade-account',
  templateUrl: './trade-account.component.html',
  styleUrls: ['./trade-account.component.css']
})
export class TradeAccountComponent implements OnInit {
  private subs = new SubSink();
  public tradeAccount : ITradeAccount;
  public isEdit : boolean = false;
  public log : ILog[];
  public chart : any;
  public minuteHeaders = new Array();
  public minuteData = new Array();
  public hourHeaders = new Array();
  public hourData = new Array();
  public dayHeaders = new Array();
  public dayData = new Array();
  public weekHeaders = new Array();
  public weekData = new Array();
  public monthHeaders = new Array();
  public monthData = new Array();
  public yearHeaders = new Array();
  public yearData = new Array();

  constructor(private readonly portfolioComponent : PortfolioComponent,
    private readonly historyService : HistoryService,
    private datepipe : DatePipe) { }

  ngOnInit(): void {
    this.tradeAccount = this.portfolioComponent.tradeAccount;
    this.subs.add(
      this.historyService.getLog(this.tradeAccount.id).subscribe(res => {
        this.log = res as ILog[];
      }),
      this.historyService.getMinuteData(this.tradeAccount.id).subscribe(res => {
        res.forEach(e => {
          this.minuteHeaders.push(this.datepipe.transform(new Date(e.date), 'HH:mm'));
          this.minuteData.push(e.tradeAccountAmount);          
        });
        this.minute();
      }),
      this.historyService.getHourData(this.tradeAccount.id).subscribe(res => {
        res.forEach(e => {
          this.hourHeaders.push(this.datepipe.transform(new Date(e.date), 'M-d-yy, HH:mm'));
          this.hourData.push(e.tradeAccountAmount);
        })
      }),
      this.historyService.getDayData(this.tradeAccount.id).subscribe(res => {
        res.forEach(e => {
          this.dayHeaders.push(this.datepipe.transform(new Date(e.date), 'MM-dd'));
          this.dayData.push(e.tradeAccountAmount);
        })
      }),
      this.historyService.getWeekData(this.tradeAccount.id).subscribe(res => {
        res.forEach(e => {
          this.weekHeaders.push(this.datepipe.transform(new Date(e.date), 'yyyy-ww'));
          this.weekData.push(e.tradeAccountAmount);
        })
      }),
      this.historyService.getMonthData(this.tradeAccount.id).subscribe(res => {
        res.forEach(e => {
          this.monthHeaders.push(this.datepipe.transform(new Date(e.date), 'MMM'));
          this.monthData.push(e.tradeAccountAmount);
        })
      }),
      this.historyService.getYearData(this.tradeAccount.id).subscribe(res => {
        res.forEach(e => {
          this.yearHeaders.push(this.datepipe.transform(new Date(e.date), 'yyyy'));
          this.yearData.push(e.tradeAccountAmount);
        })
      })
    );  
  }

  backToPortfolio() {
    this.portfolioComponent.content = 0;
  }

  minute() {
    this.chart = new Chart({
      chart: {
        type: 'line'
      },
      title: {
        text: 'Equity v. Date/Time'
      },
      subtitle : {
        text : 'Minutely'
      },
      credits: {
        enabled: false
      },
      legend : {
        enabled : false
      },
      yAxis : {
        title : {
          text : 'Equity'
        }
      },
      xAxis : {
        type : 'datetime',
        title : {
          text : 'Date/Time'
        },
        categories : this.minuteHeaders
      },
      series: [
        {
          name: 'Equity',
          type: 'line',
          data: this.minuteData
        },
      ]
    });
  }
  hour() {
    this.chart = new Chart({
      chart: {
        type: 'line'
      },
      title: {
        text: 'Equity v. Date/Time'
      },
      subtitle : {
        text : 'Hourly'
      },
      credits: {
        enabled: false
      },
      legend : {
        enabled : false
      },
      yAxis : {
        title : {
          text : 'Equity'
        }
      },
      xAxis : {
        type : 'datetime',
        title : {
          text : 'Date/Time'
        },
        categories : this.hourHeaders
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
  day() {
    this.chart = new Chart({
      chart: {
        type: 'line'
      },
      title: {
        text: 'Equity v. Date/Time'
      },
      subtitle : {
        text : 'Daily'
      },
      credits: {
        enabled: false
      },
      legend : {
        enabled : false
      },
      yAxis : {
        title : {
          text : 'Equity'
        }
      },
      xAxis : {
        type : 'datetime',
        title : {
          text : 'Date/Time'
        },
        categories : this.dayHeaders
      },
      series: [
        {
          name: 'Equity',
          type: 'line',
          data: this.dayData
        },
      ]
    });
  }
  week() {
    this.chart = new Chart({
      chart: {
        type: 'line'
      },
      title: {
        text: 'Equity v. Date/Time'
      },
      subtitle : {
        text : 'Weekly'
      },
      credits: {
        enabled: false
      },
      legend : {
        enabled : false
      },
      yAxis : {
        title : {
          text : 'Equity'
        }
      },
      xAxis : {
        type : 'datetime',
        title : {
          text : 'Date/Time'
        },
        categories : this.weekHeaders
      },
      series: [
        {
          name: 'Equity',
          type: 'line',
          data: this.weekData
        },
      ]
    });
  }
  month() {
    this.chart = new Chart({
      chart: {
        type: 'line'
      },
      title: {
        text: 'Equity v. Date/Time'
      },
      subtitle : {
        text : 'Monthly'
      },
      credits: {
        enabled: false
      },
      legend : {
        enabled : false
      },
      yAxis : {
        title : {
          text : 'Equity'
        }
      },
      xAxis : {
        type : 'datetime',
        title : {
          text : 'Date/Time'
        },
        categories : this.monthHeaders
      },
      series: [
        {
          name: 'Equity',
          type: 'line',
          data: this.monthData
        },
      ]
    });
  }
  year() {
    this.chart = new Chart({
      chart: {
        type: 'line'
      },
      title: {
        text: 'Equity v. Date/Time'
      },
      subtitle : {
        text : 'Yearly'
      },
      credits: {
        enabled: false
      },
      yAxis : {
        title : {
          text : 'Equity'
        }
      },
      legend : {
        enabled : false
      },
      xAxis : {
        type : 'datetime',
        title : {
          text : 'Date/Time'
        },
        categories : this.yearHeaders
      },
      series: [
        {
          name: 'Equity',
          type: 'line',
          data: this.yearData
        },
      ]
    });
  }

  editTa() {
    this.isEdit = true;
  }

}
