import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { ExpenseReportBarchartComponent } from './components/expense-report-barchart/expense-report-barchart.component';
import { DateRange } from '../../shared/types/date-range';
import { startOfMonth, endOfMonth, addMonths } from 'date-fns';
import { ExpensesReportHeaderComponent } from './components/expenses-report-header/expenses-report-header.component';
import { ExpenseReportFilters } from './expenses-report.dtos';

@Component({
  selector: 'app-expenses-report',
  imports: [TabsModule, RouterModule, CommonModule, ExpenseReportBarchartComponent, ExpensesReportHeaderComponent],
  templateUrl: './expenses-report.component.html',
  styleUrl: './expenses-report.component.scss'
})
export class ExpensesReportComponent {
  protected initialStartDate = startOfMonth(addMonths(new Date(), -2));
  protected initialEndDate = endOfMonth(addMonths(new Date(), -1));
  protected reportDateRange: DateRange = {startDate: this.initialStartDate, endDate: this.initialEndDate}
  protected reportCurrency: string = 'PLN';
  protected reportFilters: ExpenseReportFilters;

  constructor() {
    this.reportFilters = {
      dateRange: this.reportDateRange,
      categories: undefined
    }
  }

  onFiltersChange(filters: ExpenseReportFilters) {
    this.reportFilters = filters;
  }
}
