import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { DatePipe } from "@angular/common";
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { AddUpdateForecastComponent } from './add-update-forecast/add-update-forecast.component';
import { ForecastListingComponent } from './forecast-listing/forecast-listing.component';
import { WeatherForecastService } from './api_services/weather-forecast.service';
//Root module, everything needs to be imported here first
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AddUpdateForecastComponent,
    ForecastListingComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' }
    ])
  ],
  providers: [WeatherForecastService, DatePipe],
  bootstrap: [AppComponent]
})
export class AppModule { }
