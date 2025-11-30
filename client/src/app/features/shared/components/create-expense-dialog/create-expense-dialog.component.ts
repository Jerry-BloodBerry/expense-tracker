import { Component, EventEmitter, Output, Input, signal } from '@angular/core';
import { Dialog } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { CreateExpenseFormComponent } from "../create-expense-form/create-expense-form.component";
import { Expense } from '../../../../shared/models/expense';

@Component({
  selector: 'app-create-expense-dialog',
  imports: [
    Dialog,
    ButtonModule,
    SelectModule,
    CreateExpenseFormComponent
],
  templateUrl: './create-expense-dialog.component.html',
  styleUrl: './create-expense-dialog.component.scss'
})
export class CreateExpenseDialogComponent {
    visible = signal<boolean>(false);
    expenseTemplate = signal<Expense | null>(null);
    @Output() expenseCreated = new EventEmitter<void>();

    showDialog(templateExpense?: Expense) {
      this.expenseTemplate.set(templateExpense || null);
      this.visible.set(true);
    }

    onExpenseCreated() {
      this.visible.set(false);
      this.expenseTemplate.set(null);
      this.expenseCreated.emit();
    }

    onCancelClicked() {
      this.visible.set(false);
      this.expenseTemplate.set(null);
    }
}
