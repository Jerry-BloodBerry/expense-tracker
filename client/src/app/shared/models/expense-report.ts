import { ExpenseReportDataPoint } from "./expense-report-data-point";

export interface ExpenseReport {
  currency: string;
  startDate: Date;
  endDate: Date;
  dataPoints: ExpenseReportDataPoint[];
}
