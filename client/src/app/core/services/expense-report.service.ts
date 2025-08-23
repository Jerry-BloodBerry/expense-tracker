import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ExpenseReport } from '../../shared/models/expense-report';
import { SingleResponse } from '../../shared/models/single-response';
import { ExpenseReportFilters } from '../../features/expenses-report/expenses-report.dtos';

@Injectable({
  providedIn: 'root'
})
export class ExpenseReportService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

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

    params = params.append('currency', currency);

    // if (tagIds && tagIds.length > 0) {
    //   params = params.append('tagIds', tagIds.join(','));
    // }

    return this.http.get<SingleResponse<ExpenseReport>>(this.baseUrl + 'reports/expenses', { params });
  }
}
