import { Component, OnDestroy, OnInit, EventEmitter, Output } from '@angular/core';
import {Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { throwMatDialogContentAlreadyAttachedError } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { __asyncGenerator } from 'tslib';
import { IPii } from '../interfaces';

@Component({
  selector: 'app-personal-information',
  templateUrl: './personal-information.component.html',
  styleUrls: ['./personal-information.component.css']
})
export class PersonalInformationComponent implements OnInit, OnDestroy {

  @Output() onSaveEvent = new EventEmitter<IPii>();

  constructor(private formBuilder: FormBuilder) { }

  piiForm: FormGroup;
  isSubmitAttempt: boolean = false;
  isAgeLimit: boolean = false;


  get f() { return this.piiForm.controls };

  ngOnInit(): void {
    this.piiForm = this.formBuilder.group({
      fName: [null, Validators.required],
      lName: [null, Validators.required],
      dob: [null, Validators.required ],
      email: [null, Validators.required],
      phone: [null]
    });
  }

  ngOnDestroy(): void {

  }

  emitPii(): void {
    this.onSaveEvent.emit(this.piiForm.value);
  }

  onSubmit() {
    this.isSubmitAttempt = true;
    let age = this.getAge(new Date(),(this.piiForm.get("dob").value));
    if(age < 18) {
      this.isAgeLimit = true;
      return;
    }
    if (this.piiForm.invalid) {
      return;
    }
    this.emitPii();
    this.isSubmitAttempt = false;
    this.isAgeLimit = false;
  }

  getAge(currentDate: Date, userDob: Date): number {
    console.log(userDob);
    let age = currentDate.getFullYear() - userDob.getFullYear();
    let m = currentDate.getMonth() - userDob.getMonth();
    if (m < 0 || (m === 0 && currentDate.getDate() < userDob.getDate())) {
      age--;
    }
    return age;
  }
}
