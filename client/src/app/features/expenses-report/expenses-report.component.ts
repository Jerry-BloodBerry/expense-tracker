import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { ExpenseReportBarchartComponent } from './components/expense-report-barchart/expense-report-barchart.component';
import { DateRange } from '../../shared/types/date-range';
import { startOfMonth, endOfMonth, addMonths } from 'date-fns';
import { ExpensesReportHeaderComponent } from './components/expenses-report-header/expenses-report-header.component';
import { ExpenseReportFilters } from './expenses-report.dtos';
import { ExpenseReportPiechartComponent } from "./components/expense-report-piechart/expense-report-piechart.component";
import { ExpensesReportFacadeService } from './expenses-report-facade.service';

@Component({
  selector: 'app-expenses-report',
  imports: [TabsModule, RouterModule, CommonModule, ExpenseReportBarchartComponent, ExpensesReportHeaderComponent, ExpenseReportPiechartComponent],
  templateUrl: './expenses-report.component.html',
  styleUrl: './expenses-report.component.scss'
})
export class ExpensesReportComponent {
  private facadeService = inject(ExpensesReportFacadeService);

  protected filters = this.facadeService.filters;
  protected expensesData = this.facadeService.expensesData;
  protected categorySummaryData = this.facadeService.categorySummaryData;

  onFiltersChange(filters: ExpenseReportFilters) {
    this.facadeService.updateFilters(filters);
  }
}
