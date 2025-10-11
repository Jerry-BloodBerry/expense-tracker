import { Component } from '@angular/core';

import { RecentExpensesTableComponent } from "./components/recent-expenses-table/recent-expenses-table.component";
import { ExpenseSummaryCardComponent } from "./components/expense-summary-card/expense-summary-card.component";
import { FormatCurrencyPipe } from '../../shared/pipes/format-currency.pipe';

@Component({
  selector: 'app-dashboard',
  imports: [RecentExpensesTableComponent, ExpenseSummaryCardComponent, FormatCurrencyPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent{

}
