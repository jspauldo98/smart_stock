import { Component, OnInit, OnDestroy, Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { IUser, IPii, ICredential } from '../interfaces';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit, OnDestroy {

  constructor(private readonly userService: UserService, private toastr : ToastrService) { }
  public tabIndex: number = 0;
  public tabCount: number = 4;
  public canViewCredentials: boolean = false;
  public canViewSetup: boolean = false;
  public canViewPreferences: boolean = false;
  public user: IUser;
  public pii: IPii;
  public credential: ICredential;
  private createUserSub: Subscription;


  ngOnInit(): void {
    //Just initialize a new user object, so that it can be dynamically updated as the user
    //completes form information. Date fields will be adjusted in the API, just can't be null
    //or the httpClient mapper throws errors for the lulz.
    this.user = {
      id: null,
      joinDate: new Date(),
      dateAdded: new Date(),
      dateConfirmed: new Date(),
      pii: null,
      credential: null,
      alpacaKey: null,
      alpacaKeyId: null
    };
  }

  ngOnDestroy(): void {
    if (this.createUserSub) this.createUserSub.unsubscribe();
  }

  public createPii(event: any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    this.pii = {
      id: null,
      fName: event.fName,
      lName: event.lName,
      dob: event.dob,
      email: event.email,
      phone: event.phone
    };
    this.user.pii = this.pii;
  }

  public createCredentials(event: any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    this.credential = {
      id: null,
      username: event.username,
      password: event.password,
      loginResultUserId: null
    };
    this.user.credential = this.credential;
  }

  public createAlpacaAccount(event: any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    this.user.alpacaKeyId = event.alpacaKeyId;
    this.user.alpacaKey = event.alpacaKey;

    this.createUserSub = this.userService.createNewUser(this.user).subscribe(x => {
      this.user.credential.password = null;
      this.user.alpacaKeyId = null;
      this.user.alpacaKey = null;
      this.toastr.success('Registration and setup successful', 'Welcome ' + this.user.pii.fName);
      console.log(JSON.stringify(x));
    });
  }

  public unlockCredentialsView(event: any) {
    this.canViewCredentials = event;
  }

  public unlockSetupView(event: any) {
    this.canViewSetup = event;
  }

  public unlockPreferencesView(event: any) {
    this.canViewPreferences = event;
  }
}
