import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ILog } from '../interfaces';

@Injectable({
  providedIn: 'root'
})
export class HistoryService {
  private readonly apiPath : string;

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL')baseUrl: string) {
    this.apiPath = baseUrl + 'api/portfolio/tradeaccount/log';
  }

  public getLog(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/" + tradeAccountId);
  }

  public getMinuteData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/minute/" + tradeAccountId);
  }

  public getHourData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/hour/" + tradeAccountId);
  }

  public getDayData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/day/" + tradeAccountId);
  }

  public getWeekData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/week/" + tradeAccountId);
  }

  public getMonthData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/month/" + tradeAccountId);
  }

  public getYearData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/year/" + tradeAccountId);
  }
}
