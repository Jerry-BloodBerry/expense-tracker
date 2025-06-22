import { Component, inject, OnInit } from '@angular/core';
import { Dialog } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumber } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { LocalStorageService } from '../../../../core/services/local-storage.service';
import { FloatLabel } from 'primeng/floatlabel';
import { TextareaModule } from 'primeng/textarea';
import { CURRENCIES, Currency } from '../../../../shared/utils/currencies.util';
import { ExpenseService } from '../../../../core/services/expense.service';
import { Category } from '../../../../shared/models/category';
import { DatePicker } from 'primeng/datepicker';
import { ToggleSwitchModule } from 'primeng/toggleswitch';

@Component({
  selector: 'app-create-expense-dialog',
  imports: [
    Dialog,
    ButtonModule,
    DatePicker,
    InputTextModule,
    FloatLabel,
    FormsModule,
    InputNumber,
    SelectModule,
    TextareaModule,
    ToggleSwitchModule
  ],
  templateUrl: './create-expense-dialog.component.html',
  styleUrl: './create-expense-dialog.component.scss'
})
export class CreateExpenseDialogComponent implements OnInit {
    expenseService = inject(ExpenseService);
    currencies = CURRENCIES;
    visible: boolean = false;

    amount: number = 0;
    description: string | null = null;
    selectedCategory?: Category;
    selectedCurrency!: Currency;

    categoryFilterValue: string = '';
    showCreateCategory: boolean = false;

    date: Date | undefined;
    isRecurring: boolean = false;


    constructor(private localStorageService: LocalStorageService) {}

    ngOnInit() {
      this.loadLastUsedCurrency();
    }

    loadLastUsedCurrency() {
      const lastUsedCurrencyCode = this.localStorageService.getLastUsedCurrency();
      this.selectedCurrency = CURRENCIES.find(c => c.code === lastUsedCurrencyCode) || CURRENCIES[0];
    }

    onCurrencyChange() {
      this.localStorageService.setLastUsedCurrency(this.selectedCurrency.code);
    }

    saveExpense() {

    }

    showDialog() {
      this.visible = true;
    }

    createCategory(categoryName: string) {
      // TODO: Implement category creation logic
      console.log('Creating category:', categoryName);
    }

    filterCallback(event: any) {
      this.categoryFilterValue = event.filter;
      this.showCreateCategory = !this.expenseService.categories.some(cat => cat.name.toLowerCase().startsWith(this.categoryFilterValue.toLowerCase()));
    }
}
