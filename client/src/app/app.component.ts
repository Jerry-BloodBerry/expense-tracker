import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { HeaderComponent } from "./layout/header/header.component";
import { SideMenuComponent } from './layout/side-menu/side-menu.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ButtonModule, HeaderComponent, SideMenuComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'client';
}
