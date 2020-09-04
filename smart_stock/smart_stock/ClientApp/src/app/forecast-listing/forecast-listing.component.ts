import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { IWeatherForecast } from '../interfaces';

@Component({
  selector: 'app-forecast-listing',
  templateUrl: './forecast-listing.component.html',
  styleUrls: ['./forecast-listing.component.css']
})
export class ForecastListingComponent implements OnInit {
  @Input() forecasts: Array<IWeatherForecast>;
  @Output() recordDeleted = new EventEmitter();
  @Output() newClicked = new EventEmitter();
  @Output() editClicked = new EventEmitter();

  constructor() { }
// This is populated by emitters that detect a change in an event, and send new data to the home component.
//Otherwise, just list the data we alreayd have.
  ngOnInit(): void {
  }

  public delete(data: IWeatherForecast) {
    this.recordDeleted.emit(data);
  }

  public edit(data: IWeatherForecast) {
    this.editClicked.emit(data);
  }

  public new() {
    this.newClicked.emit();
  }

}
