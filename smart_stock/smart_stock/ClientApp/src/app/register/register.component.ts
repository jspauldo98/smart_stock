import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { IUser, IPii, ICredential } from '../interfaces';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit, OnDestroy {

  constructor(private readonly userService: UserService) { }
  public tabIndex: number = 0;
  public tabCount: number = 3;
  public canViewCredentials: boolean = false;
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
      credential: null
    };
  }

  ngOnDestroy(): void {
    if (this.createUserSub) this.createUserSub.unsubscribe();
  }

  public updatePii(event: any) {
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

  public updateCredentials(event: any) {
    this.tabIndex = (this.tabIndex + 1) % this.tabCount;
    this.credential = {
      id: null,
      username: event.username,
      password: event.password
    };
    this.user.credential = this.credential;
    this.createUserSub = this.userService.createNewUser(this.user).subscribe(x => {
      console.log(x);
      this.user.credential.password = null;
    });
  }

  public unlockCredentialsView(event: any) {
    this.canViewCredentials = event;
  }

  public unlockPreferencesView(event: any) {
    this.canViewPreferences = event;
  }
}
