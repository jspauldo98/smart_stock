import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { IPortfolio, ITradeAccount } from 'src/app/interfaces';
import { PortfolioService } from 'src/app/services/portfolio.service';
import { SubSink } from 'subsink';
import { PortfolioComponent } from '../portfolio.component';

@Component({
  selector: 'app-transfer',
  templateUrl: './transfer.component.html',
  styleUrls: ['./transfer.component.css']
})
export class TransferComponent implements OnInit, OnDestroy {

  @Input() portfolio : IPortfolio;

  constructor(private portfolioService : PortfolioService, private readonly portfolioComponent : PortfolioComponent,
    private toastr : ToastrService) { }
  public subs = new SubSink();
  public tradeAccounts : ITradeAccount[];
  public selectedAccount : ITradeAccount;
  public flipV : boolean = false;
  public amount : number;

  ngOnInit(): void {
    this.subs.add(
      this.portfolioService.getTradeAccounts(this.portfolio.id).subscribe(res => {
        this.tradeAccounts = res as ITradeAccount[];
        console.log(res);
      })
    );
  }

  confirm() {
    if (!this.flipV) {
      if (this.amount < 1 || this.amount > this.portfolio.cash) {
        return
      }
      this.portfolio.cash = this.portfolio.cash - this.amount;
      this.selectedAccount.cash = this.selectedAccount.cash + this.amount;
      this.selectedAccount.amount = this.selectedAccount.amount + this.amount;
    } else {
      if (this.amount < 1 || this.amount > this.selectedAccount.cash) {
        return
      }
      this.portfolio.cash = this.portfolio.cash + this.amount;
      this.selectedAccount.cash = this.selectedAccount.cash - this.amount;
      this.selectedAccount.amount = this.selectedAccount.amount - this.amount;
    }
    this.selectedAccount.portfolio = this.portfolio;
    this.subs.add(
      this.portfolioService.putTradeAccount(this.selectedAccount).subscribe(res => {
        this.portfolioComponent.content = 0;
        this.portfolioComponent.getData();
      }),
      this.portfolioService.putPortfolio(this.portfolio).subscribe(res => {
        this.portfolioComponent.content = 0;
        this.portfolioComponent.getData();
        this.toastr.success('Transfer Successful');
      })
    );
  }

  flip(value : boolean) {
    this.flipV = value;
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  formatLabel(value: number) {
    if (value >= 1000) {
      return '$' + Math.round(value / 1000) + 'k';
    }

    return '$' + value;
  }
}
