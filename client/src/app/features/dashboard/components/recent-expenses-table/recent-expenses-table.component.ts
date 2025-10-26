import { Component, inject, OnInit } from '@angular/core';
import { Expense } from '../../../../shared/models/expense';
import { DatePipe } from '@angular/common';
import { FormatCurrencyPipe } from '../../../../shared/pipes/format-currency.pipe';
import { Tag } from 'primeng/tag';
import { RouterLink } from '@angular/router';
import { ExpenseService } from '../../../../core/services/expense.service';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { CreateExpenseDialogComponent } from "../../../shared/components/create-expense-dialog/create-expense-dialog.component";
import { getCategoryDisplay } from '../../../../shared/utils/category-display';
import { ExpenseCategoriesService } from '../../../../core/services/expense-categories.service';
import { ExpenseQueryParams } from '../../../../shared/models/expenseQueryParams';
import { LoaderComponent } from "../../../shared/components/loader/loader.component";
@Component({
  selector: 'app-recent-expenses-table',
  imports: [CardModule, DatePipe, FormatCurrencyPipe, TableModule, Tag, RouterLink, CreateExpenseDialogComponent, LoaderComponent],
  templateUrl: './recent-expenses-table.component.html',
  styleUrl: './recent-expenses-table.component.scss'
})
export class RecentExpensesTableComponent implements OnInit {
  expenseQueryParams = new ExpenseQueryParams();
  expenseService = inject(ExpenseService);
  expenseCategoriesService = inject(ExpenseCategoriesService);
  categories = this.expenseCategoriesService.categories;
  expenses?: Expense[];
  isLoading: boolean = false;

  constructor() {
    this.expenseQueryParams.page = 1;
    this.expenseQueryParams.pageSize = 5;
  }

  ngOnInit(): void {
    this.refreshData();
  }

  getCategoryDisplay = getCategoryDisplay;

  refreshData() {
    if (this.isLoading) return;
    this.isLoading = true;

    this.expenseService.getExpenses(this.expenseQueryParams).subscribe({
      next: res => {
        this.expenses = res.data;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
}
