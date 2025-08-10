import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePicker } from 'primeng/datepicker';
import { FloatLabel } from "primeng/floatlabel";
import { ButtonFilterComponent } from '../../../shared/components/button-filter/button-filter.component';
import { ExpenseService } from '../../../../core/services/expense.service';
import { Category } from '../../../../shared/models/category';
import { Checkbox } from 'primeng/checkbox';
import { AutoComplete } from 'primeng/autocomplete';
import { getCategoryDisplay } from '../../../../shared/utils/category-display';
import { CommonModule } from '@angular/common';
import { DateRange } from '../../../../shared/types/date-range';

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
  @Output() public dateRangeUpdated = new EventEmitter<DateRange>();
  public rangeDates: Date[] | undefined;
  public categories?: Category[];
  public filteredCategories?: Category[];
  public selectedCategories: Category[] = [];
  public categorySearch: any;

  constructor(private expenseService: ExpenseService)
  {
  }

  ngOnInit(): void {
    this.categories = this.expenseService.categories.sort((a,b) => (a.name.toLowerCase() > b.name.toLowerCase()) ? 1 : ((b.name.toLowerCase() > a.name.toLowerCase()) ? -1 : 0));
    this.filteredCategories = this.categories;
    this.rangeDates = this.initialDateRange;
  }

  filterCallback(event: any) {

  }

  getCategoryDisplay = getCategoryDisplay;

  filterCategories(event: AutoCompleteCompleteEvent) {
    this.filteredCategories = this.categories?.filter(c => c.name.toLowerCase().startsWith(event.query.toLowerCase())) ?? [];
  }

  onDateRangeUpdate() {
    if (this.rangeDates?.length == 2 && this.rangeDates[0] && this.rangeDates[1]) {
      this.dateRangeUpdated.emit({
        startDate: this.rangeDates[0],
        endDate: this.rangeDates[1]
      });
    }
  }

}
