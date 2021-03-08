import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ITradeAccount, ITradeStrategies } from 'src/app/interfaces';
import { __asyncGenerator } from 'tslib';

@Component({
  selector: 'app-cosmetics-tab',
  templateUrl: './cosmetics-tab.component.html',
  styleUrls: ['./cosmetics-tab.component.css']
})
export class CosmeticsTabComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<[string, string]>();
  @Output() onNextTabChange = new EventEmitter<boolean>();
  @Input() ta : ITradeAccount;

  public cosmetics : [string, string];

  constructor() { }

  public isNextAttempt : boolean = false;

  ngOnInit(): void {
    this.dontClear();
  }

  dontClear(form? : NgForm) {
    if (form != null)
    {
      this.cosmetics = [form.value.title, form.value.desc];
    }
    else if (this.ta.id != null && form == null) 
    {
      this.cosmetics = [this.ta.title, this.ta.description];
    }
    else
    {
      this.cosmetics = ["", ""];
    }
  }

  emitCosmetics(form : NgForm) : void {
    this.onSaveEvent.emit([form.value.title, form.value.desc]);
    this.onNextTabChange.emit(true);
  }

  onSubmit(form : NgForm) {
    this.isNextAttempt = true;

    if (form.value.title === null || form.value.desc === null || 
      form.value.title === "" || form.value.desc === "") {
      return;
    }

    this.dontClear(form);
    this.emitCosmetics(form);
    this.isNextAttempt = false;
  }

}
