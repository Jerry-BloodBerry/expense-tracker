import { Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePicker } from 'primeng/datepicker';
import { FloatLabel } from "primeng/floatlabel";
import { ButtonFilterComponent } from '../../../shared/components/button-filter/button-filter.component';
import { Category } from '../../../../shared/models/category';
import { Checkbox } from 'primeng/checkbox';
import { AutoComplete } from 'primeng/autocomplete';
import { getCategoryDisplay } from '../../../../shared/utils/category-display';
import { CommonModule } from '@angular/common';
import { ExpenseReportFilters } from '../../expenses-report.dtos';
import { ExpenseCategoriesService } from '../../../../core/services/expense-categories.service';

interface AutoCompleteCompleteEvent {
  originalEvent: Event;
  query: string;
}

@Component({
  selector: 'app-expenses-report-header',
  imports: [FormsModule, DatePicker, FloatLabel, ButtonFilterComponent, Checkbox, AutoComplete, CommonModule],
  templateUrl: './expenses-report-header.component.html',
  styleUrl: './expenses-report-header.component.scss'
})
export class ExpensesReportHeaderComponent implements OnInit {
  @Input() public initialDateRange: Date[] | undefined;
  @Output() public filtersChange = new EventEmitter<ExpenseReportFilters>();
  protected rangeDates: Date[] | undefined;
  protected categories?: Category[];
  protected filteredCategories?: Category[];
  protected selectedCategories: Category[] = [];
  protected categorySearch: any;
  protected filters: ExpenseReportFilters;

  constructor(private expenseCategoriesService: ExpenseCategoriesService)
  {
    this.filters = {};
  }

  ngOnInit(): void {
    this.rangeDates = this.initialDateRange;
    this.filters.categories = [];
    this.filters.dateRange = {
      startDate: this.initialDateRange?.[0],
      endDate: this.initialDateRange?.[1]
    }
    this.expenseCategoriesService.getCategories().subscribe({
      next: res => {
        this.categories = res.sort((a,b) => (a.name.toLowerCase() > b.name.toLowerCase()) ? 1 : ((b.name.toLowerCase() > a.name.toLowerCase()) ? -1 : 0));
        this.filteredCategories = this.categories;
      }
    });
  }

  filterCallback(event: any) {

  }

  getCategoryDisplay = getCategoryDisplay;

  filterCategories(event: AutoCompleteCompleteEvent) {
    this.filteredCategories = this.categories?.filter(c => c.name.toLowerCase().startsWith(event.query.toLowerCase())) ?? [];
  }

  onDateRangeUpdate() {
    if (this.rangeDates?.length == 2 && this.rangeDates[0] && this.rangeDates[1]) {
      this.filters.dateRange = {
        startDate: this.rangeDates[0],
        endDate: this.rangeDates[1]
      }
      this.filtersChange.emit(this.filters);
    }
  }

  onCategoriesFilter() {
    this.filters.categories = this.selectedCategories;
    this.filtersChange.emit(this.filters);
  }

}
