import { Component, Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ICredential, IUser } from '../interfaces';
import { Observable, of, scheduled } from 'rxjs'
import { catchError, map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class UserService {
  private readonly apiPath: string;

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.apiPath = baseUrl + 'api/user';
  }
  
  public createNewUser(user: IUser): Observable<IUser> {
      return this.httpClient.post<IUser>(`${this.apiPath}`, user);
  }

  public getAllUserInformation(userId: number, username: string): Observable<IUser> {
    return this.httpClient.get<IUser>(`${this.apiPath}/${userId}/${username}`);
  } 
}