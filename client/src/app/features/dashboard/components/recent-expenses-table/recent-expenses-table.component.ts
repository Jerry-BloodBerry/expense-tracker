import { Component, inject, Input } from '@angular/core';
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

@Component({
  selector: 'app-recent-expenses-table',
  imports: [CardModule, DatePipe, FormatCurrencyPipe, TableModule, Tag, RouterLink, CreateExpenseDialogComponent],
  templateUrl: './recent-expenses-table.component.html',
  styleUrl: './recent-expenses-table.component.scss'
})
export class RecentExpensesTableComponent {
  @Input() expenses?: Expense[];
  expenseService = inject(ExpenseService);

  getCategoryDisplay = getCategoryDisplay;
}
