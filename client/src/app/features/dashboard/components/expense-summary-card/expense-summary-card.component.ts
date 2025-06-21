import { Component, Input } from '@angular/core';
import { PanelModule } from 'primeng/panel';

@Component({
  selector: 'app-expense-summary-card',
  imports: [PanelModule],
  templateUrl: './expense-summary-card.component.html',
  styleUrl: './expense-summary-card.component.scss'
})
export class ExpenseSummaryCardComponent {
  @Input() header: string = "";
  @Input() description?: string;
  @Input() icon?: string;
}
