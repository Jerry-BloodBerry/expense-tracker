import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ExpensesReportComponent } from './features/expenses-report/expenses-report.component';

export const routes: Routes = [
  {path: '', component: DashboardComponent},
  {path: 'expenses/report', component: ExpensesReportComponent}
];
