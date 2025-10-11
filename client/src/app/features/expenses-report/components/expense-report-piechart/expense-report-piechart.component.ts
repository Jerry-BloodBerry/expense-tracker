import { isPlatformBrowser } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { ChangeDetectorRef, Component, inject, input, OnChanges, OnInit, PLATFORM_ID, SimpleChanges } from '@angular/core';
import { ExpenseReportService } from '../../../../core/services/expense-report.service';
import { ExpenseCategorySummary } from '../../../../shared/models/category-summary';
import { FormatCurrencyPipe } from '../../../../shared/pipes/format-currency.pipe';
import { ExpenseReportFilters } from '../../expenses-report.dtos';

@Component({
  selector: 'app-expense-report-piechart',
  imports: [ChartModule],
  providers: [FormatCurrencyPipe],
  templateUrl: './expense-report-piechart.component.html',
  styleUrl: './expense-report-piechart.component.scss'
})
export class ExpenseReportPiechartComponent implements OnInit, OnChanges {
    public filters = input.required<ExpenseReportFilters>();
    public currency = input.required<string>();

    categorySummary?: ExpenseCategorySummary;
    data: any;
    options: any;
    platformId = inject(PLATFORM_ID);

    constructor(
      private cd: ChangeDetectorRef,
      private expenseReportService: ExpenseReportService,
      private currencyPipe: FormatCurrencyPipe
    ) {}

    ngOnChanges(changes: SimpleChanges): void {
      this.fetchCategorySummaryData(this.filters(), this.currency());
    }

    ngOnInit() {
        this.fetchCategorySummaryData(this.filters(), this.currency());
    }

    fetchCategorySummaryData(filters: ExpenseReportFilters, currency: string) {
      if (!filters.dateRange?.startDate || !filters.dateRange?.endDate) {
        return;
      }
      this.expenseReportService.getExpenseCategorySummary(filters, currency).subscribe((response) => {
        this.categorySummary = response.data;
        this.initChart();
      });
    }

    initChart() {
        if (isPlatformBrowser(this.platformId)) {
            const documentStyle = getComputedStyle(document.documentElement);
            const textColor = documentStyle.getPropertyValue('--p-text-color');
            const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');

            // Generate colors for categories
            const colors = [
                documentStyle.getPropertyValue('--p-blue-500'),
                documentStyle.getPropertyValue('--p-green-500'),
                documentStyle.getPropertyValue('--p-orange-500'),
                documentStyle.getPropertyValue('--p-purple-500'),
                documentStyle.getPropertyValue('--p-red-500'),
                documentStyle.getPropertyValue('--p-cyan-500'),
                documentStyle.getPropertyValue('--p-pink-500'),
                documentStyle.getPropertyValue('--p-indigo-500')
            ];

            this.data = {
                labels: this.categorySummary?.dataPoints.map(category => category.categoryName) ?? [],
                datasets: [
                    {
                        data: this.categorySummary?.dataPoints.map(category => category.totalAmount) ?? [],
                        backgroundColor: colors.slice(0, this.categorySummary?.dataPoints.length ?? 0),
                        hoverBackgroundColor: colors.slice(0, this.categorySummary?.dataPoints.length ?? 0)
                    }
                ]
            };

            this.options = {
                maintainAspectRatio: false,
                aspectRatio: 0.8,
                plugins: {
                    legend: {
                        labels: {
                            color: textColor,
                            usePointStyle: true,
                            padding: 20
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: (context: any) => {
                                const category = this.categorySummary?.dataPoints[context.dataIndex];
                                const percentage = ((context.parsed / this.getTotalAmount()) * 100).toFixed(1);
                                return `${category?.categoryName}: ${this.currencyPipe.transform(category?.totalAmount ?? 0, this.currency())} (${percentage}%)`;
                            }
                        }
                    }
                }
            };
            this.cd.markForCheck();
        }
    }

    private getTotalAmount(): number {
        return this.categorySummary?.dataPoints.reduce((sum, category) => sum + category.totalAmount, 0) ?? 0;
    }
}
