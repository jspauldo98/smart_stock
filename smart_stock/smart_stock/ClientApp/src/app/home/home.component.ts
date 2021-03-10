import { Component, OnInit, ChangeDetectorRef, Inject} from '@angular/core';
import { LoginService } from '../services/login.service';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';
import { ICredential, IUser } from '../interfaces';
import { first } from 'rxjs/operators';
import { Observable } from 'rxjs';
import {MatDialog, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { UserDialogComponent } from '../mat-dialog-views/user-dialog.component';
 
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private readonly loginService: LoginService,
    private router : Router,
    private readonly userService: UserService,
    public changeDetectorRef: ChangeDetectorRef,
    public dialog: MatDialog) {}

  user: IUser
  page : number = 0;
  title : string = "About";
  userCredentials: ICredential

  ngOnInit(): void {
    this.changeDetectorRef.detectChanges();
    this.loginService.userCredentials$.subscribe(x => {
      this.userCredentials = x;
      this.userService.getAllUserInformation(this.userCredentials.loginResultUserId, this.userCredentials.username).pipe(first()).subscribe(x => {
        this.user = x;
      });
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

  public launchUserDialog(): void {
    this.dialog.open(UserDialogComponent, {
      data: {
        userInfo: this.user
      }
    });
  }
}
