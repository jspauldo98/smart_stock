import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ICredential, IPortfolio, ITradeAccount, IUser } from '../interfaces';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PortfolioService {
  private readonly apiPath : string;

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL')baseUrl: string) {
    this.apiPath = baseUrl + 'api/portfolio';
  }

  public getPortfolio(userCreds : ICredential) {
    return this.httpClient.get<IPortfolio>(`${this.apiPath}` + "/" + userCreds.loginResultUserId);
  }

  public getTradeAccounts(portfolioId : number)
  {
    return this.httpClient.get<ITradeAccount[]>(`${this.apiPath}` + "/tradeaccount/" + portfolioId);
  }

  public postTradeAccount(portfolioId : number) {
    return this.httpClient.post(`${this.apiPath}` + "/tradeaccount/"+portfolioId, portfolioId);
  }
}
