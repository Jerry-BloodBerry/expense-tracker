import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Category } from '../../shared/models/category';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ListResponse } from '../../shared/models/list-response';
import { Expense } from '../../shared/models/expense';
import { Pagination } from '../../shared/models/pagination';
import { ExpenseQueryParams } from '../../shared/models/expenseQueryParams';
import { CreateExpenseDto } from '../../shared/models/expense';
import { Tag } from '../../shared/models/tag';
import { SingleResponse } from '../../shared/models/single-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  tags: Tag[] = [];

  getExpenses(queryParams: ExpenseQueryParams) {
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

    params = params.append('pageSize', queryParams.pageSize);
    params = params.append('page', queryParams.page);

    return this.http.get<Pagination<Expense>>(this.baseUrl + 'expenses', {params});
  }

  getTags() {
    if (this.tags.length > 0) return;
    this.http.get<ListResponse<Tag>>(this.baseUrl + 'tags').subscribe({
      next: response => this.tags = response.data,
      error: error => console.log(error)
    })
  }

  createExpense(expenseData: CreateExpenseDto) {
    return this.http.post<SingleResponse<Expense>>(this.baseUrl + 'expenses', expenseData);
  }

  createTag(name: string) {
    return this.http.post<SingleResponse<Tag>>(this.baseUrl + 'tags', { name });
  }
}
