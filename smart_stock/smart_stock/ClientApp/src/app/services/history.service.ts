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
}
