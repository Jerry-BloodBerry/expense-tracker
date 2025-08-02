import { Component } from '@angular/core';
import { Menu } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { BadgeModule } from 'primeng/badge';
import { RippleModule } from 'primeng/ripple';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-side-menu',
  imports: [Menu, RouterLink, BadgeModule, RippleModule],
  templateUrl: './side-menu.component.html',
  styleUrls: ['./side-menu.component.scss']
})
export class SideMenuComponent {
  items: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: '/'
    },
    {
      label: 'Expenses Report',
      icon: 'pi pi-chart-bar',
      routerLink: '/expenses/report'
    },
    {
      label: 'Categories',
      icon: 'pi pi-tags',
      routerLink: '/categories'
    },
    {
      label: 'Tags',
      icon: 'pi pi-bookmark',
      routerLink: '/tags'
    }
  ];
}
