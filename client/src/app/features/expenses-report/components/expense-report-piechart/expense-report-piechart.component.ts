import { isPlatformBrowser } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { ChangeDetectorRef, Component, inject, input, OnInit, PLATFORM_ID, effect } from '@angular/core';
import { ExpenseCategorySummary } from '../../../../shared/models/category-summary';
import { FormatCurrencyPipe } from '../../../../shared/pipes/format-currency.pipe';

@Component({
  selector: 'app-expense-report-piechart',
  imports: [ChartModule],
  providers: [FormatCurrencyPipe],
  templateUrl: './expense-report-piechart.component.html'
})
export class ExpenseReportPiechartComponent implements OnInit {
    public categorySummaryData = input.required<ExpenseCategorySummary>();
    public currency = input.required<string>();

    data: any;
    options: any;
    platformId = inject(PLATFORM_ID);

    constructor(
      private cd: ChangeDetectorRef,
      private currencyPipe: FormatCurrencyPipe
    ) {
      effect(() => {
        this.initChart();
      });
    }

    ngOnInit() {
      this.initChart();
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
                labels: this.categorySummaryData()?.dataPoints.map(category => category.categoryName) ?? [],
                datasets: [
                    {
                        data: this.categorySummaryData()?.dataPoints.map(category => category.totalAmount) ?? [],
                        backgroundColor: colors.slice(0, this.categorySummaryData()?.dataPoints.length ?? 0),
                        hoverBackgroundColor: colors.slice(0, this.categorySummaryData()?.dataPoints.length ?? 0)
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
                                const category = this.categorySummaryData()?.dataPoints[context.dataIndex];
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
        return this.categorySummaryData()?.dataPoints.reduce((sum, category) => sum + category.totalAmount, 0) ?? 0;
    }
}
