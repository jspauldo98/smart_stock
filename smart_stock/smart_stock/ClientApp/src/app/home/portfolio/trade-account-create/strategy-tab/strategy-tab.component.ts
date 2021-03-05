import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ITradeAccount, ITradeStrategies } from 'src/app/interfaces';
import { __asyncGenerator } from 'tslib';

@Component({
  selector: 'app-strategy-tab',
  templateUrl: './strategy-tab.component.html',
  styleUrls: ['./strategy-tab.component.css']
})
export class StrategyTabComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<ITradeStrategies>();
  @Output() onNextTabChange = new EventEmitter<boolean>();
  @Input() ta : ITradeAccount;

  constructor(private formbuilder : FormBuilder) { }
  public strategyForm : FormGroup;
  public isNextAttempt : boolean = false;
  public isSubmissionAttemptValid : boolean = false;
  public stratTransformedValues: boolean[] = [false, false, false, false, false];
  private strat : ITradeStrategies;

  ngOnInit(): void {
    if (this.ta.id == null)
    {
      this.strategyForm = this.formbuilder.group({
        'blue chip' : new FormControl(false),
        'long term' : new FormControl(false),
        'swing'     : new FormControl(false),
        'scalp'     : new FormControl(false),
        'day'       : new FormControl(false)
      });
    }
    else {
      this.strategyForm = this.formbuilder.group({
        'blue chip' : new FormControl(this.ta.preference.tradeStrategy.blueChip),
        'long term' : new FormControl(this.ta.preference.tradeStrategy.longTerm),
        'swing'     : new FormControl(this.ta.preference.tradeStrategy.swing),
        'scalp'     : new FormControl(this.ta.preference.tradeStrategy.scalp),
        'day'       : new FormControl(this.ta.preference.tradeStrategy.day)
      });
    }
  }

  emitStrategy() : void {
    this.onSaveEvent.emit(this.strat);
    this.onNextTabChange.emit(true);
  }

  onSubmit() {
    this.isNextAttempt = true;

    let submittedStratValues: boolean[] = Object.values(this.strategyForm.value);
    submittedStratValues.forEach(x => {
      if (x === true) {
        this.isSubmissionAttemptValid = true;
        return;
      }
    });

    if (!this.isSubmissionAttemptValid) {
      return;
    }
    else {
      for(let [index, obj] of submittedStratValues.entries()) {
        if (obj) {
          this.stratTransformedValues[index]=true;
        }
      }
      if (this.ta.id == null) {
        this.strat = {
          id       : null,
          blueChip : this.stratTransformedValues[0],
          longTerm : this.stratTransformedValues[1],
          swing    : this.stratTransformedValues[2],
          scalp    : this.stratTransformedValues[3],
          day      : this.stratTransformedValues[4]
        };
      } else {
        this.strat = {
          id       : this.ta.preference.tradeStrategy.id,
          blueChip : this.stratTransformedValues[0],
          longTerm : this.stratTransformedValues[1],
          swing    : this.stratTransformedValues[2],
          scalp    : this.stratTransformedValues[3],
          day      : this.stratTransformedValues[4]
        };
      }
      
      this.emitStrategy();
      this.isNextAttempt = false;
    }
  }
}
