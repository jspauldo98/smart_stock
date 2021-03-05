//All data models that are a part of both the client side and API side should always be declared here, as interfaces
  export interface ICredential {
    id       : number;
    username : string;
    password : string;
    loginResultUserId: number;
  }

  export interface IPii {
    id   : number;
    fName: string;
    lName: string;
    dob  : Date  ;
    email: string;
    phone: string;
  }

  export interface IUser {
    id            : number     ;
    joinDate      : Date       ;
    dateAdded     : Date       ;
    dateConfirmed : Date       ;
    pii           : IPii       ;
    credential    : ICredential;
  }

  export interface ITradeStrategies {
    id        : number ;
    blueChip  : boolean;
    longTerm  : boolean;
    swing     : boolean;
    scalp     : boolean;
    day       : boolean;
  }

  export interface IPortfolio {
    id       : number;
    user     : IUser ;
    amount   : number;
    profit   : number;
    loss     : number;
    net      : number;
    invested : number;
    cash     : number;
  }

  export interface IRiskLevel {
    id        : number;
    risk      : string;
    dateAdded : Date  ;
  }

  export interface ISector {
    id                    : number ;
    informationTechnology : boolean;
    healthCare            : boolean;
    financials            : boolean;
    consumerDiscretionary : boolean;
    communication         : boolean;
    industrials           : boolean;
    consumerStaples       : boolean;
    energy               : boolean;
    utilities             : boolean;
    realEstate            : boolean;
    materials             : boolean;
  }

  export interface IPreference {
    id            : number          ;
    riskLevel     : IRiskLevel      ;
    tradeStrategy : ITradeStrategies;
    sector        : ISector         ;
    capitalToRisk : number          ;
  }

  export interface ITradeAccount {
    id           : number     ;
    portfolio    : IPortfolio ;
    preference   : IPreference;
    title        : string     ;
    description  : string     ;
    amount       : number     ;
    profit       : number     ;
    loss         : number     ;
    net          : number     ;
    numTrades    : number     ;
    numSTrades   : number     ;
    numFTrades   : number     ;
    invested     : number     ;
    cash         : number     ;
    dateCreated  : Date       ;
    dateModified : Date       ;
  }

  export interface ILoginResult {
    username: string;
    userId: number;
    accessToken: string;
    refreshToken: string;
  }