import { Component, input, OnInit, output, inject, computed, signal} from '@angular/core';
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
import { SelectModule } from 'primeng/select';
import { CURRENCIES } from '../../../../shared/utils/currencies.util';

@Component({
  selector: 'app-expenses-report-header',
  imports: [FormsModule, DatePicker, FloatLabel, ButtonFilterComponent, Checkbox, AutoComplete, CommonModule, SelectModule],
  templateUrl: './expenses-report-header.component.html'
})
export class ExpensesReportHeaderComponent implements OnInit {
  private expenseCategoriesService = inject(ExpenseCategoriesService);

  initialDateRange = input<Date[] | undefined>();
  initialCurrency = input<string>('PLN');
  filtersChange = output<ExpenseReportFilters>();

  protected rangeDates: Date[] | undefined;
  protected selectedCategories: Category[] = [];
  protected categorySearch = signal<string>('');
  protected selectedCurrency: string = 'PLN';

  protected categories = computed(() =>
    this.expenseCategoriesService.categories()
      .sort((a,b) => (
        a.name.toLowerCase() > b.name.toLowerCase() ? 1 :
        b.name.toLowerCase() > a.name.toLowerCase() ? -1 : 0
      )
    ));

  // Available currencies from utility
  protected currencies = CURRENCIES.map(currency => ({
    label: `${currency.name} (${currency.code})`,
    value: currency.code,
    symbol: currency.symbol
  }));

  ngOnInit(): void {
    this.rangeDates = this.initialDateRange();
    this.selectedCurrency = this.initialCurrency();
  }

  protected getCategoryDisplay = getCategoryDisplay;

  protected filteredCategories = computed(() =>
    this.categories().filter(c => c.name.toLowerCase().startsWith(this.categorySearch().toLowerCase())
  ));

  protected onDateRangeUpdate() {
    if (this.rangeDates?.length == 2 && this.rangeDates[0] && this.rangeDates[1]) {
      this.emitFilters();
    }
  }

  protected onCategoriesFilter() {
    this.emitFilters();
  }

  protected onCurrencyChange() {
    this.emitFilters();
  }

  private emitFilters() {
    const filters: ExpenseReportFilters = {
      dateRange: {
        startDate: this.rangeDates?.[0],
        endDate: this.rangeDates?.[1]
      },
      categories: this.selectedCategories,
      currency: this.selectedCurrency
    };

    this.filtersChange.emit(filters);
  }

}
