import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { LoginService } from '../services/login.service';
import { ICredential, IUser } from '../interfaces';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private readonly loginService: LoginService,
    public changeDetectorRef: ChangeDetectorRef) {}

  userCredentials: ICredential

  ngOnInit(): void {
    this.changeDetectorRef.detectChanges();
    this.loginService.userCredentials$.subscribe(x => {
      this.userCredentials = x;
      console.log(this.userCredentials);
    });
  }

  public userLogout() {
    this.loginService.userLogout();
  }

}
