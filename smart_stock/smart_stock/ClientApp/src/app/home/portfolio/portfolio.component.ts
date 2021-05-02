import { Component, OnDestroy, OnInit } from '@angular/core';
import { IPortfolio, ITradeAccount, IUser, ICredential, ILog } from '../../interfaces';
import { LoginService } from '../../services/login.service';
import { UserService } from '../../services/user.service';
import { PortfolioService } from '../../services/portfolio.service';
import { HistoryService } from 'src/app/services/history.service';
import { first } from 'rxjs/operators';
import { SubSink } from 'subsink';
import { Chart } from 'angular-highcharts';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})

export class PortfolioComponent implements OnInit, OnDestroy {
  private subs = new SubSink();
  public userCredentials: ICredential; 
  public portfolio : IPortfolio;
  public tradeAccount : ITradeAccount;
  public user: IUser;
  public content : number;
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

  constructor(private readonly loginService : LoginService, 
    private readonly portfolioService : PortfolioService,
    private readonly userService: UserService,
    private readonly historyService : HistoryService,
    private datepipe : DatePipe) { }

  ngOnInit(): void {
    this.getData();
    this.content = 0;
  }

  getData() {
    this.subs.add(
      this.loginService.userCredentials$.subscribe(x=>{
        this.userCredentials = x;
        this.userService.getAllUserInformation(this.userCredentials.loginResultUserId, this.userCredentials.username).pipe(first()).subscribe(x => {
          this.user = x;
        });
      }),
      this.portfolioService.getPortfolio(this.userCredentials).subscribe(res => {
        this.portfolio = res;
        this.historyService.getMinuteDataByPortfolio(res.id).subscribe(res => {
          res.forEach(e => {
            this.minuteHeaders.push(this.datepipe.transform(new Date(e.date), 'HH:mm'));
            this.minuteData.push(e.portfolioAmount);          
          });
          this.minute();
        }),
        this.historyService.getHourDataByPortfolio(res.id).subscribe(res => {
          res.forEach(e => {
            this.hourHeaders.push(this.datepipe.transform(new Date(e.date), 'M-d-yy, HH:mm'));
            this.hourData.push(e.portfolioAmount);
          })
        }),
        this.historyService.getDayDataByPortfolio(res.id).subscribe(res => {
          res.forEach(e => {
            this.dayHeaders.push(this.datepipe.transform(new Date(e.date), 'MM-dd'));
            this.dayData.push(e.portfolioAmount);
          })
        }),
        this.historyService.getWeekDataByPortfolio(res.id).subscribe(res => {
          res.forEach(e => {
            this.weekHeaders.push(this.datepipe.transform(new Date(e.date), 'yyyy-ww'));
            this.weekData.push(e.portfolioAmount);
          })
        }),
        this.historyService.getMonthDataByPortfolio(res.id).subscribe(res => {
          res.forEach(e => {
            this.monthHeaders.push(this.datepipe.transform(new Date(e.date), 'MMM'));
            this.monthData.push(e.portfolioAmount);
          })
        }),
        this.historyService.getYearData(res.id).subscribe(res => {
          res.forEach(e => {
            this.yearHeaders.push(this.datepipe.transform(new Date(e.date), 'yyyy'));
            this.yearData.push(e.portfolioAmount);
          })
        })
      })
    );
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

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}