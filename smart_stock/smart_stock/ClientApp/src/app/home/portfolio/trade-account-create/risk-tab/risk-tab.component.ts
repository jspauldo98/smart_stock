import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { IRiskLevel } from 'src/app/interfaces';
import { PreferenceService } from 'src/app/services/preference.service';

@Component({
  selector: 'app-risk-tab',
  templateUrl: './risk-tab.component.html',
  styleUrls: ['./risk-tab.component.css']
})
export class RiskTabComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<[IRiskLevel, number]>();
  @Output() onNextTabChange = new EventEmitter<boolean>();

  constructor(private readonly preferenceService : PreferenceService) { }
  public riskControl = new FormControl();
  public isNextAttempt : boolean = false;
  public isSubmissionAttemptValid : boolean = false;
  public capital : number = 1;
  public riskLevels : IRiskLevel[];
  public riskLevel : IRiskLevel;

  ngOnInit(): void {
    this.riskLevel = {
      id        : null,
      risk      : null,
      dateAdded : null
    };
    this.preferenceService.getRiskLevels().then(res => this.riskLevels = res as IRiskLevel[]);
  }

  emitRisk() : void {
    this.onSaveEvent.emit([this.riskLevel, this.capital]);
    this.onNextTabChange.emit(true);
  }

  formatLabel(value : number) {
    if (value <= 100) {
      return value + '%';
    }
  }

  onSubmit() {
    this.isNextAttempt = true;

    if (this.riskControl.value == null) {
      return;
    }

    this.riskLevel.id = this.riskControl.value.id;
    this.riskLevel.id = this.riskControl.value.risk;
    this.riskLevel.id = this.riskControl.value.dateAdded;

    this.emitRisk();
    this.isNextAttempt = false;
  }
}
