import { Component, Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ITradeStrategies} from '../interfaces';
import { Observable } from 'rxjs'


@Injectable({
    providedIn: 'root'
})
export class TradeStrategiesService {
  private readonly apiPath: string;

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.apiPath = baseUrl + 'api/strategies';
  }
  
  public createTradeStrategy(strategy: ITradeStrategies): Observable<ITradeStrategies> {
      return this.httpClient.post<ITradeStrategies>(`${this.apiPath}`, strategy);
  }
}