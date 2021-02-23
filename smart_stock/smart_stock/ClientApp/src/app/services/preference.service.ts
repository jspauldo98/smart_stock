import { Component, Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IPreference, IRiskLevel, ITradeAccount, ITradeStrategies} from '../interfaces';
import { Observable } from 'rxjs'

@Injectable({
    providedIn: 'root'
})
export class PreferenceService {
  private readonly apiPath: string;

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.apiPath = baseUrl + 'api/preference';
  }

  public getRiskLevels() {
    return this.httpClient.get<IRiskLevel[]>(`${this.apiPath}`).toPromise();
  }
  
  public createPreference(preference: IPreference): Observable<IPreference> {
    return this.httpClient.post<IPreference>(`${this.apiPath}`, preference);
  }
}