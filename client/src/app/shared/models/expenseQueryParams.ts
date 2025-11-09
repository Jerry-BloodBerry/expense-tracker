export class ExpenseQueryParams {
  startDate?: string;
  endDate?: string;
  categoryId?: number;
  tagIds: number[] = [];
  minAmount?: number;
  maxAmount?: number;
  isRecurring?: boolean;
  search?: string;
  page = 1;
  pageSize = 10;
}