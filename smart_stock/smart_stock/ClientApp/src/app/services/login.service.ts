import { Component, Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ICredential, IUser } from '../interfaces';
import { Observable, of, scheduled } from 'rxjs'
import { catchError, map } from 'rxjs/operators';

//This is the peice of client-side software that communicates with our API endpoints, or controller functions. This is one of the most
//import parts of data transfer. If you are getting server side errors, log here first.
@Injectable({
    providedIn: 'root'
})
export class LoginService {
  private readonly apiPath: string;

  private user : IUser;
  private userObservable : Observable<IUser>;

  isLoggedIn(): Observable<boolean> {
    if (this.user != null) {
      return of (true);
    }
    else {
      return of (false);
    }
  }

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.apiPath = baseUrl + 'api/login';
  }
  
  public getUserLogin(username: string): Observable<IUser> {
    console.log(username);
      return this.httpClient.get<IUser>(`${this.apiPath}/${username}`)
      .pipe(
        map(x =>  {
          this.user = (x as IUser);
          console.log(this.user);
          //shorthand for return observable of IUser
          return of (x);
        }),
        catchError(e => {
          console.error(e);
          return of (null);
        })
      );
  }
}