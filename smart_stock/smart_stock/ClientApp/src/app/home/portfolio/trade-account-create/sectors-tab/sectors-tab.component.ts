import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ISector, ITradeAccount } from 'src/app/interfaces';

@Component({
  selector: 'app-sectors-tab',
  templateUrl: './sectors-tab.component.html',
  styleUrls: ['./sectors-tab.component.css']
})
export class SectorsTabComponent implements OnInit {

  @Output() onSaveEvent = new EventEmitter<ISector>();
  @Output() onNextTabChange = new EventEmitter<boolean>();
  @Input() ta : ITradeAccount;

  constructor(private formbuilder : FormBuilder) { }
  public sectorForm : FormGroup;
  public isNextAttempt : boolean = false;
  public isSubmissionAttemptValid : boolean = false;
  public sectorTransformedValues: boolean[] = [false, false, false, false, false,false, false, false, false, false, false];
  private sector : ISector;

  ngOnInit(): void {
    if (this.ta.id == null)
    {
      this.sectorForm = this.formbuilder.group({
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
    }
    else
    {
      this.sectorForm = this.formbuilder.group({
        'informationTechnology' : new FormControl(this.ta.preference.sector.informationTechnology),
        'healthCare'            : new FormControl(this.ta.preference.sector.healthCare),
        'financials'            : new FormControl(this.ta.preference.sector.financials),
        'consumerDiscretionary' : new FormControl(this.ta.preference.sector.consumerDiscretionary),
        'communication'         : new FormControl(this.ta.preference.sector.communication),
        'industrials'           : new FormControl(this.ta.preference.sector.industrials),
        'consumerStaples'       : new FormControl(this.ta.preference.sector.consumerStaples),
        'energy'                : new FormControl(this.ta.preference.sector.energy),
        'utilities'             : new FormControl(this.ta.preference.sector.utilities),
        'realEstate'            : new FormControl(this.ta.preference.sector.realEstate),
        'materials'             : new FormControl(this.ta.preference.sector.materials)
      });
    }
  }

  emitSector() : void {
    this.onSaveEvent.emit(this.sector);
    this.onNextTabChange.emit(true);
  }

  onSubmit() {
    this.isNextAttempt = true;

    let submittedSecValues: boolean[] = Object.values(this.sectorForm.value);
    submittedSecValues.forEach(x => {
      if (x === true) {
        this.isSubmissionAttemptValid = true;
        return;
      }
    });

    if (!this.isSubmissionAttemptValid) {
      return;
    }
    else {
      for(let [index, obj] of submittedSecValues.entries()) {
        if (obj) {
          this.sectorTransformedValues[index]=true;
        }
      }
      if (this.ta.id == null)
      {
        this.sector = {
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
          materials             : this.sectorTransformedValues[10]
        };
      }
      else 
      {
        this.sector = {
          id                    : this.ta.preference.sector.id,
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
          materials             : this.sectorTransformedValues[10]
        };
      }
      this.emitSector();
      this.isNextAttempt = false;
    }
  }
}
