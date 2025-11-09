import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecentExpensesTableComponent } from "./components/recent-expenses-table/recent-expenses-table.component";
import { ExpenseSummaryCardComponent } from "./components/expense-summary-card/expense-summary-card.component";
import { FormatCurrencyPipe } from '../../shared/pipes/format-currency.pipe';
import { DashboardService, DashboardSummary } from '../../core/services/dashboard.service';
import { LoaderComponent } from '../shared/components/loader/loader.component';

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    RecentExpensesTableComponent,
    ExpenseSummaryCardComponent,
    FormatCurrencyPipe,
    LoaderComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);

  protected summary = signal<DashboardSummary | null>(null);
  protected isLoading = signal<boolean>(true);

  ngOnInit(): void {
    this.loadDashboardSummary();
  }

  private loadDashboardSummary() {
    this.isLoading.set(true);
    this.dashboardService.getDashboardSummary().subscribe({
      next: (summary) => {
        this.summary.set(summary);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load dashboard summary:', err);
        this.isLoading.set(false);
      }
    });
  }
}
