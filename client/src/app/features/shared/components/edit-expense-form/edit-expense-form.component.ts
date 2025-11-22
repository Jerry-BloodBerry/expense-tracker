import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Expense } from '../../../../shared/models/expense';
import { CreateExpenseFormComponent } from '../create-expense-form/create-expense-form.component';

@Component({
  selector: 'app-edit-expense-form',
  imports: [CreateExpenseFormComponent],
  template: `
    <app-create-expense-form
      [mode]="'edit'"
      [expenseToEdit]="expense"
      (expenseUpdated)="expenseUpdated.emit($event)"
      (cancelButtonClicked)="cancelButtonClicked.emit()"
    />
  `,
  styleUrl: './edit-expense-form.component.scss'
})
export class EditExpenseFormComponent {
  @Input() expense!: Expense;
  @Output() expenseUpdated = new EventEmitter<Expense>();
  @Output() cancelButtonClicked = new EventEmitter();
}
