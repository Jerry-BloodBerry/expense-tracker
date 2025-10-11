import { inject, Injectable } from '@angular/core';
import { Tag } from '../../shared/models/tag';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../../shared/models/list-response';
import { map, Observable, of, switchMap, tap } from 'rxjs';
import { SingleResponse } from '../../shared/models/single-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenseTagsService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  tags: Tag[] = [];

  constructor() { }

  getTags(): Observable<Tag[]> {
    if (this.tags.length > 0) return of(this.tags);
    return this.http.get<ListResponse<Tag>>(this.baseUrl + 'tags').pipe(
      map(res => res.data),
      tap(tags => this.tags = tags)
    );
  }

  createTag(name: string): Observable<Tag> {
    return this.http.post<SingleResponse<Tag>>(this.baseUrl + 'tags', { name }).pipe(
      switchMap(createTagResponse => {
        this.clearCache();
        return this.getTags().pipe(
          map(tags => tags.find(t => t.id === createTagResponse.data.id)!)
        );
      })
    );
  }

  private clearCache(): void {
    this.tags = [];
  }
}
