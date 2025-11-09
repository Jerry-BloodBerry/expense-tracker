import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ExpensesReportComponent } from './features/expenses-report/expenses-report.component';
import { ExpensesListComponent } from './features/expenses-list/expenses-list.component';

export const routes: Routes = [
  {path: '', component: DashboardComponent},
  {path: 'expenses', component: ExpensesListComponent},
  {path: 'expenses/report', component: ExpensesReportComponent}
];
