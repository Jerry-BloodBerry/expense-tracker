import { Component} from '@angular/core';
import { Dialog } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { CreateExpenseFormComponent } from "../create-expense-form/create-expense-form.component";

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
    visible: boolean = false;

    showDialog() {
      this.visible = true;
    }

    onExpenseCreated() {
      this.visible = false;
    }

    onCancelClicked() {
      this.visible = false;
    }
}
