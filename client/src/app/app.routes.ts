import { Routes } from '@angular/router';

export const routes: Routes = [
  {path: '', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)},
  {path: 'expenses/report', loadComponent: () => import('./features/expenses-report/expenses-report.component').then(m => m.ExpensesReportComponent)},
  {path: 'expenses/:id', loadComponent: () => import('./features/expense-details/expense-details.component').then(m => m.ExpenseDetailsComponent)},
  {path: 'expenses', loadComponent: () => import('./features/expenses-list/expenses-list.component').then(m => m.ExpensesListComponent)},
  {path: 'categories', loadComponent: () => import('./features/categories-list/categories-list.component').then(m => m.CategoriesListComponent)},
  {path: 'tags', loadComponent: () => import('./features/tags-list/tags-list.component').then(m => m.TagsListComponent)}
];
