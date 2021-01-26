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

  get currentUser(): Observable<IUser> {
    if (this.isLoggedIn()) {
      return of (this.user);
    }
    return of (null);
  }

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.apiPath = baseUrl + 'api/login';
  }
  
  //I am using a POST method instead of a GET method such that I can send the entire credential 
  //body back to the controller for further processing. 
  public getUserLogin(credential: ICredential): Observable<IUser> {
      return this.httpClient.post<IUser>(`${this.apiPath}`, credential)
      .pipe(
        map(x =>  {
          this.user = (x as IUser);
          //shorthand for return observable of IUser
          return of (x);
        }),
        catchError(e => {
          console.error(e);
          return of (null);
        })
      );
  }

  public isLoggedIn(): Observable<boolean> {
    if (this.user != null) {
      return of (true);
    }
    else {
      return of (false);
    }
  }
}