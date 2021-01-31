import { Component, OnInit } from '@angular/core';
import { IUser, IPii, ICredential } from "../interfaces";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor() { }
  public tabIndex: number = 0;
  public tabCount: number = 3;
  public user: IUser;
  public pii: IPii;
  public credential: ICredential;


  ngOnInit(): void {
  }

  public updatePii(event: any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    this.pii = {
      id: null,
      f_Name: event.fName,
      l_Name: event.lName,
      dob: event.dob,
      email: event.email,
      phone: event.phone
    };
  }
}
