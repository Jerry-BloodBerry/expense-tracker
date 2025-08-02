import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { ExpenseReportBarchartComponent } from './components/expense-report-barchart/expense-report-barchart.component';
import { DateRange } from '../../shared/types/date-range';
import { startOfMonth, endOfMonth, addMonths } from 'date-fns';

@Component({
  selector: 'app-expenses-report',
  imports: [TabsModule, RouterModule, CommonModule, ExpenseReportBarchartComponent],
  templateUrl: './expenses-report.component.html',
  styleUrl: './expenses-report.component.scss'
})
export class ExpensesReportComponent {
  private initialStartDate = startOfMonth(addMonths(new Date(), -2));
  private initialEndDate = endOfMonth(addMonths(new Date(), -1));
  public reportDateRange: DateRange = {startDate: this.initialStartDate, endDate: this.initialEndDate}
  public reportCurrency: string = 'PLN';
}
