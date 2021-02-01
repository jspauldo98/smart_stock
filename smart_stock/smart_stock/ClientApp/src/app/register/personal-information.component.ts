import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import {FormBuilder, FormGroup, Validators } from '@angular/forms';
import { __asyncGenerator } from 'tslib';
import { IPii } from '../interfaces';

@Component({
  selector: 'app-personal-information',
  templateUrl: './personal-information.component.html',
  styleUrls: ['./personal-information.component.css']
})
export class PersonalInformationComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<IPii>();
  @Output() onNextTabChange = new EventEmitter<boolean>();

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
      email: [null, [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]],
      phone: [null]
    });
  }

  ngOnDestroy(): void {

  }

  emitPii(): void {
    this.onSaveEvent.emit(this.piiForm.value);
    this.onNextTabChange.emit(true);
  }

  onSubmit() {
    this.isSubmitAttempt = true;

    if (this.piiForm.invalid) {
      return;
    }

    let age = this.getAge(new Date(),(this.piiForm.get("dob").value));
    if(age < 18) {
      this.isAgeLimit = true;
      return;
    }
    this.emitPii();
    this.isSubmitAttempt = false;
    this.isAgeLimit = false;
  }

  getAge(currentDate: Date, userDob: Date): number {

    let age = currentDate.getFullYear() - userDob.getFullYear();
    let m = currentDate.getMonth() - userDob.getMonth();
    if (m < 0 || (m === 0 && currentDate.getDate() < userDob.getDate())) {
      age--;
    }
    return age;
  }
}
