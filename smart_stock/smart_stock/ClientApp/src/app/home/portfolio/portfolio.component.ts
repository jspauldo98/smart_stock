import { Component, OnDestroy, OnInit } from '@angular/core';
import { IPortfolio, ITradeAccount, IUser, ICredential } from '../../interfaces';
import { LoginService } from '../../services/login.service';
import { UserService } from '../../services/user.service';
import { PortfolioService } from '../../services/portfolio.service';
import { first } from 'rxjs/operators';
import { SubSink } from 'subsink';
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

  constructor(private readonly loginService : LoginService, 
    private readonly portfolioService : PortfolioService,
    private readonly userService: UserService) { }

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
        console.log(res);
      })
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}