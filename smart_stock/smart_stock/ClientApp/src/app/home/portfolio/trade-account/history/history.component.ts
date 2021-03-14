import { Component, Input, OnInit } from '@angular/core';
import { ILog, ITradeAccount, ITradeStrategies } from 'src/app/interfaces';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css']
})
export class HistoryComponent implements OnInit {

  @Input() log : ILog[];

  constructor() { }

  ngOnInit(): void {
  }

}
