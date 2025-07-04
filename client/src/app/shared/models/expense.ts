export enum RecurrenceInterval {
  Daily = 'Daily',
  Weekly = 'Weekly',
  Monthly = 'Monthly',
  Quarterly = 'Quarterly',
  Yearly = 'Yearly'
}

export interface Expense {
  id: number;
  name: string;
  amount: number;
  category: number;
  tags: number[];
  date: Date;
  description: string | null;
  currency: string;
  isRecurring: boolean;
  recurrenceInterval: RecurrenceInterval | null;
}

export interface CreateExpenseDto {
  name: string;
  amount: number;
  categoryId: number;
  tagIds: number[];
  date: Date;
  description: string | null;
  currency: string;
  isRecurring: boolean;
  recurrenceInterval: RecurrenceInterval | null;
}
