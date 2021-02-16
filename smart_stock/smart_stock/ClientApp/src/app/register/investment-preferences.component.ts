import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ITradeStrategies } from '../interfaces';
import { Router } from '@angular/router';
import {TradeStrategiesService } from '../services/trade-strategies.service';

@Component({
  selector: 'app-investment-preferences',
  templateUrl: './investment-preferences.component.html',
  styleUrls: ['./investment-preferences.component.css']
})
export class InvestmentPreferencesComponent implements OnInit {

  constructor(private readonly formBuilder: FormBuilder,
    private router: Router,
    private tradeStrategiesService: TradeStrategiesService) { }

  strategiesForm: FormGroup;
  isSubmissionValid: boolean = false; 
  isSubmissionAttempt: boolean = false;
  transformedValues: string[] = [];

  get f() { return this.strategiesForm.controls };

  ngOnInit(): void {
    this.strategiesForm = this.formBuilder.group({
      'blue chip': new FormControl(false),
      'long term': new FormControl(false),
      'swing': new FormControl(false),
      'scalp': new FormControl(false),
      'day': new FormControl(false) 
    });
  }

  onSubmit() {
    this.isSubmissionAttempt = true;
    let submittedValues: boolean[] = Object.values(this.strategiesForm.value);
    submittedValues.forEach(x => {
      if (x === true) {
        this.isSubmissionValid = true;
        return;
      }
    });
    if (!this.isSubmissionValid) {
      return;
    }
    else {
      for(let [index, obj] of submittedValues.entries()) {
        if (index === 0) {
          this.transformedValues.push("Blue Chip: " + obj.toString());
        }
        else if (index === 1) {
          this.transformedValues.push("Long Term: " + obj.toString());
        }
        else if (index === 2) {
          this.transformedValues.push("Swing: " + obj.toString());
        }
        else if (index === 3) {
          this.transformedValues.push("Scalp: " + obj.toString());
        }
        else if (index === 4) {
          this.transformedValues.push("Day: " + obj.toString());
        }
      }
      let tradeStrategiesObj: ITradeStrategies = {
        id: 0,
        strategy: this.transformedValues,
        dateAdded: new Date()
      };
      console.log(tradeStrategiesObj);
      this.tradeStrategiesService.createTradeStrategy(tradeStrategiesObj).subscribe(() => {
        this.router.navigateByUrl("/login");
      });
    }
  }
}
