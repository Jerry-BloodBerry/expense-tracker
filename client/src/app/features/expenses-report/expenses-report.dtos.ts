import { Category } from "../../shared/models/category";
import { DateRange } from "../../shared/types/date-range";

export interface ExpenseReportFilters {
  dateRange?: DateRange;
  categories?: Category[] | undefined;
}
