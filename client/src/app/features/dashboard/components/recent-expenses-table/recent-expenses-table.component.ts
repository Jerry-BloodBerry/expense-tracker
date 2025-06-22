import { Component, inject, Input } from '@angular/core';
import { Expense } from '../../../../shared/models/expense';
import { DatePipe } from '@angular/common';
import { FormatCurrencyPipe } from '../../../../shared/pipes/format-currency.pipe';
import { Tag } from 'primeng/tag';
import { RouterLink } from '@angular/router';
import { ExpenseService } from '../../../../core/services/expense.service';
import { TableModule } from 'primeng/table';
import { getRandomTagColor } from '../../../../shared/utils/random-tag-color.util';
import { Category } from '../../../../shared/models/category';
import { CardModule } from 'primeng/card';
import { CreateExpenseDialogComponent } from "../../../shared/components/create-expense-dialog/create-expense-dialog.component";

@Component({
  selector: 'app-recent-expenses-table',
  imports: [CardModule, DatePipe, FormatCurrencyPipe, TableModule, Tag, RouterLink, CreateExpenseDialogComponent],
  templateUrl: './recent-expenses-table.component.html',
  styleUrl: './recent-expenses-table.component.scss'
})
export class RecentExpensesTableComponent {
  @Input() expenses?: Expense[];
  expenseService = inject(ExpenseService);

  getCategoryDisplay(categoryId: number, categories: Category[]) {
    const category = categories.find(c => c.id === categoryId);
    if (!category) return { name: 'Unknown', color: '#BDBDBD' };
    return { name: category.name, color: getRandomTagColor(category.name) };
  }
}
