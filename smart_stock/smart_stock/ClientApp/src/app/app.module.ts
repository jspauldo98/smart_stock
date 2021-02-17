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

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { PlatformModule } from '@angular/cdk/platform';
 
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from '../app/home/home.component';
import { LoginComponent } from '../app/login/login.component';
import { RegisterComponent } from '../app/register/register.component';
import { PersonalInformationComponent } from '../app/register/personal-information.component';
import { CredentialsComponent } from '../app/register/credentials.component';
import { InvestmentPreferencesComponent } from '../app/register/investment-preferences.component';

import { LoginService } from './services/login.service';
import { UserService } from './services/user.service';
//Root module, everything needs to be imported here first
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    PersonalInformationComponent,
    CredentialsComponent,
    InvestmentPreferencesComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    FlexLayoutModule,
    BrowserAnimationsModule,
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
    MatSidenavModule
  ],
  providers: [DatePipe,
  LoginService,
  UserService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
