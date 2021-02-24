import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IPortfolio, ITradeAccount, IUser } from '../interfaces';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PortfolioService {
  private readonly apiPath : string;

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL')baseUrl: string) {
    this.apiPath = baseUrl + 'api/portfolio';
  }

  public getPortfolio(user : IUser) {
    return this.httpClient.get<IPortfolio>(`${this.apiPath}` + "/" + user.id);
  }

  public getTradeAccounts(portfolioId : number)
  {
    return this.httpClient.get<ITradeAccount[]>(`${this.apiPath}` + "/tradeaccount/" + portfolioId);
  }

  public postTradeAccount(portfolioId : number) {
    return this.httpClient.post(`${this.apiPath}` + "/tradeaccount/"+portfolioId, portfolioId);
  }
}
