import { Component, OnInit } from '@angular/core';
import { WeatherForecastService } from "../api_services/weather-forecast.service";
//Lodash is really useful when you need to perform lots of array based operations, and don't want the hassle of constantly indexing.
import * as _ from 'lodash';
import { IWeatherForecast } from '../interfaces';

//This component has two children components, forecast-listing, and add-update. They are tied and layered into this component
//through the Angular input/ouput hierarchy. You don't have to do this for every component, this is just to show how to layer
//parent-child relationships via client side software.
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public forecasts: Array<IWeatherForecast>;
  public currentForecast: IWeatherForecast

  constructor (
    private weatherForecastService: WeatherForecastService)
    {}
    
    //For when the "new" button is pressed, triggers an event to let the forms in add-update change their values and get reayd for input.
    public getEmptyForecast() {
      var emptyForecast: IWeatherForecast;
      emptyForecast = {
        id: undefined,
        dateOfForecast: null,
        summary: "Put Something New Here!",
        temperatureC: null
      }
      return emptyForecast;
    }

    //Combined add and update function that will make calls to the service the directly calls the API based on the objects that have 
    //been sent via the client side software

    public createUpdateForecast = function(forecast: IWeatherForecast) {
      let forecastWithId = _.find(this.forecasts, (element => element.id = forecast.id));
      if (forecastWithId) {
        //If this forecast exists, update it with PUT
        const updateIndex = _.findIndex(this.forecasts, {id: forecastWithId.id});
        this.weatherForecastService.update(forecast).subscribe(update =>
          this.forecasts.splice(updateIndex, 1, update));
      }
      else {
        //if this forecast does not exist, add it with POST
        this.weatherForecastService.add(forecast).subscribe(forecastRecord => {
          this.forecasts.push(forecastRecord);
        });
      }
      //Set default forecast as the first record
      this.currentForecast = this.forecasts[0];
    }

    public editForecast = function(record: IWeatherForecast) {
      this.currentForecast = record;
    }

    public newForecast = function() {
      this.currentForecast = this.getEmptyForecast();
    }

    public deleteForecast(record: IWeatherForecast) {
      const deleteIndex = _.findIndex(this.forecasts, { Id: record.id });
      this.weatherForecastService.delete(record.id).subscribe(
        result => this.forecasts.splice(deleteIndex, 1));
    }

    //Better to use this over a constructor, a constructor should be used to initialize local variables only, not API related data.
    ngOnInit() {
      this.weatherForecastService.get().subscribe((data: Array<IWeatherForecast>) => {
        this.forecasts = data;
        this.currentForecast = data[0];
       });
      
    } 
}
