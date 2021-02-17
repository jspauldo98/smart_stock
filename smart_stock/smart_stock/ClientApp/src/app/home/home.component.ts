import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { LoginService } from '../services/login.service';
import { IUser } from '../interfaces';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private readonly loginService: LoginService,
    public changeDetectorRef: ChangeDetectorRef) {}

  user: IUser

  ngOnInit(): void {
    this.changeDetectorRef.detectChanges();
    this.loginService.currentUser.subscribe(x => {
      this.user = x;
      console.log(this.user)
    });
  }

}
