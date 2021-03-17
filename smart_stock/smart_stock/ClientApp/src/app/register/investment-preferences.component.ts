import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { IPreference, IRiskLevel, ISector, ITradeAccount, ITradeStrategies } from '../interfaces';
import { Router } from '@angular/router';
import {PreferenceService } from '../services/preference.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-investment-preferences',
  templateUrl: './investment-preferences.component.html',
  styleUrls: ['./investment-preferences.component.css']
})
export class InvestmentPreferencesComponent implements OnInit {

  constructor(private readonly formBuilder: FormBuilder,
    private router: Router,
    private preferenceService: PreferenceService,
    private toastr : ToastrService) { }

  strategiesForm: FormGroup;
  sectorsForm : FormGroup;
  riskControl = new FormControl();
  capital :number = 1;
  isSubmissionStratValid: boolean = false; 
  isSubmissionSectorValid: boolean = false; 
  isSubmissionAttempt: boolean = false;
  stratTransformedValues: boolean[] = [false, false, false, false, false];
  sectorTransformedValues: boolean[] = [false, false, false, false, false, false, false, false, false, false, false];
  riskLevels : IRiskLevel[];
  // capitalToRisk : number;

  get f() { return this.strategiesForm.controls };

  ngOnInit(): void {
    this.strategiesForm = this.formBuilder.group({
      'blue chip' : new FormControl(false),
      'long term' : new FormControl(false),
      'swing'     : new FormControl(false),
      'scalp'     : new FormControl(false),
      'day'       : new FormControl(false)
    });
    this.sectorsForm = this.formBuilder.group({
      'informationTechnology' : new FormControl(false),
      'healthCare'            : new FormControl(false),
      'financials'            : new FormControl(false),
      'consumerDiscretionary' : new FormControl(false),
      'communication'         : new FormControl(false),
      'industrials'           : new FormControl(false),
      'consumerStaples'       : new FormControl(false),
      'energy'                : new FormControl(false),
      'utilities'             : new FormControl(false),
      'realEstate'            : new FormControl(false),
      'materials'             : new FormControl(false)
    });
    this.preferenceService.getRiskLevels().then(res => {this.riskLevels = res as IRiskLevel[], console.log(res)});      
  }

  formatLabel(value : number) {
    if (value <= 100) {
      return value + '%';
    }
  }

  onSubmit() {
    this.isSubmissionAttempt = true;
    let submittedStratValues: boolean[] = Object.values(this.strategiesForm.value);
    submittedStratValues.forEach(x => {
      if (x === true) {
        this.isSubmissionStratValid = true;
        return;
      }
    });
    let submittedSecValues: boolean[] = Object.values(this.sectorsForm.value);
    submittedSecValues.forEach(x => {
      if (x === true) {
        this.isSubmissionSectorValid = true;
        return;
      }
    });
    if (!this.isSubmissionStratValid || !this.isSubmissionSectorValid || this.riskControl.value == null) {
      return;
    }
    else {
      for(let [index, obj] of submittedStratValues.entries()) {
        if (obj) {
          this.stratTransformedValues[index]=true;
        }
      }  
      for(let [index, obj] of submittedSecValues.entries()) {
        if (obj) {
          this.sectorTransformedValues[index]=true;
        }
      } 
      // Init strategy obj   
      let tradeStrategiesObj: ITradeStrategies = {
        id       : null,
        blueChip : this.stratTransformedValues[0],
        longTerm : this.stratTransformedValues[1],
        swing    : this.stratTransformedValues[2],
        scalp    : this.stratTransformedValues[3],
        day      : this.stratTransformedValues[4]
      };
      // Init risk level obj
      let riskLevelObj : IRiskLevel = {
        id        : this.riskControl.value.id,
        risk      : this.riskControl.value.risk,
        dateAdded : new  Date()
      };
      // Init sectors level obj
      let sectorsObj : ISector = {
        id                    : null,
        informationTechnology : this.sectorTransformedValues[0],
        healthCare            : this.sectorTransformedValues[1],
        financials            : this.sectorTransformedValues[2],
        consumerDiscretionary : this.sectorTransformedValues[3],
        communication         : this.sectorTransformedValues[4],
        industrials           : this.sectorTransformedValues[5],
        consumerStaples       : this.sectorTransformedValues[6],
        energy                : this.sectorTransformedValues[7],
        utilities             : this.sectorTransformedValues[8],
        realEstate            : this.sectorTransformedValues[9],
        materials             : this.sectorTransformedValues[10],
      };
      console.log(sectorsObj);
      // Init preference Obj
      let preferenceObj : IPreference = {
        id            : 0,
        riskLevel     : riskLevelObj,
        tradeStrategy : tradeStrategiesObj,
        sector        : sectorsObj,
        capitalToRisk : this.capital
      };
      console.log(preferenceObj);
      this.preferenceService.createPreference(preferenceObj).subscribe(() => {
        this.router.navigateByUrl("/login");
        this.toastr.success('Updated successfully', 'Investment Preferences');
      });
    }
  }
}
