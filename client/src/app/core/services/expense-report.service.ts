import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ExpenseReport } from '../../shared/models/expense-report';
import { SingleResponse } from '../../shared/models/single-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenseReportService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getExpenseReport(
    startDate: Date,
    endDate: Date,
    currency: string,
    tagIds?: number[],
    categoryIds?: number[]
  ): Observable<SingleResponse<ExpenseReport>> {
    let params = new HttpParams()
      .append('startDate', startDate.toISOString())
      .append('endDate', endDate.toISOString())
      .append('currency', currency);

    if (tagIds && tagIds.length > 0) {
      params = params.append('tagIds', tagIds.join(','));
    }

    if (categoryIds && categoryIds.length > 0) {
      params = params.append('categoryIds', categoryIds.join(','));
    }

    return this.http.get<SingleResponse<ExpenseReport>>(this.baseUrl + 'reports/expenses', { params });
  }
}
