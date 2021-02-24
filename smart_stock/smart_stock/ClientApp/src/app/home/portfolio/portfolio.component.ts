import { Component, OnInit } from '@angular/core';
import { IPortfolio, ITradeAccount, IUser } from '../../interfaces';
import { LoginService } from '../../services/login.service';
import { PortfolioService } from '../../services/portfolio.service';

@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css']
})
export class PortfolioComponent implements OnInit {
  user : IUser; 
  portfolio : IPortfolio;

  constructor(private readonly loginService : LoginService, private readonly portfolioService : PortfolioService) { }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loginService.currentUser.subscribe(x=>{
      this.user = x;
      console.log(this.user);
    });
    this.portfolioService.getPortfolio(this.user).subscribe(res => {
      this.portfolio = res;
      console.log(res);
    });
  }
}