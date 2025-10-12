import { Category } from "../../shared/models/category";
import { Tag } from "../../shared/models/tag";
import { DateRange } from "../../shared/types/date-range";

export interface ExpenseReportFilters {
  dateRange?: DateRange;
  categories?: Category[] | undefined;
  tags?: Tag[] | undefined;
  currency?: string;
}
