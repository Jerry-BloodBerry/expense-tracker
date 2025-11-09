import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { Tag } from 'primeng/tag';
import { FloatLabel } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { ExpenseCategoriesService } from '../../core/services/expense-categories.service';
import { Category } from '../../shared/models/category';
import { getRandomTagColor } from '../../shared/utils/random-tag-color.util';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-categories-list',
  imports: [
    CommonModule,
    FormsModule,
    CardModule,
    Tag,
    FloatLabel,
    InputTextModule,
    ButtonModule
  ],
  templateUrl: './categories-list.component.html',
  styleUrl: './categories-list.component.scss'
})
export class CategoriesListComponent implements OnInit, OnDestroy {
  private expenseCategoriesService = inject(ExpenseCategoriesService);

  protected categories = this.expenseCategoriesService.categories;
  protected searchQuery = signal<string>('');
  
  private searchSubject = new Subject<string>();
  private subscriptions = new Subscription();

  // Filtered categories based on search, sorted alphabetically
  protected filteredCategories = computed(() => {
    const search = this.searchQuery().toLowerCase().trim();
    const allCategories = this.categories();
    
    let filtered = allCategories;
    
    if (search) {
      filtered = allCategories.filter(cat => 
        cat.name.toLowerCase().includes(search)
      );
    }
    
    // Sort alphabetically by name
    return filtered.sort((a, b) => 
      a.name.toLowerCase().localeCompare(b.name.toLowerCase())
    );
  });

  // Check if we should show create category option
  protected showCreateCategory = computed(() => {
    const search = this.searchQuery().trim();
    if (!search) return false;
    
    const searchLower = search.toLowerCase();
    return !this.categories().some(cat => 
      cat.name.toLowerCase() === searchLower
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

  protected getCategoryColor(category: Category): string {
    return getRandomTagColor(category.name);
  }

  protected createCategory(categoryName: string) {
    if (!categoryName.trim()) return;
    
    this.expenseCategoriesService.createCategory(categoryName.trim()).subscribe({
      next: (res: Category) => {
        // Clear search to show the newly created category
        this.searchQuery.set('');
        // The service automatically refreshes the categories list
      },
      error: (err) => {
        console.error('Failed to create category:', err);
      }
    });
  }

  protected onCreateCategoryClick() {
    const categoryName = this.searchQuery().trim();
    if (categoryName) {
      this.createCategory(categoryName);
    }
  }
}

