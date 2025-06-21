import { Component, inject, OnInit } from '@angular/core';
import { Expense } from '../../shared/models/expense';
import { ExpenseService } from '../../core/services/expense.service';

import { RecentExpensesTableComponent } from "./components/recent-expenses-table/recent-expenses-table.component";
import { Pagination } from '../../shared/models/pagination';
import { ExpenseQueryParams } from '../../shared/models/expenseQueryParams';
import { ExpenseSummaryCardComponent } from "./components/expense-summary-card/expense-summary-card.component";
import { FormatCurrencyPipe } from '../../shared/pipes/format-currency.pipe';

@Component({
  selector: 'app-dashboard',
  imports: [RecentExpensesTableComponent, ExpenseSummaryCardComponent, FormatCurrencyPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  expenseService = inject(ExpenseService);
  expenseQueryParams = new ExpenseQueryParams();
  expenses?: Pagination<Expense>;

  ngOnInit(): void {
    this.initializeDashboard();
  }

  initializeDashboard() {
    this.expenseService.getCategories();
    this.expenseQueryParams.page = 1;
    this.expenseQueryParams.pageSize = 5;
    this.getExpenses();
  }

  getExpenses() {
    this.expenseService.getExpenses(this.expenseQueryParams).subscribe({
      next: response => this.expenses = response,
      error: error => console.log(error)
    })
  }

}
