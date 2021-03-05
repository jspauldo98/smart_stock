import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, NgForm } from '@angular/forms';
import { IRiskLevel, ITradeAccount } from 'src/app/interfaces';
import { PreferenceService } from 'src/app/services/preference.service';

@Component({
  selector: 'app-risk-tab',
  templateUrl: './risk-tab.component.html',
  styleUrls: ['./risk-tab.component.css']
})
export class RiskTabComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<[IRiskLevel, number]>();
  @Output() onNextTabChange = new EventEmitter<boolean>();
  @Input() ta : ITradeAccount;

  public risk : [IRiskLevel, number];

  constructor(private readonly preferenceService : PreferenceService) { }
  // public riskControl = new FormControl();
  public isNextAttempt : boolean = false;
  // public isSubmissionAttemptValid : boolean = false;
  // public capital : number = 1;
  public riskLevels : IRiskLevel[];

  ngOnInit(): void {
    this.dontClear();
    
    this.preferenceService.getRiskLevels().then(res => this.riskLevels = res as IRiskLevel[]);
  }

  dontClear(form? : NgForm) {
    if (form != null)
    {
      this.risk = [form.value.risk, form.value.capital];
    }
    else if (this.ta.id != null && form == null) 
    {
      this.risk = [this.ta.preference.riskLevel, this.ta.preference.capitalToRisk];
    }
    else
    {
      let nullRisk : IRiskLevel = {
        id : null,
        risk : null,
        dateAdded : new Date()
      };
      this.risk = [nullRisk, 1];
    }
  }

  emitRisk(form : NgForm) : void {
    this.onSaveEvent.emit([form.value.risk, form.value.capital]);
    this.onNextTabChange.emit(true);
  }

  formatLabel(value : number) {
    if (value <= 100) {
      return value + '%';
    }
  }

  onSubmit(form : NgForm) {
    this.isNextAttempt = true;

    if (form.value.risk === null || form.value.capital === null || 
        form.value.capital === "") {
      return;
    }

    this.dontClear(form);
    this.emitRisk(form);
    this.isNextAttempt = false;
  }
}
