import { Pipe, PipeTransform } from '@angular/core';
import { Category } from '../models/category';

@Pipe({
  name: 'categoryName'
})
export class CategoryNamePipe implements PipeTransform {
  transform(categoryId: number, categories: Category[] | null | undefined): string {
    if (!categories) return 'Loading...';
    const category = categories.find(c => c.id === categoryId);
    return category ? category.name : 'Unknown';
  }
}