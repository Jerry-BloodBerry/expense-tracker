import { isPlatformBrowser } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { ChangeDetectorRef, Component, inject, input, OnInit, PLATFORM_ID, effect } from '@angular/core';
import { ExpenseReport } from '../../../../shared/models/expense-report';
import { format } from 'date-fns';
import { FormatCurrencyPipe } from '../../../../shared/pipes/format-currency.pipe';
import { SingleResponse } from '../../../../shared/models/single-response';

@Component({
  selector: 'app-expense-report-barchart',
  imports: [ChartModule],
  providers: [FormatCurrencyPipe],
  templateUrl: './expense-report-barchart.component.html'
})
export class ExpenseReportBarchartComponent implements OnInit {
    public expensesData = input.required<ExpenseReport>();
    public currency = input.required<string>();

    data: any;
    options: any;
    platformId = inject(PLATFORM_ID);

    constructor(
      private cd: ChangeDetectorRef,
      private currencyPipe: FormatCurrencyPipe
    ) {
      // React to changes in the expenses data
      effect(() => {
        this.initChart();
      });
    }

    ngOnInit() {
      // Initialize chart if data is already available
      this.initChart();
    }

    initChart() {
        if (isPlatformBrowser(this.platformId)) {
            const documentStyle = getComputedStyle(document.documentElement);
            const textColor = documentStyle.getPropertyValue('--p-text-color');
            const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');

            this.data = {
                labels: this.expensesData()?.dataPoints.map(expense => format(expense.date, 'dd/MM/yyyy')) ?? [],

                datasets: [
                    {
                        label: `Money spent (${this.currency()})` ,
                        backgroundColor: documentStyle.getPropertyValue('--p-cyan-500'),
                        borderColor: documentStyle.getPropertyValue('--p-cyan-500'),
                          data: this.expensesData()?.dataPoints.map(expense => expense.amount) ?? []
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
