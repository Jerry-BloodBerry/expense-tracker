import { inject, Injectable, signal } from '@angular/core';
import { Tag } from '../../shared/models/tag';
import { HttpClient } from '@angular/common/http';
import { ListResponse } from '../../shared/models/list-response';
import { map, Observable, of, switchMap, tap } from 'rxjs';
import { SingleResponse } from '../../shared/models/single-response';

@Injectable({
  providedIn: 'root'
})
export class ExpenseTagsService {
  private http = inject(HttpClient);
  tags = signal<Tag[]>([]);

  constructor() {
    this.getTags().subscribe();
  }

  getTags(): Observable<Tag[]> {
    let currentTags = this.tags();
    if (currentTags.length > 0) return of(currentTags);
    return this.http.get<ListResponse<Tag>>('/tags').pipe(
      map(res => res.data),
      tap(tags => this.tags.set(tags))
    );
  }

  createTag(name: string): Observable<Tag> {
    return this.http.post<SingleResponse<Tag>>('/tags', { name }).pipe(
      switchMap(createTagResponse => {
        this.clearCache();
        return this.getTags().pipe(
          map(tags => tags.find(t => t.id === createTagResponse.data.id)!)
        );
      })
    );
  }

  private clearCache(): void {
    this.tags.set([]);
  }
}
