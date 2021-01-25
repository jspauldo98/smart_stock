import { Component, OnInit, OnDestroy } from '@angular/core';
import {FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { IUser } from '../interfaces';
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


    public getUserSub: Subscription;
    userForm: FormGroup;
    isSubmitted: boolean = false;
    isLoginInvalid: boolean = false;
    user: IUser;
    
    get formControls() { return this.userForm.controls; }

  ngOnInit(): void {
    this.userForm = this.formBuilder.group({
      username: [null, Validators.required],
      password: [null, Validators.required]
    });
  }

  ngOnDestroy(): void {
    if (this.getUserSub) this.getUserSub.unsubscribe();
  }

  onSubmit(): void {
    if (!this.userForm.valid) {
      return;
    }
    this.isSubmitted = true;
    this.loginService.getUserLogin(this.userForm.value.username).subscribe(user => {
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
