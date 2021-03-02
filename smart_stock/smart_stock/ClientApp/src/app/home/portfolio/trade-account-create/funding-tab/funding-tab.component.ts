import { Component, OnInit, EventEmitter, Output, OnDestroy } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { ICredential, IPortfolio, IUser } from 'src/app/interfaces';
import { LoginService } from 'src/app/services/login.service';
import { PortfolioService } from 'src/app/services/portfolio.service';
import { UserService } from 'src/app/services/user.service';
import { SubSink } from 'subsink';

@Component({
  selector: 'app-funding-tab',
  templateUrl: './funding-tab.component.html',
  styleUrls: ['./funding-tab.component.css']
})
export class FundingTabComponent implements OnInit, OnDestroy {

  // TODO - for now use IPortfolio since paper trading. In the furture if using real money use bank object or portfolio
  @Output() onSaveEvent = new EventEmitter<[IPortfolio, number]>();
  @Output() onNextTabChange = new EventEmitter<boolean>();

  constructor(private readonly loginService : LoginService,
    private readonly userService : UserService,
    private readonly portfolioService : PortfolioService) { }
  private subs = new SubSink();
  private user : IUser;
  private userCredentials : ICredential;
  public accounts : IPortfolio[];
  public submitAttempt : boolean = false;
  // TODO - again this would be a bank or portfolio past Smart Stock Beta version
  public selectedAccount : IPortfolio;
  public accountControl : FormControl;
  public depositAmount : number;

  ngOnInit(): void {
    this.subs.add(
      this.loginService.userCredentials$.subscribe(x=>{
        this.userCredentials = x;
        this.userService.getAllUserInformation(this.userCredentials.loginResultUserId, this.userCredentials.username).pipe(first()).subscribe(x => {
          this.user = x;
        });
      }),
      this.portfolioService.getPortfolio(this.userCredentials).subscribe(res => {
        // TODO - For now just push current portfolio unto account stack. In the future bank accounts would be here
        this.accounts = [res];
      }
    ));
    this.accountControl = new FormControl('', Validators.required);
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  emitFunding() : void {
    this.onSaveEvent.emit([this.selectedAccount, this.depositAmount]);
    this.onNextTabChange.emit(true);
  }

  onSubmit() {
    this.submitAttempt = true;

    this.selectedAccount = this.accountControl.value;

    if (this.selectedAccount == null || this.depositAmount == null) {
      return;
    }

    this.emitFunding();
    this.submitAttempt = false;
  }

}
