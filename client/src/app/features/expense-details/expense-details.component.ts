import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { Tag as PrimeTag } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { ExpenseService } from '../../core/services/expense.service';
import { ExpenseCategoriesService } from '../../core/services/expense-categories.service';
import { ExpenseTagsService } from '../../core/services/expense-tags.service';
import { Expense, RecurrenceInterval } from '../../shared/models/expense';
import { LoaderComponent } from '../shared/components/loader/loader.component';
import { FormatCurrencyPipe } from '../../shared/pipes/format-currency.pipe';
import { getCategoryDisplay } from '../../shared/utils/category-display';
import { EditExpenseFormComponent } from '../shared/components/edit-expense-form/edit-expense-form.component';

@Component({
  selector: 'app-expense-details',
  imports: [
    CommonModule,
    RouterModule,
    CardModule,
    ButtonModule,
    PrimeTag,
    DialogModule,
    ConfirmDialogModule,
    DatePipe,
    FormatCurrencyPipe,
    LoaderComponent,
    EditExpenseFormComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './expense-details.component.html',
  styleUrl: './expense-details.component.scss'
})
export class ExpenseDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private expenseService = inject(ExpenseService);
  private expenseCategoriesService = inject(ExpenseCategoriesService);
  private expenseTagsService = inject(ExpenseTagsService);
  private confirmationService = inject(ConfirmationService);

  protected expense = signal<Expense | null>(null);
  protected isLoading = signal<boolean>(true);
  protected showEditDialog = signal<boolean>(false);
  protected getCategoryDisplay = getCategoryDisplay;
  protected categories = this.expenseCategoriesService.categories;
  protected tags = this.expenseTagsService.tags;

  ngOnInit(): void {
    const expenseId = this.route.snapshot.paramMap.get('id');
    if (expenseId) {
      this.loadExpense(parseInt(expenseId, 10));
    }
  }

  private loadExpense(id: number): void {
    this.isLoading.set(true);
    this.expenseService.getExpenseById(id).subscribe({
      next: (response: any) => {
        this.expense.set(response.data);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Failed to load expense:', err);
        this.isLoading.set(false);
        // Navigate back on error
        this.router.navigate(['/expenses']);
      }
    });
  }

  protected getCategoryName(categoryId: number): string {
    const category = this.categories().find(c => c.id === categoryId);
    return category ? category.name : 'Unknown';
  }

  protected getTagNames(tagIds: number[]): string {
    return tagIds
      .map(id => {
        const tag = this.tags().find(t => t.id === id);
        return tag ? tag.name : 'Unknown';
      })
      .join(', ');
  }

  protected getTagObject(tagId: number) {
    return this.tags().find(t => t.id === tagId);
  }

  protected getRecurrenceDisplay(isRecurring: boolean, interval: RecurrenceInterval | null): string {
    if (!isRecurring) return 'No';
    return interval ? `Yes (${interval})` : 'Yes';
  }

  protected openEditDialog(): void {
    this.showEditDialog.set(true);
  }

  protected closeEditDialog(): void {
    this.showEditDialog.set(false);
  }

  protected onExpenseUpdated(updatedExpense: Expense): void {
    this.expense.set(updatedExpense);
    this.showEditDialog.set(false);
  }

  protected deleteExpense(): void {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete the expense "${this.expense()?.name}"?`,
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        const expenseId = this.expense()?.id;
        if (expenseId) {
          this.expenseService.deleteExpense(expenseId).subscribe({
            next: () => {
              this.router.navigate(['/expenses']);
            },
            error: (err: any) => {
              console.error('Failed to delete expense:', err);
            }
          });
        }
      }
    });
  }

  protected goBack(): void {
    this.router.navigate(['/expenses']);
  }
}
