import { isPlatformBrowser } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { ChangeDetectorRef, Component, inject, input, OnChanges, PLATFORM_ID, SimpleChanges } from '@angular/core';
import { DateRange } from '../../../../shared/types/date-range';
import { ExpenseReportService } from '../../../../core/services/expense-report.service';
import { ExpenseReport } from '../../../../shared/models/expense-report';
import { format } from 'date-fns';
import { FormatCurrencyPipe } from '../../../../shared/pipes/format-currency.pipe';
import { ExpenseReportFilters } from '../../expenses-report.dtos';

@Component({
  selector: 'app-expense-report-barchart',
  imports: [ChartModule],
  providers: [FormatCurrencyPipe],
  templateUrl: './expense-report-barchart.component.html',
  styleUrl: './expense-report-barchart.component.scss'
})
export class ExpenseReportBarchartComponent implements OnChanges {
    public filters = input.required<ExpenseReportFilters>();
    public currency = input.required<string>();

    expensesReport?: ExpenseReport;
    data: any;
    options: any;
    platformId = inject(PLATFORM_ID);

    constructor(
      private cd: ChangeDetectorRef,
      private expenseReportService: ExpenseReportService,
      private currencyPipe: FormatCurrencyPipe
    ) {}

    ngOnChanges(changes: SimpleChanges): void {
      this.fetchExpensesChartData(this.filters(), this.currency());
    }

    ngOnInit() {
        this.fetchExpensesChartData(this.filters(), this.currency());
    }

    fetchExpensesChartData(filters: ExpenseReportFilters, currency: string) {
      if (!filters.dateRange?.startDate || !filters.dateRange?.endDate) {
        return;
      }
      this.expenseReportService.getExpenseReport(filters, currency).subscribe((response) => {
        this.expensesReport = response.data;
        this.initChart();
      });
    }

    initChart() {
        if (isPlatformBrowser(this.platformId)) {
            const documentStyle = getComputedStyle(document.documentElement);
            const textColor = documentStyle.getPropertyValue('--p-text-color');
            const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');

            this.data = {
                labels: this.expensesReport?.dataPoints.map(expense => format(expense.date, 'dd/MM/yyyy')) ?? [],

                datasets: [
                    {
                        label: `Money spent (${this.currency()})` ,
                        backgroundColor: documentStyle.getPropertyValue('--p-cyan-500'),
                        borderColor: documentStyle.getPropertyValue('--p-cyan-500'),
                        data: this.expensesReport?.dataPoints.map(expense => expense.amount) ?? []
                    },
                ]
            };

            this.options = {
                maintainAspectRatio: false,
                aspectRatio: 0.8,
                plugins: {
                    legend: {
                        labels: {
                            color: textColor
                        }
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            color: textColorSecondary,
                            font: {
                                weight: 500
                            },
                            autoSkip: true,
                            maxTicksLimit: 15
                        },
                        grid: {
                            color: 'transparent',
                            drawBorder: false
                        }
                    },
                    y: {
                        ticks: {
                            color: textColorSecondary,
                            callback: (value: number) => `${this.currencyPipe.transform(value, this.currency())}`,
                            maxTicksLimit: 9
                        },
                        grid: {
                            color: 'transparent',
                            drawBorder: false
                        }
                    }
                }
            };
            this.cd.markForCheck()
        }
    }
}
