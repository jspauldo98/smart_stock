import { Component, Inject, Injectable, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ICredential, IUser, ILoginResult } from '../interfaces';
import { Observable, of, scheduled, Subscription, BehaviorSubject } from 'rxjs'
import { catchError, map, tap, delay, finalize } from 'rxjs/operators';


//This is the peice of client-side software that communicates with our API endpoints, or controller functions. This is one of the most
//import parts of data transfer. If you are getting server side errors, log here first.
@Injectable({
    providedIn: 'root'
})
export class LoginService {

  private readonly apiPath: string;
  private timer: Subscription;
  private userCredentials = new BehaviorSubject<ICredential>(null);
  userCredentials$: Observable<ICredential> = this.userCredentials.asObservable();

  private storageEventListener(event: StorageEvent) {
    if (event.storageArea === localStorage) {
      if (event.key === 'logout-event') {
        this.stopTokenTimer();
        this.userCredentials.next(null);
      }
      if (event.key === 'login-event') {
        this.stopTokenTimer();
        this.httpClient.get<ILoginResult>(`${this.apiPath}/user`).subscribe((x) => {
          this.userCredentials.next({
            id: null,
            username: x.username,
            loginResultUserId: x.userId,
            password: null
          });
        });
      }
    }
  }

  constructor(private router: Router,
    private readonly httpClient: HttpClient, 
    @Inject('BASE_URL') baseUrl: string) {
    this.apiPath = baseUrl + 'api/login';
    window.addEventListener('storage', this.storageEventListener.bind(this));
  }

  ngOnDestroy(): void {
    window.removeEventListener('storage', this.storageEventListener.bind(this));
  }
  
  //I am using a POST method instead of a GET method such that I can send the entire credential 
  //body back to the controller for further processing. 
  public getUserLogin(credential: ICredential): Observable<ILoginResult> {
      return this.httpClient.post<ILoginResult>(`${this.apiPath}`, credential)
      .pipe(
        map(x =>  {
          this.userCredentials.next({
            id: null,
            username: x.username,
            password: null,
            loginResultUserId: x.userId
          });
          this.setLocalStorage(x);
          this.startTokenTimer();
          return x;
        }),
        catchError(e => {
          console.error(e);
          return of (null);
        })
      );
  }

  public userLogout(): Subscription {
    return this.httpClient.post<void>(`${this.apiPath}/logout`, null)
    .pipe(
      finalize(() => {
        this.clearLocalStorage();
        this.userCredentials.next(null);
        this.stopTokenTimer();
        this.router.navigateByUrl("/login");
      })
    ).subscribe();
  }

  public refreshToken() {
    const refreshToken = localStorage.getItem('refresh_token');
    
    if (!refreshToken) {
      this.clearLocalStorage();
      return of (null);
    }
    
    return this.httpClient.post<ILoginResult>(`${this.apiPath}/refresh-token`, {refreshToken})
    .pipe(
      map(x => {
        this.userCredentials.next({
          username: x.username,
          id: null,
          password: null,
          loginResultUserId: x.userId
        });
        this.setLocalStorage(x);
        this.startTokenTimer();
        return x;
      })
    );
  }

  public setLocalStorage(result: ILoginResult) {
    localStorage.setItem('access_token', result.accessToken);
    localStorage.setItem('refresh_token', result.refreshToken);
    localStorage.setItem('login-event', 'login' + Math.random());
  }

  public clearLocalStorage() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.setItem('logout-event', 'logout' + Math.random());
  }

  private getTokenRemainingTime() {
    const accessToken = localStorage.getItem('access_token');
    if (!accessToken)
    {
      return 0;
    }
    const jwtToken = JSON.parse(atob(accessToken.split('.')[1]));
    const expires = new Date(jwtToken.exp * 1000);
    return expires.getTime() - Date.now();
  }

  private startTokenTimer() {
    const timeout = this.getTokenRemainingTime();
    this.timer = of(true).pipe(delay(timeout), tap(() => this.refreshToken().subscribe())).subscribe();
  }

  private stopTokenTimer() {
    this.timer?.unsubscribe();
  }
}