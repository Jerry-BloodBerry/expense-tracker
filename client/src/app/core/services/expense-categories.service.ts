import { inject, Injectable } from '@angular/core';
import { map, Observable, of, switchMap, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Category } from '../../shared/models/category';
import { ListResponse } from '../../shared/models/list-response';
import { SingleResponse } from '../../shared/models/single-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenseCategoriesService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  categories: Category[] = [];

  getCategories(): Observable<Category[]> {
    if (this.categories.length > 0) return of(this.categories);
    return this.http.get<ListResponse<Category>>(this.baseUrl + 'categories').pipe(
      map(res => res.data),
      tap(categories => this.categories = categories)
    )
  }

  createCategory(name: string, description?: string): Observable<Category> {
    return this.http.post<SingleResponse<Category>>(this.baseUrl + 'categories', {name, description}).pipe(
      switchMap(createdCategoryResponse => {
        this.clearCache();
        return this.getCategories().pipe(
          map(categories => categories.find(c => c.id === createdCategoryResponse.data.id)!)
        );
      })
    );
  }

  private clearCache(): void {
    this.categories = [];
  }
}
