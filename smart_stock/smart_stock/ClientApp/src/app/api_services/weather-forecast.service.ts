import { Component, Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IWeatherForecast } from '../interfaces';
import { Observable } from 'rxjs'

//This is the peice of client-side software that communicates with our API endpoints, or controller functions. This is one of the most
//import parts of data transfer. If you are getting server side errors, log here first.
@Injectable({
    providedIn: 'root'
})
export class WeatherForecastService {
  private readonly apiPath: string;

  constructor(private readonly httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.apiPath = baseUrl + 'api/forecast';
  }
  
  public get(): Observable<Array<IWeatherForecast>> {
      return this.httpClient.get<Array<IWeatherForecast>>(`${this.apiPath}/forecasts`);
  }

  public add(forecast: IWeatherForecast): Observable<IWeatherForecast> {
    return this.httpClient.post<IWeatherForecast>(`${this.apiPath}/add`, forecast);
  }

  public update(forecast: IWeatherForecast): Observable<IWeatherForecast> {
    return this.httpClient.put<IWeatherForecast>(`${this.apiPath}/update/${forecast.id}`, forecast);
  }

  public delete(id: number): Observable<Object> {
    return this.httpClient.delete(`${this.apiPath}/${id}`);
  }
}
