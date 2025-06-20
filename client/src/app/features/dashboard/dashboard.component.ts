import { Component, inject, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { Expense } from '../../shared/models/expense';
import { FormatCurrencyPipe } from '../../shared/pipes/format-currency.pipe';
import { ExpenseService } from '../../core/services/expense.service';
import { Tag } from 'primeng/tag';
import { Category } from '../../shared/models/category';
import { getRandomTagColor } from '../../shared/utils/random-tag-color.util';
import { RouterLink } from '@angular/router';
import { Button } from 'primeng/button';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [Button, DatePipe, TableModule, FormatCurrencyPipe, Tag, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  expenseService = inject(ExpenseService);

  expenses: Expense[] = [
    {
      id: 1,
      name: "Groceries",
      amount: 54.23,
      category: 2,
      tags: [1, 3],
      date: new Date("2024-06-10"),
      description: "Weekly supermarket shopping",
      currency: "USD",
      isRecurring: false,
      recurrenceInterval: null
    },
    {
      id: 2,
      name: "Netflix Subscription",
      amount: 15.99,
      category: 4,
      tags: [2],
      date: new Date("2024-06-01"),
      description: "Monthly streaming service",
      currency: "PLN",
      isRecurring: true,
      recurrenceInterval: "Monthly"
    },
    {
      id: 3,
      name: "Bus Ticket",
      amount: 2.50,
      category: 3,
      tags: [4],
      date: new Date("2024-06-09"),
      description: null,
      currency: "EUR",
      isRecurring: false,
      recurrenceInterval: null
    },
    {
      id: 4,
      name: "Electricity Bill",
      amount: 60.75,
      category: 5,
      tags: [5, 6],
      date: new Date("2024-06-05"),
      description: "Monthly utility bill",
      currency: "USD",
      isRecurring: true,
      recurrenceInterval: "Monthly"
    },
    {
      id: 5,
      name: "Coffee",
      amount: 3.80,
      category: 1,
      tags: [1],
      date: new Date("2024-06-10"),
      description: "Morning coffee",
      currency: "USD",
      isRecurring: false,
      recurrenceInterval: null
    }
  ];

  ngOnInit(): void {
    this.initializeDashboard();
  }

  initializeDashboard() {
    this.expenseService.getCategories();
  }

  getCategoryDisplay(categoryId: number, categories: Category[]) {
    const category = categories.find(c => c.id === categoryId);
    if (!category) return { name: 'Unknown', color: '#BDBDBD' };
    return { name: category.name, color: getRandomTagColor(category.name) };
  }
}
