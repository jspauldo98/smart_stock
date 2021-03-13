import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { first } from 'rxjs/operators';
import { ICredential, IPortfolio, IPreference, ISector, ITradeAccount, ITradeStrategies, IUser } from 'src/app/interfaces';
import { LoginService } from 'src/app/services/login.service';
import { PortfolioService } from 'src/app/services/portfolio.service';
import { UserService } from 'src/app/services/user.service';
import { SubSink } from 'subsink';
import { PortfolioComponent } from '../portfolio.component';

@Component({
  selector: 'app-trade-account-create',
  templateUrl: './trade-account-create.component.html',
  styleUrls: ['./trade-account-create.component.css']
})
export class TradeAccountCreateComponent implements OnInit, OnDestroy {  

  constructor(private readonly loginService : LoginService,
    private readonly userService : UserService,
    private readonly portfolioService : PortfolioService,
    private portfolioComponent : PortfolioComponent,
    private toastr : ToastrService) { }

  private subs = new SubSink();
  public tabIndex : number = 0;
  public tabCount : number = 5;
  public canViewStrategy : boolean = false;
  public canViewSectors : boolean = false;
  public canViewRisk : boolean = false;
  public canViewFunding : boolean = false;
  private user : IUser;
  private userCredentials : ICredential;
  public portfolio : IPortfolio;
  public strategy : ITradeStrategies;
  public sectors : ISector;
  public preference : IPreference;
  public tradingAccount : ITradeAccount;
  public isEdit : boolean = false;

  @Input() ta: ITradeAccount;

  ngOnInit(): void {
    this.subs.add(
      this.loginService.userCredentials$.subscribe(x=>{
        this.userCredentials = x;
        this.userService.getAllUserInformation(this.userCredentials.loginResultUserId, this.userCredentials.username).pipe(first()).subscribe(x => {
          this.user = x;
        });
      }),
      this.portfolioService.getPortfolio(this.userCredentials).subscribe(res => {
        this.portfolio = res;
        console.log(res);
      })
    );
    // console.log(this.ta);
    if (this.ta == null)
    {
      // Init trade account and preference objects with null values that will be dynamically updated
      this.tradingAccount = {
        id           : null,
        portfolio    : this.portfolio,
        preference   : null,
        title        : null,
        description  : null,
        amount       : 0,
        profit       : 0,
        loss         : 0,
        net          : 0,
        numTrades    : 0,
        numSTrades   : 0,
        numFTrades   : 0,
        invested     : 0,
        cash         : 0,
        dateCreated  : new Date(),
        dateModified : new Date()
      };
      this.preference = {
        id            : null,
        riskLevel     : null,
        tradeStrategy : null,
        sector        : null,
        capitalToRisk : null
      };
    }
    else 
    {
      this.tradingAccount = this.ta;
      this.preference = this.ta.preference;
      this.isEdit = true;
    }
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  public updateTradeAccount(event : any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    this.tradingAccount.title = event[0];
    this.tradingAccount.description = event[1];
  }

  public updateStrategy(event : any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    this.strategy = event;
  }

  public updateSectors(event : any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    this.sectors = event;
  }

  public updatePreference(event : any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    
  }

  public submitTradeAccount(event : any) {    
    this.preference.riskLevel = event[0];
    this.preference.capitalToRisk = event[1];
    this.preference.tradeStrategy = this.strategy;
    this.preference.sector = this.sectors;
    this.tradingAccount.preference = this.preference;
    this.tradingAccount.portfolio = this.portfolio;
    this.tradingAccount.preference = this.preference;
    if (this.isEdit) 
    {
      this.subs.add(this.portfolioService.putTradeAccount(this.tradingAccount).subscribe(res => {
        this.portfolioComponent.getData();
        this.toastr.success('Account Update Successful', this.tradingAccount.title);
        this.portfolioComponent.content = 0;
      }));
    }
    else if (!this.isEdit) {
      this.subs.add(this.portfolioService.postTradeAccount(this.tradingAccount).subscribe(res => {
        this.portfolioComponent.getData();
        this.toastr.success('Account Creation Successful', this.tradingAccount.title);
        this.portfolioComponent.content = 0;
      }));
    }
  }

  public unlockStrategyView(event : any) {
    this.canViewStrategy = event;
  }
  public unlockSectorsView(event : any) {
    this.canViewSectors = event;
  }
  public unlockRiskView(event : any) {
    this.canViewRisk = event;
  }
  public unlockFundingView(event : any) {
    this.canViewFunding = event;
  }
}