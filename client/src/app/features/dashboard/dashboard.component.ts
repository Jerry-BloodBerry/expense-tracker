import { Component, inject, OnInit } from '@angular/core';
import { Expense } from '../../shared/models/expense';
import { ExpenseService } from '../../core/services/expense.service';

import { RecentExpensesTableComponent } from "./components/recent-expenses-table/recent-expenses-table.component";
import { Pagination } from '../../shared/models/pagination';
import { ExpenseQueryParams } from '../../shared/models/expenseQueryParams';
import { ExpenseSummaryCardComponent } from "./components/expense-summary-card/expense-summary-card.component";
import { FormatCurrencyPipe } from '../../shared/pipes/format-currency.pipe';
import { ExpenseCategoriesService } from '../../core/services/expense-categories.service';
import { Observable } from 'rxjs';
import { Category } from '../../shared/models/category';

@Component({
  selector: 'app-dashboard',
  imports: [RecentExpensesTableComponent, ExpenseSummaryCardComponent, FormatCurrencyPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  expenseService = inject(ExpenseService);
  expenseCategoriesService = inject(ExpenseCategoriesService);
  expenses?: Pagination<Expense>;
  categories$: Observable<Category[]>;

  constructor() {
    this.categories$ = this.expenseCategoriesService.getCategories();
  }

  ngOnInit(): void {
    this.initializeDashboard();
  }

  initializeDashboard() {
    this.expenseCategoriesService.getCategories().subscribe();
    this.expenseService.getTags();
  }

}
