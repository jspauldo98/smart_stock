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
