import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Menu } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { BadgeModule } from 'primeng/badge';
import { RippleModule } from 'primeng/ripple';

@Component({
  selector: 'app-side-menu',
  imports: [Menu, RouterModule, BadgeModule, RippleModule],
  templateUrl: './side-menu.component.html',
  styleUrls: ['./side-menu.component.scss']
})
export class SideMenuComponent {
  items: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: ['/']
    },
    {
      label: 'Expenses',
      icon: 'pi pi-wallet',
      routerLink: ['/expenses']
    },
    {
      label: 'Categories',
      icon: 'pi pi-tags',
      routerLink: ['/categories']
    },
    {
      label: 'Tags',
      icon: 'pi pi-bookmark',
      routerLink: ['/tags']
    }
  ];
}
