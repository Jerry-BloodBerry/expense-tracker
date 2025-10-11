export interface CategorySummaryDataPoint {
  categoryId: number;
  categoryName: string;
  totalAmount: number;
  count: number;
}

export interface ExpenseCategorySummary {
  currency: string;
  startDate: Date;
  endDate: Date;
  dataPoints: CategorySummaryDataPoint[];
}
