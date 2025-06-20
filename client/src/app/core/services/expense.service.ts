import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Category } from '../../shared/models/category';
import { HttpClient } from '@angular/common/http';
import { ListResponse } from '../../shared/models/list-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  categories: Category[] = [];

  getCategories() {
    if (this.categories.length > 0) return;
    this.http.get<ListResponse<Category>>(this.baseUrl + 'categories').subscribe({
      next: response => this.categories = response.data,
      error: error => console.log(error)
    })
  }
}
