import { Component, OnInit } from '@angular/core';
import { IPortfolio, ITradeAccount, IUser, ICredential } from '../../interfaces';
import { LoginService } from '../../services/login.service';
import { UserService } from '../../services/user.service';
import { PortfolioService } from '../../services/portfolio.service';
import { first } from 'rxjs/operators';
@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})
export class PortfolioComponent implements OnInit {
  userCredentials: ICredential; 
  portfolio : IPortfolio;
  user: IUser;

  constructor(private readonly loginService : LoginService, 
    private readonly portfolioService : PortfolioService,
    private readonly userService: UserService) { }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loginService.userCredentials$.subscribe(x=>{
      this.userCredentials = x;
      this.userService.getAllUserInformation(this.userCredentials.loginResultUserId, this.userCredentials.username).pipe(first()).subscribe(x => {
        this.user = x;
      });
    });
    this.portfolioService.getPortfolio(this.userCredentials).subscribe(res => {
      this.portfolio = res;
      console.log(res);
    });
  }
}