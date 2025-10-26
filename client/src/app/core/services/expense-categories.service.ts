import { inject, Injectable, signal } from '@angular/core';
import { map, Observable, of, switchMap, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Category } from '../../shared/models/category';
import { ListResponse } from '../../shared/models/list-response';
import { SingleResponse } from '../../shared/models/single-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenseCategoriesService {
  private http = inject(HttpClient);
  #categories = signal<Category[]>([]);

  readonly categories = this.#categories.asReadonly();

  constructor() {
    this.getCategories().subscribe();
  }

  getCategories(): Observable<Category[]> {
    let currentCategories = this.#categories();
    if (currentCategories.length > 0) return of(currentCategories);
    return this.http.get<ListResponse<Category>>('/categories').pipe(
      map(res => res.data),
      tap(categories => this.#categories.set(categories))
    )
  }

  createCategory(name: string, description?: string): Observable<Category> {
    return this.http.post<SingleResponse<Category>>('/categories', {name, description}).pipe(
      switchMap(createdCategoryResponse => {
        this.clearCache();
        return this.getCategories().pipe(
          map(categories => categories.find(c => c.id === createdCategoryResponse.data.id)!)
        );
      })
    );
  }

  private clearCache(): void {
    this.#categories.set([]);
  }
}
