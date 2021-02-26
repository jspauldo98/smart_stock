import { Component, OnInit } from '@angular/core';
import { IPortfolio, ITradeAccount, IUser, ICredential } from '../../interfaces';
import { LoginService } from '../../services/login.service';
import { PortfolioService } from '../../services/portfolio.service';

@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})
export class PortfolioComponent implements OnInit {
  userCredentials: ICredential; 
  portfolio : IPortfolio;

  constructor(private readonly loginService : LoginService, private readonly portfolioService : PortfolioService) { }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loginService.userCredentials$.subscribe(x=>{
      this.userCredentials = x;
      console.log(this.userCredentials);
    });
    this.portfolioService.getPortfolio(this.userCredentials).subscribe(res => {
      this.portfolio = res;
      console.log(res);
    });
  }
}