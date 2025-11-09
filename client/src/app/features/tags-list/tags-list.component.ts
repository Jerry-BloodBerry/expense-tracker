import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { Tag } from 'primeng/tag';
import { FloatLabel } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { ExpenseTagsService } from '../../core/services/expense-tags.service';
import { Tag as TagModel } from '../../shared/models/tag';
import { getRandomTagColor } from '../../shared/utils/random-tag-color.util';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-tags-list',
  imports: [
    CommonModule,
    FormsModule,
    CardModule,
    Tag,
    FloatLabel,
    InputTextModule,
    ButtonModule
  ],
  templateUrl: './tags-list.component.html',
  styleUrl: './tags-list.component.scss'
})
export class TagsListComponent implements OnInit, OnDestroy {
  private expenseTagsService = inject(ExpenseTagsService);

  protected tags = this.expenseTagsService.tags;
  protected searchQuery = signal<string>('');
  
  private searchSubject = new Subject<string>();
  private subscriptions = new Subscription();

  // Filtered tags based on search, sorted alphabetically
  protected filteredTags = computed(() => {
    const search = this.searchQuery().toLowerCase().trim();
    const allTags = this.tags();
    
    let filtered = allTags;
    
    if (search) {
      filtered = allTags.filter(tag => 
        tag.name.toLowerCase().includes(search)
      );
    }
    
    // Sort alphabetically by name
    return filtered.sort((a, b) => 
      a.name.toLowerCase().localeCompare(b.name.toLowerCase())
    );
  });

  // Check if we should show create tag option
  protected showCreateTag = computed(() => {
    const search = this.searchQuery().trim();
    if (!search) return false;
    
    const searchLower = search.toLowerCase();
    return !this.tags().some(tag => 
      tag.name.toLowerCase() === searchLower
    );
  });

  ngOnInit(): void {
    // Setup debounced search (optional, for better UX)
    this.subscriptions.add(
      this.searchSubject.pipe(
        debounceTime(300),
        distinctUntilChanged()
      ).subscribe(searchTerm => {
        // Search is handled by computed signal, no action needed
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  protected onSearchChange() {
    this.searchSubject.next(this.searchQuery());
  }

  protected getTagColor(tag: TagModel): string {
    return getRandomTagColor(tag.name);
  }

  protected createTag(tagName: string) {
    if (!tagName.trim()) return;
    
    this.expenseTagsService.createTag(tagName.trim()).subscribe({
      next: (res: TagModel) => {
        // Clear search to show the newly created tag
        this.searchQuery.set('');
        // The service automatically refreshes the tags list
      },
      error: (err) => {
        console.error('Failed to create tag:', err);
      }
    });
  }

  protected onCreateTagClick() {
    const tagName = this.searchQuery().trim();
    if (tagName) {
      this.createTag(tagName);
    }
  }
}

