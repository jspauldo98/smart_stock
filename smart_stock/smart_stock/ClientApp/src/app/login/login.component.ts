import { Component, OnInit, OnDestroy } from '@angular/core';
import {FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ICredential } from '../interfaces';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {

  constructor(private readonly loginService: LoginService,
    private router: Router,
    private formBuilder: FormBuilder) { }


    public getUserLoginSub: Subscription;
    userForm: FormGroup;
    isSubmitted: boolean = false;
    isLoginInvalid: boolean = false;
    
    get formControls() { return this.userForm.controls; }

  ngOnInit(): void {
    this.userForm = this.formBuilder.group({
      username: [null, Validators.required],
      password: [null, Validators.required]
    });
  }

  toRegisterView() : void {
    this.router.navigateByUrl("/register");
  }

  ngOnDestroy(): void {
    if (this.getUserLoginSub) this.getUserLoginSub.unsubscribe();
  }

  onSubmit(): void {
    if (!this.userForm.valid) {
      return;
    }
    this.isSubmitted = true;
    let credentialBody: ICredential = {
      id: null,
      username: this.userForm.value.username,
      password: this.userForm.value.password
    };
    this.loginService.getUserLogin(credentialBody).subscribe(user => {
      if (user === null) {
        this.isLoginInvalid = true;
        return;
      }
      else {
        this.router.navigateByUrl('/home');
      }
    });
  }

}
