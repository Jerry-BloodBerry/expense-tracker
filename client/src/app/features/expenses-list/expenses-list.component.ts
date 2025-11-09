import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { Tag } from 'primeng/tag';
import { DatePicker } from 'primeng/datepicker';
import { FloatLabel } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule } from 'primeng/paginator';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ExpenseService } from '../../core/services/expense.service';
import { ExpenseCategoriesService } from '../../core/services/expense-categories.service';
import { ExpenseQueryParams } from '../../shared/models/expenseQueryParams';
import { Expense } from '../../shared/models/expense';
import { Pagination } from '../../shared/models/pagination';
import { FormatCurrencyPipe } from '../../shared/pipes/format-currency.pipe';
import { getCategoryDisplay } from '../../shared/utils/category-display';
import { CreateExpenseDialogComponent } from '../shared/components/create-expense-dialog/create-expense-dialog.component';
import { LoaderComponent } from '../shared/components/loader/loader.component';

@Component({
  selector: 'app-expenses-list',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    CardModule,
    TableModule,
    Tag,
    DatePicker,
    FloatLabel,
    InputTextModule,
    ButtonModule,
    PaginatorModule,
    DatePipe,
    FormatCurrencyPipe,
    CreateExpenseDialogComponent,
    LoaderComponent
  ],
  templateUrl: './expenses-list.component.html',
  styleUrl: './expenses-list.component.scss'
})
export class ExpensesListComponent implements OnInit, OnDestroy {
  private expenseService = inject(ExpenseService);
  private expenseCategoriesService = inject(ExpenseCategoriesService);

  protected categories = this.expenseCategoriesService.categories;
  protected getCategoryDisplay = getCategoryDisplay;

  protected expenses = signal<Expense[]>([]);
  protected pagination = signal<Pagination<Expense> | null>(null);
  protected isLoading = signal<boolean>(false);

  protected searchQuery = signal<string>('');
  protected dateRange = signal<Date[] | undefined>(undefined);

  protected queryParams = new ExpenseQueryParams();

  // Pagination
  protected first = signal<number>(0);
  protected rows = signal<number>(10);

  // Debounce subjects
  private searchSubject = new Subject<string>();
  private dateRangeSubject = new Subject<Date[] | undefined>();
  private subscriptions = new Subscription();

  ngOnInit(): void {
    this.queryParams.pageSize = this.rows();
    this.queryParams.page = 1;

    // Setup debounced search
    this.subscriptions.add(
      this.searchSubject.pipe(
        debounceTime(300),
        distinctUntilChanged()
      ).subscribe(searchTerm => {
        this.queryParams.search = searchTerm.trim() || undefined;
        this.queryParams.page = 1; // Reset to first page on search
        this.first.set(0);
        this.loadExpenses();
      })
    );

    // Setup debounced date range
    this.subscriptions.add(
      this.dateRangeSubject.pipe(
        debounceTime(300),
        distinctUntilChanged((prev, curr) => {
          if (!prev && !curr) return true;
          if (!prev || !curr) return false;
          if (prev.length !== curr.length) return false;
          return prev[0]?.getTime() === curr[0]?.getTime() &&
                 prev[1]?.getTime() === curr[1]?.getTime();
        })
      ).subscribe(dateRange => {
        this.queryParams.page = 1; // Reset to first page on date filter change
        this.first.set(0);
        this.loadExpenses();
      })
    );

    this.loadExpenses();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  protected loadExpenses() {
    this.isLoading.set(true);

    // Update query params with date range
    if (this.dateRange() && this.dateRange()!.length === 2) {
      const startDate = this.dateRange()![0];
      const endDate = this.dateRange()![1];
      this.queryParams.startDate = startDate.toISOString().split('T')[0];
      this.queryParams.endDate = endDate.toISOString().split('T')[0];
    } else {
      this.queryParams.startDate = undefined;
      this.queryParams.endDate = undefined;
    }

    this.expenseService.getExpenses(this.queryParams).subscribe({
      next: (res: Pagination<Expense>) => {
        this.expenses.set(res.data);
        this.pagination.set(res);
        this.first.set((res.page - 1) * res.pageSize);
      },
      error: (err) => {
        console.error('Failed to load expenses:', err);
        this.isLoading.set(false);
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  protected onSearchChange() {
    // Trigger debounced search
    this.searchSubject.next(this.searchQuery());
  }

  protected onDateRangeChange() {
    // Only trigger if both dates are set, or if the range is cleared
    const currentRange = this.dateRange();
    if (!currentRange || currentRange.length === 0) {
      // Range is cleared - trigger immediately
      this.dateRangeSubject.next(undefined);
    } else if (currentRange.length === 2 && currentRange[0] && currentRange[1]) {
      // Both dates are set - trigger debounced filter
      this.dateRangeSubject.next(currentRange);
    }
    // If only one date is set, don't trigger yet
  }

  protected onPageChange(event: any) {
    this.first.set(event.first);
    this.rows.set(event.rows);
    this.queryParams.page = Math.floor(event.first / event.rows) + 1;
    this.queryParams.pageSize = event.rows;
    this.loadExpenses();
  }

  protected onExpenseCreated() {
    this.loadExpenses();
  }

  protected clearDateFilter() {
    this.dateRange.set(undefined);
    this.dateRangeSubject.next(undefined);
  }
}

