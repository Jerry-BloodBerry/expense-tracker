import { Category } from "../models/category";
import { getRandomTagColor } from "./random-tag-color.util";

export interface CategoryDisplay {
  name: string;
  color: string;
}

export function getCategoryDisplay(categoryId: number, categories: Category[]) {
  console.log("called!");
  const category = categories.find(c => c.id === categoryId);
  if (!category) return { name: 'Unknown', color: '#BDBDBD' };
  return { name: category.name, color: getRandomTagColor(category.name) };
}
