import { Injectable, signal, effect, inject } from '@angular/core';
import { ExpenseReportFilters } from './expenses-report.dtos';
import { addMonths, endOfMonth, startOfMonth } from 'date-fns';
import { ExpenseReportService } from '../../core/services/expense-report.service';
import { debounceTime, distinctUntilChanged, switchMap, Subject, map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root'
})
export class ExpensesReportFacadeService {
  private expenseReportService = inject(ExpenseReportService);

  private readonly defaultCurrency = 'PLN';
  private readonly debounceTime = 300;

  filters = signal<ExpenseReportFilters>({
    dateRange: {
      startDate: startOfMonth(addMonths(new Date(), -2)),
      endDate: endOfMonth(addMonths(new Date(), -1))
    },
    categories: undefined,
    tags: undefined,
    currency: 'PLN'
  });

  private filtersSubject = new Subject<ExpenseReportFilters>();

  expensesData$ = this.filtersSubject.pipe(
      debounceTime(this.debounceTime),
      distinctUntilChanged((prev, curr) =>
        JSON.stringify(prev) === JSON.stringify(curr)
      ),
      switchMap(filters =>
        this.expenseReportService.getExpenseReport(filters, filters.currency || this.defaultCurrency)
      ),
      map(response => response.data)
    );

  expensesData = toSignal(this.expensesData$, { initialValue: {
    dataPoints: [],
    currency: this.defaultCurrency,
    startDate: this.filters().dateRange?.startDate ?? new Date(),
    endDate: this.filters().dateRange?.endDate ?? new Date() }
  });

  // Computed signal for category summary data
  categorySummaryData$ =
    this.filtersSubject.pipe(
      debounceTime(this.debounceTime),
      distinctUntilChanged((prev, curr) =>
        JSON.stringify(prev) === JSON.stringify(curr)
      ),
      switchMap(filters =>
        this.expenseReportService.getExpenseCategorySummary(filters, filters.currency || this.defaultCurrency)
      ),
      map(response => response.data)
    );

  categorySummaryData = toSignal(this.categorySummaryData$, { initialValue: {
    dataPoints: [],
    currency: this.defaultCurrency,
    startDate: this.filters().dateRange?.startDate ?? new Date(),
    endDate: this.filters().dateRange?.endDate ?? new Date() }
  });

  constructor() {
    // Effect to emit filter changes to the subject
    effect(() => {
      this.filtersSubject.next(this.filters());
    });
  }

  // Method to update filters
  updateFilters(newFilters: Partial<ExpenseReportFilters>) {
    this.filters.set({ ...this.filters(), ...newFilters });
  }
}
