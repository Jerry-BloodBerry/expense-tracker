import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Expense } from '../../shared/models/expense';
import { Pagination } from '../../shared/models/pagination';
import { ExpenseQueryParams } from '../../shared/models/expenseQueryParams';
import { CreateExpenseDto } from '../../shared/models/expense';
import { SingleResponse } from '../../shared/models/single-response';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private http = inject(HttpClient);

  getExpenses(queryParams: ExpenseQueryParams): Observable<Pagination<Expense>> {
    let params = new HttpParams();

    if (queryParams.startDate && queryParams.startDate.length) {
      params = params.append('startDate', queryParams.startDate);
    }
    if (queryParams.endDate && queryParams.endDate.length) {
      params = params.append('endDate', queryParams.endDate);
    }

    if (queryParams.categoryId !== undefined) {
      params = params.append("categoryId", queryParams.categoryId);
    }

    if (queryParams.tagIds && queryParams.tagIds.length) {
      params = params.append('tagIds', queryParams.tagIds.join(','));
    }

    if (queryParams.minAmount !== undefined) {
      params = params.append('minAmount', queryParams.minAmount);
    }
    if (queryParams.maxAmount !== undefined) {
      params = params.append('maxAmount', queryParams.maxAmount);
    }

    if (queryParams.isRecurring !== undefined) {
      params = params.append('isRecurring', queryParams.isRecurring);
    }

    if (queryParams.search && queryParams.search.trim()) {
      params = params.append('search', queryParams.search.trim());
    }

    params = params.append('pageSize', queryParams.pageSize);
    params = params.append('page', queryParams.page);

    return this.http.get<Pagination<Expense>>('/expenses', {params});
  }

  createExpense(expenseData: CreateExpenseDto) {
    return this.http.post<SingleResponse<Expense>>('/expenses', expenseData);
  }

}
