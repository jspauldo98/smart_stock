import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { IWeatherForecast } from '../interfaces';

@Component({
  selector: 'app-add-update-forecast',
  templateUrl: './add-update-forecast.component.html',
  styleUrls: ['./add-update-forecast.component.css']
})
export class AddUpdateForecastComponent implements OnInit {
  
  @Output() forecastCreated = new EventEmitter<IWeatherForecast>();
  @Input() forecast: IWeatherForecast;

  constructor() {
   }

  //Only uses a member function to call a event emitter when the user wants to save a change they just made.
  ngOnInit(): void {
    
  }

  public addUpdateForecast = function(event) {
    this.forecastCreated.emit(this.forecast);
  }
}
