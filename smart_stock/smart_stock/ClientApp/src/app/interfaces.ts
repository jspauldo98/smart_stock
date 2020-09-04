//All data models that are a part of both the client side and API side should always be declared here, as interfaces
export interface IWeatherForecast {
    id: number;
    dateOfForecast: string;
    temperatureC: number;
    summary: string;
  }