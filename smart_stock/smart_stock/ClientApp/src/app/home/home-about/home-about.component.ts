import { Component, OnInit } from '@angular/core';
import { FirstPaperTradeService } from '../../services/first-paper-trade.service';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-home-about',
  templateUrl: './home-about.component.html',
  styleUrls: ['./home-about.component.css']
})
export class HomeAboutComponent implements OnInit {

  constructor(private readonly firstPaperTradeService: FirstPaperTradeService) { }

  ngOnInit(): void {
  }

  public callWorkerShutdown() {
    this.firstPaperTradeService.stopWorkerThread().pipe(first()).subscribe(() => {});  
  }

}
