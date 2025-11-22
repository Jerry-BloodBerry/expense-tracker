import { Component, inject, OnInit, OnChanges, Output, EventEmitter, Input, computed, SimpleChanges } from '@angular/core';
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
import { Expense, CreateExpenseDto, RecurrenceInterval } from '../../../../shared/models/expense';
import { MultiSelectModule } from 'primeng/multiselect';
import { Tag } from '../../../../shared/models/tag';
import { SingleResponse } from '../../../../shared/models/single-response';
import { ExpenseCategoriesService } from '../../../../core/services/expense-categories.service';
import { ExpenseTagsService } from '../../../../core/services/expense-tags.service';

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
export class CreateExpenseFormComponent implements OnInit, OnChanges {
  private formBuilder = inject(NonNullableFormBuilder);
  private localStorageService = inject(LocalStorageService);
  protected expenseService = inject(ExpenseService);
  private expenseCategoriesService = inject(ExpenseCategoriesService);
  private expenseTagsService = inject(ExpenseTagsService);

  protected currencies = CURRENCIES;
  protected categories = this.expenseCategoriesService.categories;
  protected tags = this.expenseTagsService.tags;

  @Input() mode: 'create' | 'edit' = 'create';
  @Input() expenseToEdit: Expense | null = null;

  @Output() expenseCreated = new EventEmitter<Expense>();
  @Output() expenseUpdated = new EventEmitter<Expense>();
  @Output() cancelButtonClicked = new EventEmitter();

  protected submitButtonLabel = computed(() => this.mode === 'edit' ? 'Update' : 'Create');

  categoryFilterValue: string = '';
  showCreateCategory: boolean = false;

  showCreateTag: boolean = false;
  tagFilterValue: string = '';

  recurrenceIntervals = Object.values(RecurrenceInterval);
  isSubmitting = false;

  expenseForm = this.formBuilder.group({
    title: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(200)]],
    category: [null as number | null, [Validators.required]],
    description: ['', [Validators.maxLength(1000)]],
    currency: [null as Currency | null, [Validators.required]],
    amount: [0.00, [Validators.required, Validators.min(0)]],
    date: [null as Date | null, [Validators.required]],
    isRecurring: [false, [Validators.required]],
    tagIds: this.formBuilder.control<number[]>([], []),
    recurrenceInterval: [null as RecurrenceInterval | null, []]
  });

  ngOnInit(): void {
    if (this.mode === 'edit' && this.expenseToEdit) {
      this.populateFormForEdit();
    } else {
      this.loadLastUsedCurrency();
    }
    // Initially disable the control if not recurring
    if (!this.expenseForm.get('isRecurring')!.value) {
      this.expenseForm.get('recurrenceInterval')!.disable();
    }
    this.expenseForm.controls.isRecurring.valueChanges.subscribe((checked) => {
      if (!checked) {
        this.expenseForm.controls.recurrenceInterval.setValue(null);
        this.expenseForm.controls.recurrenceInterval.disable();
      } else {
        this.expenseForm.controls.recurrenceInterval.enable();
      }
    })
  }

  ngOnChanges(changes: SimpleChanges): void {
    // If expenseToEdit changes after initialization, repopulate the form
    if (changes['expenseToEdit'] && !changes['expenseToEdit'].firstChange && this.mode === 'edit' && this.expenseToEdit) {
      this.populateFormForEdit();
    }
  }

  private populateFormForEdit(): void {
    if (!this.expenseToEdit) return;

    const currency = CURRENCIES.find(c => c.code === this.expenseToEdit!.currency) || CURRENCIES[0];
    const dateValue = this.expenseToEdit.date instanceof Date
      ? this.expenseToEdit.date
      : new Date(this.expenseToEdit.date);

    this.expenseForm.patchValue({
      title: this.expenseToEdit.name,
      amount: this.expenseToEdit.amount,
      category: this.expenseToEdit.category,
      description: this.expenseToEdit.description || '',
      currency: currency,
      date: dateValue,
      isRecurring: this.expenseToEdit.isRecurring,
      tagIds: this.expenseToEdit.tags || [],
      recurrenceInterval: this.expenseToEdit.recurrenceInterval || null
    });
  }  onSubmit() {
    if (this.expenseForm.invalid) {
      this.expenseForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    // Map form values to the expected DTO for the backend
    const formValue = this.expenseForm.value;
    const expenseDto: CreateExpenseDto = {
      name: formValue.title!,
      amount: formValue.amount!,
      categoryId: formValue.category!,
      tagIds: (formValue.tagIds ?? []) as number[],
      date: formValue.date!,
      description: formValue.description || null,
      currency: formValue.currency!.code,
      isRecurring: formValue.isRecurring!,
      recurrenceInterval: formValue.isRecurring ? (formValue.recurrenceInterval ?? null) : null
    };

    if (this.mode === 'edit' && this.expenseToEdit) {
      this.expenseService.updateExpense(this.expenseToEdit.id, expenseDto).subscribe({
        next: (res: SingleResponse<Expense>) => {
          this.expenseUpdated.emit(res.data);
          this.isSubmitting = false;
        },
        error: (err: unknown) => {
          console.error('Failed to update expense:', err);
          this.isSubmitting = false;
        }
      });
    } else {
      this.expenseService.createExpense(expenseDto).subscribe({
        next: (res: SingleResponse<Expense>) => {
          this.expenseCreated.emit(res.data);
          this.expenseForm.reset();
          this.isSubmitting = false;
        },
        error: (err: unknown) => {
          console.error('Failed to create expense:', err);
          this.expenseForm.enable();
          this.isSubmitting = false;
        }
      });
    }
  }

  createCategory(categoryName: string) {
    this.expenseCategoriesService.createCategory(categoryName).subscribe({
      next: (res: Category) => {
        this.expenseForm.patchValue({ category: res.id });
        this.showCreateCategory = false;
      },
      error: (err) => {
        console.error('Failed to create tag:', err);
      }
    });
  }

  filterCallback(event: any) {
    this.categoryFilterValue = event.filter;
    this.showCreateCategory = !this.expenseCategoriesService.categories().some(
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

  handleCancel($event: MouseEvent) {
    this.cancelButtonClicked.emit();
  }

  get title() {
    return this.expenseForm.get('title');
  }

  createTag(tagName: string) {
    this.expenseTagsService.createTag(tagName).subscribe({
      next: (res: Tag) => {
        const current = this.expenseForm.value.tagIds ?? [];
        this.expenseForm.patchValue({ tagIds: [...current, res.id] });
        this.showCreateTag = false;
      },
      error: (err) => {
        console.error('Failed to create tag:', err);
      }
    });
  }

  onTagFilter(event: any) {
    this.tagFilterValue = event.filter;
    this.showCreateTag = !this.expenseTagsService.tags().some(
      tag => tag.name.toLowerCase() === this.tagFilterValue.toLowerCase()
    ) && this.tagFilterValue.length > 0;
  }

  getTagName(tagId: number): string {
    const tag = this.expenseTagsService.tags().find(t => t.id === tagId);
    return tag ? tag.name : '';
  }
}
