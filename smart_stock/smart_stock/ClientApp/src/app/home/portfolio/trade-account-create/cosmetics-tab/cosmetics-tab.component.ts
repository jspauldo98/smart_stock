import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { __asyncGenerator } from 'tslib';

@Component({
  selector: 'app-cosmetics-tab',
  templateUrl: './cosmetics-tab.component.html',
  styleUrls: ['./cosmetics-tab.component.css']
})
export class CosmeticsTabComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<[string, string]>();
  @Output() onNextTabChange = new EventEmitter<boolean>();

  constructor(private formbuilder : FormBuilder) { }

  public cosmeticsForm : FormGroup;
  public isNextAttempt : boolean = false;

  get f() { return this.cosmeticsForm.controls };

  ngOnInit(): void {
    this.cosmeticsForm = this.formbuilder.group({
      title: [null, Validators.required],
      description: [null, Validators.required]
    });
  }

  emitCosmetics() : void {
    this.onSaveEvent.emit(this.cosmeticsForm.value);
    this.onNextTabChange.emit(true);
  }

  onSubmit() {
    this.isNextAttempt = true;

    if (this.cosmeticsForm.invalid) {
      return;
    }

    this.emitCosmetics();
    this.isNextAttempt = false;
  }

}
