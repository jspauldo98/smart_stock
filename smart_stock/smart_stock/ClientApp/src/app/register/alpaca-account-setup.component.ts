import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-alpaca-account-setup',
  templateUrl: './alpaca-account-setup.component.html',
  styleUrls: ['./alpaca-account-setup.component.css']
})
export class AlpacaAccountSetupComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<any>();
  @Output() onNextTabChange = new EventEmitter<boolean>();

  constructor(private formBuilder: FormBuilder) { }

  alpacaForm: FormGroup;
  isSubmitAttempt: boolean = false;
  
  get f() { return this.alpacaForm.controls };

  ngOnInit(): void {
    this.alpacaForm = this.formBuilder.group({
      alpacaKeyId: [null, Validators.required],
      alpacaKey: [null, Validators.required]
    });
  }

  emitAlpaca(): void {
    this.onSaveEvent.emit(this.alpacaForm.value);
    this.onNextTabChange.emit(true);
  }

  toAlpaca() {
    window.open('https://app.alpaca.markets/signup', "_blank")
  }

  onSubmit() {
    this.isSubmitAttempt = true;

    if (this.alpacaForm.invalid) {
      return;
    }
    this.emitAlpaca();
    this.isSubmitAttempt = false;
  }
}
