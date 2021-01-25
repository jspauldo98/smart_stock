//All data models that are a part of both the client side and API side should always be declared here, as interfaces
  export interface ICredential {
    id: number;
    username: string;
    password: string;
  }

  export interface IPii {
    id: number;
    f_Name: string;
    l_Name: string;
    dob: Date;
    email: string;
    phone: string;
  }

  export interface IUser {
    id: number;
    join_Date: Date;
    date_Added: Date;
    date_Confirmed: Date;
    pii: IPii;
    credential: ICredential;
  }