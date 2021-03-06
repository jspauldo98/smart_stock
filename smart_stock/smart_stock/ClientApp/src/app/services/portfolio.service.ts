import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ICredential, IPortfolio, ITradeAccount, ITradeStrategies, IUser } from '../interfaces';
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

  public postPorfolio(portfolioId : number) {
    return this.httpClient.post(`${this.apiPath}` + "/tradeaccount/"+portfolioId, portfolioId);
  }

  public putPortfolio(portfolio : IPortfolio){
    return this.httpClient.put(`${this.apiPath}` + "/" + portfolio.id, portfolio);
  }

  public postTradeAccount(tradeAccount : ITradeAccount){
    return this.httpClient.post(`${this.apiPath}` + "/tradeaccount", tradeAccount);
  }

  public putTradeAccount(tradeAccount : ITradeAccount){
    return this.httpClient.put(`${this.apiPath}` + "/tradeaccount/" + tradeAccount.id, tradeAccount);
  }
}
