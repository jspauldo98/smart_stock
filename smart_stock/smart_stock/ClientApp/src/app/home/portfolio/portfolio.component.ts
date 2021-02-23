import { Component, OnInit } from '@angular/core';
import { IPortfolio, IUser } from '../../interfaces';
import { LoginService } from '../../services/login.service';
import { PortfolioService } from '../../services/portfolio.service';

@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})
export class PortfolioComponent implements OnInit {
  private user : IUser;  

  constructor(private readonly loginService : LoginService, public portfolioService : PortfolioService) { }

  ngOnInit(): void {
    this.loginService.currentUser.subscribe(x=>{
      this.user = x;
      console.log(this.user);
    })
    this.portfolioService.getPortfolio(this.user);
  }

  createTradeAccount() {
    this.portfolioService.postTradeAccount().subscribe(res => this.portfolioService.getTradeAccounts());
  }
}
