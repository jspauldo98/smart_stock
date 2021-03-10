import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule, } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatGridListModule } from '@angular/material/grid-list';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatSliderModule} from '@angular/material/slider';
import {MatRadioModule} from '@angular/material/radio';
import { MatDialogModule } from '@angular/material/dialog';

import { BrowserModule } from '@angular/platform-browser';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { PlatformModule } from '@angular/cdk/platform';
import { ToastrModule } from "ngx-toastr";

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CommonModule } from '@angular/common';
import { HomeComponent } from '../app/home/home.component';
import { LoginComponent } from '../app/login/login.component';
import { RegisterComponent } from '../app/register/register.component';
import { PersonalInformationComponent } from '../app/register/personal-information.component';
import { CredentialsComponent } from '../app/register/credentials.component';
import { InvestmentPreferencesComponent } from '../app/register/investment-preferences.component';
import { UserDialogComponent } from '../app/mat-dialog-views/user-dialog.component';

import { LoginService } from './services/login.service';
import { UserService } from './services/user.service';
import { DashboardComponent } from './home/dashboard/dashboard.component';
import { PortfolioComponent } from './home/portfolio/portfolio.component';
import { PreferenceService } from './services/preference.service';
import { TradeAccountListComponent } from './home/portfolio/trade-account-list/trade-account-list.component';
import { HomeAboutComponent } from './home/home-about/home-about.component';
import { appInitializer } from './services/app-initializer';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { UnauthorizedInterceptor } from './interceptors/unauthorized.interceptor';
import { TradeAccountComponent } from './home/portfolio/trade-account/trade-account.component';
import { TradeAccountCreateComponent } from './home/portfolio/trade-account-create/trade-account-create.component';
import { CosmeticsTabComponent } from './home/portfolio/trade-account-create/cosmetics-tab/cosmetics-tab.component';
import { StrategyTabComponent } from './home/portfolio/trade-account-create/strategy-tab/strategy-tab.component';
import { SectorsTabComponent } from './home/portfolio/trade-account-create/sectors-tab/sectors-tab.component';
import { RiskTabComponent } from './home/portfolio/trade-account-create/risk-tab/risk-tab.component';
import { HistoryComponent } from './home/portfolio/trade-account/history/history.component';
import { TransferComponent } from './home/portfolio/transfer/transfer.component';
//Root module, everything needs to be imported here first
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    PersonalInformationComponent,
    CredentialsComponent,
    InvestmentPreferencesComponent,
    DashboardComponent,
    PortfolioComponent,
    TradeAccountListComponent,
    HomeAboutComponent,
    UserDialogComponent,
    TradeAccountComponent,
    TradeAccountCreateComponent,
    CosmeticsTabComponent,
    StrategyTabComponent,
    SectorsTabComponent,
    RiskTabComponent,
    HistoryComponent,
    TransferComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    FlexLayoutModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({positionClass :'toast-bottom-right'}),
    PlatformModule,
    MatToolbarModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatInputModule,
    MatCardModule,
    MatSlideToggleModule,
    MatSelectModule, 
    MatTabsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    MatListModule,
    MatSidenavModule,
    MatGridListModule,
    MatTooltipModule,
    MatSliderModule,
    MatRadioModule,
    MatDialogModule
  ],
  providers:[
    DatePipe,
    LoginService,
    UserService,
    PreferenceService,
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializer,
      multi: true,
      deps: [LoginService]
    },
    {
      provide: HTTP_INTERCEPTORS, 
      useClass: JwtInterceptor, 
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: UnauthorizedInterceptor,
      multi: true
    },
  ],
  bootstrap: [AppComponent],
  entryComponents: [UserDialogComponent]
})
export class AppModule { }
