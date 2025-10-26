import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExpenseReport } from '../../shared/models/expense-report';
import { ExpenseCategorySummary } from '../../shared/models/category-summary';
import { SingleResponse } from '../../shared/models/single-response';
import { ExpenseReportFilters } from '../../features/expenses-report/expenses-report.dtos';

@Injectable({
  providedIn: 'root'
})
export class ExpenseReportService {
  private http = inject(HttpClient);

  getExpenseReport(
    filters: ExpenseReportFilters,
    currency: string
  ): Observable<SingleResponse<ExpenseReport>> {
    let params = new HttpParams();
    if (filters.dateRange?.startDate) {
      params = params.append('startDate', filters.dateRange.startDate.toISOString())
    }
    if (filters.dateRange?.endDate) {
      params = params.append('endDate', filters.dateRange.endDate.toISOString())
    }
    if (filters.categories && filters.categories.length) {
      params = params.append('categoryIds', filters.categories.map(c => c.id).join(','));
    }
    if (filters.tags && filters.tags.length) {
      params = params.append('tagIds', filters.tags.map(t => t.id).join(','));
    }

    params = params.append('currency', currency);

    return this.http.get<SingleResponse<ExpenseReport>>('/reports/expenses', { params });
  }

  getExpenseCategorySummary(
    filters: ExpenseReportFilters,
    currency: string
  ): Observable<SingleResponse<ExpenseCategorySummary>> {
    let params = new HttpParams();
    if (filters.dateRange?.startDate) {
      params = params.append('startDate', filters.dateRange.startDate.toISOString())
    }
    if (filters.dateRange?.endDate) {
      params = params.append('endDate', filters.dateRange.endDate.toISOString())
    }
    if (filters.categories && filters.categories.length) {
      params = params.append('categoryIds', filters.categories.map(c => c.id).join(','));
    }
    if (filters.tags && filters.tags.length) {
      params = params.append('tagIds', filters.tags.map(t => t.id).join(','));
    }

    params = params.append('currency', currency);

    return this.http.get<SingleResponse<ExpenseCategorySummary>>('/reports/expenses/category-summary', { params });
  }
}
