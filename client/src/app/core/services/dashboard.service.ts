import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, combineLatest, map } from 'rxjs';
import { SingleResponse } from '../../shared/models/single-response';

export interface DashboardSummary {
  totalExpenses: number;
  topCategoryName: string;
  totalRecurringExpenses: number;
}

export interface TopCategoryResponse {
  categoryId: number;
  categoryName: string;
  totalAmount: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);

  getDashboardSummary(): Observable<DashboardSummary> {
    // Fetch all three summary values in parallel
    return combineLatest([
      this.getTotalExpensesThisMonth(),
      this.getTopSpendingCategoryThisMonth(),
      this.getTotalRecurringExpensesThisMonth()
    ]).pipe(
      map(([totalExpenses, topCategory, totalRecurring]) => ({
        totalExpenses: totalExpenses.data,
        topCategoryName: topCategory.data.categoryName,
        totalRecurringExpenses: totalRecurring.data
      }))
    );
  }

  private getTotalExpensesThisMonth(): Observable<SingleResponse<number>> {
    return this.http.get<SingleResponse<number>>('/dashboard/total-expenses-this-month');
  }

  private getTopSpendingCategoryThisMonth(): Observable<SingleResponse<TopCategoryResponse>> {
    return this.http.get<SingleResponse<TopCategoryResponse>>('/dashboard/top-spending-category-this-month');
  }

  private getTotalRecurringExpensesThisMonth(): Observable<SingleResponse<number>> {
    return this.http.get<SingleResponse<number>>('/dashboard/total-recurring-expenses-this-month');
  }
}

