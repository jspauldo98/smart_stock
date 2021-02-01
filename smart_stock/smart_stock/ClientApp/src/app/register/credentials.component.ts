import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import {FormBuilder, FormGroup, Validators } from '@angular/forms';
import { __asyncGenerator } from 'tslib';
import { ICredential } from '../interfaces';

@Component({
  selector: 'app-credentials',
  templateUrl: './credentials.component.html',
  styleUrls: ['./credentials.component.css']
})
export class CredentialsComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<ICredential>();
  @Output() onNextTabChange = new EventEmitter<boolean>();

  constructor(private formBuilder: FormBuilder) { }

  credentialForm: FormGroup;
  isSubmitAttempt: boolean = false;

  get f() { return this.credentialForm.controls };

  ngOnInit(): void {
    this.credentialForm = this.formBuilder.group({
      username: [null, Validators.required],
      password: [null, Validators.required]
    });
  }


  emitCredentials(): void {
    this.onSaveEvent.emit(this.credentialForm.value);
    this.onNextTabChange.emit(true);
  }

  onSubmit() {
    this.isSubmitAttempt = true;

    if (this.credentialForm.invalid) {
      return;
    }

    this.emitCredentials();
    this.isSubmitAttempt = false;
  }
}
