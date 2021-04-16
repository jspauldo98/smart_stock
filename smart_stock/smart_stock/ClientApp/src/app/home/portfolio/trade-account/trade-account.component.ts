import { Component, OnInit } from '@angular/core';
import { ILog, ITradeAccount } from 'src/app/interfaces';
import { HistoryService } from 'src/app/services/history.service';
import { SubSink } from 'subsink';
import { PortfolioComponent } from '../portfolio.component';

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
  title = '\'s Total History';
  type = 'LineChart';
  data = [
     ["Jan",  7.0],
     ["Feb",  6.9],
     ["Mar",  9.5],
     ["Apr",  14.5],
     ["May",  18.2],
     ["Jun",  21.5],
     ["Jul",  25.2],
     ["Aug",  26.5],
     ["Sep",  23.3],
     ["Oct",  18.3],
     ["Nov",  13.9],
     ["Dec",  9.6]
  ];
  columnNames = ["Month", "Tokyo"];
  options = {   
     hAxis: {
        title: 'Month'
     },
     vAxis:{
        title: 'Temperature'
     },
  };
 width = 1150;
 height = 300;
  constructor(private readonly portfolioComponent : PortfolioComponent,
    private readonly historyService : HistoryService) { }

  ngOnInit(): void {
    this.tradeAccount = this.portfolioComponent.tradeAccount;
    this.subs.add(
      this.historyService.getLog(this.tradeAccount.id).subscribe(res => {
        console.log(res);
        this.log = res as ILog[];
      })
    );
    console.log(this.tradeAccount);
  }

  backToPortfolio() {
    this.portfolioComponent.content = 0;
  }

  editTa() {
    this.isEdit = true;
  }

}
