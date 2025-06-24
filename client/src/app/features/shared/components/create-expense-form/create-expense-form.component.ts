import { Component, inject, OnInit, Output, EventEmitter } from '@angular/core';
import { FormsModule, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePicker } from 'primeng/datepicker';
import { FloatLabel } from 'primeng/floatlabel';
import { InputNumber } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { ExpenseService } from '../../../../core/services/expense.service';
import { LocalStorageService } from '../../../../core/services/local-storage.service';
import { CURRENCIES, Currency } from '../../../../shared/utils/currencies.util';
import { Category } from '../../../../shared/models/category';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { RippleModule } from 'primeng/ripple';
import { Expense, CreateExpenseDto } from '../../../../shared/models/expense';
import { MultiSelectModule } from 'primeng/multiselect';
import { Tag } from '../../../../shared/models/tag';

@Component({
  selector: 'app-create-expense-form',
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    RippleModule,
    DatePicker,
    InputTextModule,
    FloatLabel,
    FormsModule,
    InputNumber,
    SelectModule,
    TextareaModule,
    ToggleSwitch,
    MultiSelectModule,
  ],
  templateUrl: './create-expense-form.component.html',
  styleUrl: './create-expense-form.component.scss'
})
export class CreateExpenseFormComponent implements OnInit {
  private formBuilder = inject(NonNullableFormBuilder);
  private localStorageService = inject(LocalStorageService);
  expenseService = inject(ExpenseService);
  currencies = CURRENCIES;

  categoryFilterValue: string = '';
  showCreateCategory: boolean = false;

  @Output() expenseCreated = new EventEmitter<Expense>();

  get tags(): Tag[] { return this.expenseService.tags; }
  showCreateTag: boolean = false;
  tagFilterValue: string = '';

  expenseForm = this.formBuilder.group({
    title: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(200)]],
    category: [null as Category | null, [Validators.required]],
    description: ['', [Validators.maxLength(1000)]],
    currency: [null as Currency | null, [Validators.required]],
    amount: [0.00, [Validators.required, Validators.min(0)]],
    date: [null as Date | null, [Validators.required]],
    isRecurring: [false, [Validators.required]],
    tagIds: this.formBuilder.control<number[]>([], [])
  });

  ngOnInit(): void {
    this.loadLastUsedCurrency();
  }

  onSubmit() {
    if (this.expenseForm.invalid) {
      this.expenseForm.markAllAsTouched();
      return;
    }

    this.expenseForm.disable();

    // Map form values to the expected DTO for the backend
    const formValue = this.expenseForm.value;
    const expenseDto: CreateExpenseDto = {
      name: formValue.title!,
      amount: formValue.amount!,
      categoryId: formValue.category!.id,
      tagIds: (formValue.tagIds ?? []) as number[],
      date: formValue.date!,
      description: formValue.description || null,
      currency: formValue.currency!.code,
      isRecurring: formValue.isRecurring!,
      recurrenceInterval: null // TODO: Add recurrence interval support
    };

    this.expenseService.createExpense(expenseDto).subscribe({
      next: (createdExpense: Expense) => {
        this.expenseCreated.emit(createdExpense);
        this.expenseForm.reset();
        this.expenseForm.enable();
      },
      error: (err: unknown) => {
        console.error('Failed to create expense:', err);
        this.expenseForm.enable();
      }
    });
  }

  createCategory(categoryName: string) {
    // TODO: Implement category creation logic
    console.log('Creating category:', categoryName);
  }

  filterCallback(event: any) {
    this.categoryFilterValue = event.filter;
    this.showCreateCategory = !this.expenseService.categories.some(
      cat => cat.name.toLowerCase().startsWith(this.categoryFilterValue.toLowerCase())
    );
  }

  loadLastUsedCurrency() {
    const lastUsedCurrencyCode = this.localStorageService.getLastUsedCurrency();
    this.expenseForm.patchValue({ currency: CURRENCIES.find(c => c.code === lastUsedCurrencyCode) || CURRENCIES[0]});
  }

  onCurrencyChange() {
    if (this.expenseForm.value.currency) {
      this.localStorageService.setLastUsedCurrency(this.expenseForm.value.currency.code);
    }
  }

  handleCancel(event: any) {
    console.log("Cancel button clicked!");
  }

  get title() {
    return this.expenseForm.get('title');
  }

  createTag(tagName: string) {
    this.expenseService.createTag(tagName).subscribe({
      next: (newTag: Tag) => {
        this.expenseService.tags.push(newTag);
        const current = this.expenseForm.value.tagIds ?? [];
        this.expenseForm.patchValue({ tagIds: [...current, newTag.id] });
        this.showCreateTag = false;
      },
      error: (err) => {
        console.error('Failed to create tag:', err);
      }
    });
  }

  onTagFilter(event: any) {
    this.tagFilterValue = event.filter;
    this.showCreateTag = !this.tags.some(
      tag => tag.name.toLowerCase() === this.tagFilterValue.toLowerCase()
    ) && this.tagFilterValue.length > 0;
  }

  getTagName(tagId: number): string {
    const tag = this.tags.find(t => t.id === tagId);
    return tag ? tag.name : '';
  }
}
