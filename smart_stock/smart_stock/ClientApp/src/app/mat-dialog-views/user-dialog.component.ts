import { Component, OnInit, Inject } from '@angular/core';
import { IPortfolio, ITradeAccount, IUser, ICredential } from '../interfaces';
import { LoginService } from '../services/login.service';
import { UserService } from '../services/user.service';
import { PortfolioService } from '../services/portfolio.service';
import { first } from 'rxjs/operators';
import { MAT_DIALOG_DATA } from '@angular/material/dialog'
@Component({
  selector: 'app-user-dialog',
  templateUrl: './user-dialog.component.html',
  styleUrls: ['./user-dialog.component.css']
})
export class UserDialogComponent implements OnInit {

    constructor(@Inject(MAT_DIALOG_DATA) public data: IUser) {}

    ngOnInit(): void {
        console.log("dialog successfully called with " + JSON.stringify(this.data));        
    }


}