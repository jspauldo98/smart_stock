import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ILog } from '../interfaces';

@Injectable({
  providedIn: 'root'
})
export class HistoryService {
  private readonly apiPath : string;

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL')baseUrl: string) {
    this.apiPath = baseUrl + 'api/portfolio';
  }

  public getLog(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/tradeaccount/log/" + tradeAccountId);
  }

  public getMinuteData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/tradeaccount/log/minute/" + tradeAccountId);
  }

  public getHourData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/tradeaccount/log/hour/" + tradeAccountId);
  }

  public getDayData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/tradeaccount/log/day/" + tradeAccountId);
  }

  public getWeekData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/tradeaccount/log/week/" + tradeAccountId);
  }

  public getMonthData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/tradeaccount/log/month/" + tradeAccountId);
  }

  public getYearData(tradeAccountId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/tradeaccount/log/year/" + tradeAccountId);
  }
  public getMinuteDataByPortfolio(portfolioId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/log/minute/" + portfolioId);
  }

  public getHourDataByPortfolio(portfolioId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/log/hour/" + portfolioId);
  }

  public getDayDataByPortfolio(portfolioId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/log/day/" + portfolioId);
  }

  public getWeekDataByPortfolio(portfolioId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/log/week/" + portfolioId);
  }

  public getMonthDataByPortfolio(portfolioId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/log/month/" + portfolioId);
  }

  public getYearDataByPortfolio(portfolioId : number) {
    return this.httpClient.get<ILog[]>(`${this.apiPath}` + "/log/year/" + portfolioId);
  }
}
