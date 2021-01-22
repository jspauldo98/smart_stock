//All data models that are a part of both the client side and API side should always be declared here, as interfaces
export interface IWeatherForecast {
    id: number;
    dateOfForecast: string;
    temperatureC: number;
    summary: string;
  }

  export interface ICredential {
    id: number;
    username: string;
    password: string;
  }

  export interface IPii {
    id: number;
    firstName: string;
    lastName: string;
    dob: Date;
    email: string;
    phone: string;
  }

  export interface IUser {
    id: number;
    joinDate: Date;
    dateAdded: Date;
    dateConfirmed: Date;
    pii: IPii;
    credential: ICredential;
  }