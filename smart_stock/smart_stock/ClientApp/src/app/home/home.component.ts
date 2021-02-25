import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { LoginService } from '../services/login.service';
import { Router } from '@angular/router';
import { ICredential, IUser } from '../interfaces';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private readonly loginService: LoginService,
    private router : Router,
    public changeDetectorRef: ChangeDetectorRef) {}

  user: IUser
  page : number = 0;
  title : string = "About";
  userCredentials: ICredential

  ngOnInit(): void {
    this.changeDetectorRef.detectChanges();
    this.loginService.userCredentials$.subscribe(x => {
      this.userCredentials = x;
      console.log(this.userCredentials);
    });
  }

  toAboutView() : void {
    this.page = 0;
    this.title = "About"
  }

  toDashboardView() : void {
    this.page = 1;
    this.title = "DashBoard"
  }

  toPortfolioView() : void {
    this.page = 2;
    this.title = "Portfolio"
  }
  
  public userLogout() {
    this.loginService.userLogout();
  }

}
