import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IPortfolio, ITradeAccount, IUser } from '../interfaces';

@Injectable({
  providedIn: 'root'
})
export class PortfolioService {
  private readonly apiPath : string;
  portfolio : IPortfolio;
  tradeAccounts : ITradeAccount[];

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL')baseUrl: string) {
    this.apiPath = baseUrl + 'api/portfolio';
  }

  public async getPortfolio(user : IUser) {
    const res = await this.httpClient.get<IPortfolio>(`${this.apiPath}` + "/" + user.id).toPromise();
    this.portfolio = (res as IPortfolio),
      console.log(res);
  }

  public async getTradeAccounts()
  {
    const res = await this.httpClient.get<ITradeAccount[]>(`${this.apiPath}` + "/tradeaccount/" + this.portfolio.id).toPromise();
    this.tradeAccounts = (res as ITradeAccount[]),
      console.log(res);
  }

  public postTradeAccount() {
    return this.httpClient.post(`${this.apiPath}` + "/tradeaccount/"+this.portfolio.id, this.portfolio.id);
  }
}
